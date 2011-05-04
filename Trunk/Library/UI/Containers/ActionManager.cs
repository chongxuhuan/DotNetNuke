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
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Containers
    /// Class	 : ActionManager
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ActionManager is a helper class that provides common Action Behaviours that can
    /// be used by any IActionControl implementation
    /// </summary>
    /// <history>
    /// 	[cnurse]	12/23/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ActionManager
    {
        private readonly PortalSettings PortalSettings = PortalController.GetCurrentPortalSettings();
        private readonly HttpRequest Request = HttpContext.Current.Request;
        private readonly HttpResponse Response = HttpContext.Current.Response;

        public ActionManager(IActionControl actionControl)
        {
            ActionControl = actionControl;
        }

        public IActionControl ActionControl { get; set; }

        protected ModuleInstanceContext ModuleContext
        {
            get
            {
                return ActionControl.ModuleControl.ModuleContext;
            }
        }

        private void ClearCache(ModuleAction Command)
        {
            ModuleController.SynchronizeModule(ModuleContext.ModuleId);
            Response.Redirect(Request.RawUrl, true);
        }

        private void Delete(ModuleAction Command)
        {
            var objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(int.Parse(Command.CommandArgument), ModuleContext.TabId, true);
            if (objModule != null)
            {
                objModules.DeleteTabModule(ModuleContext.TabId, int.Parse(Command.CommandArgument), true);
                UserInfo m_UserInfo = UserController.GetCurrentUserInfo();
                var objEventLog = new EventLogController();
                objEventLog.AddLog(objModule, PortalSettings, m_UserInfo.UserID, "", EventLogController.EventLogType.MODULE_SENT_TO_RECYCLE_BIN);
            }
            Response.Redirect(Request.RawUrl, true);
        }

        private void DoAction(ModuleAction Command)
        {
            if (Command.NewWindow)
            {
                UrlUtils.OpenNewWindow(ActionControl.ModuleControl.Control.Page, GetType(), Command.Url);
            }
            else
            {
                Response.Redirect(Command.Url, true);
            }
        }

        private void Localize(ModuleAction Command)
        {
            var moduleCtrl = new ModuleController();
            ModuleInfo sourceModule = moduleCtrl.GetModule(ModuleContext.ModuleId, ModuleContext.TabId, false);

            switch (Command.CommandName)
            {
                case ModuleActionType.LocalizeModule:
                    moduleCtrl.LocalizeModule(sourceModule, LocaleController.Instance.GetCurrentLocale(ModuleContext.PortalId));
                    break;
                case ModuleActionType.DeLocalizeModule:
                    moduleCtrl.DeLocalizeModule(sourceModule);
                    break;
            }

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }

        private void Translate(ModuleAction Command)
        {
            var moduleCtrl = new ModuleController();
            ModuleInfo sourceModule = moduleCtrl.GetModule(ModuleContext.ModuleId, ModuleContext.TabId, false);
            switch (Command.CommandName)
            {
                case ModuleActionType.TranslateModule:
                    moduleCtrl.UpdateTranslationStatus(sourceModule, true);
                    break;
                case ModuleActionType.UnTranslateModule:
                    moduleCtrl.UpdateTranslationStatus(sourceModule, false);
                    break;
            }

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }


        private void MoveToPane(ModuleAction Command)
        {
            var objModules = new ModuleController();
            objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, -1, Command.CommandArgument);
            objModules.UpdateTabModuleOrder(ModuleContext.TabId);
            Response.Redirect(Request.RawUrl, true);
        }

        private void MoveUpDown(ModuleAction Command)
        {
            var objModules = new ModuleController();
            switch (Command.CommandName)
            {
                case ModuleActionType.MoveTop:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, 0, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveUp:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder - 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveDown:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder + 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveBottom:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, (ModuleContext.Configuration.PaneModuleCount*2) + 1, Command.CommandArgument);
                    break;
            }
            objModules.UpdateTabModuleOrder(ModuleContext.TabId);
            Response.Redirect(Request.RawUrl, true);
        }

        public bool DisplayControl(DNNNodeCollection objNodes)
        {
            if (objNodes != null && objNodes.Count > 0 && PortalSettings.UserMode != PortalSettings.Mode.View)
            {
                DNNNode objRootNode = objNodes[0];
                if (objRootNode.HasNodes && objRootNode.DNNNodes.Count == 0)
                {
                    return true;
                }
                else if (objRootNode.DNNNodes.Count > 0)
                {
                    foreach (DNNNode childNode in objRootNode.DNNNodes)
                    {
                        if (!childNode.IsBreak)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public ModuleAction GetAction(string commandName)
        {
            return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByCommandName(commandName);
        }

        public ModuleAction GetAction(int id)
        {
            return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(id);
        }

        public void GetClientScriptURL(ModuleAction action, WebControl control)
        {
            if (!String.IsNullOrEmpty(action.ClientScript))
            {
                string Script = action.ClientScript;
                int JSPos = Script.ToLower().IndexOf("javascript:");
                if (JSPos > -1)
                {
                    Script = Script.Substring(JSPos + 11);
                }
                string FormatScript = "javascript: return {0};";
                control.Attributes.Add("onClick", string.Format(FormatScript, Script));
            }
        }

        public bool IsVisible(ModuleAction action)
        {
            bool _IsVisible = false;
            if (action.Visible && ModulePermissionController.HasModuleAccess(action.Secure, Null.NullString, ModuleContext.Configuration))
            {
                if ((ModuleContext.PortalSettings.UserMode == PortalSettings.Mode.Edit) || (action.Secure == SecurityAccessLevel.Anonymous || action.Secure == SecurityAccessLevel.View))
                {
                    _IsVisible = true;
                }
                else
                {
                    _IsVisible = false;
                }
            }
            else
            {
                _IsVisible = false;
            }
            return _IsVisible;
        }

        public bool ProcessAction(string id)
        {
            bool bProcessed = true;
            int nid = 0;
            if (Int32.TryParse(id, out nid))
            {
                bProcessed = ProcessAction(ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(nid));
            }
            return bProcessed;
        }

        public bool ProcessAction(ModuleAction action)
        {
            bool bProcessed = true;
            switch (action.CommandName)
            {
                case ModuleActionType.ModuleHelp:
                    DoAction(action);
                    break;
                case ModuleActionType.OnlineHelp:
                    DoAction(action);
                    break;
                case ModuleActionType.ModuleSettings:
                    DoAction(action);
                    break;
                case ModuleActionType.DeleteModule:
                    Delete(action);
                    break;
                case ModuleActionType.PrintModule:
                case ModuleActionType.SyndicateModule:
                    DoAction(action);
                    break;
                case ModuleActionType.ClearCache:
                    ClearCache(action);
                    break;
                case ModuleActionType.MovePane:
                    MoveToPane(action);
                    break;
                case ModuleActionType.MoveTop:
                case ModuleActionType.MoveUp:
                case ModuleActionType.MoveDown:
                case ModuleActionType.MoveBottom:
                    MoveUpDown(action);
                    break;
                case ModuleActionType.LocalizeModule:
                    Localize(action);
                    break;
                case ModuleActionType.DeLocalizeModule:
                    Localize(action);
                    break;
                case ModuleActionType.TranslateModule:
                    Translate(action);
                    break;
                case ModuleActionType.UnTranslateModule:
                    Translate(action);
                    break;
                default:
                    if (!String.IsNullOrEmpty(action.Url) && action.UseActionEvent == false)
                    {
                        DoAction(action);
                    }
                    else
                    {
                        bProcessed = false;
                    }
                    break;
            }
            return bProcessed;
        }
    }
}
