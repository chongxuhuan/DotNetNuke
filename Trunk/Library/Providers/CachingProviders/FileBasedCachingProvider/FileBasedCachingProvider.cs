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
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Services.Cache.FileBasedCachingProvider
{
    public class FBCachingProvider : CachingProvider
    {
        internal const string CacheFileExtension = ".resources";
        internal static string CachingDirectory = "Cache\\";

        public override void Insert(string Key, object Value, DNNCacheDependency Dependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority,
                                    CacheItemRemovedCallback OnRemoveCallback)
        {
            DNNCacheDependency d = Dependency;
            if (IsWebFarm())
            {
                var f = new string[1];
                f[0] = GetFileName(Key);
                CreateCacheFile(f[0], Key);
                d = new DNNCacheDependency(f, null, Dependency);
            }
            base.Insert(Key, Value, d, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
        }

        public override bool IsWebFarm()
        {
            bool _IsWebFarm = Null.NullBoolean;
            if (!string.IsNullOrEmpty(Config.GetSetting("IsWebFarm")))
            {
                _IsWebFarm = bool.Parse(Config.GetSetting("IsWebFarm"));
            }
            return _IsWebFarm;
        }

        public override string PurgeCache()
        {
            return PurgeCacheFiles(Globals.HostMapPath + CachingDirectory);
        }

        public override void Remove(string Key)
        {
            base.Remove(Key);
            if (IsWebFarm())
            {
                string f = GetFileName(Key);
                DeleteCacheFile(f);
            }
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            var sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i <= arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        private static void CreateCacheFile(string FileName, string CacheKey)
        {
            StreamWriter s = null;
            try
            {
                if (!File.Exists(FileName))
                {
                    s = File.CreateText(FileName);
                    s.Write(CacheKey);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }
        }

        private static void DeleteCacheFile(string FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
        }

        private static string GetFileName(string CacheKey)
        {
            byte[] FileNameBytes = Encoding.ASCII.GetBytes(CacheKey);
            var md5 = new MD5CryptoServiceProvider();
            FileNameBytes = md5.ComputeHash(FileNameBytes);
            string FinalFileName = ByteArrayToString(FileNameBytes);
            return Path.GetFullPath(Globals.HostMapPath + CachingDirectory + FinalFileName + CacheFileExtension);
        }

        private string PurgeCacheFiles(string Folder)
        {
            int PurgedFiles = 0;
            int PurgeErrors = 0;
            int i;
            string[] f;
            f = Directory.GetFiles(Folder);
            for (i = 0; i <= f.Length - 1; i++)
            {
                DateTime dtLastWrite;
                dtLastWrite = File.GetLastWriteTime(f[i]);
                if (dtLastWrite < DateTime.Now.Subtract(new TimeSpan(2, 0, 0)))
                {
                    string strCacheKey = Path.GetFileNameWithoutExtension(f[i]);
                    if (DataCache.GetCache(strCacheKey) == null)
                    {
                        try
                        {
                            File.Delete(f[i]);
                            PurgedFiles += 1;
                        }
                        catch (Exception exc)
                        {
                            Instrumentation.DnnLog.Error(exc);

                            PurgeErrors += 1;
                        }
                    }
                }
            }
            return string.Format("Cache Synchronization Files Processed: " + f.Length + ", Purged: " + PurgedFiles + ", Errors: " + PurgeErrors);
        }
    }
}