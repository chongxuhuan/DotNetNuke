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
           
                GetSnowcoveredFiles();
            CheckSnowcoveredConnection();

        }

        public void CheckSnowcoveredConnection()
        {
            snowcoveredLogin.Visible = false;
            deleteCredentials.Visible = false;
            fetchExtensions.Visible = false;
            snowcoveredLogin.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "AppGallerySnowcovered",
                                                                        true);
            deleteCredentials.NavigateUrl = ModuleContext.NavigateUrl(ModuleContext.TabId, "AppGallerySnowcovered",
                                                                        true);
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(ModuleContext.PortalId);
            if (settings.ContainsKey("Snowcovered_Username"))
            {
                fetchExtensions.Visible = true;
                snowcoveredLogin.Visible = true;
            }
            else
            {
                deleteCredentials.Visible = true;
            }

        }

        private void GetSnowcoveredFiles()
        {
            string fileCheck = Localization.GetString("SnowCoveredFile", LocalResourceFile);
            string strPost = "";
            bool blnValid = false;
                    
					//reconstruct post for postback validation
					strPost += string.Format("&{0}={1}", Globals.HTTPPOSTEncode("username"), Globals.HTTPPOSTEncode(""));
                    strPost += string.Format("&{0}={1}", Globals.HTTPPOSTEncode("password"), Globals.HTTPPOSTEncode(""));
            
                    var objRequest = (HttpWebRequest) WebRequest.Create(fileCheck.ToString());
                    objRequest.Method = "POST";
                    objRequest.ContentLength = strPost.Length;
                    objRequest.ContentType = "application/x-www-form-urlencoded";
                    using (var objStream = new StreamWriter(objRequest.GetRequestStream()))
                    {
                        objStream.Write(strPost);
                    }

                    string strResponse;
                    using (var objResponse = (HttpWebResponse) objRequest.GetResponse())
                    {
                        using (var sr = new StreamReader(objResponse.GetResponseStream()))
                        {
                            strResponse = sr.ReadToEnd();
                        }
                    }
                    switch (strResponse)
                    {
                        case null:
                            //failure to connect/validate credentials/no data
                            blnValid = false;
                            break;
                        default:
							blnValid = true;
                            break;
                    }
           
            //  returnText=@"<orders><order orderid=""311326"" orderdate=""2011-03-21T14:12:23""><orderdetails><orderdetail packageid=""20524"" optionid=""19366"" packagename=""FREE Synapse 2 & Skin Tuner / 5 Colors / jQuery Banner (New)"" optionname=""Free Synapse & Skin Tuner""><files>  <file fileid=""68966"" filename=""Please Read Download Instructions.zip"" deploy=""false"" />   </files>  </orderdetail>  </orderdetails>  </order></orders>";

            
            if (blnValid)
            {
                XmlTextReader oReader = new XmlTextReader(strResponse);
        
                grdSnow.DataSource = oReader;
                grdSnow.DataBind();    
            }
        }

        protected string GetLocalizedString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }
       
    }
}