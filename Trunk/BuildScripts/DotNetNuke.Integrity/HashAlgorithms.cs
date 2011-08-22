using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using DotNetNuke.Integrity.Properties;
using System.Globalization;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Static factory for constructing hash algorithms
    /// </summary>
    public static class HashAlgorithms {
        /// <summary>
        /// Constructs the hash algorithm with the specified name
        /// <seealso cref="HashAlgorithms.GetName"/>
        /// </summary>
        /// <param name="name">The name of the algorithm to construct</param>
        /// <returns>An instance of the requested hash algorithm</returns>
        /// <remarks>
        /// The following is a table of algorithms names and their corresponding <see cref="HashAlgorithm"/> implementations:
        /// <list type="table">
        /// <listheader>
        /// <term>Name</term>
        /// <description>Algorithm</description>
        /// </listheader>
        /// <item>
        /// <term>sha, sha1, sha-1</term>
        /// <description><see cref="SHA1Managed"/></description>
        /// </item>
        /// <item>
        /// <term>sha256, sha-256</term>
        /// <description><see cref="SHA256Managed"/></description>
        /// </item>
        /// <item>
        /// <term>sha384, sha-384</term>
        /// <description><see cref="SHA384Managed"/></description>
        /// </item>
        /// <item>
        /// <term>sha512, sha-512</term>
        /// <description><see cref="SHA512Managed"/></description>
        /// </item>
        /// <item>
        /// <term>ripemd160, ripemd-160</term>
        /// <description><see cref="RIPEMD160Managed"/></description>
        /// </item>
        /// <item>
        /// <term>md5</term>
        /// <description><see cref="MD5CryptoServiceProvider"/></description>
        /// </item>
        /// <item>
        /// <term>[Any Assembly-Qualified Type Name]</term>
        /// <description>
        /// An instance of that type, if it can be created and converted to an instance of <see cref="HashAlgorithm"/>
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name" /> parameter is null or an empty string.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The hash algorithm of the specified <paramref name="name"/> is not supported
        /// </exception>
        public static HashAlgorithm CreateHashAlgorithm(string name) {
            ArgumentContract.StringNotNullOrEmpty(name, "name");

            switch (name.ToUpperInvariant()) {
                case "SHA":
                case "SHA1":
                case "SHA-1":
                    return new SHA1Managed();
                case "SHA256":
                case "SHA-256":
                    return new SHA256Managed();
                case "SHA384":
                case "SHA-384":
                    return new SHA384Managed();
                case "SHA512":
                case "SHA-512":
                    return new SHA512Managed();
                case "RIPEMD160":
                case "RIPEMD-160":
                    return new RIPEMD160Managed();
                case "MD5":
                    return new MD5CryptoServiceProvider();
                default:
                    Type typ = Type.GetType(name);
                    if (typ == null) {
                        throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_HashAlgorithmNotSupported, name));
                    }
                    HashAlgorithm alg = null;
                    try {
                        alg = Activator.CreateInstance(typ) as HashAlgorithm;
                    }
                    catch (MissingMethodException ex) {
                        throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_HashAlgorithmNotSupported, name), ex);
                    }
                    if (alg == null) {
                        throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture, Resources.Error_HashAlgorithmNotSupported, name));
                    }
                    return alg;
            }
        }

        /// <summary>
        /// Converts an instance of a hash algorithm to its name.
        /// <seealso cref="HashAlgorithms.CreateHashAlgorithm"/>
        /// </summary>
        /// <param name="algorithm">An instance of <see cref="HashAlgorithm"/> to retrieve the name for</param>
        /// <returns>The name of the algorithm</returns>
        /// <remarks>
        /// <para>
        /// Note: In many cases, an algorithm, such as "SHA1" has multiple implementations, for example:
        /// <see cref="SHA1CryptoServiceProvider"/> and <see cref="SHA1Managed"/>.  All of these (or, in fact,
        /// any class inheriting from the appropriate base class, <see cref="SHA1"/> for example, will be converted to the appropriate name).
        /// </para>
        /// <para>
        /// If an algorithm is not in the predefined list of algorithms (see <see cref="HashAlgorithms.CreateHashAlgorithm" />), the
        /// Assembly-Qualified Type Name (see <see cref="Type.AssemblyQualifiedName"/>) of the instance is returned.
        /// </para>
        /// </remarks>
        public static string GetName(HashAlgorithm algorithm) {
            if(algorithm is SHA1) {
                return "sha1";
            } else if(algorithm is SHA256) {
                return "sha256";
            } else if(algorithm is SHA384) {
                return "sha384";
            } else if(algorithm is SHA512) {
                return "sha512";
            } else if(algorithm is RIPEMD160) {
                return "ripemd160";
            } else if(algorithm is MD5) {
                return "md5";
            }
            return algorithm.GetType().AssemblyQualifiedName;
        }
    }
}
