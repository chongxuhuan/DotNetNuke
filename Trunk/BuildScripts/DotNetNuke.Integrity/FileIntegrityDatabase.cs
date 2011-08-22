using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Collections.Specialized;
using DotNetNuke.Integrity.Properties;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Encapsulates a method which verifies if an X.509 certificate matches certain criteria
    /// </summary>
    /// <param name="certificate">An <see cref="X509Certificate2"/> object containing the certificate to verify</param>
    /// <returns>A boolean indicating if the certificate matches the criteria</returns>
    public delegate bool CertificateVerifier(X509Certificate2 certificate);

    /// <summary>
    /// Encapsulates a method which maps an object of type <typeparamref name="TIn"/> to an object of
    /// type <typeparamref name="TOut"/>
    /// </summary>
    /// <typeparam name="TIn">The type of the input parameter</typeparam>
    /// <typeparam name="TOut">The type of the output parameter</typeparam>
    /// <param name="input">An object of type <typeparamref name="TIn"/> to be convered</param>
    /// <returns>A boolean indicating if the certificate matches the criteria</returns>
    public delegate TOut Mapper<TIn, TOut>(TIn input);

    /// <summary>
    /// Represents a database of signature entries that can be used to verify the integrity of files.
    /// </summary>
    /// <remarks>
    /// This is an immutable database, it cannot be changed, even if the 
    /// </remarks>
    // This class is sealed so that it cannot be compromised through the use of derived classes.
    public sealed class FileIntegrityDatabase : IDisposable {
        private const string DefaultHashAlgorithm = "sha512";

        private bool _closed;
        private bool _readOnly;
        private SignedFileCollection _files = new SignedFileCollection();
        private X509Certificate2 _certificate;
        private string _rootFolder;

        // TODO: Purchase a Code Signing Certificate from a trusted provider and put its thumbprint here
        // If the DotNetNuke File Integrity Authority certificate is compromised and a new one is issued, this value must be
        // updated.  However, if a signing certificate is compromised, this value should not need to change.
        //private const string DnnCoreAuthorityThumbprint = "PUT CORE AUTHORITY THUMBPRINT HERE";
        private const string DnnCoreAuthorityThumbprint = "15ff7387cc86e72be312b0aab81e9a35bdae0a9a";

        private static FileIntegrityDatabaseSerializer _serializer = new SignedXmlHashListSerializer();
        internal static FileIntegrityDatabaseSerializer DatabaseSerializer {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Creates a <see cref="CertificateVerifier"/> delegate which verifies a certificate using the specified thumbprint
        /// </summary>
        /// <param name="thumbprint">The thumbprint to verify against the provided certificate</param>
        /// <returns>A <see cref="CertificateVerifier"/> delegate which verifies a certificate using the specified thumbprint</returns>
        private static CertificateVerifier CreateThumbprintVerifier(string thumbprint) {
            return new CertificateVerifier(delegate(X509Certificate2 certificate) {
                return certificate.Thumbprint == thumbprint;
            });
        }
        private static SignedFile SignedFileIdentityMapper(SignedFile file) {
            return file;
        }
        
        /// <summary>
        /// Creates a <see cref="FileIntegrityDatabase"/> used for checking files which are part of the core
        /// of a DotNetNuke installation (i.e. those issued by DotNetNuke Corp.) or a null reference if no database exists
        /// </summary>
        /// <returns>
        /// A <see cref="FileIntegrityDatabase"/> configured for checking files which are part of the core
        /// of a DotNetNuke installation or a null reference if no database exists
        /// </returns>
        /// <example>
        /// The following is are examples of accessing the DotNetNuke Core File Integrity Database.
        /// <para>
        /// <code lang="VB">
        /// Using db As FileIntegrityDatabase = FileIntegrityDatabase.OpenDotNetNukeCoreDatabase
        ///     If db IsNot Nothing Then
        ///         If db.GetFileState("bin\DotNetNuke.dll") &lt;&gt; SignedFileState.Matched Then
        ///             ' Handle missing or modified file
        ///         End If
        ///     End If
        /// End Using
        /// </code>
        /// </para>
        /// <para>
        /// <code lang="C#">
        /// using(FileIntegrityDatabase db = FileIntegrityDatabase.OpenDotNetNukeCoreDatabase) {
        ///     if(db != null) {
        ///         if(db.GetFileState("bin\DotNetNuke.dll") != SignedFileState.Matched) {
        ///             // Handle missing or modified file
        ///         }
        ///     }
        /// }
        /// </code>
        /// </para>
        /// </example>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading the database file. Associated enumerations: <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>
        /// </permission>
        public static FileIntegrityDatabase OpenDotNetNukeCoreDatabase() {
            // Find the database
            string databaseFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Signatures/DotNetNukeCore.signatures");

            if (!File.Exists(databaseFile)) {
                return null;
            }

            // Create the database
            return new FileIntegrityDatabase(databaseFile, AppDomain.CurrentDomain.BaseDirectory, DnnCoreAuthorityThumbprint);
        }

        /// <summary>
        /// Gets the internal collection of <see cref="SignedFile"/> objects representing the entries in the signed file
        /// database.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        internal SignedFileCollection Files {
            get {
                VerifyNotClosed();
                return _files;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="SignedFile"/> objects representing the entries in the database
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public IEnumerable<SignedFile> SignedFiles {
            get {
                VerifyNotClosed();
                return _files;
            }
        }

        /// <summary>
        /// Gets a boolean indicating if the database is opened in read-only mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public bool ReadOnly {
            get {
                VerifyNotClosed();
                return _readOnly;
            }
        }

        /// <summary>
        /// The folder which contains all the files stored in this database
        /// </summary>
        public string RootFolder {
            get {
                VerifyNotClosed();
                return _rootFolder;
            }
        }

        /// <summary>
        /// Gets the certificate that was used to verify the database
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public X509Certificate2 Certificate {
            get {
                VerifyNotClosed();
                return _certificate;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="FileIntegrityDatabase"/> object for read-only access to data from an existing database file
        /// </summary>
        /// <param name="databasePath">The path to the database file</param>
        /// <param name="rootFolder">
        /// The folder in which the signed files are stored (this is the folder which the paths
        /// stored in <see cref="SignedFile.RelativePath"/> are relative to).  It is used to create the value
        /// for the <see cref="SignedFile.AbsolutePath"/> property</param>
        /// <param name="trustedIssuerThumbprint">
        /// The thumbprint of the trusted root issuer
        /// </param>
        /// <remarks>
        /// <para>
        /// Even if the certificate embedded in the file includes a private key, the database will be opened as read-only.
        /// </para>
        /// <para>
        /// File Integrity Database files embed the certificate used to sign the file in the database itself.
        /// Thus, the certificate must be issued by a trusted root authority.  However, we are also interested in the
        /// intermediate certificates in the trust chain.  For example, when checking a database containing files issued by 
        /// DotNetNuke Corp., the consumer must be assured that the certificate used to sign the database is part of a trust chain 
        /// involving a certificate issued by DotNetNuke Corp.  This is the purpose of the 
        /// parameter.
        /// </para>
        /// <para>
        /// It is recommended that a database is signed by a "3rd-level" certificate.  This means that the certificate trust chain
        /// should look something like the following ('->' indicates the certificate on the left is the parent of the certificate on the
        /// right): Trusted Root -> Trusted File Issuer -> Signing Certificate.  The "Trusted Root" certificate is a certificate that is 
        /// located in the "Root" store of the local machine's certificate store.  The "Trusted File Issuer" is a certificate which is trusted
        /// by a Trusted Root Authority (i.e. it was issued by a trusted certificate authority such as Thawte, Equifax, VeriSign, etc.). The
        /// thumbprint (unique ID) of the Trusted File Issuer is passed in as the  parameter.  The 
        /// database will check up the trust chain (including the Signing Certificate itself) to find a certificate with the specified 
        /// thumbprint.  This allows new Signing Certificates to be issued without changing the code which verifies the certificate.  
        /// If a Signing Certificate is compromised, it can be revoked, and a new one issued without having to change the verification 
        /// code (though a new database, signed with the new certificate, will need to be issued).
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidFileIntegrityDatabaseException">
        /// The database file is not a valid signature store file
        /// </exception>
        /// <exception cref="SignatureVerificationException">
        /// The signature could not be verified OR 
        /// the certificate used to verify the signature is not trusted by the system OR
        /// the certificate used to verify the signature is not trusted by a certificate with the thumbprint  OR
        /// there is no certificate embedded in the file.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An error occurred loading the file.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="databasePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading the database file. Associated enumerations: <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>
        /// </permission>
        public FileIntegrityDatabase(string databasePath, string rootFolder, string trustedIssuerThumbprint) {
            ArgumentContract.StringNotNullOrEmpty(databasePath, "databasePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.StringNotNullOrEmpty(trustedIssuerThumbprint, "trustedIssuerThumbprint");
            LoadDatabase(null, databasePath, rootFolder, CreateThumbprintVerifier(trustedIssuerThumbprint));
        }
        
        /// <summary>
        /// Constructs a new <see cref="FileIntegrityDatabase"/> object using data from an existing database file.  The
        /// database will be opened in either read-only or read-write mode, depending on the <paramref name="certificate"/> parameter
        /// (see Remarks).
        /// </summary>
        /// <param name="certificate">
        /// An <see cref="X509Certificate2"/> object representing the certificate to use to verify (and optionally sign) the database 
        /// (ignoring the embedded certificate, if any).
        /// </param>
        /// <param name="databasePath">The path to the database file</param>
        /// <param name="rootFolder">
        /// The folder in which the signed files are stored (this is the folder which the paths
        /// stored in <see cref="SignedFile.RelativePath"/> are relative to).  It is used to create the value
        /// for the <see cref="SignedFile.AbsolutePath"/> property</param>
        /// <param name="trustedIssuerThumbprint">
        /// The thumbprint of the trusted root issuer
        /// </param>
        /// <remarks>
        /// <para>
        /// This override ignores any certificates embedded in the file, and instead uses the provided certificate to verify
        /// the signature of the file.  This can also be used if the file does not include a certificate.  If the provided certificate
        /// includes a private key (i.e. <see cref="X509Certificate2.PrivateKey"/> is not null), the database is opened for read-write
        /// access.  Otherwise, it is opened for read-only access.
        /// </para>
        /// <para>
        /// File Integrity Database files embed the certificate used to sign the file in the database itself.
        /// Thus, the certificate must be issued by a trusted root authority.  However, we are also interested in the
        /// intermediate certificates in the trust chain.  For example, when checking a database containing files issued by 
        /// DotNetNuke Corp., the consumer must be assured that the certificate used to sign the database is part of a trust chain 
        /// involving a certificate issued by DotNetNuke Corp.  This is the purpose of the 
        /// parameter.
        /// </para>
        /// <para>
        /// It is recommended that a database is signed by a "3rd-level" certificate.  This means that the certificate trust chain
        /// should look something like the following ('->' indicates the certificate on the left is the parent of the certificate on the
        /// right): Trusted Root -> Trusted File Issuer -> Signing Certificate.  The "Trusted Root" certificate is a certificate that is 
        /// located in the "Root" store of the local machine's certificate store.  The "Trusted File Issuer" is a certificate which is trusted
        /// by a Trusted Root Authority (i.e. it was issued by a trusted certificate authority such as Thawte, Equifax, VeriSign, etc.). The
        /// thumbprint (unique ID) of the Trusted File Issuer is passed in as the  parameter.  The 
        /// database will check up the trust chain (including the Signing Certificate itself) to find a certificate with the specified 
        /// thumbprint.  This allows new Signing Certificates to be issued without changing the code which verifies the certificate.  
        /// If a Signing Certificate is compromised, it can be revoked, and a new one issued without having to change the verification 
        /// code (though a new database, signed with the new certificate, will need to be issued).
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidFileIntegrityDatabaseException">
        /// The database file is not a valid signature store file
        /// </exception>
        /// <exception cref="SignatureVerificationException">
        /// The signature could not be verified OR the certificate used to verify the signature is not trusted by the system OR
        /// the certificate used to verify the signature is not trusted a certificate with the thumbprint .
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An error occurred loading the file.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="databasePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="certificate" /> parameter is null.
        /// </exception>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading the database file. Associated enumerations: <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>
        /// </permission>
        public FileIntegrityDatabase(X509Certificate2 certificate, string databasePath, string rootFolder, string trustedIssuerThumbprint) {
            ArgumentContract.NotNull(certificate, "certificate");
            ArgumentContract.StringNotNullOrEmpty(databasePath, "databasePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.StringNotNullOrEmpty(trustedIssuerThumbprint, "trustedIssuerThumbprint");
            LoadDatabase(certificate, databasePath, rootFolder, CreateThumbprintVerifier(trustedIssuerThumbprint));
        }

        /// <summary>
        /// Constructs a new <see cref="FileIntegrityDatabase"/> object for read-only access to data from an existing database file
        /// </summary>
        /// <param name="databasePath">The path to the database file</param>
        /// <param name="rootFolder">
        /// The folder in which the signed files are stored (this is the folder which the paths
        /// stored in <see cref="SignedFile.RelativePath"/> are relative to).  It is used to create the value
        /// for the <see cref="SignedFile.AbsolutePath"/> property</param>
        /// <param name="trustedIssuerVerifier">
        /// A <see cref="CertificateVerifier"/> delegate that will be invoked on each certificate in the trust chain for the
        /// certificate used to sign the document.  If the delegate returns true on at least one of these certificates, the
        /// signed certificate is accepted as being issued by a trusted file issuer
        /// </param>
        /// <remarks>
        /// <para>
        /// Even if the certificate embedded in the file includes a private key, the database will be opened as read-only.
        /// </para>
        /// <para>
        /// File Integrity Database files embed the certificate used to sign the file in the database itself.
        /// Thus, the certificate must be issued by a trusted root authority.  However, we are also interested in the
        /// intermediate certificates in the trust chain.  For example, when checking a database containing files issued by 
        /// DotNetNuke Corp., the consumer must be assured that the certificate used to sign the database is part of a trust chain 
        /// involving a certificate issued by DotNetNuke Corp.  This is the purpose of the 
        /// parameter.
        /// </para>
        /// <para>
        /// It is recommended that a database is signed by a "3rd-level" certificate.  This means that the certificate trust chain
        /// should look something like the following ('->' indicates the certificate on the left is the parent of the certificate on the
        /// right): Trusted Root -> Trusted File Issuer -> Signing Certificate.  The "Trusted Root" certificate is a certificate that is 
        /// located in the "Root" store of the local machine's certificate store.  The "Trusted File Issuer" is a certificate which is trusted
        /// by a Trusted Root Authority (i.e. it was issued by a trusted certificate authority such as Thawte, Equifax, VeriSign, etc.). The
        /// thumbprint (unique ID) of the Trusted File Issuer is passed in as the  parameter.  The 
        /// database will check up the trust chain (including the Signing Certificate itself) to find a certificate with the specified 
        /// thumbprint.  This allows new Signing Certificates to be issued without changing the code which verifies the certificate.  
        /// If a Signing Certificate is compromised, it can be revoked, and a new one issued without having to change the verification 
        /// code (though a new database, signed with the new certificate, will need to be issued).
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidFileIntegrityDatabaseException">
        /// The database file is not a valid signature store file
        /// </exception>
        /// <exception cref="SignatureVerificationException">
        /// The signature could not be verified OR 
        /// the certificate used to verify the signature is not trusted by the system OR
        /// the certificate used to verify the signature is not trusted by a certificate with the thumbprint  OR
        /// there is no certificate embedded in the file.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An error occurred loading the file.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="databasePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading the database file. Associated enumerations: <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>
        /// </permission>
        public FileIntegrityDatabase(string databasePath, string rootFolder, CertificateVerifier trustedIssuerVerifier) {
            ArgumentContract.StringNotNullOrEmpty(databasePath, "databasePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.NotNull(trustedIssuerVerifier, "trustedIssuerVerifier");
            LoadDatabase(null, databasePath, rootFolder, trustedIssuerVerifier);
        }

        /// <summary>
        /// Constructs a new <see cref="FileIntegrityDatabase"/> object using data from an existing database file.  The
        /// database will be opened in either read-only or read-write mode, depending on the <paramref name="certificate"/> parameter
        /// (see Remarks).
        /// </summary>
        /// <param name="certificate">
        /// An <see cref="X509Certificate2"/> object representing the certificate to use to verify (and optionally sign) the database 
        /// (ignoring the embedded certificate, if any).
        /// </param>
        /// <param name="databasePath">The path to the database file</param>
        /// <param name="rootFolder">
        /// The folder in which the signed files are stored (this is the folder which the paths
        /// stored in <see cref="SignedFile.RelativePath"/> are relative to).  It is used to create the value
        /// for the <see cref="SignedFile.AbsolutePath"/> property</param>
        /// <param name="trustedIssuerVerifier">
        /// A <see cref="CertificateVerifier"/> delegate that will be invoked on each certificate in the trust chain for the
        /// certificate used to sign the document.  If the delegate returns true on at least one of these certificates, the
        /// signed certificate is accepted as being issued by a trusted file issuer
        /// </param>
        /// <remarks>
        /// <para>
        /// This override ignores any certificates embedded in the file, and instead uses the provided certificate to verify
        /// the signature of the file.  This can also be used if the file does not include a certificate.  If the provided certificate
        /// includes a private key (i.e. <see cref="X509Certificate2.PrivateKey"/> is not null), the database is opened for read-write
        /// access.  Otherwise, it is opened for read-only access.
        /// </para>
        /// <para>
        /// File Integrity Database files embed the certificate used to sign the file in the database itself.
        /// Thus, the certificate must be issued by a trusted root authority.  However, we are also interested in the
        /// intermediate certificates in the trust chain.  For example, when checking a database containing files issued by 
        /// DotNetNuke Corp., the consumer must be assured that the certificate used to sign the database is part of a trust chain 
        /// involving a certificate issued by DotNetNuke Corp.  This is the purpose of the 
        /// parameter.
        /// </para>
        /// <para>
        /// It is recommended that a database is signed by a "3rd-level" certificate.  This means that the certificate trust chain
        /// should look something like the following ('->' indicates the certificate on the left is the parent of the certificate on the
        /// right): Trusted Root -> Trusted File Issuer -> Signing Certificate.  The "Trusted Root" certificate is a certificate that is 
        /// located in the "Root" store of the local machine's certificate store.  The "Trusted File Issuer" is a certificate which is trusted
        /// by a Trusted Root Authority (i.e. it was issued by a trusted certificate authority such as Thawte, Equifax, VeriSign, etc.). The
        /// thumbprint (unique ID) of the Trusted File Issuer is passed in as the  parameter.  The 
        /// database will check up the trust chain (including the Signing Certificate itself) to find a certificate with the specified 
        /// thumbprint.  This allows new Signing Certificates to be issued without changing the code which verifies the certificate.  
        /// If a Signing Certificate is compromised, it can be revoked, and a new one issued without having to change the verification 
        /// code (though a new database, signed with the new certificate, will need to be issued).
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidFileIntegrityDatabaseException">
        /// The database file is not a valid signature store file
        /// </exception>
        /// <exception cref="SignatureVerificationException">
        /// The signature could not be verified OR the certificate used to verify the signature is not trusted by the system OR
        /// the certificate used to verify the signature is not trusted a certificate with the thumbprint .
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An error occurred loading the file.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="databasePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="certificate" /> parameter is null.
        /// </exception>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading the database file. Associated enumerations: <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>
        /// </permission>
        public FileIntegrityDatabase(X509Certificate2 certificate, string databasePath, string rootFolder, CertificateVerifier trustedIssuerVerifier) {
            ArgumentContract.NotNull(certificate, "certificate");
            ArgumentContract.StringNotNullOrEmpty(databasePath, "databasePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.NotNull(trustedIssuerVerifier, "trustedIssuerVerifier");
            LoadDatabase(certificate, databasePath, rootFolder, trustedIssuerVerifier);
        }

        /// <summary>
        /// Constructs an empty <see cref="FileIntegrityDatabase"/> object.
        /// </summary>
        /// <param name="certificate">
        /// An <see cref="X509Certificate2"/> object representing the certificate to use to sign the database.  The certificate
        /// must have a private key, since it will be used to sign the database
        /// </param>
        /// <param name="rootFolder">
        /// The folder in which the signed files are stored (this is the folder which the paths
        /// stored in <see cref="SignedFile.RelativePath"/> are relative to).  It is used to create the value
        /// for the <see cref="SignedFile.AbsolutePath"/> property</param>
        /// <remarks>
        /// <para>
        /// The provided certificate must be able to sign the database when saved, so it must include a private key (i.e
        /// <see cref="X509Certificate2.PrivateKey"/> property must not be null).  If it does not, a <see cref="SignatureGenerationException"/>
        /// will be thrown when the <see cref="Save"/> method is called.
        /// </para>
        /// <para>
        /// File Integrity Database files embed the certificate used to sign the file in the database itself.
        /// Thus, the certificate must be issued by a trusted root authority.  However, we are also interested in the
        /// intermediate certificates in the trust chain.  For example, when checking a database containing files issued by 
        /// DotNetNuke Corp., the consumer must be assured that the certificate used to sign the database is part of a trust chain 
        /// involving a certificate issued by DotNetNuke Corp.  This is the purpose of the 
        /// parameter.
        /// </para>
        /// <para>
        /// It is recommended that a database is signed by a "3rd-level" certificate.  This means that the certificate trust chain
        /// should look something like the following ('->' indicates the certificate on the left is the parent of the certificate on the
        /// right): Trusted Root -> Trusted File Issuer -> Signing Certificate.  The "Trusted Root" certificate is a certificate that is 
        /// located in the "Root" store of the local machine's certificate store.  The "Trusted File Issuer" is a certificate which is trusted
        /// by a Trusted Root Authority (i.e. it was issued by a trusted certificate authority such as Thawte, Equifax, VeriSign, etc.). The
        /// thumbprint (unique ID) of the Trusted File Issuer is passed in as the  parameter.  The 
        /// database will check up the trust chain (including the Signing Certificate itself) to find a certificate with the specified 
        /// thumbprint.  This allows new Signing Certificates to be issued without changing the code which verifies the certificate.  
        /// If a Signing Certificate is compromised, it can be revoked, and a new one issued without having to change the verification 
        /// code (though a new database, signed with the new certificate, will need to be issued).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The <paramref name="rootFolder" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="certificate" /> parameter is null.
        /// </exception>
        public FileIntegrityDatabase(X509Certificate2 certificate, string rootFolder) {
            ArgumentContract.NotNull(certificate, "certificate");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");

            Initialize(certificate, rootFolder);
        }

        /// <summary>
        /// Returns a list of <see cref="SignedFile"/> objects representing the files which are present in the database, 
        /// but do not exist on disk
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SignedFile}"/> object which can be used to enumerate over the missing files</returns>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public IEnumerable<SignedFile> FindMissingFiles() {
            VerifyNotClosed();
            return MapFilesWithState<SignedFile>(SignedFileState.Missing, SignedFileIdentityMapper);
        }

        /// <summary>
        /// Returns a list of <see cref="SignedFile"/> objects representing the files for which the signature store in the database
        /// does not match the hash calculated based on the current content of the file.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SignedFile}"/> object which can be used to enumerate over the modified files</returns>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public IEnumerable<SignedFile> FindModifiedFiles() {
            VerifyNotClosed();
            return MapFilesWithState<SignedFile>(SignedFileState.Modified, SignedFileIdentityMapper);
        }

        /// <summary>
        /// Returns a list of <see cref="SignedFile"/> objects representing the files for which the signature store in the database
        /// matches the hash calculated based on the current content of the file.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SignedFile}"/> object which can be used to enumerate over the unmodified files</returns>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public IEnumerable<SignedFile> FindUnmodifiedFiles() {
            VerifyNotClosed();
            return MapFilesWithState<SignedFile>(SignedFileState.Matched, SignedFileIdentityMapper);
        }

        /// <summary>
        /// Iterates over each file and if the file's <see cref="SignedFile.State"/> property matches the specified
        /// <paramref name="state" />, performs the specified action on it
        /// </summary>
        /// <param name="state">
        /// The state which each file's <see cref="SignedFile.State"/> property will be matched against to determine if
        /// the specified <paramref name="action"/> should be performed.
        /// </param>
        /// <param name="action">The action to perform on each file matching the specified <paramref name="state" /></param>
        public void ForEachFileWithState(SignedFileState state, Action<SignedFile> action) {
            VerifyNotClosed();
            foreach (SignedFile file in Files) {
                if (file.State == state) {
                    action(file);
                }
            }
        }

        /// <summary>
        /// Iterates over each file, maps the file to an instance of <typeparamref name="TTarget"/> and returns a list
        /// of all the mapped objects
        /// </summary>
        /// <param name="mapper">The mapping to perform on each file</param>
        public IEnumerable<TTarget> MapAllFiles<TTarget>(Mapper<SignedFile, TTarget> mapper) {
            List<TTarget> outputList = new List<TTarget>();
            foreach (SignedFile file in SignedFiles) {
                outputList.Add(mapper(file));
            }
            return outputList;
        }
        

        /// <summary>
        /// Iterates over each file and if the file's <see cref="SignedFile.State"/> property matches the specified
        /// <paramref name="state" />, maps the file to an instance of <typeparamref name="TTarget"/> and returns a list
        /// of all the mapped objects
        /// </summary>
        /// <param name="state">
        /// The state which each file's <see cref="SignedFile.State"/> property will be matched against to determine if
        /// the specified mapping should be performed.
        /// </param>
        /// <param name="mapper">The mapping to perform on each file matching the specified <paramref name="state" /></param>
        public IEnumerable<TTarget> MapFilesWithState<TTarget>(SignedFileState state, Mapper<SignedFile, TTarget> mapper) {
            List<TTarget> outputList = new List<TTarget>();
            ForEachFileWithState(state, delegate(SignedFile file) {
                outputList.Add(mapper(file));
            });
            return outputList;
        }
        
        
        /// <summary>
        /// Returns the database entry for the file at the specified relative path, or null if the file has no entry in the database
        /// </summary>
        /// <param name="relativePath">The relative path to the file to retrieve the entry for</param>
        /// <returns>
        /// A <see cref="SignedFile"/> object representing the file, or a null reference if there is no entry for the specified file
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public SignedFile FindFile(string relativePath) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            VerifyNotClosed();

            if (Files.Contains(relativePath)) {
                return Files[relativePath];
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the current state of the file with the specified relative path
        /// </summary>
        /// <param name="relativePath">The relative path of the file to check</param>
        /// <returns>
        /// A value from the <see cref="SignedFileState"/> enumeration indicating if the file is not signed,
        /// modified, missing, or successfully matched
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public SignedFileState GetFileState(string relativePath) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            VerifyNotClosed();

            if (!Files.Contains(relativePath)) {
                return SignedFileState.NotSigned;
            }

            return Files[relativePath].State;
        }

        /// <summary>
        /// Gets a boolean indicating if the database contains an entry for the specified file
        /// </summary>
        /// <param name="relativePath">The relative path to the file</param>
        /// <remarks>
        /// The file need not exist, the path is used only as a key into the database
        /// </remarks>
        /// <returns>True if there is an entry for the specified file in the database, false if not.</returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public bool ContainsEntryForFile(string relativePath) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            VerifyNotClosed();

            return Files.Contains(relativePath);
        }

        /// <summary>
        /// Adds the hash of a file to the database
        /// </summary>
        /// <remarks>
        /// The database must be open for read-write access (i.e. the <see cref="ReadOnly"/> property must be false).
        /// </remarks>
        /// <param name="relativePath">The relative path to the file</param>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="SignatureGenerationException">
        /// An error occurred hashing the file for inclusion in the database.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurred reading from the file
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public void AddFile(string relativePath) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            VerifyNotClosed();
            VerifyReadWrite();

            // Get the full path
            string fullPath = Path.Combine(_rootFolder, relativePath);

            // Create the default hash algorithm
            HashAlgorithm hasher = HashAlgorithms.CreateHashAlgorithm(DefaultHashAlgorithm);

            // Open the file and hash the contents
            byte[] hash = null;
            using (Stream strm = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                hash = hasher.ComputeHash(strm);
            }

            // Create the signed file entry
            SignedFile file = new SignedFile(relativePath, _rootFolder, hash, hasher);

            // Add it to the list
            Files.Add(file);
        }

        /// <summary>
        /// Adds the specified entry to the database
        /// </summary>
        /// <remarks>
        /// The database must be open for read-write access (i.e. the <see cref="ReadOnly"/> property must be false).
        /// </remarks>
        /// <param name="file">The <see cref="SignedFile"/> object to add to the database</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="file" /> parameter is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public void AddFile(SignedFile file) {
            ArgumentContract.NotNull(file, "file");
            VerifyNotClosed();
            VerifyReadWrite();
            file.RootFolder = _rootFolder;
            Files.Add(file);
        }

        /// <summary>
        /// Removes the specified entry from the database
        /// </summary>
        /// <param name="relativePath">The relative path to the file to retrieve a <see cref="SignedFile"/> entry for</param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public void RemoveFile(string relativePath) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            VerifyNotClosed();
            VerifyReadWrite();

            if (Files.Contains(relativePath)) {
                Files.Remove(relativePath);
            }
        }

        /// <summary>
        /// Saves the current list of signatures to the specified file.
        /// </summary>
        /// <remarks>
        /// In order to save the database, the <see cref="ReadOnly"/> property must be false.  This indicates that a
        /// private key is available in the certificate provided to the constructor.
        /// </remarks>
        /// <param name="databaseFile">The path to the database file to save</param>
        /// <param name="embedCertificate">A boolean indicating if the certificate should be embedded in the file</param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="databaseFile" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="SignatureGenerationException">
        /// There was an error signing the database file.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        /// <permission cref="System.Security.Permissions.FileIOPermission">
        /// for reading, writing and appending to the database file. Associated enumerations: 
        /// <see cref="System.Security.Permissions.FileIOPermissionAccess.Read"/>,
        /// <see cref="System.Security.Permissions.FileIOPermissionAccess.Write"/> and
        /// <see cref="System.Security.Permissions.FileIOPermissionAccess.Append"/>
        /// </permission>
        public void Save(string databaseFile, bool embedCertificate) {
            ArgumentContract.StringNotNullOrEmpty(databaseFile, "databaseFile");
            VerifyNotClosed();
            VerifyReadWrite();

            DatabaseSerializer.SaveFile(databaseFile, _files, _certificate, embedCertificate);
        }

        /// <summary>
        /// Saves the current list of signatures to the specified <see cref="XmlWriter"/> object.
        /// </summary>
        /// <remarks>
        /// In order to save the database, the <see cref="ReadOnly"/> property must be false.  This indicates that a
        /// private key is available in the certificate provided to the constructor.
        /// </remarks>
        /// <param name="writer">The <see cref="XmlWriter"/> object to write the file to</param>
        /// <param name="embedCertificate">A boolean indicating if the certificate should be embedded in the file</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer" /> parameter is null.
        /// </exception>
        /// <exception cref="SignatureGenerationException">
        /// There was an error signing the database file.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database is read-only.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The database has been closed.
        /// </exception>
        public void SaveToSignedXmlHashList(XmlWriter writer, bool embedCertificate) {
            VerifyNotClosed();
            VerifyReadWrite();
            
            SignedXmlHashListSerializer.SaveFile(writer, _files, _certificate, embedCertificate);
        }

        /// <summary>
        /// Closes the database, releasing any locks held on the database file
        /// </summary>
        public void Close() {
            _closed = true;
        }

        #region IDisposable Members

        /// <summary>
        /// Closes the database, releasing any locks held on the database file
        /// </summary>
        public void Dispose() {
            Close();
        }

        #endregion

        private void VerifyReadWrite() {
            if (ReadOnly) {
                throw new InvalidOperationException(Resources.Error_DatabaseIsReadOnly);
            }
        }

        private void VerifyNotClosed() {
            if (_closed) {
                throw new ObjectDisposedException("FileIntegrityDatabase");
            }
        }

        private void LoadDatabase(X509Certificate2 certificate, string databasePath, string rootFolder, CertificateVerifier trustedIssuerVerifier) {
            X509Certificate2 embeddedCert = DatabaseSerializer.LoadFile(databasePath, rootFolder, _files, certificate, trustedIssuerVerifier);
            
            if (certificate == null) {
                certificate = embeddedCert;
            }

            Initialize(certificate, rootFolder);
        }

        private void Initialize(X509Certificate2 certificate, string rootFolder) {
            _certificate = certificate;
            _rootFolder = rootFolder;
            if (_certificate == null || _certificate.PrivateKey == null) {
                _readOnly = true;
            }
        }
    }
}
