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
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.ModuleCache;
using DotNetNuke.UI.WebControls;

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.UI.Modules
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Modules
    /// Class	 : ModuleHost
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ModuleHost hosts a Module Control (or its cached Content).
    /// </summary>
    /// <history>
    /// 	[cnurse]	12/15/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public sealed class ModuleHost : Panel
    {
        #region Private Members

        private readonly ModuleInfo _moduleConfiguration;
        private Control _control;
        private bool _isCached;

        #endregion

        #region Constructors

        public ModuleHost(ModuleInfo moduleConfiguration, Skins.Skin skin, Containers.Container container)
        {
            ID = "ModuleContent";
            Container = container;
            _moduleConfiguration = moduleConfiguration;
            Skin = skin;
        }

        #endregion

        #region Public Properties

        public Containers.Container Container { get; private set; }

        public IModuleControl ModuleControl
        {
            get
            {
                EnsureChildControls();
                return _control as IModuleControl;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        public Skins.Skin Skin { get; private set; }

        #endregion

        #region Private Methods

        private void InjectModuleContent(Control content)
        {
            if (_moduleConfiguration.IsWebSlice && !Globals.IsAdminControl())
            {
                CssClass = "hslice";
                var titleLabel = new Label
                                     {
                                         CssClass = "entry-title Hidden",
                                         Text = !string.IsNullOrEmpty(_moduleConfiguration.WebSliceTitle) ? _moduleConfiguration.WebSliceTitle : _moduleConfiguration.ModuleTitle
                                     };
                Controls.Add(titleLabel);

                var websliceContainer = new Panel {CssClass = "entry-content"};
                websliceContainer.Controls.Add(content);

                var expiry = new HtmlGenericControl {TagName = "abbr"};
                expiry.Attributes["class"] = "endtime";
                if (!Null.IsNull(_moduleConfiguration.WebSliceExpiryDate))
                {
                    expiry.Attributes["title"] = _moduleConfiguration.WebSliceExpiryDate.ToString("o");
                    websliceContainer.Controls.Add(expiry);
                }
                else if (_moduleConfiguration.EndDate < DateTime.MaxValue)
                {
                    expiry.Attributes["title"] = _moduleConfiguration.EndDate.ToString("o");
                    websliceContainer.Controls.Add(expiry);
                }

                var ttl = new HtmlGenericControl {TagName = "abbr"};
                ttl.Attributes["class"] = "ttl";
                if (_moduleConfiguration.WebSliceTTL > 0)
                {
                    ttl.Attributes["title"] = _moduleConfiguration.WebSliceTTL.ToString();
                    websliceContainer.Controls.Add(ttl);
                }
                else if (_moduleConfiguration.CacheTime > 0)
                {
                    ttl.Attributes["title"] = (_moduleConfiguration.CacheTime/60).ToString();
                    websliceContainer.Controls.Add(ttl);
                }

                Controls.Add(websliceContainer);
            }
            else
            {
                Controls.Add(content);
            }
        }

        private bool DisplayContent()
        {
            var content = PortalSettings.UserMode != PortalSettings.Mode.Layout;
            if (Page.Request.QueryString["content"] != null)
            {
                switch (Page.Request.QueryString["Content"].ToLower())
                {
                    case "1":
                    case "true":
                        content = true;
                        break;
                    case "0":
                    case "false":
                        content = false;
                        break;
                }
            }
            if (Globals.IsAdminControl())
            {
                content = true;
            }
            return content;
        }

        private static void InjectMessageControl(Control container)
        {
            var messagePlaceholder = new PlaceHolder {ID = "MessagePlaceHolder", Visible = false};
            container.Controls.Add(messagePlaceholder);
        }

        private bool IsViewMode()
        {
            return !(ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, _moduleConfiguration)) || PortalSettings.UserMode == PortalSettings.Mode.View;
        }

        private void LoadModuleControl()
        {
            try
            {
                if (DisplayContent())
                {
                    if (SupportsCaching() && IsViewMode())
                    {
                        _isCached = TryLoadCached();
                    }
                    if (!_isCached)
                    {
                    // load the control dynamically
                        _control = ModuleControlFactory.LoadModuleControl(Page, _moduleConfiguration);
                    }
                }
                else
                {
                    _control = ModuleControlFactory.CreateModuleControl(_moduleConfiguration);
                }
                if (Skin != null)
                {
                    Skin.Communicator.LoadCommunicator(_control);
                }
                ModuleControl.ModuleContext.Configuration = _moduleConfiguration;
            }
            catch (ThreadAbortException exc)
            {
                DnnLog.Debug(exc);

                Thread.ResetAbort();
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                _control = ModuleControlFactory.CreateModuleControl(_moduleConfiguration);
                ModuleControl.ModuleContext.Configuration = _moduleConfiguration;
                if (TabPermissionController.CanAdminPage())
                {
                    Exceptions.ProcessModuleLoadException(_control, exc);
                }
            }
        }

        private void LoadAjaxPanel()
        {
            var loadingPanel = new RadAjaxLoadingPanel { ID = _control.ID + "_Prog", Skin = "Default" };

            Controls.Add(loadingPanel);

            var ajaxPanel = new RadAjaxPanel {
                ID = _control.ID + "_UP",
                LoadingPanelID = loadingPanel.ID,
                RestoreOriginalRenderDelegate = false
            };
            InjectMessageControl(ajaxPanel); 
            ajaxPanel.Controls.Add(_control);

            Controls.Add(ajaxPanel);

        }

        private void LoadUpdatePanel()
        {
            AJAX.RegisterScriptManager();
            var scriptManager = AJAX.GetScriptManager(Page);
            if (scriptManager != null)
            {
                scriptManager.EnablePartialRendering = true;
            }
            var updatePanel = new UpdatePanel
                                  {
                                      UpdateMode = UpdatePanelUpdateMode.Conditional, 
                                      ID = _control.ID + "_UP"
                                  };

            var templateContainer = updatePanel.ContentTemplateContainer;
            InjectMessageControl(templateContainer);
            templateContainer.Controls.Add(_control);

            InjectModuleContent(updatePanel);

            var image = new Image
                            {
                                ImageUrl = "~/images/progressbar.gif", 
                                AlternateText = "ProgressBar"
                            };

            var updateProgress = new UpdateProgress
                                     {
                                         AssociatedUpdatePanelID = updatePanel.ID, 
                                         ID = updatePanel.ID + "_Prog", 
                                         ProgressTemplate = new LiteralTemplate(image)
                                     };
            Controls.Add(updateProgress);
        }

        private bool SupportsCaching()
        {
            return _moduleConfiguration.CacheTime > 0;
        }

        private bool TryLoadCached()
        {
            bool success = false;
            string cachedContent = string.Empty;
            try
            {
                var cache = ModuleCachingProvider.Instance(_moduleConfiguration.GetEffectiveCacheMethod());
                var varyBy = new SortedDictionary<string, string> {{"locale", Thread.CurrentThread.CurrentUICulture.ToString()}};

                string cacheKey = cache.GenerateCacheKey(_moduleConfiguration.TabModuleID, varyBy);
                byte[] cachedBytes = ModuleCachingProvider.Instance(_moduleConfiguration.GetEffectiveCacheMethod()).GetModule(_moduleConfiguration.TabModuleID, cacheKey);

                if (cachedBytes != null && cachedBytes.Length > 0)
                {
                    cachedContent = Encoding.UTF8.GetString(cachedBytes);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                cachedContent = string.Empty;
                Exceptions.LogException(ex);
                success = false;
            }
            if (success)
            {
                _control = ModuleControlFactory.CreateCachedControl(cachedContent, _moduleConfiguration);
                Controls.Add(_control);
            }
            return success;
        }

        #endregion

        #region Protected Methods

        protected override void CreateChildControls()
        {
            Controls.Clear();
            LoadModuleControl();
            if (ModuleControl != null)
            {
                if (!_isCached && _moduleConfiguration.ModuleControl.SupportsPartialRendering && AJAX.IsInstalled())
                {
                    LoadAjaxPanel();
                }
                else
                {
                    InjectMessageControl(this);
                    InjectModuleContent(_control);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Host.EnableCustomModuleCssClass)
            {
                string moduleName = ModuleControl.ModuleContext.Configuration.DesktopModule.ModuleName;
                if (moduleName != null)
                {
                    moduleName = Globals.CleanName(moduleName);
                }
                Attributes.Add("class", string.Format("DNNModuleContent Mod{0}C", moduleName));
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (_isCached)
            {
                base.RenderContents(writer);
            }
            else
            {
                if (SupportsCaching() && IsViewMode() && !Globals.IsAdminControl())
                {
                    var tempWriter = new StringWriter();

                    _control.RenderControl(new HtmlTextWriter(tempWriter));
                    string cachedOutput = tempWriter.ToString();

                    if (!string.IsNullOrEmpty(cachedOutput) && (!HttpContext.Current.Request.Browser.Crawler))
                    {
                        var moduleContent = Encoding.UTF8.GetBytes(cachedOutput);
                        var cache = ModuleCachingProvider.Instance(_moduleConfiguration.GetEffectiveCacheMethod());

                        var varyBy = new SortedDictionary<string, string> {{"locale", Thread.CurrentThread.CurrentUICulture.ToString()}};

                        var cacheKey = cache.GenerateCacheKey(_moduleConfiguration.TabModuleID, varyBy);
                        cache.SetModule(_moduleConfiguration.TabModuleID, cacheKey, new TimeSpan(0, 0, _moduleConfiguration.CacheTime), moduleContent);
                    }
                    writer.Write(cachedOutput);
                }
                else
                {
                    base.RenderContents(writer);
                }
            }
        }

        #endregion

    }
}
