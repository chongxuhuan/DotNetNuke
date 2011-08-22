using System;
using System.Collections.Generic;
using System.Text;
using DotNetNuke.Integrity.DatabaseBuilder.Properties;
using System.IO;
using System.Globalization;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public class TextWriterProgressReporter : ProgressReporter {
        private TextWriter _outputWriter;

        public TextWriter OutputWriter {
            get { return _outputWriter; }
        }

        public TextWriterProgressReporter(TextWriter outputWriter) {
            _outputWriter = outputWriter;
        }

        public override void NotifyEnterDirectory(DirectoryInfo directory) {
            OutputWriter.WriteLine(BuildDefaultEnterDirectoryMessage(directory));
        }

        public override void NotifyExitDirectory(DirectoryInfo directory) {
            OutputWriter.WriteLine(BuildDefaultExitDirectoryMessage(directory));
        }

        public override void NotifyProcessingFile(FileInfo file) {
            OutputWriter.WriteLine(BuildDefaultProcessingFileMessage(file));
        }

        public override void NotifyProcessedFile(FileInfo file) {
            base.NotifyProcessedFile(file);
        }

        public override void NotifyError(DatabaseBuildError error, FileInfo file) {
            OutputWriter.WriteLine(BuildDefaultErrorMessage(error, file));
        }

        public override void NotifyStartBuilding(DirectoryInfo rootDirectory) {
            OutputWriter.WriteLine(BuildDefaultStartBuildingMessage(rootDirectory));
        }

        public override void NotifyFinishedBuilding(DatabaseBuildResult result) {
            OutputWriter.WriteLine(BuildDefaultFinishedBuildingMessage(result));
        }

        public override void NotifyFileExcluded(FileInfo file) {
            OutputWriter.WriteLine(BuildDefaultFileExcludedMessage(file));
        }
    }
}
