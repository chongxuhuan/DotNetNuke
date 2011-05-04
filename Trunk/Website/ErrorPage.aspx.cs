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
using System.Web;
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;

#endregion

namespace DotNetNuke.Services.Exceptions
{
    public partial class ErrorPage : Page
    {
        private void ManageError(string status)
        {
            string strErrorMessage = HttpUtility.HtmlEncode(Request.QueryString["error"]);
            string strLocalizedMessage = Localization.Localization.GetString(status + ".Error",
                                                                             Localization.Localization.
                                                                                 GlobalResourceFile);
            if (strLocalizedMessage != null)
            {
                strLocalizedMessage = strLocalizedMessage.Replace("src=\"images/403-3.gif\"",
                                                                  "src=\"" + ResolveUrl("~/images/403-3.gif") + "\"");
                ErrorPlaceHolder.Controls.Add(new LiteralControl(string.Format(strLocalizedMessage, strErrorMessage)));
            }

            int statusCode;
            Int32.TryParse(status, out statusCode);

            if (statusCode > -1)
                Response.StatusCode = statusCode;
        }

        [Obsolete(
            "Function obsoleted in 5.6.1 as no longer used in core - version identification can be useful to potential hackers if used incorrectly"
            )]
        public string ExtractOSVersion()
        {
            string commonName = Environment.OSVersion.ToString();
            switch (Environment.OSVersion.Version.Major)
            {
                case 5:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            commonName = "Windows 2000";
                            break;
                        case 1:
                            commonName = "Windows XP";
                            break;
                        case 2:
                            commonName = "Windows Server 2003";
                            break;
                    }
                    break;
                case 6:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            commonName = "Windows Vista";
                            break;
                        case 1:
                            commonName = "Windows Server 2008";
                            break;
                        case 2:
                            commonName = "Windows 7";
                            break;
                    }
                    break;
            }
            return commonName;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StyleSheet.Attributes["href"] = ResolveUrl("~/Install/Install.css");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string strLocalizedMessage = Null.NullString;
            var objSecurity = new PortalSecurity();
            string status = objSecurity.InputFilter(Request.QueryString["status"],
                                                    PortalSecurity.FilterFlag.NoScripting |
                                                    PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(status))
                ManageError(status);
            else
            {
                Exception exc = Server.GetLastError();
                try
                {
                    if (Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
                        ErrorPlaceHolder.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(exc.ToString())));
                    else
                    {
                        PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                        var lex = new PageLoadException(exc.Message, exc);
                        Exceptions.LogException(lex);
                        strLocalizedMessage = Localization.Localization.GetString("Error.Text",
                                                                                  Localization.Localization.
                                                                                      GlobalResourceFile);
                        ErrorPlaceHolder.Controls.Add(
                            new ErrorContainer(_portalSettings, strLocalizedMessage, lex).Container);
                    }
                } catch
                {
                    strLocalizedMessage = Localization.Localization.GetString("UnhandledError.Text",
                                                                              Localization.Localization.
                                                                                  GlobalResourceFile);
                    ErrorPlaceHolder.Controls.Add(new LiteralControl(strLocalizedMessage));
                }
            }
            strLocalizedMessage = Localization.Localization.GetString("Return.Text",
                                                                      Localization.Localization.GlobalResourceFile);
            hypReturn.Text = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " +
                             strLocalizedMessage;
        }
    }
}