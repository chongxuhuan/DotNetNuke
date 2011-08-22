using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using DotNetNuke.Integrity.Properties;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Exception thrown when an error occurs trying to verify a signature for input data
    /// </summary>
    [Serializable]
    public class SignatureVerificationException : Exception {
        /// <summary>
        /// Constructs an instance of <see cref="SignatureVerificationException"/> class with the default message.
        /// </summary>
        public SignatureVerificationException() : base(Resources.Error_SignatureVerificationFailed) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureVerificationException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        public SignatureVerificationException(string message) : base(message) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureVerificationException"/> class with the specified message and
        /// inner exception.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        /// <param name="innerException">The exception which caused this error</param>
        public SignatureVerificationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureVerificationException"/> class with serialization data
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected SignatureVerificationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
