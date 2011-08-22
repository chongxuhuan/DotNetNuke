using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DotNetNuke.Integrity.DatabaseBuilder.Properties;
using System.Diagnostics;
using System.Globalization;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public static class DatabaseBuilder {
        private delegate bool Filterer(FileInfo file);

        public static DatabaseBuildResult BuildDatabase(string outputFile,
                                                        bool embedCertificate,
                                                        string rootFolder,
                                                        IEnumerable<FileFilter> excludeFilters,
                                                        IEnumerable<FileFilter> reincludeFilters,
                                                        X509Certificate2 signingCertificate) {
            return BuildDatabase(outputFile, embedCertificate, rootFolder, excludeFilters, reincludeFilters, signingCertificate, new SilentProgressReporter());
        }

        public static DatabaseBuildResult BuildDatabase(string outputFile,
                                                        bool embedCertificate,
                                                        string rootFolder,
                                                        IEnumerable<FileFilter> excludeFilters,
                                                        IEnumerable<FileFilter> reincludeFilters,
                                                        X509Certificate2 signingCertificate,
                                                        ProgressReporter reporter) {
            DatabaseBuildResult result = InternalBuildDatabase(rootFolder, excludeFilters, reincludeFilters, signingCertificate, reporter, false);
            if (result.Succeeded) {
                try {
                    result.BuiltDb.Save(outputFile, embedCertificate);
                }
                catch (SignatureGenerationException ex) {
                    RaiseError(result, reporter, false, null, ex);
                }
            }
            reporter.NotifyFinishedBuilding(result);
            return result;
        }

        public static DatabaseBuildResult BuildDatabase(string rootFolderPath,
                                                        IEnumerable<FileFilter> excludeFilters,
                                                        IEnumerable<FileFilter> reincludeFilters,
                                                        X509Certificate2 signingCertificate) {
            return InternalBuildDatabase(rootFolderPath, excludeFilters, reincludeFilters, signingCertificate, new SilentProgressReporter(), true);
        }

        public static DatabaseBuildResult BuildDatabase(string rootFolderPath,
                                                        IEnumerable<FileFilter> excludeFilters,
                                                        IEnumerable<FileFilter> reincludeFilters,
                                                        X509Certificate2 signingCertificate,
                                                        ProgressReporter reporter) {
            return InternalBuildDatabase(rootFolderPath, excludeFilters, reincludeFilters, signingCertificate, reporter, true);
        }

        private static DatabaseBuildResult InternalBuildDatabase(string rootFolderPath,
                                                                 IEnumerable<FileFilter> excludeFilters,
                                                                 IEnumerable<FileFilter> reincludeFilters, 
                                                                 X509Certificate2 signingCertificate,
                                                                 ProgressReporter reporter,
                                                                 bool notifyFinished) {
        

            DatabaseBuildResult result = new DatabaseBuildResult();
            result.Succeeded = true;

            if (!signingCertificate.HasPrivateKey) {
                RaiseError(result, reporter, false, null, new SignatureGenerationException(Resources.Error_NoPrivateKey));
                // Can continue, just can't sign
            }
            
            DirectoryInfo rootFolder = new DirectoryInfo(rootFolderPath);
            if (!rootFolder.Exists) {
                RaiseError(result, reporter, false, null, new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, Resources.Error_RootFolderDoesNotExist, rootFolderPath)));
                return result; // Can't continue after this error
            }
            
            reporter.NotifyStartBuilding(rootFolder);

            FileIntegrityDatabase db = new FileIntegrityDatabase(signingCertificate, rootFolder.FullName);
            
            // Create "filterer"
            Filterer filterer = new Filterer(delegate(FileInfo file) {
                bool excluded = false;
                // Process excludes
                foreach(FileFilter excludeFilter in excludeFilters) {
                    if(excludeFilter.Matches(file)) {
                        excluded = true;
                        break;
                    }
                }
                
                // Give excluded files a second chance, using the reinclude filters
                if(excluded) {
                    foreach(FileFilter reincludeFilter in reincludeFilters) {
                        if(reincludeFilter.Matches(file)) {
                            excluded = false;
                            break;
                        }
                    }
                }
                return !excluded;
            });

            // Run recursively through the contents
            ProcessDirectory(result, db, filterer, rootFolder, reporter);

            if(result.Succeeded) {
                result.SetBuiltDb(db);
            }

            if (notifyFinished) {
                reporter.NotifyFinishedBuilding(result);
            }

            return result;
        }

        private static void RaiseError(DatabaseBuildResult result, ProgressReporter reporter, bool warning, FileInfo currentFile, Exception ex) {
            DatabaseBuildError error = new DatabaseBuildError(warning, ex);
            result.Errors.Add(error);
            reporter.NotifyError(error, currentFile);
            if(!warning) {
                result.Succeeded = false;
            }
        }

        private static void ProcessDirectory(DatabaseBuildResult result, FileIntegrityDatabase db, Filterer filterer, DirectoryInfo directory, ProgressReporter reporter) {
            reporter.NotifyEnterDirectory(directory);

            foreach (FileSystemInfo directoryEntry in directory.GetFileSystemInfos()) {
                DirectoryInfo dir = directoryEntry as DirectoryInfo;
                if (dir != null) {
                    ProcessDirectory(result, db, filterer, dir, reporter);
                }
                FileInfo file = directoryEntry as FileInfo;
                if (file != null) {
                    ProcessFile(result, db, filterer, file, reporter);
                }
            }

            reporter.NotifyExitDirectory(directory);
        }

        private static void ProcessFile(DatabaseBuildResult result, FileIntegrityDatabase db, Filterer filterer, FileInfo file, ProgressReporter reporter) {
            // Check if the file has been filtered out
            if (!filterer(file)) {
                reporter.NotifyFileExcluded(file);
                return;
            }

            reporter.NotifyProcessingFile(file);

            // Add the file to the database
            string relativePath = file.FullName.Substring(db.RootFolder.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Verify the relative path
            Debug.Assert(String.Equals(file.FullName, Path.Combine(db.RootFolder, relativePath)));

            // Add the file
            try {
                db.AddFile(relativePath);
            }
            catch (SignatureGenerationException ex) {
                RaiseError(result, reporter, false, file, ex);
            }
            catch (IOException ex) {
                RaiseError(result, reporter, false, file, ex);
            }

            reporter.NotifyProcessedFile(file);
        }
    }
}
