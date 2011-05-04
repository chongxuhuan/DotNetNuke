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
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Authentication;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The AuthenticationInstaller installs Authentication Service Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/25/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class AuthenticationInstaller : ComponentInstallerBase
    {
        private AuthenticationInfo AuthSystem;
        private AuthenticationInfo TempAuthSystem;

        public override string AllowableFiles
        {
            get
            {
                return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html";
            }
        }

        private void DeleteAuthentiation()
        {
            try
            {
                AuthenticationInfo authSystem = AuthenticationController.GetAuthenticationServiceByPackageID(Package.PackageID);
                if (authSystem != null)
                {
                    AuthenticationController.DeleteAuthentication(authSystem);
                }
                Log.AddInfo(string.Format(Util.AUTHENTICATION_UnRegistered, authSystem.AuthenticationType));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Commit()
        {
        }

        public override void Install()
        {
            bool bAdd = Null.NullBoolean;
            try
            {
                TempAuthSystem = AuthenticationController.GetAuthenticationServiceByType(AuthSystem.AuthenticationType);
                if (TempAuthSystem == null)
                {
                    AuthSystem.IsEnabled = true;
                    bAdd = true;
                }
                else
                {
                    AuthSystem.AuthenticationID = TempAuthSystem.AuthenticationID;
                    AuthSystem.IsEnabled = TempAuthSystem.IsEnabled;
                }
                AuthSystem.PackageID = Package.PackageID;
                if (bAdd)
                {
                    AuthenticationController.AddAuthentication(AuthSystem);
                }
                else
                {
                    AuthenticationController.UpdateAuthentication(AuthSystem);
                }
                Completed = true;
                Log.AddInfo(string.Format(Util.AUTHENTICATION_Registered, AuthSystem.AuthenticationType));
            }
            catch (Exception ex)
            {
            
                Log.AddFailure(ex);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            AuthSystem = new AuthenticationInfo();
            AuthSystem.AuthenticationType = Util.ReadElement(manifestNav, "authenticationService/type", Log, Util.AUTHENTICATION_TypeMissing);
            AuthSystem.SettingsControlSrc = Util.ReadElement(manifestNav, "authenticationService/settingsControlSrc", Log, Util.AUTHENTICATION_SettingsSrcMissing);
            AuthSystem.LoginControlSrc = Util.ReadElement(manifestNav, "authenticationService/loginControlSrc", Log, Util.AUTHENTICATION_LoginSrcMissing);
            AuthSystem.LogoffControlSrc = Util.ReadElement(manifestNav, "authenticationService/logoffControlSrc");
            if (Log.Valid)
            {
                Log.AddInfo(Util.AUTHENTICATION_ReadSuccess);
            }
        }

        public override void Rollback()
        {
            if (TempAuthSystem == null)
            {
                DeleteAuthentiation();
            }
            else
            {
                AuthenticationController.UpdateAuthentication(TempAuthSystem);
            }
        }

        public override void UnInstall()
        {
            DeleteAuthentiation();
        }
    }
}
