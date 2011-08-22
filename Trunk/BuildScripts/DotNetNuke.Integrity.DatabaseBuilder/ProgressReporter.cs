using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using DotNetNuke.Integrity.DatabaseBuilder.Properties;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public abstract class ProgressReporter {
        public virtual void NotifyStartBuilding(DirectoryInfo rootDirectory) {
        }

        public virtual void NotifyEnterDirectory(DirectoryInfo directory) {
        }

        public virtual void NotifyProcessingFile(FileInfo file) {
        }

        public virtual void NotifyProcessedFile(FileInfo file) {
        }

        public virtual void NotifyExitDirectory(DirectoryInfo directory) {
        }

        public virtual void NotifyFinishedBuilding(DatabaseBuildResult result) {
        }

        public virtual void NotifyError(DatabaseBuildError error, FileInfo file) {
        }

        public virtual void NotifyFileExcluded(FileInfo file) {
        }

        protected string BuildDefaultStartBuildingMessage(DirectoryInfo rootDirectory) {
            return String.Format(Resources.Message_StartBuilding, rootDirectory.FullName);
        }

        protected string BuildDefaultFinishedBuildingMessage(DatabaseBuildResult result) {
            // Count errors and warnings
            int errors = 0;
            int warnings = 0;
            foreach (DatabaseBuildError error in result.Errors) {
                if (error.Warning) {
                    warnings++;
                }
                else {
                    errors++;
                }
            }

            return String.Format(Resources.Message_FinishedBuilding, result.Succeeded ? Resources.Message_Succeeded : Resources.Message_Failed, errors, warnings);
        }

        protected string BuildDefaultEnterDirectoryMessage(DirectoryInfo directory) {
            return String.Format(Resources.Message_EnteringDirectory, directory.Name);
        }

        protected string BuildDefaultExitDirectoryMessage(DirectoryInfo directory) {
            return String.Format(Resources.Message_ExitingDirectory, directory.Name);
        }

        protected string BuildDefaultProcessingFileMessage(FileInfo file) {
            return String.Format(Resources.Message_ProcessingFile, file.Name);
        }

        //protected string BuildDefaultProcessedFileMessage(FileInfo file) {
        //}

        protected string BuildDefaultErrorMessage(DatabaseBuildError error, FileInfo file) {
            string fileReference = String.Empty;
            if (file != null) {
                fileReference = String.Format(CultureInfo.CurrentCulture, Resources.Message_ErrorFileReference, file.Name);
            }
            string prefix = error.Warning ? Resources.Message_WarningPrefix : Resources.Message_ErrorPrefix;
            return String.Format(Resources.Message_Error, prefix, fileReference, error.Message);
        }

        protected string BuildDefaultFileExcludedMessage(FileInfo file) {
            return String.Format(Resources.Message_SkippingFile, file.Name);
        }
    }

    public sealed class SilentProgressReporter : ProgressReporter { }
}
