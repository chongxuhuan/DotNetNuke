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
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Containers.EventListeners;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins;

#endregion

namespace DotNetNuke
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Containers
    /// Class	 : Container
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Container is the base for the Containers
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/04/2005	Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    [Obsolete("This class is obsolete.  Please use DotNetNuke.UI.Containers.Container.")]
    public class Container : UI.Containers.Container
    {
    }
}

namespace DotNetNuke.UI.Containers
{
    public class Container : UserControl
    {
        private const string ContainerAdminBorder = "containerAdminBorder";
        private HtmlContainerControl _contentPane;
        private ModuleInfo _moduleConfiguration;
        private ModuleHost _moduleHost;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Content Pane Control (Id="ContentPane")
        /// </summary>
        /// <returns>An HtmlContainerControl</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected HtmlContainerControl ContentPane
        {
            get
            {
                return _contentPane ?? (_contentPane = FindControl(Globals.glbDefaultPane) as HtmlContainerControl);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Portal Settings for the current Portal
        /// </summary>
        /// <returns>A PortalSettings object</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ModuleControl object that this container is displaying
        /// </summary>
        /// <returns>A ModuleHost object</returns>
        /// <history>
        /// 	[cnurse]	01/12/2009  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public IModuleControl ModuleControl
        {
            get
            {
                IModuleControl moduleControl = null;
                if (ModuleHost != null)
                {
                    moduleControl = ModuleHost.ModuleControl;
                }
                return moduleControl;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the ModuleInfo object that this container is displaying
        /// </summary>
        /// <returns>A ModuleInfo object</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return _moduleConfiguration;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ModuleHost object that this container is displaying
        /// </summary>
        /// <returns>A ModuleHost object</returns>
        /// <history>
        /// 	[cnurse]	01/12/2009  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleHost ModuleHost
        {
            get
            {
                return _moduleHost;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Parent Container for this container
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public Skins.Skin ParentSkin
        {
            get
            {
                return Skins.Skin.GetParentSkin(this);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Path for this container
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/05/2007  documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ContainerPath
        {
            get
            {
                return TemplateSourceDirectory + "/";
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets the Source for this container
 /// </summary>
 /// <returns>A String</returns>
 /// <history>
 /// 	[cnurse]	06/10/2009  documented
 /// </history>
 /// -----------------------------------------------------------------------------
        public string ContainerSrc { get; set; }

        [Obsolete("Deprecated in 5.1. Replaced by ContainerPath")]
        public string SkinPath
        {
            get
            {
                return ContainerPath;
            }
        }

        private void AddAdministratorOnlyHighlighting(string message)
        {
            string cssclass = ContentPane.Attributes["class"];
            if (string.IsNullOrEmpty(cssclass))
            {
                ContentPane.Attributes["class"] = ContainerAdminBorder;
            }
            else
            {
                ContentPane.Attributes["class"] = string.Format("{0} {1}", cssclass.Replace(ContainerAdminBorder, "").Trim().Replace(" ", " "), ContainerAdminBorder);
            }

            ContentPane.Controls.Add(new LiteralControl(string.Format("<div class=\"NormalRed DNNAligncenter\">{0}</div>", message)));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessChildControls parses all the controls in the container, and if the
        /// control is an action (IActionControl) it attaches the ModuleControl (IModuleControl)
        /// and an EventHandler to respond to the Actions Action event.  If the control is a
        /// Container Object (IContainerControl) it attaches the ModuleControl.
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessChildControls(Control control)
        {
            IActionControl actions;
            ISkinControl skinControl;
            foreach (Control childControl in control.Controls)
            {
                actions = childControl as IActionControl;
                if (actions != null)
                {
                    actions.ModuleControl = ModuleControl;
                    actions.Action += ModuleActionClick;
                }
                skinControl = childControl as ISkinControl;
                if (skinControl != null)
                {
                    skinControl.ModuleControl = ModuleControl;
                }
                if (childControl.HasControls())
                {
                    ProcessChildControls(childControl);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessContentPane processes the ContentPane, setting its style and other
        /// attributes.
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessContentPane()
        {
            SetAlignment();

            SetBackground();

            SetBorder();

            string viewRoles = ModuleConfiguration.InheritViewPermissions
                                   ? TabPermissionController.GetTabPermissions(ModuleConfiguration.TabID, ModuleConfiguration.PortalID).ToString("VIEW")
                                   : ModuleConfiguration.ModulePermissions.ToString("VIEW");
            viewRoles = viewRoles.Replace(";", string.Empty).Trim().ToLowerInvariant();

            var showMessage = false;
            var adminMessage = Null.NullString;
            if (viewRoles == PortalSettings.AdministratorRoleName.ToLowerInvariant())
            {
                adminMessage = Localization.GetString("ModuleVisibleAdministrator.Text");
                showMessage = !ModuleConfiguration.HideAdminBorder;
            }
            if (ModuleConfiguration.StartDate >= DateTime.Now)
            {
                adminMessage = string.Format(Localization.GetString("ModuleEffective.Text"), ModuleConfiguration.StartDate.ToShortDateString());
                showMessage = true;
            }
            if (ModuleConfiguration.EndDate <= DateTime.Now)
            {
                adminMessage = string.Format(Localization.GetString("ModuleExpired.Text"), ModuleConfiguration.EndDate.ToShortDateString());
                showMessage = true;
            }
            if (showMessage)
            {
                AddAdministratorOnlyHighlighting(adminMessage);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessFooter adds an optional footer (and an End_Module comment)..
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessFooter()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Footer))
            {
                var objLabel = new Label {Text = ModuleConfiguration.Footer, CssClass = "Normal"};
                ContentPane.Controls.Add(objLabel);
            }
            if (!Globals.IsAdminControl())
            {
                ContentPane.Controls.Add(new LiteralControl("<!-- End_Module_" + ModuleConfiguration.ModuleID + " -->"));
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessHeader adds an optional header (and a Start_Module_ comment)..
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessHeader()
        {
            if (!Globals.IsAdminControl())
            {
                ContentPane.Controls.Add(new LiteralControl("<!-- Start_Module_" + ModuleConfiguration.ModuleID + " -->"));
            }
            if (!String.IsNullOrEmpty(ModuleConfiguration.Header))
            {
                var objLabel = new Label {Text = ModuleConfiguration.Header, CssClass = "Normal"};
                ContentPane.Controls.Add(objLabel);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ProcessModule processes the module which is attached to this container
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/05/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ProcessModule()
        {
            if (ContentPane != null)
            {
                ProcessContentPane();
                ProcessHeader();
                _moduleHost = new ModuleHost(ModuleConfiguration, ParentSkin, this);
                ContentPane.Controls.Add(ModuleHost);
                ProcessFooter();
                if (ModuleHost != null && ModuleControl != null)
                {
                    ProcessChildControls(this);
                }
                ProcessStylesheets(ModuleHost != null);
            }
        }

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ProcessStylesheets processes the Module and Container stylesheets and adds
		/// them to the Page.
		/// </summary>
		/// <history>
		/// 	[cnurse]	12/05/2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------

        private void ProcessStylesheets(bool includeModuleCss)
        {
            PageBase.RegisterStyleSheet(Page, ContainerPath + "container.css");

            PageBase.RegisterStyleSheet(Page,ContainerSrc.Replace(".ascx", ".css"));

            if (includeModuleCss)
            {
                string controlSrc = ModuleConfiguration.ModuleControl.ControlSrc;
                string folderName = ModuleConfiguration.DesktopModule.FolderName;

                PageBase.RegisterStyleSheet(Page, Globals.ApplicationPath + "/DesktopModules/" + folderName + "/module.css");

                if (controlSrc.LastIndexOf("/") > 0)
                {
                    PageBase.RegisterStyleSheet(Page, Globals.ApplicationPath + "/" + controlSrc.Substring(0, controlSrc.LastIndexOf("/") + 1) + "module.css");
                }
            }
        }

        private void SetAlignment()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Alignment))
            {
                if (ContentPane.Attributes["class"] != null)
                {
                    ContentPane.Attributes["class"] = ContentPane.Attributes["class"] + " DNNAlign" + ModuleConfiguration.Alignment.ToLower();
                }
                else
                {
                    ContentPane.Attributes["class"] = "DNNAlign" + ModuleConfiguration.Alignment.ToLower();
                }
            }
        }

        private void SetBackground()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Color))
            {
                ContentPane.Style["background-color"] = ModuleConfiguration.Color;
            }
        }

        private void SetBorder()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Border))
            {
                ContentPane.Style["border-top"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-bottom"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-right"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-left"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            foreach (var listener in DotNetNukeContext.Current.ContainerEventListeners.Where(listener => listener.EventType == ContainerEventType.OnContainerInit))
            {
                listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (var listener in DotNetNukeContext.Current.ContainerEventListeners.Where(listener => listener.EventType == ContainerEventType.OnContainerLoad))
            {
                listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var listener in DotNetNukeContext.Current.ContainerEventListeners.Where(listener => listener.EventType == ContainerEventType.OnContainerPreRender))
            {
                listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            foreach (var listener in DotNetNukeContext.Current.ContainerEventListeners.Where(listener => listener.EventType == ContainerEventType.OnContainerUnLoad))
            {
                listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
            }
        }

        public void SetModuleConfiguration(ModuleInfo configuration)
        {
            _moduleConfiguration = configuration;
            ProcessModule();
        }

        private void ModuleActionClick(object sender, ActionEventArgs e)
        {
            foreach (ModuleActionEventListener listener in ParentSkin.ActionEventListeners)
            {
                if (e.ModuleConfiguration.ModuleID == listener.ModuleID)
                {
                    listener.ActionEvent.Invoke(sender, e);
                }
            }
        }

        [Obsolete("Deprecated in 5.0. Shouldn't need to be used any more.  ContainerObjects (IContainerControl implementations) have a property ModuleControl.")]
        public static PortalModuleBase GetPortalModuleBase(UserControl control)
        {
            PortalModuleBase moduleControl = null;
            Panel panel;
            if (control is SkinObjectBase)
            {
                panel = (Panel) control.Parent.FindControl("ModuleContent");
            }
            else
            {
                panel = (Panel) control.FindControl("ModuleContent");
            }
            if (panel != null)
            {
                try
                {
                    moduleControl = (PortalModuleBase) panel.Controls[1];
                }
                catch
                {
                    try
                    {
                        moduleControl = (PortalModuleBase) panel.Controls[0].Controls[0].Controls[1];
                    }
                    catch (Exception exc)
                    {
                        Exceptions.LogException(exc);
                    }
                }
            }
            return moduleControl ?? (new PortalModuleBase {ModuleConfiguration = new ModuleInfo()});
        }
    }
}
