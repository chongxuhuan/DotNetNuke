using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetNuke.Integrity {
    /// <summary>
    /// Represents the state of a signed file on the file system, relative to a particular <see cref="FileIntegrityDatabase"/>
    /// </summary>
    public enum SignedFileState {
        /// <summary>
        /// The <see cref="FileIntegrityDatabase"/> does not contain a <see cref="SignedFile"/> entry for the specified file. 
        /// This value does not imply that the file is (or is not) present on disk
        /// </summary>
        NotSigned,
        /// <summary>
        /// The specified file's current contents do not match the hash value stored in the <see cref="FileIntegrityDatabase"/>
        /// </summary>
        Modified,
        /// <summary>
        /// The specified file has a <see cref="SignedFile"/> entry in the <see cref="FileIntegrityDatabase"/> but does not exist on disk
        /// </summary>
        Missing,
        /// <summary>
        /// The specified file's current contents match the hash value stored in the <see cref="FileIntegrityDatabase"/>
        /// </summary>
        Matched
    }
}
