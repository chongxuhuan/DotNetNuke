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

using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Modules.Admin.AppGallery
{
    public partial class AppGalleryDownload : ModuleUserControlBase
    {
        public static int BufferSize = 1024;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //if (!UserInfo.IsSuperUser)
            //{
            //    Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            //}

            string catalogAction = Request.QueryString["action"];
            string downloadURL = Request.QueryString["downloadURL"];

            string extensionFolder = GetInstallationFolder(Request.QueryString["eType"]);
            string installFolder = HttpContext.Current.Server.MapPath("~/Install/") + extensionFolder;

            if (downloadURL.Contains("codeplex.com"))
            {
                ProcessCodeplex(downloadURL, installFolder, catalogAction);
            }
            if (downloadURL.Contains("snowcovered.com"))
            {
                ProcessSnowcovered(downloadURL, installFolder, catalogAction);
            }
        }

        private void ProcessSnowcovered(string downloadURL, string installFolder, string catalogAction)
        {
            WebResponse wr;
            string myfile = "";

            wr = HTTPAsWebResponse(downloadURL,
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
            WebResponse wr;
            string myfile = "";

            wr = HTTPAsWebResponse(downloadURL,
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


        public static WebResponse HTTPAsWebResponse(string URL, byte[] Data, string Username, string Password, string Domain, string ProxyAddress, int ProxyPort, bool DoPOST, string UserAgent,
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
                case "module":
                    extensionFolder = "Module";
                    break;
                case "Provider":
                    extensionFolder = "Provider";
                    break;
                case "skin":
                    extensionFolder = "skin";
                    break;
                case "Skin Object":
                    extensionFolder = "skin";
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

     
    }
}