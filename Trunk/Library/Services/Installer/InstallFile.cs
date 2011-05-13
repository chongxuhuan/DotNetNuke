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
using System.ComponentModel;
using System.Text.RegularExpressions;

using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace DotNetNuke.Services.Installer
{
    [Serializable]
    public class InstallFile
    {
        private readonly InstallerInfo _InstallerInfo;
        private readonly string _SourceFileName;
        private string _Action;
        private TextEncoding _Encoding = TextEncoding.UTF8;
        private string _Name;
        private string _Path;
        private InstallFileType _Type;
        private Version _Version;

        public InstallFile(ZipInputStream zip, ZipEntry entry, InstallerInfo info)
        {
            _InstallerInfo = info;
            ReadZip(zip, entry);
        }

        public InstallFile(string fileName)
        {
            ParseFileName(fileName);
        }

        public InstallFile(string fileName, InstallerInfo info)
        {
            ParseFileName(fileName);
            _InstallerInfo = info;
        }

        public InstallFile(string fileName, string sourceFileName, InstallerInfo info)
        {
            ParseFileName(fileName);
            _SourceFileName = sourceFileName;
            _InstallerInfo = info;
        }

        public InstallFile(string fileName, string filePath)
        {
            _Name = fileName;
            _Path = filePath;
        }

        public string Action
        {
            get
            {
                return _Action;
            }
            set
            {
                _Action = value;
            }
        }

        public string BackupFileName
        {
            get
            {
                return System.IO.Path.Combine(BackupPath, Name + ".config");
            }
        }

        public virtual string BackupPath
        {
            get
            {
                return System.IO.Path.Combine(InstallerInfo.TempInstallFolder, System.IO.Path.Combine("Backup", Path));
            }
        }

        public TextEncoding Encoding
        {
            get
            {
                return _Encoding;
            }
        }

        public string Extension
        {
            get
            {
                string ext = System.IO.Path.GetExtension(_Name);
                if (String.IsNullOrEmpty(ext))
                {
                    return "";
                }
                else
                {
                    return ext.Substring(1);
                }
            }
        }

        public string FullName
        {
            get
            {
                return System.IO.Path.Combine(_Path, _Name);
            }
        }

        [Browsable(false)]
        public InstallerInfo InstallerInfo
        {
            get
            {
                return _InstallerInfo;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public string Path
        {
            get
            {
                return _Path;
            }
        }

        public string SourceFileName
        {
            get
            {
                return _SourceFileName;
            }
        }

        public string TempFileName
        {
            get
            {
                string fileName = SourceFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = FullName;
                }
                return System.IO.Path.Combine(InstallerInfo.TempInstallFolder, fileName);
            }
        }

        public InstallFileType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public Version Version
        {
            get
            {
                return _Version;
            }
        }

        private TextEncoding GetTextEncodingType(byte[] Buffer)
        {
            if (Buffer[0] == 255 && Buffer[1] == 254)
            {
                return TextEncoding.UTF16LittleEndian;
            }
            if (Buffer[0] == 254 && Buffer[1] == 255)
            {
                return TextEncoding.UTF16BigEndian;
            }
            if (Buffer[0] == 239 && Buffer[1] == 187 && Buffer[2] == 191)
            {
                return TextEncoding.UTF8;
            }
            int i;
            for (i = 0; i <= 100; i++)
            {
                if (Buffer[i] > 127)
                {
                    return TextEncoding.Unknown;
                }
            }
            return TextEncoding.UTF7;
        }

        private void ParseFileName(string fileName)
        {
            int i = fileName.Replace("\\", "/").LastIndexOf("/");
            if (i < 0)
            {
                _Name = fileName.Substring(0, fileName.Length);
                _Path = "";
            }
            else
            {
                _Name = fileName.Substring(i + 1, fileName.Length - (i + 1));
                _Path = fileName.Substring(0, i);
            }
            if (string.IsNullOrEmpty(_Path) && fileName.StartsWith("[app_code]"))
            {
                _Name = fileName.Substring(10, fileName.Length - 10);
                _Path = fileName.Substring(0, 10);
            }
            if (_Name.ToLower() == "manifest.xml")
            {
                _Type = InstallFileType.Manifest;
            }
            else
            {
                switch (Extension.ToLower())
                {
                    case "ascx":
                        _Type = InstallFileType.Ascx;
                        break;
                    case "dll":
                        _Type = InstallFileType.Assembly;
                        break;
                    case "dnn":
                    case "dnn5":
                        _Type = InstallFileType.Manifest;
                        break;
                    case "resx":
                        _Type = InstallFileType.Language;
                        break;
                    case "resources":
                    case "zip":
                        _Type = InstallFileType.Resources;
                        break;
                    default:
                        if (Extension.ToLower().EndsWith("dataprovider") || Extension.ToLower() == "sql")
                        {
                            _Type = InstallFileType.Script;
                        }
                        else if (_Path.StartsWith("[app_code]"))
                        {
                            _Type = InstallFileType.AppCode;
                        }
                        else
                        {
                            if (Regex.IsMatch(_Name, Util.REGEX_Version + ".txt"))
                            {
                                _Type = InstallFileType.CleanUp;
                            }
                            else
                            {
                                _Type = InstallFileType.Other;
                            }
                        }
                        break;
                }
            }
            _Path = _Path.Replace("[app_code]", "");
            if (_Path.StartsWith("\\"))
            {
                _Path = _Path.Substring(1);
            }
        }

        private void ReadZip(ZipInputStream unzip, ZipEntry entry)
        {
            ParseFileName(entry.Name);
            Util.WriteStream(unzip, TempFileName);
        }

        public void SetVersion(Version version)
        {
            _Version = version;
        }
    }
}
