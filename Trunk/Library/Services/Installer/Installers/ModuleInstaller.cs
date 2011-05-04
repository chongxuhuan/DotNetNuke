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
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.EventQueue;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ModuleInstaller installs Module Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/15/2008  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ModuleInstaller : ComponentInstallerBase
    {
        private DesktopModuleInfo _desktopModule;
        private EventMessage _eventMessage;
        private DesktopModuleInfo _installedDesktopModule;

        public override string AllowableFiles
        {
            get
            {
                return "cshtml, vbhtml, ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html, xml, psd";
            }
        }

        private void DeleteModule()
        {
            try
            {
                DesktopModuleInfo tempDesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(Package.PackageID);
                if (tempDesktopModule != null)
                {
                    if ((_desktopModule != null) && (!string.IsNullOrEmpty(_desktopModule.CodeSubDirectory)))
                    {
                        Config.RemoveCodeSubDirectory(_desktopModule.CodeSubDirectory);
                    }
                    var controller = new DesktopModuleController();
                    controller.DeleteDesktopModule(tempDesktopModule);
                }
                Log.AddInfo(string.Format(Util.MODULE_UnRegistered, tempDesktopModule.ModuleName));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Commit()
        {
            if (!string.IsNullOrEmpty(_desktopModule.CodeSubDirectory))
            {
                Config.AddCodeSubDirectory(_desktopModule.CodeSubDirectory);
            }
            if (_desktopModule.SupportedFeatures == Null.NullInteger)
            {
                //Set an Event Message so the features are loaded by reflection on restart
                var oAppStartMessage = new EventMessage();
                oAppStartMessage.Priority = MessagePriority.High;
                oAppStartMessage.ExpirationDate = DateTime.Now.AddYears(-1);
                oAppStartMessage.SentDate = DateTime.Now;
                oAppStartMessage.Body = "";
                oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke";
                oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures";

                //Add custom Attributes for this message
                oAppStartMessage.Attributes.Add("BusinessControllerClass", _desktopModule.BusinessControllerClass);
                oAppStartMessage.Attributes.Add("desktopModuleID", _desktopModule.DesktopModuleID.ToString());

                //send it to occur on next App_Start Event
                EventQueueController.SendMessage(oAppStartMessage, "Application_Start_FirstRequest");
            }
            if (_eventMessage != null)
            {
                _eventMessage.Attributes.Set("desktopModuleID", _desktopModule.DesktopModuleID.ToString());
                EventQueueController.SendMessage(_eventMessage, "Application_Start");
            }
            if (!_desktopModule.IsPremium)
            {
                DesktopModuleController.AddDesktopModuleToPortals(_desktopModule.DesktopModuleID);
            }
        }

        public override void Install()
        {
            try
            {
                _installedDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(_desktopModule.ModuleName, Package.InstallerInfo.PortalID);
                if (_installedDesktopModule != null)
                {
                    _desktopModule.DesktopModuleID = _installedDesktopModule.DesktopModuleID;
                }
                DataCache.RemoveCache(DataCache.ModuleDefinitionCacheKey);
                DataCache.RemoveCache(DataCache.ModuleControlsCacheKey);
                _desktopModule.PackageID = Package.PackageID;
                _desktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(_desktopModule, true, false);
                Completed = true;
                Log.AddInfo(string.Format(Util.MODULE_Registered, _desktopModule.ModuleName));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            _desktopModule = CBO.DeserializeObject<DesktopModuleInfo>(new StringReader(manifestNav.InnerXml));
            _desktopModule.FriendlyName = Package.FriendlyName;
            _desktopModule.Description = Package.Description;
            _desktopModule.Version = Globals.FormatVersion(Package.Version);
            _desktopModule.CompatibleVersions = Null.NullString;
            _desktopModule.Dependencies = Null.NullString;
            _desktopModule.Permissions = Null.NullString;
            if (string.IsNullOrEmpty(_desktopModule.BusinessControllerClass))
            {
                _desktopModule.SupportedFeatures = 0;
            }
            XPathNavigator eventMessageNav = manifestNav.SelectSingleNode("eventMessage");
            if (eventMessageNav != null)
            {
                _eventMessage = new EventMessage();
                _eventMessage.Priority = MessagePriority.High;
                _eventMessage.ExpirationDate = DateTime.Now.AddYears(-1);
                _eventMessage.SentDate = DateTime.Now;
                _eventMessage.Body = "";
                _eventMessage.ProcessorType = Util.ReadElement(eventMessageNav, "processorType", Log, Util.EVENTMESSAGE_TypeMissing);
                _eventMessage.ProcessorCommand = Util.ReadElement(eventMessageNav, "processorCommand", Log, Util.EVENTMESSAGE_CommandMissing);
                foreach (XPathNavigator attributeNav in eventMessageNav.Select("attributes/*"))
                {
                    _eventMessage.Attributes.Add(attributeNav.Name, attributeNav.Value);
                }
            }
            foreach (XPathNavigator moduleDefinitionNav in manifestNav.Select("desktopModule/moduleDefinitions/moduleDefinition"))
            {
                string friendlyName = Util.ReadElement(moduleDefinitionNav, "friendlyName");
                foreach (XPathNavigator permissionNav in moduleDefinitionNav.Select("permissions/permission"))
                {
                    var permission = new PermissionInfo();
                    permission.PermissionCode = Util.ReadAttribute(permissionNav, "code");
                    permission.PermissionKey = Util.ReadAttribute(permissionNav, "key");
                    permission.PermissionName = Util.ReadAttribute(permissionNav, "name");
                    ModuleDefinitionInfo moduleDefinition = _desktopModule.ModuleDefinitions[friendlyName];
                    if (moduleDefinition != null)
                    {
                        moduleDefinition.Permissions.Add(permission.PermissionKey, permission);
                    }
                }
            }
            if (Log.Valid)
            {
                Log.AddInfo(Util.MODULE_ReadSuccess);
            }
        }

        public override void Rollback()
        {
            if (_installedDesktopModule == null)
            {
                DeleteModule();
            }
            else
            {
                DesktopModuleController.SaveDesktopModule(_installedDesktopModule, true, false);
            }
        }

        public override void UnInstall()
        {
            DeleteModule();
        }
    }
}
