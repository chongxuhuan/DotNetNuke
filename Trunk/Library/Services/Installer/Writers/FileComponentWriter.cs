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

using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Installer.Packages;

#endregion

namespace DotNetNuke.Services.Installer.Writers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The FileComponentWriter class handles creating the manifest for File Component(s)
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	02/01/2008	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class FileComponentWriter
    {
        private readonly string _BasePath;
        private readonly Dictionary<string, InstallFile> _Files;
        private readonly PackageInfo _Package;
        private int _InstallOrder = Null.NullInteger;
        private int _UnInstallOrder = Null.NullInteger;

        public FileComponentWriter(string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
        {
            _Files = files;
            _BasePath = basePath;
            _Package = package;
        }

        protected virtual string CollectionNodeName
        {
            get
            {
                return "files";
            }
        }

        protected virtual string ComponentType
        {
            get
            {
                return "File";
            }
        }

        protected virtual string ItemNodeName
        {
            get
            {
                return "file";
            }
        }

        protected virtual Logger Log
        {
            get
            {
                return _Package.Log;
            }
        }

        protected virtual PackageInfo Package
        {
            get
            {
                return _Package;
            }
        }

        public int InstallOrder
        {
            get
            {
                return _InstallOrder;
            }
            set
            {
                _InstallOrder = value;
            }
        }

        public int UnInstallOrder
        {
            get
            {
                return _UnInstallOrder;
            }
            set
            {
                _UnInstallOrder = value;
            }
        }

        protected virtual void WriteCustomManifest(XmlWriter writer)
        {
        }

        protected virtual void WriteFileElement(XmlWriter writer, InstallFile file)
        {
            Log.AddInfo(string.Format(Util.WRITER_AddFileToManifest, file.Name));
            writer.WriteStartElement(ItemNodeName);
            if (!string.IsNullOrEmpty(file.Path))
            {
                string path = file.Path;
                if (!string.IsNullOrEmpty(_BasePath))
                {
                    if (file.Path.ToLowerInvariant().Contains(_BasePath.ToLowerInvariant()))
                    {
                        path = file.Path.ToLowerInvariant().Replace(_BasePath.ToLowerInvariant() + "\\", "");
                    }
                }
                writer.WriteElementString("path", path);
            }
            writer.WriteElementString("name", file.Name);
            if (!string.IsNullOrEmpty(file.SourceFileName))
            {
                writer.WriteElementString("sourceFileName", file.SourceFileName);
            }
            writer.WriteEndElement();
        }

        public virtual void WriteManifest(XmlWriter writer)
        {
            writer.WriteStartElement("component");
            writer.WriteAttributeString("type", ComponentType);
            if (InstallOrder > Null.NullInteger)
            {
                writer.WriteAttributeString("installOrder", InstallOrder.ToString());
            }
            if (UnInstallOrder > Null.NullInteger)
            {
                writer.WriteAttributeString("unInstallOrder", UnInstallOrder.ToString());
            }
            writer.WriteStartElement(CollectionNodeName);
            WriteCustomManifest(writer);
            if (!string.IsNullOrEmpty(_BasePath))
            {
                writer.WriteElementString("basePath", _BasePath);
            }
            foreach (InstallFile file in _Files.Values)
            {
                WriteFileElement(writer, file);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
