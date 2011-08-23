using NUnit.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    [TestFixture]
    class BuildSignatureDatabaseFixture
    {
        [Test]
        public void ExtensionPackager_Loads_Manifest()
        {
            var buildSignatureDatabase = new BuildSignatureDatabase()
                                        { 
                                            CertificateFile = @"X:\DotNetNuke\TFS\DNNSigning\trunk\DNNSigning\DNNSigner\dotnetnukecore.pfx",
                                            CertificatePassword = "dnn",
                                            ExcludePatterns = @".*\.pdb;App_Data\\.*\.?df;App_Data\\.*\.?DF;web\.config;(?i-msnx:siteurls\.config);Portals\\0\\.*;signatures\\.*\.signatures;Config\\Backup_.*",
                                            InputDirectory = @"Z:\DotNetNuke\Releases\EE\060002254\Install",
                                            OutputFile = @"z:\dotnetnukecore.signatures"
                                        };

            Assert.IsTrue(buildSignatureDatabase.Execute());
        }

    }
}
