using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using CryptoHashAlgorithm = System.Security.Cryptography.HashAlgorithm;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Represents an entry in a <see cref="FileIntegrityDatabase"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="SignedFile"/> object contains the information actually saved in the database, represented
    /// by the <see cref="RelativePath"/>, <see cref="Hash"/> and <see cref="HashAlgorithm"/> properties as well
    /// as helpers to wrap common operations.
    /// </para>
    /// <para>
    /// Despite the name, the <see cref="SignedFile"/> class can be used to represent other signed objects.  In this case,
    /// the <see cref="RelativePath" /> property represents the key used to identify the object in the <see cref="FileIntegrityDatabase"/>.
    /// If a <see cref="SignedFile"/> object is used to represent an "non-file" object, the <see cref="Exists"/> property may through
    /// an exception due to the fact that the <see cref="RelativePath"/> is not a valid file path.
    /// </para>
    /// </remarks>
    public class SignedFile {
        private string _relativePath;
        private string _rootFolder;
        private byte[] _hash;
        private CryptoHashAlgorithm _hashAlgorithm;
        
        /// <summary>
        /// Gets the relative path to the file in the application
        /// </summary>
        public string RelativePath {
            get { return _relativePath; }
        }

        /// <summary>
        /// Gets the hash of the signed file
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification="Hash values are generally passed around as byte arrays, since collections classes incur too much overhead for binary data")]
        public byte[] Hash {
            get { return _hash; }
        }

        /// <summary>
        /// Gets the algorithm used to calculate the hash of the signed file.
        /// </summary>
        /// <remarks>
        /// This algorithm must be used to generate hashes that can be compared against the value stored in <see cref="Hash"/>
        /// </remarks>
        public CryptoHashAlgorithm HashAlgorithm {
            get { return _hashAlgorithm; }
        }

        internal string RootFolder {
            get { return _rootFolder; }
            set { _rootFolder = value; }
        }

        /// <summary>
        /// Gets the absolute path to the file in the application, based on the base folder provided
        /// to the <see cref="FileIntegrityDatabase"/> which created this object.
        /// </summary>
        public string AbsolutePath {
            get {
                return Path.Combine(_rootFolder, _relativePath);
            }
        }

        /// <summary>
        /// Gets a boolean indicating if the file exists in the application
        /// </summary>
        public bool Exists {
            get { return File.Exists(AbsolutePath); } 
        }

        /// <summary>
        /// Gets the current state of the file
        /// </summary>
        /// <value>A value from the <see cref="SignedFileState"/> enumeration indicating the state of the file</value>
        public SignedFileState State {
            get {
                if (!Exists) {
                    return SignedFileState.Missing;
                }
                else if (!VerifyData()) {
                    return SignedFileState.Modified;
                }
                else {
                    return SignedFileState.Matched;
                }
            }
        }

        /// <summary>
        /// Constructs an empty <see cref="SignedFile"/> object
        /// </summary>
        internal SignedFile() {
        }

        /// <summary>
        /// Constructs a new <see cref="SignedFile"/> object
        /// </summary>
        /// <param name="relativePath">The relative path of the file</param>
        /// <param name="rootFolder">The base path which <paramref name="relativePath"/> is relative to</param>
        /// <param name="hash">The stored hash value for the file</param>
        /// <param name="hashAlgorithmName">
        /// The name of the hash algorithm which should be used to calculate hashes
        /// to compare against the stored hash
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The hash algorithm specified by <paramref name="hashAlgorithmName"/> is not supported.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="hash" /> parameter is null. 
        /// </exception>
        public SignedFile(string relativePath, string rootFolder, byte[] hash, string hashAlgorithmName) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.NotNull(hash, "hash");
            ArgumentContract.StringNotNullOrEmpty(hashAlgorithmName, "hashAlgorithmName");

            Initialize(relativePath, rootFolder, hash, HashAlgorithms.CreateHashAlgorithm(hashAlgorithmName));
        }
        
        /// <summary>
        /// Constructs a new <see cref="SignedFile"/> object
        /// </summary>
        /// <param name="relativePath">The relative path of the file</param>
        /// <param name="rootFolder">The base path which <paramref name="relativePath"/> is relative to</param>
        /// <param name="hash">The stored hash value for the file</param>
        /// <param name="hashAlgorithm">
        /// The <see cref="System.Security.Cryptography.HashAlgorithm"/> which should be used to calculate hashes
        /// to compare against the stored hash
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="relativePath" /> parameter is null or an empty string. Or,
        /// the <paramref name="rootFolder" /> parameter is null or an empty string. Or,
        /// the  parameter is null or an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="hash" /> parameter is null. 
        /// </exception>
        public SignedFile(string relativePath, string rootFolder, byte[] hash, CryptoHashAlgorithm hashAlgorithm) {
            ArgumentContract.StringNotNullOrEmpty(relativePath, "relativePath");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.NotNull(hash, "hash");
            ArgumentContract.NotNull(hashAlgorithm, "hashAlgorithm");

            Initialize(relativePath, rootFolder, hash, hashAlgorithm);
        }

        private void Initialize(string relativePath, string rootFolder, byte[] hash, CryptoHashAlgorithm hashAlgorithm) {
            _relativePath = relativePath;
            _rootFolder = rootFolder;
            _hash = hash;
            _hashAlgorithm = hashAlgorithm;
        }

        /// <summary>
        /// Checks if the file's contents match the value in the database
        /// </summary>
        /// <returns>A boolean indicating if the file's contents matches the hash stored in the database</returns>
        public bool VerifyData() {
            // Open the file content verify it
            using (Stream strm = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                return VerifyData(strm);
            }
        }

        /// <summary>
        /// Checks if the specified data matches the hash stored in the database
        /// </summary>
        /// <param name="inputData">A <see cref="Stream"/> which can be used to read the data to be compared to the hash</param>
        /// <returns>A boolean indicating if the data matches the hash stored in the database</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="inputData" /> parameter is null. 
        /// </exception>
        public bool VerifyData(Stream inputData) {
            ArgumentContract.NotNull(inputData, "inputData");
            return VerifyHash(HashAlgorithm.ComputeHash(inputData));
        }

        /// <summary>
        /// Checks if the specified data matches the hash stored in the database
        /// </summary>
        /// <param name="inputData">The data to be compared to the hash</param>
        /// <returns>A boolean indicating if the data matches the hash stored in the database</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="inputData" /> parameter is null. 
        /// </exception>
        public bool VerifyData(byte[] inputData) {
            ArgumentContract.NotNull(inputData, "inputData");
            return VerifyHash(HashAlgorithm.ComputeHash(inputData));
        }

        /// <summary>
        /// Checks if the specified hash matches the hash stored in the database
        /// </summary>
        /// <param name="computedHash">The hash to be compared against the stored value</param>
        /// <returns>A boolean indicating if the hash to be compared against the stored value</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="computedHash" /> parameter is null. 
        /// </exception>
        public bool VerifyHash(byte[] computedHash) {
            ArgumentContract.NotNull(computedHash, "computedHash");
            if (computedHash.Length != Hash.Length) {
                return false;
            }

            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != Hash[i]) {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Collection of <see cref="SignedFile"/> objects keyed by the <see cref="SignedFile.RelativePath"/> property
    /// </summary>
    public class SignedFileCollection : KeyedCollection<string, SignedFile> {
        /// <summary>
        /// Retrieves the key which maps to the specified <see cref="SignedFile"/> object.
        /// </summary>
        /// <param name="item">The <see cref="SignedFile"/> objects to retrieve the key for</param>
        /// <returns>The key for the <see cref="SignedFile"/> object</returns>
        protected override string GetKeyForItem(SignedFile item) {
            ArgumentContract.NotNull(item, "item");
            return item.RelativePath;
        }
    }
}
