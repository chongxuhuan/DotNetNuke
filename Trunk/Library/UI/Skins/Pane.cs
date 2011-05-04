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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.UI.Skins
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Skins
    /// Class	 : Pane
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Pane class represents a Pane within the Skin
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	12/04/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class Pane
    {
        private const string CPaneOutline = "paneOutline";
        private HtmlGenericControl _containerWrapperControl;
        private Dictionary<string, Containers.Container> _containers;

        public Pane(HtmlContainerControl pane)
        {
            PaneControl = pane;
            Name = pane.ID;
        }

        public Pane(string name, HtmlContainerControl pane)
        {
            PaneControl = pane;
            Name = name;
        }

        protected Dictionary<string, Containers.Container> Containers
        {
            get
            {
                return _containers ?? (_containers = new Dictionary<string, Containers.Container>());
            }
        }

        protected string Name { get; set; }

        protected HtmlContainerControl PaneControl { get; set; }

        protected PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        private bool CanCollapsePane()
        {
            bool canCollapsePane = true;
            if (Containers.Count > 0)
            {
                canCollapsePane = false;
            }
            else if (PaneControl.Controls.Count == 1)
            {
                canCollapsePane = false;
                var literal = PaneControl.Controls[0] as LiteralControl;
                if (literal != null)
                {
                    if (String.IsNullOrEmpty(HtmlUtils.StripWhiteSpace(literal.Text, false)))
                    {
                        canCollapsePane = true;
                    }
                }
            }
            else if (PaneControl.Controls.Count > 1)
            {
                canCollapsePane = false;
            }
            return canCollapsePane;
        }

        private Containers.Container LoadContainerByPath(string containerPath)
        {
            if (containerPath.ToLower().IndexOf("/skins/") != -1 || containerPath.ToLower().IndexOf("/skins\\") != -1 || containerPath.ToLower().IndexOf("\\skins\\") != -1 ||
                containerPath.ToLower().IndexOf("\\skins/") != -1)
            {
                throw new Exception();
            }

            Containers.Container container = null;

            try
            {
                string containerSrc = containerPath;
                if (containerPath.IndexOf(Globals.ApplicationPath, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    containerPath = containerPath.Remove(0, Globals.ApplicationPath.Length);
                }
                container = ControlUtilities.LoadControl<Containers.Container>(PaneControl.Page, containerPath);
                container.ContainerSrc = containerSrc;
                container.DataBind();
            }
            catch (Exception exc)
            {
                var lex = new ModuleLoadException(Skin.MODULELOAD_ERROR, exc);
                if (TabPermissionController.CanAdminPage())
                {
                    _containerWrapperControl.Controls.Add(new ErrorContainer(PortalSettings, string.Format(Skin.CONTAINERLOAD_ERROR, containerPath), lex).Container);
                }
                Exceptions.LogException(lex);
            }
            return container;
        }

        private Containers.Container LoadContainerFromCookie(HttpRequest request)
        {
            Containers.Container container = null;
            HttpCookie cookie = request.Cookies["_ContainerSrc" + PortalSettings.PortalId];
            if (cookie != null)
            {
                if (!String.IsNullOrEmpty(cookie.Value))
                {
                    container = LoadContainerByPath(SkinController.FormatSkinSrc(cookie.Value + ".ascx", PortalSettings));
                }
            }
            return container;
        }

        private Containers.Container LoadContainerFromPane()
        {
            Containers.Container container = null;
            string containerSrc;
            var validSrc = false;

            if ((PaneControl.Attributes["ContainerType"] != null) && (PaneControl.Attributes["ContainerName"] != null))
            {
                containerSrc = "[" + PaneControl.Attributes["ContainerType"] + "]" + SkinController.RootContainer + "/" + PaneControl.Attributes["ContainerName"] + "/" +
                               PaneControl.Attributes["ContainerSrc"];
                validSrc = true;
            }
            else
            {
                containerSrc = PaneControl.Attributes["ContainerSrc"];
                if (containerSrc.Contains("/") && !(containerSrc.ToLower().StartsWith("[g]") || containerSrc.ToLower().StartsWith("[l]")))
                {
                    containerSrc = string.Format(SkinController.IsGlobalSkin(PortalSettings.ActiveTab.SkinSrc) ? "[G]containers/{0}" : "[L]containers/{0}", containerSrc.TrimStart('/'));
                    validSrc = true;
                }
            }

            if (validSrc)
            {
                containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings);
                container = LoadContainerByPath(containerSrc);
            }
            return container;
        }

        private Containers.Container LoadContainerFromQueryString(ModuleInfo module, HttpRequest request)
        {
            Containers.Container container = null;
            int previewModuleId = -1;
            if (request.QueryString["ModuleId"] != null)
            {
                Int32.TryParse(request.QueryString["ModuleId"], out previewModuleId);
            }

            if ((request.QueryString["ContainerSrc"] != null) && (module.ModuleID == previewModuleId || previewModuleId == -1))
            {
                string containerSrc = SkinController.FormatSkinSrc(Globals.QueryStringDecode(request.QueryString["ContainerSrc"]) + ".ascx", PortalSettings);
                container = LoadContainerByPath(containerSrc);
            }
            return container;
        }

        private Containers.Container LoadNoContainer(ModuleInfo module)
        {
            string noContainerSrc = "[G]" + SkinController.RootContainer + "/_default/No Container.ascx";
            Containers.Container container = null;

            if (module.DisplayTitle == false)
            {
                bool displayTitle = ModulePermissionController.CanEditModuleContent(module) || Globals.IsAdminSkin();

                if (displayTitle)
                {
                    displayTitle = (PortalSettings.UserMode != PortalSettings.Mode.View);
                }

                if (displayTitle == false)
                {
                    container = LoadContainerByPath(SkinController.FormatSkinSrc(noContainerSrc, PortalSettings));
                }
            }
            return container;
        }

        private Containers.Container LoadModuleContainer(ModuleInfo module)
        {
            string containerSrc = Null.NullString;
            HttpRequest request = PaneControl.Page.Request;

            Containers.Container container = (LoadContainerFromQueryString(module, request) ?? LoadContainerFromCookie(request)) ?? LoadNoContainer(module);

            if (container == null)
            {
                if (module.ContainerSrc == PortalSettings.ActiveTab.ContainerSrc)
                {
                    if (PaneControl != null)
                    {
                        if ((PaneControl.Attributes["ContainerSrc"] != null))
                        {
                            container = LoadContainerFromPane();
                        }
                    }
                }
            }

            if (container == null)
            {
                containerSrc = module.ContainerSrc;
                if (!String.IsNullOrEmpty(containerSrc))
                {
                    containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings);
                    container = LoadContainerByPath(containerSrc);
                }
            }

            if (container == null)
            {
                containerSrc = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalContainer(), PortalSettings);
                container = LoadContainerByPath(containerSrc);
            }

            module.ContainerPath = SkinController.FormatSkinPath(containerSrc);
            container.ID = "ctr";
            if (module.ModuleID > -1)
            {
                container.ID += module.ModuleID.ToString();
            }
            return container;
        }

        private void ModuleMoveToPanePostBack(ClientAPIPostBackEventArgs args)
        {
            var portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            if (TabPermissionController.CanAdminPage())
            {
                var moduleId = Convert.ToInt32(args.EventArguments["moduleid"]);
                var paneName = Convert.ToString(args.EventArguments["pane"]);
                var moduleOrder = Convert.ToInt32(args.EventArguments["order"]);

                var moduleController = new ModuleController();
                moduleController.UpdateModuleOrder(portalSettings.ActiveTab.TabID, moduleId, moduleOrder, paneName);
                moduleController.UpdateTabModuleOrder(portalSettings.ActiveTab.TabID);

                PaneControl.Page.Response.Redirect(PaneControl.Page.Request.RawUrl, true);
            }
        }

        public void InjectModule(ModuleInfo module)
        {
            _containerWrapperControl = new HtmlGenericControl("div");
            PaneControl.Controls.Add(_containerWrapperControl);

            //inject module classes
            const string classFormatString = "DnnModule DnnModule-{0} DnnModule-{1}";
            string sanitizedModuleName = Null.NullString;

            if (!String.IsNullOrEmpty(module.DesktopModule.ModuleName))
            {
                sanitizedModuleName = Globals.CreateValidClass(module.DesktopModule.ModuleName, false);
            }

            _containerWrapperControl.Attributes["class"] = String.Format(classFormatString, sanitizedModuleName, module.ModuleID);

            try
            {
                if (!Globals.IsAdminControl())
                {
                    _containerWrapperControl.Controls.Add(new LiteralControl("<a name=\"" + module.ModuleID + "\"></a>"));
                }

                Containers.Container container = LoadModuleContainer(module);
                Containers.Add(container.ID, container);

                if (Globals.IsLayoutMode() && Globals.IsAdminControl() == false)
                {
                    var dragDropContainer = new Panel();
                    Control title = container.FindControl("dnnTitle");
                    dragDropContainer.ID = container.ID + "_DD";
                    _containerWrapperControl.Controls.Add(dragDropContainer);
                    dragDropContainer.Controls.Add(container);

                    if (title != null)
                    {
                        if (title.Controls.Count > 0)
                        {
                            title = title.Controls[0];
                        }
                    }

                    if (title != null)
                    {
                        DNNClientAPI.EnableContainerDragAndDrop(title, dragDropContainer, module.ModuleID);
                        ClientAPI.RegisterPostBackEventHandler(PaneControl, "MoveToPane", ModuleMoveToPanePostBack, false);
                    }
                }
                else
                {
                    _containerWrapperControl.Controls.Add(container);
                }

                container.SetModuleConfiguration(module);
                if (PaneControl.Visible == false)
                {
                    PaneControl.Visible = true;
                }
            }
            catch (Exception exc)
            {
                var lex = new ModuleLoadException(string.Format(Skin.MODULEADD_ERROR, PaneControl.ID), exc);
                if (TabPermissionController.CanAdminPage())
                {
                    _containerWrapperControl.Controls.Add(new ErrorContainer(PortalSettings, Skin.MODULELOAD_ERROR, lex).Container);
                }
                Exceptions.LogException(exc);
                throw lex;
            }
        }

        public void ProcessPane()
        {
            if (PaneControl != null)
            {
                PaneControl.Attributes.Remove("ContainerType");
                PaneControl.Attributes.Remove("ContainerName");
                PaneControl.Attributes.Remove("ContainerSrc");

                if (Globals.IsLayoutMode())
                {
                    PaneControl.Visible = true;
                    string cssclass = PaneControl.Attributes["class"];
                    if (string.IsNullOrEmpty(cssclass))
                    {
                        PaneControl.Attributes["class"] = CPaneOutline;
                    }
                    else
                    {
                        PaneControl.Attributes["class"] = cssclass.Replace(CPaneOutline, "").Trim().Replace("  ", " ") + " " + CPaneOutline;
                    }
                    var ctlLabel = new Label {Text = "<center>" + Name + "</center><br>", CssClass = "SubHead"};
                    PaneControl.Controls.AddAt(0, ctlLabel);
                }
                else
                {
                    if (TabPermissionController.CanAddContentToPage() && PaneControl.Visible == false)
                    {
                        PaneControl.Visible = true;
                    }

                    if (CanCollapsePane())
                    {
                        if (PaneControl.Attributes["style"] != null)
                        {
                            PaneControl.Attributes.Remove("style");
                        }
                        if (PaneControl.Attributes["class"] != null)
                        {
                            PaneControl.Attributes["class"] = PaneControl.Attributes["class"] + " DNNEmptyPane";
                        }
                        else
                        {
                            PaneControl.Attributes["class"] = "DNNEmptyPane";
                        }
                    }
                }
            }
        }
    }
}
