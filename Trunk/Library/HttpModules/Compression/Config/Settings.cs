#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Cache;

#endregion

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// This class encapsulates the settings for an HttpCompressionModule
    /// </summary>
    [Serializable]
    public class Settings
    {
        private readonly StringCollection _excludedPaths;
        private Algorithms _preferredAlgorithm;
        private Regex _reg;
        private bool _whitespace;

        private Settings()
        {
            _preferredAlgorithm = Algorithms.None;
            _excludedPaths = new StringCollection();
            _whitespace = false;
        }

        public static Settings Default
        {
            get
            {
                return new Settings();
            }
        }

        public Algorithms PreferredAlgorithm
        {
            get
            {
                return _preferredAlgorithm;
            }
        }

        public Regex Reg
        {
            get
            {
                return _reg;
            }
        }

        public bool Whitespace
        {
            get
            {
                return _whitespace;
            }
        }

        public static Settings GetSettings()
        {
            var settings = (Settings) DataCache.GetCache("CompressionConfig");
            if (settings == null)
            {
                settings = Default;
                try
                {
                    settings._preferredAlgorithm = (Algorithms) Host.HttpCompressionAlgorithm;
                    settings._whitespace = Host.WhitespaceFilter;
                }
                catch (Exception e)
                {
                    Services.Exceptions.Exceptions.LogException(e);
                }

                string filePath = Common.Utilities.Config.GetPathToFile(Common.Utilities.Config.ConfigFileType.Compression);

                var fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var doc = new XPathDocument(fileReader);
                settings._reg = new Regex(doc.CreateNavigator().SelectSingleNode("compression/whitespace").Value);
                foreach (XPathNavigator nav in doc.CreateNavigator().Select("compression/excludedPaths/path"))
                {
                    settings._excludedPaths.Add(nav.Value.ToLower());
                }
                if ((File.Exists(filePath)))
                {
                    DataCache.SetCache("CompressionConfig", settings, new DNNCacheDependency(filePath));
                }
            }
            return settings;
        }

        public bool IsExcludedPath(string relUrl)
        {
            bool match = false;
            foreach (string path in _excludedPaths)
            {
                if (relUrl.ToLower().Contains(path))
                {
                    match = true;
                    break;
                }
            }
            return match;
        }
    }
}