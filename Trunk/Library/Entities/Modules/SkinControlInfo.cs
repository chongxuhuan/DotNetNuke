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
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Entities.Modules
{
    [Serializable]
    public class SkinControlInfo : ControlInfo, IXmlSerializable, IHydratable
    {
        public SkinControlInfo()
        {
            PackageID = Null.NullInteger;
            SkinControlID = Null.NullInteger;
        }

        public int SkinControlID { get; set; }

        public int PackageID { get; set; }

        #region IHydratable Members

        public void Fill(IDataReader dr)
        {
            SkinControlID = Null.SetNullInteger(dr["SkinControlID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            FillInternal(dr);
        }

        public int KeyID
        {
            get
            {
                return SkinControlID;
            }
            set
            {
                SkinControlID = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (reader.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                ReadXmlInternal(reader);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("moduleControl");
            WriteXmlInternal(writer);
            writer.WriteEndElement();
        }

        #endregion
    }
}
