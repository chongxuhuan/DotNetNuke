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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;

using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.Xml;
using DotNetNuke.Entities.Host;

#endregion

namespace DotNetNuke.Modules.Admin.Extensions
{
    public partial class PurchasedExtensions : ModuleUserControlBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            fetchExtensions.Click += fetchExtensions_Click;
        }

        protected void fetchExtensions_Click(object sender, EventArgs e)
        {
            if (CheckCanCallSnowcovered())
            {
                GetSnowcoveredFiles();
            }

        }

        private void GetSnowcoveredFiles()
        {
            string fileCheck = Localization.GetString("SnowCoveredFile", LocalResourceFile);
            HttpWebRequest oRequest;
            WebResponse oResponse;

            //temp code for snowcovered feed issue
            StreamReader sReader;

            string sRequest = fileCheck;
                
            try
            {
                oRequest = (HttpWebRequest) GetExternalRequest(sRequest);
                oResponse = oRequest.GetResponse();
                sReader= new StreamReader(oResponse.GetResponseStream());
            }
            catch (Exception)
            {
                error.Visible = true;
                throw;
            }
            //temp code to workaround snowcovered feed issue

            string returnText = sReader.ReadToEnd();
            //  returnText=@"<orders><order orderid=""311326"" orderdate=""2011-03-21T14:12:23""><orderdetails><orderdetail packageid=""20524"" optionid=""19366"" packagename=""FREE Synapse 2 & Skin Tuner / 5 Colors / jQuery Banner (New)"" optionname=""Free Synapse & Skin Tuner""><files>  <file fileid=""68966"" filename=""Please Read Download Instructions.zip"" deploy=""false"" />   </files>  </orderdetail>  </orderdetails>  </order></orders>";

            string orderPass = "<orders>";
            if (returnText.Contains(orderPass))
            {
                XmlTextReader oReader = new XmlTextReader(returnText);

                grdSnow.DataSource = oReader;
                grdSnow.DataBind();    
            }
        }

        private bool CheckCanCallSnowcovered()
        {
            string cookieCheck = Localization.GetString("SnowcoveredCookie", LocalResourceFile);
            

            //code added until snowcovered feed is changed
            //string failCookie = null;
            string failCookie = "Authenticated: False";
            //check if logged in
            HttpWebRequest oRequest;
            
            oRequest = (HttpWebRequest) GetExternalRequest(cookieCheck);
            WebResponse oResponse;
            oResponse = oRequest.GetResponse();
            StreamReader sReader;
            sReader = new StreamReader(oResponse.GetResponseStream());
            
            string returnText = sReader.ReadToEnd();
            if (returnText.Contains(failCookie))
            {
                loginWarning.Visible = true;
                return false;
            }
            
            return true;
        }

        private HttpWebRequest GetExternalRequest(string address)
        {
            try
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                
                CookieContainer cookieJar = new CookieContainer();

                var objRequest = (HttpWebRequest)WebRequest.Create(address);
                objRequest.CookieContainer = cookieJar;
                objRequest.Timeout = Host.WebRequestTimeout;
                objRequest.UserAgent = "DotNetNuke";
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
                    objRequest.Proxy = Proxy;
                }
                return objRequest;
            }
            catch (Exception)
            {

                error.Visible = true;
                return null;
            }
            
        }

        protected string GetLocalizedString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }
       
    }
}