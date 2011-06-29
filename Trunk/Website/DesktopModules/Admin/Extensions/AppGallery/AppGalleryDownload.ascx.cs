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
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.AppGallery
{
    public partial class AppGalleryDownload : ModuleUserControlBase
    {
        public static int BufferSize = 1024;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cancel.Click +=new EventHandler(cancel_Click);
            deployExtension.Click +=deployExtension_Click;
            downloadExtension.Click +=downloadExtension_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                string extensionId = Request.QueryString["ExtensionID"];
                string extensionRequest = "http://appgallery.dotnetnuke.com" + "/AppGalleryService.svc/Extensions(" + extensionId + ")";
            

                XmlDocument xmlDoc = new XmlDocument();

                string xml = GetOData(extensionRequest);

                XmlNamespaceManager xmlNsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsMgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                xmlNsMgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                xmlNsMgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
           

                xmlDoc.LoadXml(xml);

                XmlNodeList elements = xmlDoc.DocumentElement.SelectNodes("/atom:entry", xmlNsMgr);
                string extName="";
                string extType = "";
                string extDesc = "";
                string extURL = "";
                string extCatalogID = "";
                foreach (XmlNode element in elements)
                {
           
                    XmlNodeList properties = element.SelectSingleNode("./atom:content/m:properties", xmlNsMgr).ChildNodes;
                
                    foreach (XmlNode property in properties)
                    {

                        string propertyName = property.LocalName;
                        switch (propertyName)
                        {
                            case "ExtensionName":
                                extName = property.InnerText;
                                ViewState["extName"] = extName;
                                break;
                            case "ExtensionType":
                                extType = property.InnerText;
                                ViewState["extType"] = extType;
                                break;
                            case "Description":
                                extDesc = property.InnerText;
                                break;
                            case "DownloadURL":
                                extURL = property.InnerText;
                                ViewState["extURL"] = extURL;
                                break;
                            case "CatalogID":
                                extCatalogID = property.InnerText;
                                break;
                            default:
                                break;   
                        }
                    }

                }

                extensionType.Text = extType;
                extensionName.Text = extName;
                extensionDesc.Text = extDesc;

                if (extURL == "")
                {
                    UI.Skins.Skin.AddModuleMessage(this, GetString("unexpectedRequest"), ModuleMessage.ModuleMessageType.RedError);
                    return;
                }
                downloadExtension.Visible = true;
                if (extCatalogID=="2")
                {
                    deployExtension.Visible = true;
                
                }
            }

       }

        private void ProcessRequest(string action,bool doInstall)
        {
             string downloadURL = ViewState["extURL"].ToString();
 
            string extensionFolder= GetInstallationFolder(ViewState["extType"].ToString());
            string installFolder = HttpContext.Current.Server.MapPath("~/Install/") + extensionFolder;

            bool unknownCatalog = true;

            if (downloadURL.Contains("codeplex.com"))
            {
                ProcessCodeplex(downloadURL, installFolder, action);
                unknownCatalog = false;
            }
            if (downloadURL.Contains("snowcovered.com"))
            {
                ProcessSnowcovered(downloadURL, installFolder, action);
                unknownCatalog = false;
            }
            if (unknownCatalog)
            {
                ProcessUnknown(downloadURL, installFolder, action);
            }
}

        private string GetOData(string extensionRequest)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(extensionRequest));
            request.Method = "GET";
            request.Accept = "application/atom+xml";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader readStream = new StreamReader(
                    response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    return readStream.ReadToEnd();
                }
            }
 
        }

        private void ProcessUnknown(string downloadURL, string installFolder, string catalogAction)
        {
            WebResponse wr;
            string myfile = "";

            wr = HttpAsWebResponse(downloadURL,
                                   null,
                                   null,
                                   null,
                                   null,
                                   null,
                                   -1,
                                   false,
                                   "DotNetNuke-Appgallery/1.0.0.0(Microsoft Windows NT 6.1.7600.0",
                                   "wpi://2.1.0.0/Microsoft Windows NT 6.1.7600.0",
                                   out myfile);
            DownloadDeploy(wr, myfile, installFolder, catalogAction);
        }

        private void ProcessSnowcovered(string downloadURL, string installFolder, string catalogAction)
        {
            WebResponse wr;
            string myfile = "";

            wr = HttpAsWebResponse(downloadURL,
                                   null,
                                   null,
                                   null,
                                   null,
                                   null,
                                   -1,
                                   false,
                                   "DotNetNuke-Appgallery/1.0.0.0(Microsoft Windows NT 6.1.7600.0",
                                   "wpi://2.1.0.0/Microsoft Windows NT 6.1.7600.0",
                                   out myfile);
            DownloadDeploy(wr, myfile, installFolder, catalogAction);
        }

        private void ProcessCodeplex(string downloadURL, string installFolder, string catalogAction)
        {
            string myfile = "";
            try
            {
                WebResponse wr;
                Uri url = new Uri(downloadURL);
                string host = url.Host;

                //convert path to download version
                string directdownloadURL = "";
                if (downloadURL.Contains("#DownloadId="))
                {
                    int start = downloadURL.IndexOf("DownloadId=");
                    directdownloadURL = "http://" + host + "/Project/Download/FileDownload.aspx?" + downloadURL.Substring(start);
                }
                else
                {
                    directdownloadURL = downloadURL;
                }
                wr = HttpAsWebResponse(directdownloadURL,
                                       null,
                                       null,
                                       null,
                                       null,
                                       null,
                                       -1,
                                       false,
                                       "DotNetNuke-Appgallery/1.0.0.0(Microsoft Windows NT 6.1.7600.0",
                                       "wpi://2.1.0.0/Microsoft Windows NT 6.1.7600.0",
                                       out myfile);
                DownloadDeploy(wr, myfile, installFolder, catalogAction);
                UI.Skins.Skin.AddModuleMessage(this, String.Format(GetString("deploySuccess"), ViewState["extName"]), ModuleMessage.ModuleMessageType.GreenSuccess);

                installExtension.NavigateUrl = Util.InstallURL(ModuleContext.TabId, "", ViewState["extType"].ToString(), myfile.ToString());
                installExtension.Visible = true;
                deployExtension.Visible = false;
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
 
        }

        private void DownloadDeploy(WebResponse wr, string myfile, string installFolder, string catalogAction)
        {
            if (catalogAction == "download")
            {
                HttpResponse objResponse = HttpContext.Current.Response;
                objResponse.ClearContent();
                objResponse.ClearHeaders();
                objResponse.AppendHeader("content-disposition", "attachment; filename=\"" + myfile + "\"");
                objResponse.AppendHeader("Content-Length", wr.ContentLength.ToString());
                objResponse.ContentType = wr.ContentType;
                objResponse.Write(wr.GetResponseStream());
                objResponse.Flush();
                objResponse.End();
            }
            else
            {
                WebResponse response = null;
                Stream remoteStream = null;
                Stream localStream = null;

                try
                {
                    // Once the WebResponse object has been retrieved,
                    // get the stream object associated with the response's data
                    remoteStream = wr.GetResponseStream();

                    // Create the local file
                    localStream = File.Create(installFolder + "/" + myfile);

                    // Allocate a 1k buffer
                    var buffer = new byte[1024];
                    int bytesRead;

                    // Simple do/while loop to read from stream until
                    // no bytes are returned
                    do
                    {
                        // Read data (up to 1k) from the stream
                        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                        // Write the data to the local file
                        localStream.Write(buffer, 0, bytesRead);

                        // Increment total bytes processed
                        //TODO fix this line bytesProcessed += bytesRead;
                    } while (bytesRead > 0);
                }
                finally
                {
                    // Close the response and streams objects here 
                    // to make sure they're closed even if an exception
                    // is thrown at some point
                    if (response != null)
                    {
                        response.Close();
                    }
                    if (remoteStream != null)
                    {
                        remoteStream.Close();
                    }
                    if (localStream != null)
                    {
                        localStream.Close();
                    }
                }


            }
        }


        public static WebResponse HttpAsWebResponse(string URL, byte[] Data, string Username, string Password, string Domain, string ProxyAddress, int ProxyPort, bool DoPOST, string UserAgent,
                                                    string Referer, out string Filename)
        {
            if (!DoPOST && Data != null && Data.Length > 0)
            {
                string restoftheurl = Encoding.ASCII.GetString(Data);

                if (URL.IndexOf("?") <= 0)
                {
                    URL = URL + "?";
                }

                URL = URL + restoftheurl;
            }

            var wreq = (HttpWebRequest)WebRequest.Create(URL);
            wreq.UserAgent = UserAgent;
            wreq.Referer = Referer;
            wreq.Method = "GET";
            if (DoPOST)
            {
                wreq.Method = "POST";
            }

            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            
            wreq.Timeout = Host.WebRequestTimeout;
            
            if (!string.IsNullOrEmpty(Host.ProxyServer))
            {
                WebProxy Proxy;
                NetworkCredential ProxyCredentials;
                Proxy = new WebProxy(Host.ProxyServer, Host.ProxyPort);
                if (!string.IsNullOrEmpty(Host.ProxyUsername))
                {
                    ProxyCredentials = new NetworkCredential(Host.ProxyUsername, Host.ProxyPassword);
                    Proxy.Credentials = ProxyCredentials;
                }
                wreq.Proxy = Proxy;
            }
            

            //if (ProxyAddress != null && ProxyAddress.Trim() != "" && ProxyPort > 0)
            //{
            //    var webProxy = new WebProxy(ProxyAddress, ProxyPort);

            //    webProxy.BypassProxyOnLocal = true;

            //    wreq.Proxy = webProxy;
            //}
            //else
            //{
            //    wreq.Proxy = WebProxy.GetDefaultProxy();
            //}
            if (Username != null && Password != null && Domain != null && Username.Trim() != "" && Password.Trim() != null && Domain.Trim() != null)
            {
                wreq.Credentials = new NetworkCredential(Username, Password, Domain);
            }
            else if (Username != null && Password != null && Username.Trim() != "" && Password.Trim() != null)
            {
                wreq.Credentials = new NetworkCredential(Username, Password);
            }

            if (DoPOST && Data != null && Data.Length > 0)
            {
                wreq.ContentType = "application/x-www-form-urlencoded";
                Stream request = wreq.GetRequestStream();
                request.Write(Data, 0, Data.Length);
                request.Close();
            }
            Filename = "";
            WebResponse wrsp = wreq.GetResponse();
            string cd = wrsp.Headers["Content-Disposition"];
            if (cd != null && cd.Trim() != "" && cd.StartsWith("attachment"))
            {
                Filename = cd.Substring(cd.IndexOf("\"")).Replace("\"", "");
            }
            return wrsp;
        }


        private string GetInstallationFolder(string extensionType)
        {
            string extensionFolder = "";


            switch (extensionType)
            {
                case "Library":
                    extensionFolder = "Module";
                    break;
                case "Module":
                    extensionFolder = "Module";
                    break;
                case "Provider":
                    extensionFolder = "Provider";
                    break;
                case "Skin":
                    extensionFolder = "Skin";
                    break;
                case "Skin Object":
                    extensionFolder = "Skin";
                    break;
                case "Widget":
                    extensionFolder = "Module";
                    break;
                case "Other":
                    extensionFolder = "Module";
                    break;
                //default:
                //    extensionFolder = "Module";
            }
            return extensionFolder;
        }

        protected string GetString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }

        protected void downloadExtension_Click(object sender, EventArgs e)
        {
            ProcessRequest("download",false);
        }

        protected void deployExtension_Click(object sender, EventArgs e)
        {
            ProcessRequest("deploy", false);
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }
}
}