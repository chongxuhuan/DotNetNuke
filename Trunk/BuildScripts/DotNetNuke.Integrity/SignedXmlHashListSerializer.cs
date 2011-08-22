using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using DotNetNuke.Integrity.Properties;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml.Schema;
using System.IO;
using System.Reflection;
using System.Security;

namespace DotNetNuke.Integrity {
    internal class SignedXmlHashListSerializer : FileIntegrityDatabaseSerializer {
        internal const string Xmlns = "http://schemas.dotnetnuke.com/2008/12/xml-shl";

        private const string SignedXmlHashListSchemaResourceName = "DotNetNuke.Integrity.Schemas.SignedXmlHashListSchema.xsd";
        private const string XmlDigitalSignatureSchemaResourceName = "DotNetNuke.Integrity.Schemas.XmlDigitalSignatureSchema.xsd";

        private const string DsigXmlns = "http://www.w3.org/2000/09/xmldsig#";
        private const string FileIntegrityDatabaseElement = "FileIntegrityDatabase";
        private const string SignedFileElement = "SignedFile";
        private const string PathAttribute = "path";
        private const string AlgorithmAttribute = "algorithm";
        private const string SignatureElement = "Signature";
        private const string SignatureXPath = "//dsig:" + SignatureElement;
        private const string SignedFileXPath = "//shl:" + SignedFileElement;
        
        internal override X509Certificate2 LoadFile(string path, string rootFolder, SignedFileCollection fileCollection, X509Certificate2 providedCertificate, CertificateVerifier trustedIssuerVerifier) {
            ArgumentContract.StringNotNullOrEmpty(path, "path");
            ArgumentContract.StringNotNullOrEmpty(rootFolder, "rootFolder");
            ArgumentContract.NotNull(fileCollection, "fileCollection");
            ArgumentContract.NotNull(trustedIssuerVerifier, "trustedIssuerVerifier");

            // Load the XmlDocument
            XmlDocument document = new XmlDocument();
            try {
                document.Load(path);
            }
            catch (IOException ioex) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_IOErrorLoadingDatabase, ioex);
            }
            catch (XmlException xex) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_XmlErrorLoadingDatabase, xex);
            }
            catch (UnauthorizedAccessException uaex) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_UnauthorizedAccessErrorLoadingDatabase, uaex);
            }
            catch (SecurityException secex) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_SecurityErrorLoadingDatabase, secex);
            }

            // Verify that the file uses the right schema
            if (document.DocumentElement.NamespaceURI != Xmlns) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_FileIsNotDatabase);
            }

            // Verify the schema
            try {
                AddSchemaResource(document.Schemas, XmlDigitalSignatureSchemaResourceName);
                AddSchemaResource(document.Schemas, SignedXmlHashListSchemaResourceName);
                document.Validate(delegate(object sender, ValidationEventArgs args) { throw args.Exception; });
            }
            catch (XmlSchemaException ex) {
                throw new InvalidFileIntegrityDatabaseException(Resources.Error_ErrorValidatingSchema, ex);
            }

            // Verify the signature
            X509Certificate2 embeddedCertificate = VerifySignature(document, providedCertificate, trustedIssuerVerifier);

            // Read the document
            ReadDocument(document, rootFolder, fileCollection);

            return embeddedCertificate;
        }

        internal override void SaveFile(string path, SignedFileCollection fileCollection, X509Certificate2 certificate, bool embed) {
            ArgumentContract.StringNotNullOrEmpty(path, "path");
            XmlDocument document = BuildAndSignDocument(fileCollection, certificate, embed);

            // Save the signed document
            document.Save(path);
        }

        internal static void SaveFile(XmlWriter writer, SignedFileCollection fileCollection, X509Certificate2 certificate, bool embed) {
            ArgumentContract.NotNull(writer, "writer");
            XmlDocument document = BuildAndSignDocument(fileCollection, certificate, embed);

            // Save the signed document
            document.Save(writer);
        }

        private static XmlDocument BuildAndSignDocument(SignedFileCollection fileCollection, X509Certificate2 certificate, bool embed) {
            ArgumentContract.NotNull(fileCollection, "fileCollection");
            ArgumentContract.NotNull(certificate, "certificate");
            if (certificate.PrivateKey == null) {
                throw new SignatureGenerationException(Resources.Error_CertificateHasNoPrivateKey);
            }

            // Build the unsigned XmlDocument
            XmlDocument document = WriteXmlDocument(fileCollection);

            // Sign the document
            SignDocument(document, certificate, embed);
            return document;
        }

        private static void ReadDocument(XmlDocument document, string rootFolder, SignedFileCollection fileCollection) {
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("shl", Xmlns);
            foreach (XmlNode node in document.DocumentElement.SelectNodes(SignedFileXPath, nsMgr)) {
                XmlElement elem = node as XmlElement;
                if (elem != null || elem.NamespaceURI == Xmlns || elem.LocalName == SignedFileElement) {
                    string path = elem.GetAttribute("path");
                    string algorithm = elem.GetAttribute("algorithm");
                    if (String.IsNullOrEmpty(algorithm)) {
                        algorithm = "sha512";
                    }
                    byte[] hash = Convert.FromBase64String(elem.InnerText);
                    fileCollection.Add(new SignedFile(path, rootFolder, hash, algorithm));
                }
            }
        }

        private static X509Certificate2 VerifySignature(XmlDocument document, X509Certificate2 providedCertificate, CertificateVerifier trustedIssuerVerifier) {
            // Locate the Signature element
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("dsig", DsigXmlns);
            XmlElement signatureElem = (XmlElement)document.DocumentElement.SelectSingleNode(SignatureXPath, nsMgr);
            document.DocumentElement.RemoveChild(signatureElem);

            // Construct a signed xml document
            SignedXml signedXml = new SignedXml(document);
            signedXml.LoadXml(signatureElem);

            // Check for an embedded certificate
            X509Certificate2 embeddedCertificate = null;
            foreach (KeyInfoClause clause in signedXml.KeyInfo) {
                KeyInfoX509Data x509clause = clause as KeyInfoX509Data;
                if (x509clause != null) {
                    if (x509clause.Certificates.Count > 0) {
                        embeddedCertificate = x509clause.Certificates[0] as X509Certificate2;
                        //if (embeddedCertificate == null) {
                        //    //X509Certificate cert = x509clause.Certificates[0] as X509Certificate;
                        //    //if (cert != null) {
                        //    //    embeddedCertificate = new X509Certificate2(cert);
                        //    //    break;
                        //    //}
                        //    // Looks like the certificate is always an X509Certificate2 so I can't really test this code
                        //}
                        //else {
                        //    break;
                        //}
                        break;
                    }
                }
            }

            // Check that we can verify the signature
            X509Certificate2 verificationCertificate = providedCertificate;
            if (verificationCertificate == null) {
                if (embeddedCertificate == null) {
                    throw new SignatureVerificationException(Resources.Error_NoEmbeddedCertificate);
                }
                verificationCertificate = embeddedCertificate;
            }

            // Verify the certificate's root authority
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; // TODO: Investigate revocation modes (check certificate extensions?)
            if (!chain.Build(verificationCertificate)) {
                throw new SignatureVerificationException(Resources.Error_CertificateNotTrustedByRoot);
            }

            // Verify the trusted file issuer
            bool foundMatch = false;
            foreach (X509ChainElement chainElement in chain.ChainElements) {
                foundMatch |= trustedIssuerVerifier(chainElement.Certificate);
                if (foundMatch) {
                    break;
                }
            }
            if (!foundMatch) {
                throw new SignatureVerificationException(Resources.Error_CertificateNotTrustedAsFileIssuer);
            }

            // Verify the signature
            if (!signedXml.CheckSignature(verificationCertificate, true)) {
                throw new SignatureVerificationException(Resources.Error_SignatureCouldNotBeVerified);
            }

            // Return the embedded certificate
            return embeddedCertificate;
        }

        private static void AddSchemaResource(XmlSchemaSet schemas, string resourceName) {
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                if (strm != null) {
                    XmlSchema schema = XmlSchema.Read(strm, delegate(object sender, ValidationEventArgs args) { throw args.Exception; });
                    schemas.Add(schema);
                }
            }
        }

        private static void SignDocument(XmlDocument document, X509Certificate2 certificate, bool embed) {
            SignedXml signedXml = new SignedXml(document);

            if (embed) {
                // Embed the certificate as KeyInfo
                signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificate, X509IncludeOption.EndCertOnly));
            }

            // Set the signing key and the signature method
            signedXml.SigningKey = certificate.PrivateKey;
            if (signedXml.SigningKey is RSA) {
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            }
            else if (signedXml.SigningKey is DSA) {
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigDSAUrl;
            }
            else {
                throw new SignatureGenerationException(String.Format(CultureInfo.CurrentCulture,
                                                                     Resources.Error_CertificatePrivateKeyNotSupported, 
                                                                     signedXml.SigningKey.GetType().FullName));
            }

            // Create a reference for the xml document itself
            Reference reference = new Reference(String.Empty);

            // Use xmldsig canonicalization (c14n)
            reference.AddTransform(new XmlDsigC14NTransform());

            // Add the reference to the xml signature
            signedXml.AddReference(reference);

            // Compute the signature
            signedXml.ComputeSignature();

            // Generate the xmldsig Signature element
            XmlElement signatureElement = signedXml.GetXml();

            // Add it to the document
            document.DocumentElement.PrependChild(signatureElement);
        }

        private static XmlDocument WriteXmlDocument(SignedFileCollection fileCollection) {
            XmlDocument doc = new XmlDocument();
            using (XmlWriter writer = doc.CreateNavigator().AppendChild()) {
                writer.WriteStartDocument();
                writer.WriteStartElement(FileIntegrityDatabaseElement, Xmlns);

                foreach (SignedFile file in fileCollection) {
                    writer.WriteStartElement(SignedFileElement, Xmlns);
                    writer.WriteAttributeString(PathAttribute, file.RelativePath);
                    writer.WriteAttributeString(AlgorithmAttribute, HashAlgorithms.GetName(file.HashAlgorithm));
                    writer.WriteBase64(file.Hash, 0, file.Hash.Length);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return doc;
        }
    }
}
