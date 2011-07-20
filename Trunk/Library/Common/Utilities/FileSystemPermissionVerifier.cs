#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.IO;

using DotNetNuke.Common.Utilities.Internal;
using DotNetNuke.Instrumentation;

#endregion

namespace DotNetNuke.Common.Utilities
{
    /// <summary>
    ///   Verifies the abililty to create and delete files and folders
    /// </summary>
    /// <remarks>
    ///   This class is not meant for use in modules, or in any other manner outside the DotNetNuke core.
    /// </remarks>
    public class FileSystemPermissionVerifier
    {
        private readonly string _basePath;

        public FileSystemPermissionVerifier(string basePath)
        {
            _basePath = basePath;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   VerifyFileCreate checks whether a file can be created
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool VerifyFileCreate()
        {
            string verifyPath = Path.Combine(_basePath, "Verify\\Verify.txt");
            bool verified = VerifyFolderCreate();

            if (verified)
            {
                //Attempt to create the File
                try
                {
                    RetryableAction.RetryEverySecondFor30Seconds(() => FileCreateAction(verifyPath), "Creating verification file");
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    verified = false;
                }
            }

            return verified;
        }

        private static void FileCreateAction(string verifyPath)
        {
            if (File.Exists(verifyPath))
            {
                File.Delete(verifyPath);
            }

            using(File.Create(verifyPath))
            {
                //do nothing just let it close
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   VerifyFileDelete checks whether a file can be deleted
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool VerifyFileDelete()
        {
            string verifyPath = Path.Combine(_basePath, "Verify\\Verify.txt");
            bool verified = VerifyFileCreate();

            if (verified)
            {
                //Attempt to delete the File
                try
                {
                    RetryableAction.RetryEverySecondFor30Seconds(()=> File.Delete(verifyPath), "Deleting verification file");
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    verified = false;
                }
            }

            return verified;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   VerifyFolderCreate checks whether a folder can be created
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool VerifyFolderCreate()
        {
            string verifyPath = Path.Combine(_basePath, "Verify");
            bool verified = true;

            //Attempt to create the Directory
            try
            {
                RetryableAction.RetryEverySecondFor30Seconds(() => FolderCreateAction(verifyPath), "Creating verification folder");
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                verified = false;
            }

            return verified;
        }

        private static void FolderCreateAction(string verifyPath)
        {
            if (Directory.Exists(verifyPath))
            {
                Directory.Delete(verifyPath, true);
            }

            Directory.CreateDirectory(verifyPath);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   VerifyFolderDelete checks whether a folder can be deleted
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool VerifyFolderDelete()
        {
            string verifyPath = Path.Combine(_basePath, "Verify");
            bool verified = VerifyFolderCreate();

            if (verified)
            {
                //Attempt to delete the Directory
                try
                {
                    RetryableAction.RetryEverySecondFor30Seconds(() => Directory.Delete(verifyPath), "Deleting verification folder");
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    verified = false;
                }
            }

            return verified;
        }

        public bool VerifyAll()
        {
            return VerifyFileDelete() && VerifyFolderDelete();
        }
    }
}
