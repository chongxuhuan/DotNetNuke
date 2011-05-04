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
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Languages
{
    public partial class LanguageEditorExt : PortalModuleBase
    {
        private string highlight;
        private string locale;
        private string mode;
        private string resfile;

        protected string ReturnUrl
        {
            get
            {
                return Globals.NavigateURL("", "ctl=Editor", "mid=" + ModuleId, "Locale=" + locale, "ResourceFile=" + Globals.QueryStringEncode(resfile), "Mode=" + mode, "Highlight=" + highlight);
            }
        }

        private string LoadFile(string mode, string type)
        {
            string file = "";
            string t = "";
            string temp;
            switch (type)
            {
                case "Edit":
                    file = ResourceFile(locale, mode);
                    temp = LoadResource(file);
                    if (temp != null)
                    {
                        t = temp;
                    }
                    break;
                case "Default":
                    file = ResourceFile(Localization.SystemLocale, "System");
                    t = LoadResource(file);
                    switch (mode)
                    {
                        case "Host":
                            if (locale != Localization.SystemLocale)
                            {
                                file = ResourceFile(locale, "System");
                                temp = LoadResource(file);
                                if (temp != null)
                                {
                                    t = temp;
                                }
                            }
                            break;
                        case "Portal":
                            file = ResourceFile(Localization.SystemLocale, "Host");
                            temp = LoadResource(file);
                            if (temp != null)
                            {
                                t = temp;
                            }
                            if (locale != Localization.SystemLocale)
                            {
                                file = ResourceFile(locale, "System");
                                temp = LoadResource(file);
                                if (temp != null)
                                {
                                    t = temp;
                                }
                                file = ResourceFile(locale, "Host");
                                temp = LoadResource(file);
                                if (temp != null)
                                {
                                    t = temp;
                                }
                            }
                            break;
                    }
                    break;
            }
            return t;
        }

        private string LoadResource(string filepath)
        {
            var d = new XmlDocument();
            bool xmlLoaded = false;
            string ret = null;
            try
            {
                d.Load(filepath);
                xmlLoaded = true;
            }
            catch
            {
                xmlLoaded = false;
            }
            if (xmlLoaded)
            {
                XmlNode node;
                node = d.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value");
                if (node != null)
                {
                    ret = node.InnerXml;
                }
            }
            return ret;
        }

        private string ResourceFile(string language, string mode)
        {
            return Localization.GetResourceFileName(Server.MapPath("~\\" + resfile), language, mode, PortalId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdUpdate.Click += cmdUpdate_Click;

            var resDoc = new XmlDocument();
            var defDoc = new XmlDocument();
            try
            {
                string defaultValue;
                string editValue;
                resfile = Globals.QueryStringDecode(Request.QueryString["resourcefile"]);
                locale = Request.QueryString["locale"];
                mode = Request.QueryString["mode"];
                highlight = Request.QueryString["highlight"];
                lblName.Text = Request.QueryString["name"];
                lblFile.Text = ResourceFile(locale, mode).Replace(Globals.ApplicationMapPath, "").Replace("\\", "/");
                if (!Page.IsPostBack)
                {
                    defaultValue = LoadFile(mode, "Default");
                    editValue = LoadFile(mode, "Edit");
                    if (string.IsNullOrEmpty(editValue))
                    {
                        editValue = defaultValue;
                    }
                    teContent.Text = editValue;
                    lblDefault.Text = Server.HtmlDecode(defaultValue);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdCancel_Click(Object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(ReturnUrl, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            XmlNode node;
            XmlNode nodeData;
            XmlNode parent;
            XmlAttribute attr;
            var resDoc = new XmlDocument();
            string filename;
            bool IsNewFile = false;
            try
            {
                if (String.IsNullOrEmpty(teContent.Text))
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("RequiredField.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                    return;
                }
                filename = ResourceFile(locale, mode);
                if (!File.Exists(filename))
                {
                    resDoc.Load(ResourceFile(Localization.SystemLocale, "System"));
                    IsNewFile = true;
                }
                else
                {
                    resDoc.Load(filename);
                }
                switch (mode)
                {
                    case "System":
                        node = resDoc.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value");
                        if (node == null)
                        {
                            nodeData = resDoc.CreateElement("data");
                            attr = resDoc.CreateAttribute("name");
                            attr.Value = lblName.Text;
                            nodeData.Attributes.Append(attr);
                            resDoc.SelectSingleNode("//root").AppendChild(nodeData);
                            node = nodeData.AppendChild(resDoc.CreateElement("value"));
                        }
                        node.InnerXml = teContent.Text;
                        resDoc.Save(filename);
                        break;
                    case "Host":
                    case "Portal":
                        if (IsNewFile)
                        {
                            if (teContent.Text != lblDefault.Text)
                            {
                                foreach (XmlNode n in resDoc.SelectNodes("//root/data"))
                                {
                                    parent = n.ParentNode;
                                    parent.RemoveChild(n);
                                }
                                nodeData = resDoc.CreateElement("data");
                                attr = resDoc.CreateAttribute("name");
                                attr.Value = lblName.Text;
                                nodeData.Attributes.Append(attr);
                                resDoc.SelectSingleNode("//root").AppendChild(nodeData);
                                node = nodeData.AppendChild(resDoc.CreateElement("value"));
                                node.InnerXml = teContent.Text;
                                resDoc.Save(filename);
                            }
                        }
                        else
                        {
                            node = resDoc.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value");
                            if (teContent.Text != lblDefault.Text)
                            {
                                if (node == null)
                                {
                                    nodeData = resDoc.CreateElement("data");
                                    attr = resDoc.CreateAttribute("name");
                                    attr.Value = lblName.Text;
                                    nodeData.Attributes.Append(attr);
                                    resDoc.SelectSingleNode("//root").AppendChild(nodeData);
                                    node = nodeData.AppendChild(resDoc.CreateElement("value"));
                                }
                                node.InnerXml = teContent.Text;
                            }
                            else if (node != null)
                            {
                                resDoc.SelectSingleNode("//root").RemoveChild(node.ParentNode);
                            }
                            if (resDoc.SelectNodes("//root/data").Count > 0)
                            {
                                resDoc.Save(filename);
                            }
                            else if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }
                        }
                        break;
                }
                Response.Redirect(ReturnUrl, true);
            }
            catch (Exception)
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("Save.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }
    }
}