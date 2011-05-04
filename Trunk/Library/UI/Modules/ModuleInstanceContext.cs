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
using System.Collections;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;
using System.Web.UI;
using System.IO;

#endregion

namespace DotNetNuke.UI.Modules
{
    /// <summary>
    /// Provides context data for a particular instance of a module
    /// </summary>
    public class ModuleInstanceContext
    {
        #region Private Members

        private readonly IModuleControl _moduleControl;
        private ModuleActionCollection _actions;
        private ModuleInfo _configuration;
        private bool? _isEditable;
        private int _nextActionId = -1;
        private Hashtable _settings;

        #endregion

        #region Constructors

        public ModuleInstanceContext()
        {
        }

        public ModuleInstanceContext(IModuleControl moduleControl)
        {
            _moduleControl = moduleControl;
        }

        #endregion

        #region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Actions for this module context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleActionCollection Actions
        {
            get
            {
                if (_actions == null)
                {
                    LoadActions(HttpContext.Current.Request);
                }
                return _actions;
            }
            set
            {
                _actions = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Module Configuration (ModuleInfo) for this context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleInfo Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The EditMode property is used to determine whether the user is in the
        /// Administrator role
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool EditMode
        {
            get
            {
                return TabPermissionController.CanAdminPage();
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets and sets the HelpUrl for this context
 /// </summary>
 /// <history>
 ///     [cnurse]    01/04/2008  Documented
 /// </history>
 /// -----------------------------------------------------------------------------
        public string HelpURL { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the module is Editable (in Admin mode)
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsEditable
        {
            get
            {
                if (!_isEditable.HasValue)
                {
                    bool blnPreview = (PortalSettings.UserMode == PortalSettings.Mode.View);
                    if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                    {
                        blnPreview = false;
                    }
                    bool blnHasModuleEditPermissions = false;
                    if (_configuration != null)
                    {
                        blnHasModuleEditPermissions = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "CONTENT", Configuration);
                    }
                    if (blnPreview == false && blnHasModuleEditPermissions)
                    {
                        _isEditable = true;
                    }
                    else
                    {
                        _isEditable = false;
                    }
                }
                return _isEditable.Value;
            }
        }

        public bool IsHostMenu
        {
            get
            {
                bool _IsHost = false;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    _IsHost = true;
                }
                return _IsHost;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the module ID for this context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int ModuleId
        {
            get
            {
                if (_configuration != null)
                {
                    return _configuration.ModuleID;
                }
                else
                {
                    return Null.NullInteger;
                }
            }
            set
            {
                if (_configuration != null)
                {
                    _configuration.ModuleID = value;
                }
            }
        }

        public PortalAliasInfo PortalAlias
        {
            get
            {
                return PortalSettings.PortalAlias;
            }
        }

        public int PortalId
        {
            get
            {
                return PortalSettings.PortalId;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the settings for this context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public Hashtable Settings
        {
            get
            {
                var controller = new ModuleController();
                if (_settings == null)
                {
                    _settings = new Hashtable(controller.GetModuleSettings(ModuleId));
                    Hashtable tabModuleSettings = controller.GetTabModuleSettings(TabModuleId);
                    foreach (string strKey in tabModuleSettings.Keys)
                    {
                        _settings[strKey] = tabModuleSettings[strKey];
                    }
                }
                return _settings;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the tab ID for this context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int TabId
        {
            get
            {
                if (_configuration != null)
                {
                    return Convert.ToInt32(_configuration.TabID);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the tabnmodule ID for this context
        /// </summary>
        /// <history>
        ///     [cnurse]    01/04/2008  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int TabModuleId
        {
            get
            {
                if (_configuration != null)
                {
                    return Convert.ToInt32(_configuration.TabModuleID);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
            set
            {
                if (_configuration != null)
                {
                    _configuration.TabModuleID = value;
                }
            }
        }

        #endregion

        #region Private Methods

        private void AddHelpActions()
        {
            var helpAction = new ModuleAction(GetNextActionID());
            helpAction.Title = Localization.GetString(ModuleActionType.ModuleHelp, Localization.GlobalResourceFile);
            helpAction.CommandName = ModuleActionType.ModuleHelp;
            helpAction.CommandArgument = "";
            helpAction.Icon = "action_help.gif";
            helpAction.Url = Globals.NavigateURL(TabId, "Help", "ctlid=" + Configuration.ModuleControlId, "moduleid=" + ModuleId);
            helpAction.Secure = SecurityAccessLevel.Edit;
            helpAction.Visible = true;
            helpAction.NewWindow = false;
            helpAction.UseActionEvent = true;
            _actions.Add(helpAction);
            string helpURL = Globals.GetOnLineHelp(Configuration.ModuleControl.HelpURL, Configuration);
            if (!string.IsNullOrEmpty(helpURL))
            {
                helpAction = new ModuleAction(GetNextActionID());
                helpAction.Title = Localization.GetString(ModuleActionType.OnlineHelp, Localization.GlobalResourceFile);
                helpAction.CommandName = ModuleActionType.OnlineHelp;
                helpAction.CommandArgument = "";
                helpAction.Icon = "action_help.gif";
                helpAction.Url = Globals.FormatHelpUrl(helpURL, PortalSettings, Configuration.DesktopModule.FriendlyName);
                helpAction.Secure = SecurityAccessLevel.Edit;
                helpAction.UseActionEvent = true;
                helpAction.Visible = true;
                helpAction.NewWindow = true;
                _actions.Add(helpAction);
            }
        }

        private void AddPrintAction()
        {
            var action = new ModuleAction(GetNextActionID());
            action.Title = Localization.GetString(ModuleActionType.PrintModule, Localization.GlobalResourceFile);
            action.CommandName = ModuleActionType.PrintModule;
            action.CommandArgument = "";
            action.Icon = "action_print.gif";
            action.Url = Globals.NavigateURL(TabId,
                                             "",
                                             "mid=" + ModuleId,
                                             "SkinSrc=" + Globals.QueryStringEncode("[G]" + SkinController.RootSkin + "/" + Globals.glbHostSkinFolder + "/" + "No Skin"),
                                             "ContainerSrc=" + Globals.QueryStringEncode("[G]" + SkinController.RootContainer + "/" + Globals.glbHostSkinFolder + "/" + "No Container"),
                                             "dnnprintmode=true");
            action.Secure = SecurityAccessLevel.Anonymous;
            action.UseActionEvent = true;
            action.Visible = true;
            action.NewWindow = true;
            _actions.Add(action);
        }

        private void AddSyndicateAction()
        {
            var action = new ModuleAction(GetNextActionID());
            action.Title = Localization.GetString(ModuleActionType.SyndicateModule, Localization.GlobalResourceFile);
            action.CommandName = ModuleActionType.SyndicateModule;
            action.CommandArgument = "";
            action.Icon = "action_rss.gif";
            action.Url = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "", "moduleid=" + ModuleId).Replace(Globals.glbDefaultPage, "RSS.aspx");
            action.Secure = SecurityAccessLevel.Anonymous;
            action.UseActionEvent = true;
            action.Visible = true;
            action.NewWindow = true;
            _actions.Add(action);
        }

        private void AddMenuMoveActions()
        {
            _actions.Add(GetNextActionID(), "~", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false);
            var MoveActionRoot = new ModuleAction(GetNextActionID(),
                                                  Localization.GetString(ModuleActionType.MoveRoot, Localization.GlobalResourceFile),
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  false,
                                                  SecurityAccessLevel.View,
                                                  true,
                                                  false);
            if (Configuration != null)
            {
                if ((Configuration.ModuleOrder != 0) && (Configuration.PaneModuleIndex > 0))
                {
                    MoveActionRoot.Actions.Add(GetNextActionID(),
                                               Localization.GetString(ModuleActionType.MoveTop, Localization.GlobalResourceFile),
                                               ModuleActionType.MoveTop,
                                               Configuration.PaneName,
                                               "action_top.gif",
                                               "",
                                               false,
                                               SecurityAccessLevel.View,
                                               true,
                                               false);
                    MoveActionRoot.Actions.Add(GetNextActionID(),
                                               Localization.GetString(ModuleActionType.MoveUp, Localization.GlobalResourceFile),
                                               ModuleActionType.MoveUp,
                                               Configuration.PaneName,
                                               "action_up.gif",
                                               "",
                                               false,
                                               SecurityAccessLevel.View,
                                               true,
                                               false);
                }
                if ((Configuration.ModuleOrder != 0) && (Configuration.PaneModuleIndex < (Configuration.PaneModuleCount - 1)))
                {
                    MoveActionRoot.Actions.Add(GetNextActionID(),
                                               Localization.GetString(ModuleActionType.MoveDown, Localization.GlobalResourceFile),
                                               ModuleActionType.MoveDown,
                                               Configuration.PaneName,
                                               "action_down.gif",
                                               "",
                                               false,
                                               SecurityAccessLevel.View,
                                               true,
                                               false);
                    MoveActionRoot.Actions.Add(GetNextActionID(),
                                               Localization.GetString(ModuleActionType.MoveBottom, Localization.GlobalResourceFile),
                                               ModuleActionType.MoveBottom,
                                               Configuration.PaneName,
                                               "action_bottom.gif",
                                               "",
                                               false,
                                               SecurityAccessLevel.View,
                                               true,
                                               false);
                }
            }
            foreach (object obj in PortalSettings.ActiveTab.Panes)
            {
                var pane = obj as string;
                if (!string.IsNullOrEmpty(pane) && !Configuration.PaneName.Equals(pane, StringComparison.InvariantCultureIgnoreCase))
                {
                    MoveActionRoot.Actions.Add(GetNextActionID(),
                                               Localization.GetString(ModuleActionType.MovePane, Localization.GlobalResourceFile) + " " + pane,
                                               ModuleActionType.MovePane,
                                               pane,
                                               "action_move.gif",
                                               "",
                                               false,
                                               SecurityAccessLevel.View,
                                               true,
                                               false);
                }
            }
            if (MoveActionRoot.Actions.Count > 0)
            {
                _actions.Add(MoveActionRoot);
            }
        }

        private static int GetActionsCount(int count, ModuleActionCollection actions)
        {
            foreach (ModuleAction action in actions)
            {
                if (action.HasChildren())
                {
                    count += action.Actions.Count;
                    count = GetActionsCount(count, action.Actions);
                }
            }
            return count;
        }

        private void LoadActions(HttpRequest request)
        {
            _actions = new ModuleActionCollection();
            int maxActionId = Null.NullInteger;
            var actionable = _moduleControl as IActionable;
            if (actionable != null)
            {
                ModuleActionCollection moduleActions = actionable.ModuleActions;
                foreach (ModuleAction action in moduleActions)
                {
                    if (ModulePermissionController.HasModuleAccess(action.Secure, "CONTENT", Configuration))
                    {
                        if (String.IsNullOrEmpty(action.Icon))
                        {
                            action.Icon = "edit.gif";
                        }
                        if (action.ID > maxActionId)
                        {
                            maxActionId = action.ID;
                        }
                        _actions.Add(action);
                    }
                }
            }
            int actionCount = GetActionsCount(_actions.Count, _actions);
            if (_nextActionId < maxActionId)
            {
                _nextActionId = maxActionId;
            }
            if (_nextActionId < actionCount)
            {
                _nextActionId = actionCount;
            }
            if (!string.IsNullOrEmpty(Configuration.DesktopModule.BusinessControllerClass))
            {
                if (Configuration.DesktopModule.IsPortable)
                {
                    if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "EXPORT", Configuration))
                    {
                        _actions.Add(GetNextActionID(),
                                     Localization.GetString(ModuleActionType.ExportModule, Localization.GlobalResourceFile),
                                     "",
                                     "",
                                     "action_export.gif",
                                     Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ExportModule", "moduleid=" + ModuleId),
                                     "",
                                     false,
                                     SecurityAccessLevel.View,
                                     true,
                                     false);
                    }
                    if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "IMPORT", Configuration))
                    {
                        _actions.Add(GetNextActionID(),
                                     Localization.GetString(ModuleActionType.ImportModule, Localization.GlobalResourceFile),
                                     "",
                                     "",
                                     "action_import.gif",
                                     Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ImportModule", "moduleid=" + ModuleId),
                                     "",
                                     false,
                                     SecurityAccessLevel.View,
                                     true,
                                     false);
                    }
                }
                if (Configuration.DesktopModule.IsSearchable && Configuration.DisplaySyndicate)
                {
                    AddSyndicateAction();
                }
            }
            string permisisonList = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE";
            if (Configuration.ModuleID > Null.NullInteger && ModulePermissionController.HasModulePermission(Configuration.ModulePermissions, permisisonList) && request.QueryString["ctl"] != "Help")
            {
                AddHelpActions();
            }
            if (Configuration.DisplayPrint)
            {
                AddPrintAction();
            }
            if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Host, "MANAGE", Configuration))
            {
                _actions.Add(GetNextActionID(),
                             Localization.GetString(ModuleActionType.ViewSource, Localization.GlobalResourceFile),
                             ModuleActionType.ViewSource,
                             "",
                             "action_source.gif",
                             Globals.NavigateURL(TabId, "ViewSource", "ctlid=" + Configuration.ModuleControlId),
                             false,
                             SecurityAccessLevel.Host,
                             true,
                             false);
            }
            if (!Globals.IsAdminControl() && ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "DELETE,MANAGE", Configuration))
            {
                _actions.Add(GetNextActionID(), "~", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false);
                if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "MANAGE", Configuration))
                {
                    _actions.Add(GetNextActionID(),
                                 Localization.GetString(ModuleActionType.ModuleSettings, Localization.GlobalResourceFile),
                                 ModuleActionType.ModuleSettings,
                                 "",
                                 "action_settings.gif",
                                 Globals.NavigateURL(TabId, "Module", "ModuleId=" + ModuleId),
                                 false,
                                 SecurityAccessLevel.Edit,
                                 true,
                                 false);
                }
                if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "DELETE", Configuration))
                {
                    _actions.Add(GetNextActionID(),
                                 Localization.GetString(ModuleActionType.DeleteModule, Localization.GlobalResourceFile),
                                 ModuleActionType.DeleteModule,
                                 Configuration.ModuleID.ToString(),
                                 "action_delete.gif",
                                 "",
                                 "confirm('" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteModule.Confirm")) + "')",
                                 false,
                                 SecurityAccessLevel.View,
                                 true,
                                 false);
                }
                if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "MANAGE", Configuration))
                {
                    _actions.Add(GetNextActionID(),
                                 Localization.GetString(ModuleActionType.ClearCache, Localization.GlobalResourceFile),
                                 ModuleActionType.ClearCache,
                                 Configuration.ModuleID.ToString(),
                                 "action_refresh.gif",
                                 "",
                                 false,
                                 SecurityAccessLevel.View,
                                 true,
                                 false);
                    AddMenuMoveActions();
                }
            }

            foreach (ModuleAction action in _actions)
            {
                if (!UIUtilities.IsLegacyUI(ModuleId, action.ControlKey, PortalId) && action.Url.Contains("ctl"))
                {
                    action.ClientScript = UrlUtils.PopUpUrl(action.Url, _moduleControl as Control, PortalSettings);
                }
            }
        }

        #endregion

        #region Public Methods

        public string EditUrl()
        {
            return EditUrl("", "", "Edit");
        }

        public string EditUrl(string ControlKey)
        {
            return EditUrl("", "", ControlKey);
        }

        public string EditUrl(string KeyName, string KeyValue)
        {
            return EditUrl(KeyName, KeyValue, "Edit");
        }

        public string EditUrl(string KeyName, string KeyValue, string ControlKey)
        {
            var parameters = new string[] {};
            return EditUrl(KeyName, KeyValue, ControlKey, parameters);
        }

        public string EditUrl(string KeyName, string KeyValue, string ControlKey, params string[] AdditionalParameters)
        {
            string key = ControlKey;
            if (string.IsNullOrEmpty(key))
            {
                key = "Edit";
            }
            string ModuleIdParam = string.Empty;
            if (Configuration != null)
            {
                ModuleIdParam = string.Format("mid={0}", Configuration.ModuleID);
            }

            if (!string.IsNullOrEmpty(KeyName) && !string.IsNullOrEmpty(KeyValue))
            {
                var parameters = new string[2 + AdditionalParameters.Length];
                parameters[0] = ModuleIdParam;
                parameters[1] = string.Format("{0}={1}", KeyName, KeyValue);
                Array.Copy(AdditionalParameters, 0, parameters, 2, AdditionalParameters.Length);
                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, parameters);
            }
            else
            {
                var parameters = new string[1 + AdditionalParameters.Length];
                parameters[0] = ModuleIdParam;
                Array.Copy(AdditionalParameters, 0, parameters, 1, AdditionalParameters.Length);
                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, parameters);
            }
        }

        public int GetNextActionID()
        {
            _nextActionId += 1;
            return _nextActionId;
        }

        #endregion

    }
}
