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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Communications;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.ControlPanels;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Skins.EventListeners;

using Microsoft.VisualBasic;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Skins
    /// Class	 : Skin
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Skin is the base for the Skins
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/04/2005	Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    [Obsolete("This class is obsolete.  Please use DotNetNuke.UI.Skins.Skin.")]
    public class Skin : UI.Skins.Skin
    {
    }
}

namespace DotNetNuke.UI.Skins
{
    public class Skin : UserControlBase
    {
        // ReSharper disable InconsistentNaming
        public static string MODULELOAD_ERROR = Localization.GetString("ModuleLoad.Error");
        public static string CONTAINERLOAD_ERROR = Localization.GetString("ContainerLoad.Error");
        public static string MODULEADD_ERROR = Localization.GetString("ModuleAdd.Error");
        private readonly ModuleCommunicate _communicator = new ModuleCommunicate();
        public string CONTRACTEXPIRED_ERROR = Localization.GetString("ContractExpired.Error");
        public string CRITICAL_ERROR = Localization.GetString("CriticalError.Error");
        public string MODULEACCESS_ERROR = Localization.GetString("ModuleAccess.Error");
        public string MODULELOAD_WARNING = Localization.GetString("ModuleLoadWarning.Error");
        public string MODULELOAD_WARNINGTEXT = Localization.GetString("ModuleLoadWarning.Text");
        public string PANE_LOAD_ERROR = Localization.GetString("PaneNotFound.Error");
        public string TABACCESS_ERROR = Localization.GetString("TabAccess.Error");
        // ReSharper restore InconsistentNaming

        private ArrayList _actionEventListeners;
        private Control _controlPanel;
        private Dictionary<string, Pane> _panes;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ControlPanel container.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        internal Control ControlPanel
        {
            get
            {
                return _controlPanel ?? (_controlPanel = FindControl("ControlPanel"));
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ModuleCommunicate instance for the skin
        /// </summary>
        /// <returns>The ModuleCommunicate instance for the Skin</returns>
        /// <history>
        /// 	[cnurse]	01/12/2009  created
        /// </history>
        internal ModuleCommunicate Communicator
        {
            get
            {
                return _communicator;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Panes.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        internal Dictionary<string, Pane> Panes
        {
            get
            {
                return _panes ?? (_panes = new Dictionary<string, Pane>());
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an ArrayList of ActionEventListeners
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/04/2007  documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList ActionEventListeners
        {
            get
            {
                return _actionEventListeners ?? (_actionEventListeners = new ArrayList());
            }
            set
            {
                _actionEventListeners = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Path for this skin
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public string SkinPath
        {
            get
            {
                return TemplateSourceDirectory + "/";
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets the Source for this skin
 /// </summary>
 /// <returns>A String</returns>
 /// <history>
 /// 	[cnurse]	12/05/2007  documented
 /// </history>
 /// -----------------------------------------------------------------------------
        public string SkinSrc { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CheckExpired checks whether the portal has expired
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool CheckExpired()
        {
            bool blnExpired = false;
            if (PortalSettings.ExpiryDate != Null.NullDate)
            {
                if (Convert.ToDateTime(PortalSettings.ExpiryDate) < DateTime.Now && PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                {
                    blnExpired = true;
                }
            }
            return blnExpired;
        }

        private Pane GetPane(ModuleInfo module)
        {
            Pane pane;
            bool found = Panes.TryGetValue(module.PaneName.ToLowerInvariant(), out pane);

            if (!found)
            {
                Panes.TryGetValue(Globals.glbDefaultPane.ToLowerInvariant(), out pane);
            }

            return pane;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InjectControlPanel injects the ControlPanel into the ControlPanel pane
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private void InjectControlPanel()
        {
          if (Request.QueryString["dnnprintmode"] != "true" && Request.QueryString["popUp"] != "true")
          {
            ControlPanel cp = ControlUtilities.FindFirstDescendent<ControlPanel>(Page);
            if (cp == null)
            {
              var objControlPanel = ControlUtilities.LoadControl<ControlPanelBase>(this, Host.ControlPanel);
              if (ControlPanel == null)
              {
                var objForm = (HtmlForm)Parent.FindControl("Form");
                if (objForm != null)
                {
                  objForm.Controls.AddAt(0, objControlPanel);
                }
                else
                {
                  Page.Controls.AddAt(0, objControlPanel);
                }
              }
              else
              {
                ControlPanel.Controls.Add(objControlPanel);
              }
            }
          }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadPanes parses the Skin and loads the "Panes"
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private void LoadPanes()
        {
            foreach (Control ctlControl in Controls)
            {
                var objPaneControl = ctlControl as HtmlContainerControl;
                if (objPaneControl != null && !string.IsNullOrEmpty(objPaneControl.ID))
                {
                    switch (objPaneControl.TagName.ToLowerInvariant())
                    {
                        case "td":
                        case "div":
                        case "span":
                        case "p":
                            if (objPaneControl.ID.ToLower() != "controlpanel")
                            {
                                PortalSettings.ActiveTab.Panes.Add(objPaneControl.ID);
                                Panes.Add(objPaneControl.ID.ToLowerInvariant(), new Pane(objPaneControl));
                            }
                            else
                            {
                                _controlPanel = objPaneControl;
                            }
                            break;
                    }
                }
            }
        }

        private bool ProcessModule(ModuleInfo module)
        {
            bool success = true;
            if (ModulePermissionController.CanViewModule(module) && module.IsDeleted == false &&
                ((module.StartDate < DateTime.Now && module.EndDate > DateTime.Now) || Globals.IsLayoutMode() || Globals.IsEditMode()))
            {
                Pane pane = GetPane(module);

                if (pane != null)
                {
                    success = InjectModule(pane, module);
                }
                else
                {
                    var lex = new ModuleLoadException(PANE_LOAD_ERROR);
                    Controls.Add(new ErrorContainer(PortalSettings, MODULELOAD_ERROR, lex).Container);
                    Exceptions.LogException(lex);
                }
            }
            return success;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessMasterModules processes all the master modules in the Active Tab's
        /// Modules Collection.
        /// </summary>
        /// <returns>A flag that indicates whether the modules were successfully processed.</returns>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool ProcessMasterModules()
        {
            bool success = true;
            if (TabPermissionController.CanViewPage())
            {
                if (!CheckExpired())
                {
                    if ((PortalSettings.ActiveTab.StartDate < DateAndTime.Now && PortalSettings.ActiveTab.EndDate > DateAndTime.Now) || TabPermissionController.CanAdminPage() || Globals.IsLayoutMode())
                    {
                        if (PortalSettings.ActiveTab.Modules.Count > 0)
                        {
                            foreach (ModuleInfo objModule in PortalSettings.ActiveTab.Modules)
                            {
                                success = ProcessModule(objModule);
                            }
                        }
                    }
                    else
                    {
                        AddPageMessage(this, "", TABACCESS_ERROR, ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                }
                else
                {
                    AddPageMessage(this,
                                   "",
                                   string.Format(CONTRACTEXPIRED_ERROR, PortalSettings.PortalName, Globals.GetMediumDate(PortalSettings.ExpiryDate.ToString()), PortalSettings.Email),
                                   ModuleMessage.ModuleMessageType.RedError);
                }
            }
            else
            {
                Response.Redirect(Globals.AccessDeniedURL(TABACCESS_ERROR), true);
            }
            return success;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessPanes processes the Attributes for the Panes
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessPanes()
        {
            foreach (KeyValuePair<string, Pane> kvp in Panes)
            {
                kvp.Value.ProcessPane();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessSlaveModule processes the slave module specifeid by the "ctl=xxx" ControlKey.
        /// </summary>
        /// <returns>A flag that indicates whether the modules were successfully processed.</returns>
        /// <history>
        /// 	[cnurse]	12/04/2007  created
        ///     [cnurse]    04/17/2009  Refactored from Skin
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool ProcessSlaveModule()
        {
            var success = true;
            var key = UIUtilities.GetControlKey();
            var moduleId = UIUtilities.GetModuleId(key);
            var slaveModule = UIUtilities.GetSlaveModule(moduleId, key, PortalSettings.ActiveTab.TabID);

            Pane pane;
            Panes.TryGetValue(Globals.glbDefaultPane.ToLowerInvariant(), out pane);
            slaveModule.PaneName = Globals.glbDefaultPane;
            slaveModule.ContainerSrc = PortalSettings.ActiveTab.ContainerSrc;
            if (String.IsNullOrEmpty(slaveModule.ContainerSrc))
            {
                slaveModule.ContainerSrc = PortalSettings.DefaultPortalContainer;
            }
            slaveModule.ContainerSrc = SkinController.FormatSkinSrc(slaveModule.ContainerSrc, PortalSettings);
            slaveModule.ContainerPath = SkinController.FormatSkinPath(slaveModule.ContainerSrc);

            var moduleControl = ModuleControlController.GetModuleControlByControlKey(key, slaveModule.ModuleDefID);
            if (moduleControl != null)
            {
                slaveModule.ModuleControlId = moduleControl.ModuleControlID;
                slaveModule.IconFile = moduleControl.IconFile;
                if (ModulePermissionController.HasModuleAccess(slaveModule.ModuleControl.ControlType, Null.NullString, slaveModule))
                {
                    success = InjectModule(pane, slaveModule);
                }
                else
                {
                    Response.Redirect(Globals.AccessDeniedURL(MODULEACCESS_ERROR), true);
                }
            }

            return success;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadPanes();

            bool success;
            success = Globals.IsAdminControl() ? ProcessSlaveModule() : ProcessMasterModules();
            InjectControlPanel();
            ProcessPanes();

            if (Request.QueryString["error"] != null)
            {
                AddPageMessage(this, CRITICAL_ERROR, Server.HtmlEncode(Request.QueryString["error"]), ModuleMessage.ModuleMessageType.RedError);
            }

            if (!TabPermissionController.CanAdminPage() && !success)
            {
                AddPageMessage(this, MODULELOAD_WARNING, string.Format(MODULELOAD_WARNINGTEXT, PortalSettings.Email), ModuleMessage.ModuleMessageType.YellowWarning);
            }

            foreach (SkinEventListener listener in DotNetNukeContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinInit)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (SkinEventListener listener in DotNetNukeContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinLoad)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (SkinEventListener listener in DotNetNukeContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinPreRender)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            foreach (SkinEventListener listener in DotNetNukeContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinUnLoad)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }

        public bool InjectModule(Pane pane, ModuleInfo module)
        {
            bool bSuccess = true;
            try
            {
                pane.InjectModule(module);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                bSuccess = false;
            }
            return bSuccess;
        }

        public void RegisterModuleActionEvent(int moduleId, ActionEventHandler e)
        {
            ActionEventListeners.Add(new ModuleActionEventListener(moduleId, e));
        }

        private static Skin LoadSkin(PageBase page, string skinPath)
        {
            Skin ctlSkin = null;
            try
            {
                string skinSrc = skinPath;
                if (skinPath.ToLower().IndexOf(Globals.ApplicationPath) != -1)
                {
                    skinPath = skinPath.Remove(0, Globals.ApplicationPath.Length);
                }
                ctlSkin = ControlUtilities.LoadControl<Skin>(page, skinPath);
                ctlSkin.SkinSrc = skinSrc;
                ctlSkin.DataBind();
            }
            catch (Exception exc)
            {
                var lex = new PageLoadException("Unhandled error loading page.", exc);
                if (TabPermissionController.CanAdminPage())
                {
                    var skinError = (Label) page.FindControl("SkinError");
                    skinError.Text = string.Format(Localization.GetString("SkinLoadError", Localization.GlobalResourceFile), skinPath, page.Server.HtmlEncode(exc.Message));
                    skinError.Visible = true;
                }
                Exceptions.LogException(lex);
            }
            return ctlSkin;
        }

        private static void AddModuleMessage(Control control, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType, string iconSrc)
        {
            if (control != null)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    var messagePlaceHolder = ControlUtilities.FindControl<PlaceHolder>(control, "MessagePlaceHolder", true);
                    if (messagePlaceHolder != null)
                    {
                        messagePlaceHolder.Visible = true;
                        ModuleMessage moduleMessage = GetModuleMessageControl(heading, message, moduleMessageType, iconSrc);
                        messagePlaceHolder.Controls.Add(moduleMessage);
                    }
                }
            }
        }

        private static void AddPageMessage(Control control, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType, string iconSrc)
        {
            if (!String.IsNullOrEmpty(message))
            {
                Control contentPane = control.FindControl(Globals.glbDefaultPane);
                if (contentPane != null)
                {
                    ModuleMessage moduleMessage = GetModuleMessageControl(heading, message, moduleMessageType, iconSrc);
                    contentPane.Controls.AddAt(0, moduleMessage);
                }
            }
        }

        public static void AddModuleMessage(PortalModuleBase control, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddModuleMessage(control, "", message, moduleMessageType, Null.NullString);
        }

        public static void AddModuleMessage(PortalModuleBase control, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddModuleMessage(control, heading, message, moduleMessageType, Null.NullString);
        }

        public static void AddModuleMessage(Control control, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddModuleMessage(control, "", message, moduleMessageType, Null.NullString);
        }

        public static void AddModuleMessage(Control control, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddModuleMessage(control, heading, message, moduleMessageType, Null.NullString);
        }

        public static void AddPageMessage(Page page, string heading, string message, string iconSrc)
        {
            AddPageMessage(page, heading, message, ModuleMessage.ModuleMessageType.GreenSuccess, iconSrc);
        }

        public static void AddPageMessage(Skin skin, string heading, string message, string iconSrc)
        {
            AddPageMessage(skin, heading, message, ModuleMessage.ModuleMessageType.GreenSuccess, iconSrc);
        }

        public static void AddPageMessage(Skin skin, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddPageMessage(skin, heading, message, moduleMessageType, Null.NullString);
        }

        public static void AddPageMessage(Page page, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddPageMessage(page, heading, message, moduleMessageType, Null.NullString);
        }

        public static ModuleMessage GetModuleMessageControl(string heading, string message, string iconImage)
        {
            return GetModuleMessageControl(heading, message, ModuleMessage.ModuleMessageType.GreenSuccess, iconImage);
        }

        public static ModuleMessage GetModuleMessageControl(string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            return GetModuleMessageControl(heading, message, moduleMessageType, Null.NullString);
        }

        public static ModuleMessage GetModuleMessageControl(string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType, string iconImage)
        {
            var s = new Skin();
            var moduleMessage = (ModuleMessage) s.LoadControl("~/admin/skins/ModuleMessage.ascx");
            moduleMessage.Heading = heading;
            moduleMessage.Text = message;
            moduleMessage.IconImage = iconImage;
            moduleMessage.IconType = moduleMessageType;
            return moduleMessage;
        }

        public static Skin GetParentSkin(PortalModuleBase module)
        {
            return GetParentSkin(module as Control);
        }

        public static Skin GetParentSkin(Control control)
        {
            return ControlUtilities.FindParentControl<Skin>(control);
        }

        public static Skin GetSkin(PageBase page)
        {
            Skin skin = null;
            string skinSource = Null.NullString;
            if ((page.Request.QueryString["SkinSrc"] != null))
            {
                skinSource = SkinController.FormatSkinSrc(Globals.QueryStringDecode(page.Request.QueryString["SkinSrc"]) + ".ascx", page.PortalSettings);
                skin = LoadSkin(page, skinSource);
            }
            if (skin == null)
            {
                HttpCookie skinCookie = page.Request.Cookies["_SkinSrc" + page.PortalSettings.PortalId];
                if (skinCookie != null)
                {
                    if (!String.IsNullOrEmpty(skinCookie.Value))
                    {
                        skinSource = SkinController.FormatSkinSrc(skinCookie.Value + ".ascx", page.PortalSettings);
                        skin = LoadSkin(page, skinSource);
                    }
                }
            }
            if (skin == null)
            {
                skinSource = Globals.IsAdminSkin() ? SkinController.FormatSkinSrc(page.PortalSettings.DefaultAdminSkin, page.PortalSettings) : page.PortalSettings.ActiveTab.SkinSrc;
                if (!String.IsNullOrEmpty(skinSource))
                {
                    skinSource = SkinController.FormatSkinSrc(skinSource, page.PortalSettings);
                    skin = LoadSkin(page, skinSource);
                }
            }
            if (skin == null)
            {
                skinSource = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalSkin(), page.PortalSettings);
                skin = LoadSkin(page, skinSource);
            }
            page.PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath(skinSource);
            skin.ID = "dnn";
            return skin;
        }
    }
}
