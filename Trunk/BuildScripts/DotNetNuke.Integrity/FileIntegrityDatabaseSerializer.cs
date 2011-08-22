using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace DotNetNuke.Integrity {
    internal abstract class FileIntegrityDatabaseSerializer {
        /// <summary>
        /// Loads the necessary information for constructing a <see cref="FileIntegrityDatabase"/> from the specified file
        /// </summary>
        /// <param name="path">The path to the file to load</param>
        /// <param name="rootFolder">The root folder containing the files signed in this database</param>
        /// <param name="fileCollection">A <see cref="SignedFileCollection"/> to populate with the data from the file</param>
        /// <param name="providedCertificate">The certificate provided to override any embedded certificate</param>
        /// <param name="trustedIssuerVerifier">
        /// A <see cref="CertificateVerifier"/> delegate that will be invoked on each certificate in the trust chain for the
        /// certificate used to sign the document.  If the delegate returns true on at least one of these certificates, the
        /// signed certificate is accepted as being issued by a trusted file issuer
        /// </param>
        /// <returns>The certificate embedded in the file, or null if there is no certificate embedded</returns>
        internal abstract X509Certificate2 LoadFile(string path, string rootFolder, SignedFileCollection fileCollection, X509Certificate2 providedCertificate, CertificateVerifier trustedIssuerVerifier);

        /// <summary>
        /// Saves the provided information to the specified file
        /// </summary>
        /// <param name="path">The path to the file to save</param>
        /// <param name="fileCollection">A <see cref="SignedFileCollection"/> to containing the entries to save</param>
        /// <param name="certificate">The certificate to use to sign the file</param>
        /// <param name="embed">A boolean indicating if the certificate should be embedded</param>
        internal abstract void SaveFile(string path, SignedFileCollection fileCollection, X509Certificate2 certificate, bool embed);
    }
}
