using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using DotNetNuke.Integrity.Properties;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Exception thrown when an error occurs trying to generate a signature for input data
    /// </summary>
    [Serializable]
    public class SignatureGenerationException : Exception {
        /// <summary>
        /// Constructs an instance of <see cref="SignatureGenerationException"/> class with the default message.
        /// </summary>
        public SignatureGenerationException() : base(Resources.Error_SignatureGenerationFailed) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureGenerationException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        public SignatureGenerationException(string message) : base(message) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureGenerationException"/> class with the specified message and
        /// inner exception.
        /// </summary>
        /// <param name="message">The message to associate with the exception</param>
        /// <param name="innerException">The exception which caused this error</param>
        public SignatureGenerationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructs an instance of <see cref="SignatureGenerationException"/> class with serialization data
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected SignatureGenerationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
