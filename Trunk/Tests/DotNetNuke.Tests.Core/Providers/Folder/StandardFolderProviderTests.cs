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

using System;
using System.IO;
using System.Linq;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

namespace DotNetNuke.Tests.Core.Providers.Folder
{
    [TestFixture]
    public class StandardFolderProviderTests
    {
        #region Private Variables

        private StandardFolderProvider _sfp;
        private Mock<IFolderInfo> _folderInfo;
        private Mock<IFileInfo> _fileInfo;
        private Mock<IFile> _fileWrapper;
        private Mock<IDirectory> _directoryWrapper;
        private Mock<IFolderManager> _folderManager;
        private Mock<IFileManager> _fileManager;
        private Mock<IPathUtils> _pathUtils;

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _sfp = new StandardFolderProvider();
            _folderInfo = new Mock<IFolderInfo>();
            _fileInfo = new Mock<IFileInfo>();
            _fileWrapper = new Mock<IFile>();
            _directoryWrapper = new Mock<IDirectory>();
            _folderManager = new Mock<IFolderManager>();
            _fileManager = new Mock<IFileManager>();
            _pathUtils = new Mock<IPathUtils>();

            FileWrapper.RegisterInstance(_fileWrapper.Object);
            DirectoryWrapper.RegisterInstance(_directoryWrapper.Object);
            FolderManager.RegisterInstance(_folderManager.Object);
            FileManager.RegisterInstance(_fileManager.Object);
            PathUtils.RegisterInstance(_pathUtils.Object);
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }

        #endregion

        #region AddFile

        [Test]
        [ExpectedArgumentException]
        public void AddFile_Throws_On_Null_Folder()
        {
            var stream = new Mock<Stream>();

            _sfp.AddFile(null, Constants.FOLDER_ValidFileName, stream.Object);
        }

        [Test]
        [Row(null)]
        [Row("")]
        [ExpectedArgumentException]
        public void AddFile_Throws_On_NullOrEmpty_FileName(string fileName)
        {
            var stream = new Mock<Stream>();

            _sfp.AddFile(_folderInfo.Object, fileName, stream.Object);
        }

        [Test]
        [ExpectedArgumentException]
        public void AddFile_Throws_On_Null_Content()
        {
            _sfp.AddFile(_folderInfo.Object, Constants.FOLDER_ValidFileName, null);
        }

        #endregion

        #region DeleteFile

        [Test]
        [ExpectedArgumentException]
        public void DeleteFile_Throws_On_Null_File()
        {
            _sfp.DeleteFile(null);
        }

        [Test]
        public void DeleteFile_Calls_FileWrapper_Delete_When_File_Exists()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(true);

            _sfp.DeleteFile(_fileInfo.Object);

            _fileWrapper.Verify(fw => fw.Delete(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void DeleteFile_Does_Not_Call_FileWrapper_Delete_When_File_Does_Not_Exist()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(false);

            _sfp.DeleteFile(_fileInfo.Object);

            _fileWrapper.Verify(fw => fw.Delete(Constants.FOLDER_ValidFilePath), Times.Never());
        }

        #endregion

        #region ExistsFile

        [Test]
        [ExpectedArgumentException]
        public void ExistsFile_Throws_On_Null_Folder()
        {
            _sfp.ExistsFile(null, Constants.FOLDER_ValidFileName);
        }

        [Test]
        [ExpectedArgumentException]
        public void ExistsFile_Throws_On_Null_FileName()
        {
            _sfp.ExistsFile(_folderInfo.Object, null);
        }

        [Test]
        public void ExistsFile_Calls_FileWrapper_Exists()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _sfp.ExistsFile(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            _fileWrapper.Verify(fw => fw.Exists(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void ExistsFile_Returns_True_When_File_Exists()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(true);

            var result = _sfp.ExistsFile(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            Assert.IsTrue(result);
        }

        [Test]
        public void ExistsFile_Returns_False_When_File_Does_Not_Exist()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(false);

            var result = _sfp.ExistsFile(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            Assert.IsFalse(result);
        }

        #endregion

        #region FolderExists

        [Test]
        [ExpectedArgumentException]
        public void ExistsFolder_Throws_On_Null_FolderMapping()
        {
            _sfp.ExistsFolder(Constants.FOLDER_ValidFolderPath, null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ExistsFolder_Throws_On_Null_FolderPath()
        {
            var folderMapping = new FolderMappingInfo();

            _sfp.ExistsFolder(null, folderMapping);
        }

        [Test]
        public void ExistsFolder_Calls_DirectoryWrapper_Exists()
        {
            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(folderMapping.PortalID, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);

            _sfp.ExistsFolder(Constants.FOLDER_ValidFolderRelativePath, folderMapping);

            _directoryWrapper.Verify(dw => dw.Exists(Constants.FOLDER_ValidFolderPath), Times.Once());
        }

        [Test]
        public void ExistsFolder_Returns_True_When_Folder_Exists()
        {
            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);

            _directoryWrapper.Setup(dw => dw.Exists(Constants.FOLDER_ValidFolderPath)).Returns(true);

            var result = _sfp.ExistsFolder(Constants.FOLDER_ValidFolderRelativePath, folderMapping);

            Assert.IsTrue(result);
        }

        [Test]
        public void ExistsFolder_Returns_False_When_Folder_Does_Not_Exist()
        {
            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);

            _directoryWrapper.Setup(dw => dw.Exists(Constants.FOLDER_ValidFolderPath)).Returns(false);

            var result = _sfp.ExistsFolder(Constants.FOLDER_ValidFolderRelativePath, folderMapping);

            Assert.IsFalse(result);
        }

        #endregion

        #region GetFile

        [Test]
        [ExpectedArgumentException]
        public void GetFile_Throws_On_Null_File()
        {
            _sfp.GetFile(null);
        }

        [Test]
        public void GetFile_Calls_FileWrapper_ReadAllBytes()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _sfp.GetFile(_fileInfo.Object);

            _fileWrapper.Verify(fw => fw.ReadAllBytes(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void GetFile_Returns_The_Content_Of_The_File_When_File_Exists()
        {
            var validFileBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.ReadAllBytes(Constants.FOLDER_ValidFilePath)).Returns(validFileBytes);

            var result = _sfp.GetFile(_fileInfo.Object);

            Assert.AreEqual<byte[]>(validFileBytes, result);
        }

        [Test]
        public void GetFile_Returns_Null_When_File_Does_Not_Exist()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.ReadAllBytes(Constants.FOLDER_ValidFilePath)).Throws<FileNotFoundException>();

            var result = _sfp.GetFile(_fileInfo.Object);

            Assert.IsNull(result);
        }

        #endregion

        #region GetFileAttributes

        [Test]
        [ExpectedArgumentException]
        public void GetFileAttributes_Throws_On_Null_File()
        {
            _sfp.GetFileAttributes(null);
        }

        [Test]
        public void GetFileAttributes_Calls_FileWrapper_GetAttributes()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _sfp.GetFileAttributes(_fileInfo.Object);

            _fileWrapper.Verify(fw => fw.GetAttributes(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void GetFileAttributes_Returns_File_Attributes_When_File_Exists()
        {
            var expectedFileAttributes = FileAttributes.Normal;

            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.GetAttributes(Constants.FOLDER_ValidFilePath)).Returns(expectedFileAttributes);

            var result = _sfp.GetFileAttributes(_fileInfo.Object);

            Assert.AreEqual<FileAttributes?>(expectedFileAttributes, result);
        }

        [Test]
        public void GetFileAttributes_Returns_Null_When_File_Does_Not_Exist()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.GetAttributes(Constants.FOLDER_ValidFilePath)).Throws<FileNotFoundException>();

            var result = _sfp.GetFileAttributes(_fileInfo.Object);

            Assert.IsNull(result);
        }

        #endregion

        #region GetFiles

        [Test]
        [ExpectedArgumentException]
        public void GetFiles_Throws_On_Null_Folder()
        {
            _sfp.GetFiles(null);
        }

        [Test]
        public void GetFiles_Calls_DirectoryWrapper_GetFiles()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _sfp.GetFiles(_folderInfo.Object);

            _directoryWrapper.Verify(dw => dw.GetFiles(Constants.FOLDER_ValidFolderPath));
        }

        [Test]
        public void GetFiles_Count_Equals_DirectoryWrapper_GetFiles_Count()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            var filesReturned = new string[] { "", "", "" };

            _directoryWrapper.Setup(dw => dw.GetFiles(Constants.FOLDER_ValidFolderPath)).Returns(filesReturned);

            var files = _sfp.GetFiles(_folderInfo.Object);

            Assert.AreEqual<int>(filesReturned.Length, files.Length);
        }

        [Test]
        public void GetFiles_Returns_Valid_FileNames_When_Folder_Contains_Files()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            var filesReturned = new string[] { "C:\\folder\\file1.txt", "C:\\folder\\file2.txt", "C:\\folder\\file3.txt" };
            var expectedValues = new string[] { "file1.txt", "file2.txt", "file3.txt" };

            _directoryWrapper.Setup(dw => dw.GetFiles(Constants.FOLDER_ValidFolderPath)).Returns(filesReturned);

            var files = _sfp.GetFiles(_folderInfo.Object);

            Assert.AreElementsEqual<string>(expectedValues, files);
        }

        #endregion

        #region GetFileContent

        [Test]
        [ExpectedArgumentException]
        public void GetFileStream_Throws_On_Null_Folder()
        {
            _sfp.GetFileStream(null, Constants.FOLDER_ValidFileName);
        }

        [Test]
        [ExpectedArgumentException]
        public void GetFileStream_Throws_On_Null_FileName()
        {
            _sfp.GetFileStream(_folderInfo.Object, null);
        }

        [Test]
        public void GetFileStream_Calls_FileWrapper_OpenRead()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _sfp.GetFileStream(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            _fileWrapper.Verify(fw => fw.OpenRead(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void GetFileStream_Returns_Valid_Stream_When_File_Exists()
        {
            var validFileBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var memoryStream = new MemoryStream(validFileBytes);

            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.OpenRead(Constants.FOLDER_ValidFilePath)).Returns(memoryStream);

            var result = _sfp.GetFileStream(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            byte[] resultBytes;
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = result.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                resultBytes = ms.ToArray();
            }

            Assert.AreEqual(validFileBytes, resultBytes);
        }

        [Test]
        public void GetFileStream_Returns_Null_When_File_Does_Not_Exist()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.OpenRead(Constants.FOLDER_ValidFilePath)).Throws<FileNotFoundException>();

            var result = _sfp.GetFileStream(_folderInfo.Object, Constants.FOLDER_ValidFileName);

            Assert.IsNull(result);
        }

        #endregion

        #region GetImageUrl

        [Test]
        [Ignore]
        public void GetImageUrl_Calls_GlobalsWrapper_ResolveUrl()
        {
            var globalsWrapper = new Mock<IGlobals>();
            GlobalsWrapper.RegisterInstance(globalsWrapper.Object);

            _sfp.GetImageUrl();

            globalsWrapper.Verify(gw => gw.ResolveUrl(It.IsAny<string>()), Times.Once());
        }

        #endregion

        #region GetLastModificationTime

        [Test]
        [ExpectedArgumentException]
        public void GetLastModificationTime_Throws_On_Null_File()
        {
            _sfp.GetLastModificationTime(null);
        }

        [Test]
        public void GetLastModificationTime_Calls_FileWrapper_GetLastWriteTime()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _sfp.GetLastModificationTime(_fileInfo.Object);

            _fileWrapper.Verify(fw => fw.GetLastWriteTime(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void GetLastModificationTime_Returns_Valid_Date_When_File_Exists()
        {
            var expectedDate = DateTime.Now;

            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.GetLastWriteTime(Constants.FOLDER_ValidFilePath)).Returns(expectedDate);

            var result = _sfp.GetLastModificationTime(_fileInfo.Object);

            Assert.AreEqual<DateTime>(expectedDate, result);
        }

        [Test]
        public void GetLastModificationTime_Returns_Null_Date_When_File_Does_Not_Exist()
        {
            var expectedDate = Null.NullDate;

            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _fileWrapper.Setup(fw => fw.GetLastWriteTime(Constants.FOLDER_ValidFilePath)).Throws<FileNotFoundException>();

            var result = _sfp.GetLastModificationTime(_fileInfo.Object);

            Assert.AreEqual<DateTime>(expectedDate, result);
        }

        #endregion

        #region GetSubFolders

        [Test]
        [ExpectedArgumentException]
        public void GetSubFolders_Throws_On_Null_FolderMapping()
        {
            _sfp.GetSubFolders(Constants.FOLDER_ValidFolderPath, null).ToList();
        }

        [Test]
        [ExpectedArgumentException]
        public void GetSubFolders_Throws_On_Null_FolderPath()
        {
            var folderMapping = new FolderMappingInfo();

            _sfp.GetSubFolders(null, folderMapping).ToList();
        }

        [Test]
        public void GetSubFolders_Calls_DirectoryWrapper_GetDirectories()
        {
            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(folderMapping.PortalID, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);

            _sfp.GetSubFolders(Constants.FOLDER_ValidFolderRelativePath, folderMapping).ToList();

            _directoryWrapper.Verify(dw => dw.GetDirectories(Constants.FOLDER_ValidFolderPath), Times.Once());
        }

        [Test]
        public void GetSubFolders_Count_Equals_DirectoryWrapper_GetDirectories_Count()
        {
            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);
            _pathUtils.Setup(pu => pu.GetRelativePath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidSubFolderPath)).Returns(Constants.FOLDER_ValidSubFolderRelativePath);
            _pathUtils.Setup(pu => pu.GetRelativePath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_OtherValidSubFolderPath)).Returns(Constants.FOLDER_OtherValidSubFolderRelativePath);

            var subFolders = new[] {
                Constants.FOLDER_ValidSubFolderPath,
                Constants.FOLDER_OtherValidSubFolderPath
            };

            _directoryWrapper.Setup(dw => dw.GetDirectories(Constants.FOLDER_ValidFolderPath)).Returns(subFolders);

            var result = _sfp.GetSubFolders(Constants.FOLDER_ValidFolderRelativePath, folderMapping).ToList();

            Assert.AreEqual(subFolders.Length, result.Count);
        }

        [Test]
        public void GetSubFolders_Returns_Valid_SubFolders_When_Folder_Is_Not_Empty()
        {
            var expectedSubFolders = new[] {
                Constants.FOLDER_ValidSubFolderRelativePath,
                Constants.FOLDER_OtherValidSubFolderRelativePath
            };

            var folderMapping = new FolderMappingInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _pathUtils.Setup(pu => pu.GetPhysicalPath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidFolderRelativePath)).Returns(Constants.FOLDER_ValidFolderPath);
            _pathUtils.Setup(pu => pu.GetRelativePath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_ValidSubFolderPath)).Returns(Constants.FOLDER_ValidSubFolderRelativePath);
            _pathUtils.Setup(pu => pu.GetRelativePath(Constants.CONTENT_ValidPortalId, Constants.FOLDER_OtherValidSubFolderPath)).Returns(Constants.FOLDER_OtherValidSubFolderRelativePath);

            var subFolders = new[] {
                Constants.FOLDER_ValidSubFolderPath,
                Constants.FOLDER_OtherValidSubFolderPath
            };

            _directoryWrapper.Setup(dw => dw.GetDirectories(Constants.FOLDER_ValidFolderPath)).Returns(subFolders);

            var result = _sfp.GetSubFolders(Constants.FOLDER_ValidFolderRelativePath, folderMapping).ToList();

            Assert.AreElementsEqual(expectedSubFolders, result);
        }

        #endregion

        #region IsInSync

        [Test]
        [ExpectedArgumentException]
        public void IsInSync_Throws_On_Null_File()
        {
            _sfp.IsInSync(null);
        }

        [Test]
        public void IsInSync_Returns_True_When_File_Is_In_Sync()
        {
            _fileInfo.Setup(fi => fi.SHA1Hash).Returns(Constants.FOLDER_UnmodifiedFileHash);

            var sfp = new Mock<StandardFolderProvider> { CallBase = true };
            sfp.Setup(fp => fp.GetHash(_fileInfo.Object)).Returns(Constants.FOLDER_UnmodifiedFileHash);

            var result = sfp.Object.IsInSync(_fileInfo.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsInSync_Returns_False_When_File_Is_Not_In_Sync()
        {
            _fileInfo.Setup(fi => fi.SHA1Hash).Returns(Constants.FOLDER_UnmodifiedFileHash);

            var sfp = new Mock<StandardFolderProvider> { CallBase = true };
            sfp.Setup(fp => fp.GetHash(_fileInfo.Object)).Returns(Constants.FOLDER_ModifiedFileHash);

            var result = sfp.Object.IsInSync(_fileInfo.Object);

            Assert.IsFalse(result);
        }

        #endregion

        #region RenameFile

        [Test]
        [ExpectedArgumentException]
        public void RenameFile_Throws_On_Null_File()
        {
            _sfp.RenameFile(null, Constants.FOLDER_ValidFileName);
        }

        [Test]
        [Row(null)]
        [Row("")]
        [ExpectedArgumentException]
        public void RenameFile_Throws_On_NullOrEmpty_NewFileName(string newFileName)
        {
            _sfp.RenameFile(_fileInfo.Object, newFileName);
        }

        [Test]
        public void RenameFile_Calls_FileWrapper_Move_When_FileNames_Are_Not_Equal()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);
            _fileInfo.Setup(fi => fi.FileName).Returns(Constants.FOLDER_ValidFileName);

            _sfp.RenameFile(_fileInfo.Object, Constants.FOLDER_OtherValidFileName);

            _fileWrapper.Verify(fw => fw.Move(Constants.FOLDER_ValidFilePath, Constants.FOLDER_OtherValidFilePath), Times.Once());
        }

        [Test]
        public void RenameFile_Does_Not_Call_FileWrapper_Move_When_FileNames_Are_Equal()
        {
            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);
            _fileInfo.Setup(fi => fi.FileName).Returns(Constants.FOLDER_ValidFileName);

            _sfp.RenameFile(_fileInfo.Object, Constants.FOLDER_ValidFileName);

            _fileWrapper.Verify(fw => fw.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region RenameFolder

        [Test]
        [ExpectedArgumentException]
        public void RenameFolder_Throws_On_Null_Folder()
        {
            _sfp.RenameFolder(null, Constants.FOLDER_ValidFolderName);
        }

        [Test]
        [Row(null)]
        [Row("")]
        [ExpectedArgumentException]
        public void RenameFolder_Throws_On_NullOrEmpty_NewFolderName(string newFolderName)
        {
            _sfp.RenameFolder(_folderInfo.Object, newFolderName);
        }

        [Test]
        public void RenameFolder_Calls_DirectoryWrapper_Move_When_FolderNames_Are_Not_Equal()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);
            _folderInfo.Setup(fi => fi.FolderName).Returns(Constants.FOLDER_ValidFolderName);

            _sfp.RenameFolder(_folderInfo.Object, Constants.FOLDER_OtherValidFolderName);

            _directoryWrapper.Verify(dw => dw.Move(Constants.FOLDER_ValidFolderPath, Constants.FOLDER_OtherValidFolderPath), Times.Once());
        }

        [Test]
        public void RenameFolder_Does_Not_Call_DirectoryWrapper_Move_When_FolderNames_Are_Equal()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);
            _folderInfo.Setup(fi => fi.FolderName).Returns(Constants.FOLDER_ValidFolderName);

            _sfp.RenameFolder(_folderInfo.Object, Constants.FOLDER_ValidFolderName);

            _directoryWrapper.Verify(dw => dw.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region SetFileAttributes

        [Test]
        [ExpectedArgumentException]
        public void SetFileAttributes_Throws_On_Null_File()
        {
            _sfp.SetFileAttributes(null, FileAttributes.Archive);
        }

        [Test]
        public void SetFileAttributes_Calls_FileWrapper_SetAttributes()
        {
            const FileAttributes validFileAttributes = FileAttributes.Archive;

            _fileInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFilePath);

            _sfp.SetFileAttributes(_fileInfo.Object, validFileAttributes);

            _fileWrapper.Verify(fw => fw.SetAttributes(Constants.FOLDER_ValidFilePath, validFileAttributes), Times.Once());
        }

        #endregion

        #region SupportsFileAttributes

        [Test]
        public void SupportsFileAttributes_Returns_True()
        {
            var result = _sfp.SupportsFileAttributes();

            Assert.IsTrue(result);
        }

        #endregion

        #region UpdateFile

        [Test]
        [ExpectedArgumentException]
        public void UpdateFile_Throws_On_Null_Folder()
        {
            var stream = new Mock<Stream>();

            _sfp.UpdateFile(null, Constants.FOLDER_ValidFileName, stream.Object);
        }

        [Test]
        [Row(null)]
        [Row("")]
        [ExpectedArgumentException]
        public void UpdateFile_Throws_On_NullOrEmpty_FileName(string fileName)
        {
            var stream = new Mock<Stream>();

            _sfp.UpdateFile(_folderInfo.Object, fileName, stream.Object);
        }

        [Test]
        [ExpectedArgumentException]
        public void UpdateFile_Throws_On_Null_Content()
        {
            _sfp.UpdateFile(_folderInfo.Object, Constants.FOLDER_ValidFileName, null);
        }

        [Test]
        public void UpdateFile_Calls_FileWrapper_Delete_When_File_Exists()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(true);

            var stream = new Mock<Stream>();

            _sfp.UpdateFile(_folderInfo.Object, Constants.FOLDER_ValidFileName, stream.Object);

            _fileWrapper.Verify(fw => fw.Delete(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        [Test]
        public void UpdateFile_Calls_FileWrapper_Create()
        {
            _folderInfo.Setup(fi => fi.PhysicalPath).Returns(Constants.FOLDER_ValidFolderPath);

            _fileWrapper.Setup(fw => fw.Exists(Constants.FOLDER_ValidFilePath)).Returns(false);

            var stream = new Mock<Stream>();

            _sfp.UpdateFile(_folderInfo.Object, Constants.FOLDER_ValidFileName, stream.Object);

            _fileWrapper.Verify(fw => fw.Create(Constants.FOLDER_ValidFilePath), Times.Once());
        }

        #endregion
    }
}
