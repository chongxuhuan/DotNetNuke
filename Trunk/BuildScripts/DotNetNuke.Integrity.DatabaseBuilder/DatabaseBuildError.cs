using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public class DatabaseBuildError {
        private bool _warning;
        private string _message;
        private Exception _exception;

        public bool Warning {
            get { return _warning; }
        }

        public string Message {
            get { return _message; }
        }

        public Exception Exception {
            get { return _exception; }
        }

        public DatabaseBuildError(Exception ex) : this(false, ex) { }
        public DatabaseBuildError(bool warning, Exception ex) : this(warning, ex.Message, ex) { }
        public DatabaseBuildError(bool warning, string message, Exception exception) {
            _warning = warning;
            _message = message;
            _exception = exception;
        }
    }
}
