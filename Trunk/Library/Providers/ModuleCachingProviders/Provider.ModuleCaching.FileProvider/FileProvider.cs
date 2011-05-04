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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Services.ModuleCache
{
    public class FileProvider : ModuleCachingProvider
    {
        private const string DataFileExtension = ".data.resources";
        private const string AttribFileExtension = ".attrib.resources";

        private string GenerateCacheKeyHash(int tabModuleId, string cacheKey)
        {
            byte[] hash = Encoding.ASCII.GetBytes(cacheKey);
            var md5 = new MD5CryptoServiceProvider();
            hash = md5.ComputeHash(hash);
            return tabModuleId + "_" + ByteArrayToString(hash);
        }

        private static string GetAttribFileName(int tabModuleId, string cacheKey)
        {
            return string.Concat(GetCacheFolder(), cacheKey, AttribFileExtension);
        }

        private static int GetCachedItemCount(int tabModuleId)
        {
            return Directory.GetFiles(GetCacheFolder(), String.Format("*{0}", DataFileExtension)).Length;
        }

        private static string GetCachedOutputFileName(int tabModuleId, string cacheKey)
        {
            return string.Concat(GetCacheFolder(), cacheKey, DataFileExtension);
        }

        private static string GetCacheFolder(int portalId)
        {
            var portalController = new PortalController();
            PortalInfo portalInfo = portalController.GetPortal(portalId);

            string homeDirectoryMapPath = portalInfo.HomeDirectoryMapPath;
            string cacheFolder = Null.NullString;

            if ((!string.IsNullOrEmpty(homeDirectoryMapPath)))
            {
                cacheFolder = string.Concat(homeDirectoryMapPath, "Cache\\Pages\\");
                if (!Directory.Exists(cacheFolder))
                {
                    Directory.CreateDirectory(cacheFolder);
                }
            }


            return cacheFolder;
        }

        private static string GetCacheFolder()
        {
            int portalId = PortalController.GetCurrentPortalSettings().PortalId;
            return GetCacheFolder(portalId);
        }

        private bool IsFileExpired(string file)
        {
            StreamReader oRead = null;
            try
            {
                oRead = File.OpenText(file);
                DateTime expires = DateTime.Parse(oRead.ReadLine(), CultureInfo.InvariantCulture);
                if (expires < DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (oRead != null)
                {
                    oRead.Close();
                }
            }
        }

        private void PurgeCache(string folder)
        {
            var filesNotDeleted = new StringBuilder();
            int i = 0;
            foreach (string File in Directory.GetFiles(folder, "*.resources"))
            {
                if (!FileSystemUtils.DeleteFileWithWait(File, 100, 200))
                {
                    filesNotDeleted.Append(String.Format("{0};", File));
                }
                else
                {
                    i += 1;
                }
            }
            if (filesNotDeleted.Length > 0)
            {
                throw new IOException(String.Format("Deleted {0} files, however, some files are locked.  Could not delete the following files: {1}", i, filesNotDeleted));
            }
        }

        private static bool IsPathInApplication(string cacheFolder)
        {
            return cacheFolder.Contains(Globals.ApplicationMapPath);
        }

        public override string GenerateCacheKey(int tabModuleId, SortedDictionary<string, string> varyBy)
        {
            var cacheKey = new StringBuilder();
            if (varyBy != null)
            {
                SortedDictionary<string, string>.Enumerator varyByParms = varyBy.GetEnumerator();
                while ((varyByParms.MoveNext()))
                {
                    string key = varyByParms.Current.Key.ToLower();
                    cacheKey.Append(string.Concat(key, "=", varyByParms.Current.Value, "|"));
                }
            }
            return GenerateCacheKeyHash(tabModuleId, cacheKey.ToString());
        }

        public override int GetItemCount(int tabModuleId)
        {
            return GetCachedItemCount(tabModuleId);
        }

        public override byte[] GetModule(int tabModuleId, string cacheKey)
        {
            string cachedModule = GetCachedOutputFileName(tabModuleId, cacheKey);
            BinaryReader br = null;
            FileStream fStream = null;
            byte[] data;
            try
            {
                if (!File.Exists(cachedModule))
                {
                    return null;
                }
                var fInfo = new FileInfo(cachedModule);
                long numBytes = fInfo.Length;
                fStream = new FileStream(cachedModule, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fStream);
                data = br.ReadBytes(Convert.ToInt32(numBytes));
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fStream != null)
                {
                    fStream.Close();
                }
            }
            return data;
        }

        public override void PurgeCache(int portalId)
        {
            PurgeCache(GetCacheFolder(portalId));
        }

        public override void PurgeExpiredItems(int portalId)
        {
            var filesNotDeleted = new StringBuilder();
            int i = 0;
            string cacheFolder = GetCacheFolder(portalId);
            if (Directory.Exists(cacheFolder) && IsPathInApplication(cacheFolder))
            {
                foreach (string File in Directory.GetFiles(cacheFolder, String.Format("*{0}", AttribFileExtension)))
                {
                    if (IsFileExpired(File))
                    {
                        string fileToDelete = File.Replace(AttribFileExtension, DataFileExtension);
                        if (!FileSystemUtils.DeleteFileWithWait(fileToDelete, 100, 200))
                        {
                            filesNotDeleted.Append(String.Format("{0};", fileToDelete));
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                }
            }
            if (filesNotDeleted.Length > 0)
            {
                throw new IOException(String.Format("Deleted {0} files, however, some files are locked.  Could not delete the following files: {1}", i, filesNotDeleted));
            }
        }

        public override void SetModule(int tabModuleId, string cacheKey, TimeSpan duration, byte[] output)
        {
            string attribFile = GetAttribFileName(tabModuleId, cacheKey);
            string cachedOutputFile = GetCachedOutputFileName(tabModuleId, cacheKey);
            try
            {
                if (File.Exists(cachedOutputFile))
                {
                    FileSystemUtils.DeleteFileWithWait(cachedOutputFile, 100, 200);
                }
                var captureStream = new FileStream(cachedOutputFile, FileMode.CreateNew, FileAccess.Write);
                captureStream.Write(output, 0, output.Length);
                captureStream.Close();
                StreamWriter oWrite;
                oWrite = File.CreateText(attribFile);
                oWrite.WriteLine(DateTime.UtcNow.Add(duration).ToString(CultureInfo.InvariantCulture));
                oWrite.Close();
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
        }

        public override void Remove(int tabModuleId)
        {
            var controller = new ModuleController();
            ModuleInfo tabModule = controller.GetTabModule(tabModuleId);
            string cacheFolder = GetCacheFolder(tabModule.PortalID);
            var filesNotDeleted = new StringBuilder();
            int i = 0;
            foreach (string File in Directory.GetFiles(cacheFolder, tabModuleId + "_*.*"))
            {
                if (!FileSystemUtils.DeleteFileWithWait(File, 100, 200))
                {
                    filesNotDeleted.Append(File + ";");
                }
                else
                {
                    i += 1;
                }
            }
            if (filesNotDeleted.Length > 0)
            {
                throw new IOException("Deleted " + i + " files, however, some files are locked.  Could not delete the following files: " + filesNotDeleted);
            }
        }
    }
}