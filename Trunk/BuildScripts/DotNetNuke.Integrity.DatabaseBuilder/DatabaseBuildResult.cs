using System;
using System.Collections.Generic;
using System.Text;
using DotNetNuke.Integrity.DatabaseBuilder.Properties;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public class DatabaseBuildResult {
        private bool _succeeded;
        private List<DatabaseBuildError> _errors = new List<DatabaseBuildError>();
        private FileIntegrityDatabase _builtDb;

        public bool Succeeded {
            get { return _succeeded; }
            set { _succeeded = value; }
        }

        public List<DatabaseBuildError> Errors {
            get { return _errors; }
        }

        public FileIntegrityDatabase BuiltDb {
            get { return _builtDb; }
        }

        public DatabaseBuildResult() {
        }

        public void SetBuiltDb(FileIntegrityDatabase db) {
            if (_builtDb != null) {
                throw new InvalidOperationException(Resources.Error_CanOnlySetBuiltDbOnce);
            }
            _builtDb = db;
        }
    }
}
