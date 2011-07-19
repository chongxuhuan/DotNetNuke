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
//

//INSTANT C# NOTE: Formerly VB project-level imports:
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using Telerik.Web.UI.Widgets;
using DotNetNuke.Services.FileSystem;
using System.IO;

namespace DotNetNuke.Providers.RadEditorProvider
{

	public class TelerikFileBrowserProvider : FileSystemContentProvider
	{

		/// <summary>
		/// The current portal will be used for file access.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="searchPatterns"></param>
		/// <param name="viewPaths"></param>
		/// <param name="uploadPaths"></param>
		/// <param name="deletePaths"></param>
		/// <param name="selectedUrl"></param>
		/// <param name="selectedItemTag"></param>
		/// <remarks></remarks>
		public TelerikFileBrowserProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) : base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
		{
		}

#region Overrides

		//Protected Overrides Function IsValid(ByVal directory As System.IO.DirectoryInfo) As Boolean
		//    Return MyBase.IsValid(directory)
		//End Function

		//Protected Overrides Function IsValid(ByVal file As System.IO.FileInfo) As Boolean
		//    Return MyBase.IsValid(file)
		//End Function

		public override bool CanCreateDirectory
		{
			get
			{
				return base.CanCreateDirectory;
			}
		}

		public override bool CheckWritePermissions(string folderPath)
		{
			return base.CheckWritePermissions(folderPath);
		}

		public override System.IO.Stream GetFile(string url)
		{
			//base calls CheckWritePermissions method
			return TelerikContent.GetFile(FileSystemValidation.ToVirtualPath(url));
		}

		public override string GetPath(string url)
		{
			return TelerikContent.GetPath(FileSystemValidation.ToVirtualPath(url));
		}

		public override string GetFileName(string url)
		{
			return TelerikContent.GetFileName(FileSystemValidation.ToVirtualPath(url));
		}

		public override string CreateDirectory(string path, string name)
		{
			try
			{
				string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(path);

				string returnValue = DNNValidator.OnCreateFolder(virtualPath, name);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				//Returns errors or empty string when successful (ie: DirectoryAlreadyExists, InvalidCharactersInPath)
				returnValue = TelerikContent.CreateDirectory(virtualPath, name);

				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return GetTelerikMessage(returnValue);
				}

				if (string.IsNullOrEmpty(returnValue))
				{
					string virtualNewPath = (string)(string)FileSystemValidation.CombineVirtualPath(virtualPath, name);
					int newFolderID = DNNFolderCtrl.AddFolder(PortalSettings.PortalId, FileSystemValidation.ToDBPath(virtualNewPath));
					FileSystemUtils.SetFolderPermissions(PortalSettings.PortalId, newFolderID, FileSystemValidation.ToDBPath(virtualNewPath));
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, name);
			}
		}

		public override string MoveDirectory(string path, string newPath)
		{
			try
			{
				string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(path);
				string virtualNewPath = (string)(string)FileSystemValidation.ToVirtualPath(newPath);
				string virtualDestinationPath = FileSystemValidation.GetDestinationFolder(virtualNewPath);

				string returnValue = string.Empty;
				if (FileSystemValidation.GetDestinationFolder(virtualPath) == virtualDestinationPath)
				{
					//rename directory
					returnValue = DNNValidator.OnRenameFolder(virtualPath);
					if (! (string.IsNullOrEmpty(returnValue)))
					{
						return returnValue;
					}
				}
				else
				{
					//move directory
					returnValue = DNNValidator.OnMoveFolder(virtualPath, virtualDestinationPath);
					if (! (string.IsNullOrEmpty(returnValue)))
					{
						return returnValue;
					}
				}

				//Are all items visible to user?
				FolderInfo folder = DNNValidator.GetUserFolder(virtualPath);
				if (! (CheckAllChildrenVisible(ref folder)))
				{
					return DNNValidator.LogDetailError(ErrorCodes.CannotMoveFolder_ChildrenVisible);
				}

				//Returns errors or empty string when successful (ie: Cannot create a file when that file already exists)
				returnValue = TelerikContent.MoveDirectory(virtualPath, virtualNewPath);

				if (string.IsNullOrEmpty(returnValue))
				{
					//Sync to remove old folder & files        
					FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualPath), FileSystemValidation.ToDBPath(virtualPath), true, true, true);
					//Sync to add new folder & files
					FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualNewPath), FileSystemValidation.ToDBPath(virtualNewPath), true, true, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, newPath);
			}
		}

		public override string CopyDirectory(string path, string newPath)
		{
			try
			{
				string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(path);
				string virtualNewPath = (string)(string)FileSystemValidation.ToVirtualPath(newPath);
				string virtualDestinationPath = FileSystemValidation.GetDestinationFolder(virtualNewPath);

				string returnValue = DNNValidator.OnCopyFolder(virtualPath, virtualDestinationPath);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				//Are all items visible to user?
				//todo: copy visible files and folders only?
				FolderInfo folder = DNNValidator.GetUserFolder(virtualPath);
				if (! (CheckAllChildrenVisible(ref folder)))
				{
					return DNNValidator.LogDetailError(ErrorCodes.CannotCopyFolder_ChildrenVisible);
				}

				returnValue = TelerikContent.CopyDirectory(virtualPath, virtualNewPath);

				if (string.IsNullOrEmpty(returnValue))
				{
					//Sync to add new folder & files
					FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualNewPath), FileSystemValidation.ToDBPath(virtualNewPath), true, true, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, newPath);
			}
		}

		public override string DeleteDirectory(string path)
		{
			try
			{
				string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(path);

				string returnValue = DNNValidator.OnDeleteFolder(virtualPath);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				//Are all items visible to user?
				FolderInfo folder = DNNValidator.GetUserFolder(virtualPath);
				if (! (CheckAllChildrenVisible(ref folder)))
				{
					return DNNValidator.LogDetailError(ErrorCodes.CannotDeleteFolder_ChildrenVisible);
				}

				returnValue = TelerikContent.DeleteDirectory(virtualPath);

				if (string.IsNullOrEmpty(returnValue))
				{
					//Sync to remove old folder & files
					FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualPath), FileSystemValidation.ToDBPath(virtualPath), true, true, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ref ex, path);
			}
		}

		public override string DeleteFile(string path)
		{
			try
			{
				string virtualPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(path);

				string returnValue = DNNValidator.OnDeleteFile(virtualPathAndFile);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				returnValue = TelerikContent.DeleteFile(virtualPathAndFile);

				if (string.IsNullOrEmpty(returnValue))
				{
					string virtualPath = (string)(string)FileSystemValidation.RemoveFileName(virtualPathAndFile);
					FolderInfo dnnFolder = DNNValidator.GetUserFolder(virtualPath);
					DNNFileCtrl.DeleteFile(PortalSettings.PortalId, System.IO.Path.GetFileName(virtualPathAndFile), dnnFolder.FolderID, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ref ex, path);
			}
		}

		public override string MoveFile(string path, string newPath)
		{
			try
			{
				string virtualPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(path);
				string virtualNewPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(newPath);

				string virtualPath = (string)(string)FileSystemValidation.RemoveFileName(virtualPathAndFile);
				string virtualNewPath = (string)(string)FileSystemValidation.RemoveFileName(virtualNewPathAndFile);

				string returnValue = string.Empty;
				if (virtualPath == virtualNewPath)
				{
					//rename file
					returnValue = DNNValidator.OnRenameFile(virtualPathAndFile);
					if (! (string.IsNullOrEmpty(returnValue)))
					{
						return returnValue;
					}
				}
				else
				{
					//move file
					returnValue = DNNValidator.OnMoveFile(virtualPathAndFile, virtualNewPathAndFile);
					if (! (string.IsNullOrEmpty(returnValue)))
					{
						return returnValue;
					}
				}

				//Returns errors or empty string when successful (ie: NewFileAlreadyExists)
				returnValue = TelerikContent.MoveFile(virtualPathAndFile, virtualNewPathAndFile);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return GetTelerikMessage(returnValue);
				}

				if (string.IsNullOrEmpty(returnValue))
				{
					FolderInfo dnnFolder = DNNValidator.GetUserFolder(virtualNewPath);
					DotNetNuke.Services.FileSystem.FileInfo dnnFileInfo = new DotNetNuke.Services.FileSystem.FileInfo();
					FillFileInfo(virtualNewPathAndFile, ref dnnFileInfo);

					DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, true);

					FolderInfo dnnOriginalFolder = DNNValidator.GetUserFolder(virtualPath);
					string originalFileName = System.IO.Path.GetFileName(virtualPathAndFile);

					DNNFileCtrl.DeleteFile(PortalSettings.PortalId, originalFileName, dnnOriginalFolder.FolderID, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, newPath);
			}
		}

		public override string CopyFile(string path, string newPath)
		{
			try
			{
				string virtualPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(path);
				string virtualNewPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(newPath);

				string returnValue = DNNValidator.OnCopyFile(virtualPathAndFile, virtualNewPathAndFile);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				//Returns errors or empty string when successful (ie: NewFileAlreadyExists)
				returnValue = TelerikContent.CopyFile(virtualPathAndFile, virtualNewPathAndFile);

				if (string.IsNullOrEmpty(returnValue))
				{
					string virtualNewPath = (string)(string)FileSystemValidation.RemoveFileName(virtualNewPathAndFile);
					FolderInfo dnnFolder = DNNValidator.GetUserFolder(virtualNewPath);
					DotNetNuke.Services.FileSystem.FileInfo dnnFileInfo = new DotNetNuke.Services.FileSystem.FileInfo();
					FillFileInfo(virtualNewPathAndFile, ref dnnFileInfo);

					DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, newPath);
			}
		}

		public override string StoreFile(System.Web.HttpPostedFile file, string path, string name, params string[] arguments)
		{
			return StoreFile(Telerik.Web.UI.UploadedFile.FromHttpPostedFile(file), path, name, arguments);
		}

		public override string StoreFile(Telerik.Web.UI.UploadedFile file, string path, string name, params string[] arguments)
		{
			try
			{
				string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(path);

				string returnValue = DNNValidator.OnCreateFile(FileSystemValidation.CombineVirtualPath(virtualPath, name), file.ContentLength);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				returnValue = TelerikContent.StoreFile(file, virtualPath, name, arguments);

				DotNetNuke.Services.FileSystem.FileInfo dnnFileInfo = new DotNetNuke.Services.FileSystem.FileInfo();
				FillFileInfo(file, ref dnnFileInfo);

				//Add or update file
				FolderInfo dnnFolder = DNNValidator.GetUserFolder(virtualPath);
				DotNetNuke.Services.FileSystem.FileInfo dnnFile = DNNFileCtrl.GetFile(name, PortalSettings.PortalId, dnnFolder.FolderID);
				if (dnnFile != null)
				{
					DNNFileCtrl.UpdateFile(dnnFile.FileId, dnnFileInfo.FileName, dnnFileInfo.Extension, file.ContentLength, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID);
				}
				else
				{
					DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, file.ContentLength, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ex, path, name);
			}
		}

		public override string StoreBitmap(System.Drawing.Bitmap bitmap, string url, System.Drawing.Imaging.ImageFormat format)
		{
			try
			{
				//base calls CheckWritePermissions method			
				string virtualPathAndFile = (string)(string)FileSystemValidation.ToVirtualPath(url);
				string virtualPath = (string)(string)FileSystemValidation.RemoveFileName(virtualPathAndFile);
				string returnValue = DNNValidator.OnCreateFile(virtualPathAndFile, 0);
				if (! (string.IsNullOrEmpty(returnValue)))
				{
					return returnValue;
				}

				returnValue = TelerikContent.StoreBitmap(bitmap, virtualPathAndFile, format);

				DotNetNuke.Services.FileSystem.FileInfo dnnFileInfo = new DotNetNuke.Services.FileSystem.FileInfo();
				FillFileInfo(virtualPathAndFile, ref dnnFileInfo);

				//check again with real contentLength
				string errMsg = DNNValidator.OnCreateFile(virtualPathAndFile, dnnFileInfo.Size);
				if (! (string.IsNullOrEmpty(errMsg)))
				{
					TelerikContent.DeleteFile(virtualPathAndFile);
					return errMsg;
				}

				FolderInfo dnnFolder = DNNValidator.GetUserFolder(virtualPath);
				DotNetNuke.Services.FileSystem.FileInfo dnnFile = DNNFileCtrl.GetFile(dnnFileInfo.FileName, PortalSettings.PortalId, dnnFolder.FolderID);

				if (dnnFile != null)
				{
					DNNFileCtrl.UpdateFile(dnnFile.FileId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, bitmap.Width, bitmap.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID);
				}
				else
				{
					DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, bitmap.Width, bitmap.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, true);
				}

				return returnValue;
			}
			catch (Exception ex)
			{
				return DNNValidator.LogUnknownError(ref ex, url);
			}
		}

		public override Telerik.Web.UI.Widgets.DirectoryItem ResolveDirectory(string path)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveDirectory: " + path);
				return GetDirectoryItemWithDNNPermissions(path, true);
			}
			catch (Exception ex)
			{
				DNNValidator.LogUnknownError(ref ex, path);
				return null;
			}
		}

		public override Telerik.Web.UI.Widgets.DirectoryItem ResolveRootDirectoryAsTree(string path)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveRootDirectoryAsTree: " + path);
				return GetDirectoryItemWithDNNPermissions(path, false);
			}
			catch (Exception ex)
			{
				DNNValidator.LogUnknownError(ref ex, path);
				return null;
			}
		}

		public override Telerik.Web.UI.Widgets.DirectoryItem[] ResolveRootDirectoryAsList(string path)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveRootDirectoryAsList: " + path);
				return GetDirectoryItemWithDNNPermissions(path, false).Directories;
			}
			catch (Exception ex)
			{
				DNNValidator.LogUnknownError(ref ex, path);
				return null;
			}
		}

#endregion

#region Properties

		private FileSystemValidation _DNNValidator = null;
		private FileSystemValidation DNNValidator
		{
			get
			{
				if (_DNNValidator == null)
				{
					_DNNValidator = new FileSystemValidation();
				}

				return _DNNValidator;
			}
		}

		private DotNetNuke.Entities.Portals.PortalSettings PortalSettings
		{
			get
			{
				return DotNetNuke.Entities.Portals.PortalSettings.Current;
			}
		}

		private DotNetNuke.Entities.Users.UserInfo CurrentUser
		{
			get
			{
				return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
			}
		}

		private Telerik.Web.UI.Widgets.FileSystemContentProvider _TelerikContent = null;
		private Telerik.Web.UI.Widgets.FileSystemContentProvider TelerikContent
		{
			get
			{
				if (_TelerikContent == null)
				{
					_TelerikContent = new Telerik.Web.UI.Widgets.FileSystemContentProvider(this.Context, this.SearchPatterns, new string[] {FileSystemValidation.HomeDirectory}, new string[] {FileSystemValidation.HomeDirectory}, new string[] {FileSystemValidation.HomeDirectory}, FileSystemValidation.ToVirtualPath(this.SelectedUrl), FileSystemValidation.ToVirtualPath(this.SelectedItemTag));
				}
				return _TelerikContent;
			}
		}

		private FolderController _DNNFolderCtrl = null;
		private FolderController DNNFolderCtrl
		{
			get
			{
				if (_DNNFolderCtrl == null)
				{
					_DNNFolderCtrl = new FolderController();
				}
				return _DNNFolderCtrl;
			}
		}

		private FileController _DNNFileCtrl = null;
		private FileController DNNFileCtrl
		{
			get
			{
				if (_DNNFileCtrl == null)
				{
					_DNNFileCtrl = new FileController();
				}
				return _DNNFileCtrl;
			}
		}

#endregion

#region Private

		private Telerik.Web.UI.Widgets.DirectoryItem GetDirectoryItemWithDNNPermissions(string path, bool loadFiles)
		{
			Telerik.Web.UI.Widgets.DirectoryItem radDirectory = TelerikContent.ResolveDirectory(FileSystemValidation.ToVirtualPath(path));
			var returnValues = AddChildDirectoriesToList(ref new Telerik.Web.UI.Widgets.DirectoryItem[] {radDirectory}, true, loadFiles);

			if (returnValues != null && returnValues.Length > 0)
			{
				return returnValues[0];
			}

			return null;
		}

		private Telerik.Web.UI.Widgets.DirectoryItem[] AddChildDirectoriesToList(ref Telerik.Web.UI.Widgets.DirectoryItem[] radDirectories, bool recursive, bool loadFiles)
		{
			ArrayList newDirectories = new ArrayList();

			foreach (Telerik.Web.UI.Widgets.DirectoryItem radDirectory in radDirectories)
			{
				System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + " AddChildDirectoriesToList " + radDirectory.Name);

				if (radDirectory == null)
				{
					continue;
				}

				string endUserPath = (string)(string)FileSystemValidation.ToEndUserPath(radDirectory.FullPath);
				FolderInfo dnnFolder = DNNValidator.GetUserFolder(radDirectory.FullPath);

				if ((dnnFolder == null))
				{
					continue;
				}

				if (dnnFolder != null)
				{
					//Don't show protected folders
					if (! (string.IsNullOrEmpty(dnnFolder.FolderPath)) && dnnFolder.IsProtected)
					{
						continue;
					}

					//Don't show Cache folder
					if (dnnFolder.FolderPath.ToLowerInvariant() == "cache/")
					{
						continue;
					}

					ArrayList showFiles = new ArrayList();
					Telerik.Web.UI.Widgets.PathPermissions folderPermissions = Telerik.Web.UI.Widgets.PathPermissions.Read;

					if (DNNValidator.CanViewFilesInFolder(dnnFolder))
					{
						if (DNNValidator.CanAddToFolder(dnnFolder))
						{
							folderPermissions = folderPermissions | Telerik.Web.UI.Widgets.PathPermissions.Upload;
						}

						if (DNNValidator.CanDeleteFolder(dnnFolder))
						{
							folderPermissions = folderPermissions | Telerik.Web.UI.Widgets.PathPermissions.Delete;
						}

						if (loadFiles)
						{
							IDictionary<string, DotNetNuke.Services.FileSystem.FileInfo> dnnFiles = GetDNNFiles(dnnFolder.FolderID);

							if (dnnFolder.StorageLocation != FolderController.StorageLocationTypes.InsecureFileSystem)
							{
								//check Telerik search patterns to filter out files
								foreach (DotNetNuke.Services.FileSystem.FileInfo dnnFile in dnnFiles.Values)
								{
									string tempVar[] = base.SearchPatterns;
									if (CheckSearchPatterns(dnnFile.FileName, ref tempVar))
									{
										string url = DotNetNuke.Common.Globals.LinkClick("fileid=" + dnnFile.FileId.ToString(), Null.NullInteger, Null.NullInteger);
										//= DotNetNuke.Common.Globals.ApplicationPath & "/LinkClick.aspx?fileticket=" & UrlUtils.EncryptParameter(dnnFile.FileId)

										Telerik.Web.UI.Widgets.FileItem fileItem = new Telerik.Web.UI.Widgets.FileItem(dnnFile.FileName, dnnFile.Extension, dnnFile.Size, "", url, "", folderPermissions);

										showFiles.Add(fileItem);
									}
								}
							}
							else
							{
								//check Telerik search patterns to filter out files
								foreach (Telerik.Web.UI.Widgets.FileItem telerikFile in radDirectory.Files)
								{
									if (dnnFiles.ContainsKey(telerikFile.Name))
									{
										Telerik.Web.UI.Widgets.FileItem fileItem = new Telerik.Web.UI.Widgets.FileItem(telerikFile.Name, telerikFile.Extension, telerikFile.Length, "", FileSystemValidation.ToVirtualPath(radDirectory.FullPath) + telerikFile.Name, "", folderPermissions);

										showFiles.Add(fileItem);
									}
								}
							}
						}
					}

					Telerik.Web.UI.Widgets.FileItem[] folderFiles = showFiles.ToArray(typeof(Telerik.Web.UI.Widgets.FileItem));

					//Root folder name
					string dirName = radDirectory.Name;
					if (dnnFolder.FolderPath == "" && dnnFolder.FolderName == "")
					{
						dirName = FileSystemValidation.EndUserHomeDirectory;
					}

					Telerik.Web.UI.Widgets.DirectoryItem newDirectory = null;
					if (recursive)
					{
						radDirectory = TelerikContent.ResolveRootDirectoryAsTree(radDirectory.Path);
						Telerik.Web.UI.Widgets.DirectoryItem tempVar2[] = radDirectory.Directories;
						newDirectory = new Telerik.Web.UI.Widgets.DirectoryItem(dirName, "", endUserPath, "", folderPermissions, folderFiles, AddChildDirectoriesToList(ref tempVar2, false, false));
						radDirectory.Directories = tempVar2;
					}
					else
					{
						newDirectory = new Telerik.Web.UI.Widgets.DirectoryItem(dirName, "", endUserPath, "", folderPermissions, folderFiles, new Telerik.Web.UI.Widgets.DirectoryItem[0]);
					}

					newDirectories.Add(newDirectory);
				}
			}

			return newDirectories.ToArray(typeof(Telerik.Web.UI.Widgets.DirectoryItem));
		}

		private IDictionary<string, DotNetNuke.Services.FileSystem.FileInfo> GetDNNFiles(int dnnFolderID)
		{
			System.Data.IDataReader drFiles = null;
			IDictionary<string, DotNetNuke.Services.FileSystem.FileInfo> dnnFiles = null;

			try
			{
				drFiles = DNNFileCtrl.GetFiles(PortalSettings.PortalId, dnnFolderID);
				dnnFiles = CBO.FillDictionary<string, DotNetNuke.Services.FileSystem.FileInfo>("FileName", drFiles);
			}
			finally
			{
				if (drFiles != null)
				{
					if (! drFiles.IsClosed)
					{
						drFiles.Close();
					}
				}
			}

			return dnnFiles;
		}

		private bool CheckAllChildrenVisible(ref FolderInfo folder)
		{
			string virtualPath = (string)(string)FileSystemValidation.ToVirtualPath(folder.FolderPath);

			//check files are visible
			IDictionary<string, DotNetNuke.Services.FileSystem.FileInfo> files = GetDNNFiles(folder.FolderID);
			int visibleFileCount = 0;
			foreach (DotNetNuke.Services.FileSystem.FileInfo fileItem in files.Values)
			{
				string tempVar[] = base.SearchPatterns;
				if (CheckSearchPatterns(fileItem.FileName, ref tempVar))
				{
					visibleFileCount = visibleFileCount + 1;
				}
			}

			if (visibleFileCount != Directory.GetFiles(HttpContext.Current.Request.MapPath(virtualPath)).Length)
			{
				return false;
			}

			//check folders
			if (folder != null)
			{
				IDictionary<string, FolderInfo> childUserFolders = DNNValidator.GetChildUserFolders(virtualPath);

				if (childUserFolders.Count != Directory.GetDirectories(HttpContext.Current.Request.MapPath(virtualPath)).Length)
				{
					return false;
				}

				//check children
				foreach (FolderInfo childFolder in childUserFolders.Values)
				{
					//do recursive check
					FolderInfo tempVar2 = childFolder;
					if (! (CheckAllChildrenVisible(ref tempVar2)))
					{
						return false;
					}
				}
			}

			return true;
		}

		private void FillFileInfo(string virtualPathAndFile, ref DotNetNuke.Services.FileSystem.FileInfo fileInfo)
		{
			fileInfo.FileName = Path.GetFileName(virtualPathAndFile);
			fileInfo.Extension = Path.GetExtension(virtualPathAndFile);
			if (fileInfo.Extension.StartsWith("."))
			{
				fileInfo.Extension = fileInfo.Extension.Remove(0, 1);
			}

			fileInfo.ContentType = FileSystemUtils.GetContentType(fileInfo.Extension);

			FileStream fileStream = null;
			try
			{
				fileStream = File.OpenRead(HttpContext.Current.Request.MapPath(virtualPathAndFile));
				FillImageInfo(fileStream, ref fileInfo);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}

		private void FillFileInfo(Telerik.Web.UI.UploadedFile file, ref DotNetNuke.Services.FileSystem.FileInfo fileInfo)
		{
			//The core API expects the path to be stripped off the filename
			fileInfo.FileName = ((file.FileName.Contains("\\")) ? System.IO.Path.GetFileName(file.FileName) : file.FileName);
			fileInfo.Extension = file.GetExtension();
			if (fileInfo.Extension.StartsWith("."))
			{
				fileInfo.Extension = fileInfo.Extension.Remove(0, 1);
			}

			fileInfo.ContentType = FileSystemUtils.GetContentType(fileInfo.Extension);

			FillImageInfo(file.InputStream, ref fileInfo);
		}

		private void FillImageInfo(Stream fileStream, ref DotNetNuke.Services.FileSystem.FileInfo fileInfo)
		{
			if (Convert.ToBoolean((DotNetNuke.Common.Globals.glbImageFileTypes + ",".IndexOf(fileInfo.Extension.ToLowerInvariant() + ",", 0) + 1)))
			{
				System.Drawing.Image img = null;
				try
				{
					img = System.Drawing.Image.FromStream(fileStream);
					if (fileStream.Length > int.MaxValue)
					{
						fileInfo.Size = int.MaxValue;
					}
					else
					{
						fileInfo.Size = int.Parse(fileStream.Length.ToString());
					}
					fileInfo.Width = img.Width;
					fileInfo.Height = img.Height;
				}
				catch
				{
					// error loading image file
					fileInfo.ContentType = "application/octet-stream";
				}
				finally
				{
					if (img != null)
					{
						img.Dispose();
					}
				}
			}
		}

#endregion

#region Search Patterns

		private bool CheckSearchPatterns(string dnnFileName, ref string[] searchPatterns)
		{
			if (searchPatterns == null | searchPatterns.Length < 1)
			{
				return true;
			}

			bool returnValue = false;
			foreach (string pattern in searchPatterns)
			{
				bool result = new System.Text.RegularExpressions.Regex(ConvertToRegexPattern(pattern), System.Text.RegularExpressions.RegexOptions.IgnoreCase).IsMatch(dnnFileName);

				if (result)
				{
					returnValue = true;
					break;
				}
			}

			return returnValue;
		}

		private object ConvertToRegexPattern(string pattern)
		{
			string returnValue = System.Text.RegularExpressions.Regex.Escape(pattern);
			returnValue = returnValue.Replace("\\*", ".*");
			returnValue = returnValue.Replace("\\?", ".") + "$";
			return returnValue;
		}

		private object GetTelerikMessage(string key)
		{
			string returnValue = key;
			switch (key)
			{
				case "DirectoryAlreadyExists":
					returnValue = DNNValidator.GetString("ErrorCodes.DirectoryAlreadyExists");
					break;
				case "InvalidCharactersInPath":
					returnValue = DNNValidator.GetString("ErrorCodes.InvalidCharactersInPath");
					break;
				case "NewFileAlreadyExists":
					returnValue = DNNValidator.GetString("ErrorCodes.NewFileAlreadyExists");
					break;
					//Case ""
					//	Exit Select
			}

			return returnValue;
		}

#endregion

	}

}