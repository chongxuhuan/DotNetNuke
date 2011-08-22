using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using DotNetNuke.Integrity.Properties;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Exception thrown when a signature store file is invalid
    /// </summary>
    [Serializable]
    public class InvalidFileIntegrityDatabaseException : Exception {
        /// <summary>
        /// Constructs an instance of <see cref="InvalidFileIntegrityDatabaseException"/> class with the default message.
        /// </summary>
        public InvalidFileIntegrityDatabaseException() : base(Resources.Error_InvalidDatabaseFile) { }

        /// <summary>
        /// Constructs an instance of <see cref="InvalidFileIntegrityDatabaseException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        public InvalidFileIntegrityDatabaseException(string message) : base(message) { }

        /// <summary>
        /// Constructs an instance of <see cref="InvalidFileIntegrityDatabaseException"/> class with the specified message and
        /// inner exception.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        /// <param name="innerException">The exception which caused this error</param>
        public InvalidFileIntegrityDatabaseException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructs an instance of <see cref="InvalidFileIntegrityDatabaseException"/> class with serialization data
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvalidFileIntegrityDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
