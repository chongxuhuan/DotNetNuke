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
using System.Web.UI;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    public abstract class ActionBase : UserControl, IActionControl
    {
        private ActionManager _ActionManager;
        private ModuleAction _ActionRoot;

        [Obsolete("Obsoleted in DotNetNuke 5.1.2. The concept of an adminControl no longer exists.")]
        protected bool m_adminControl;

        [Obsolete("Obsoleted in DotNetNuke 5.1.2. The concept of an adminModule no longer exists.")]
        protected bool m_adminModule;

        [Obsolete("Obsoleted in DotNetNuke 5.1.2 Replaced by ActionRoot Property")]
        protected ModuleAction m_menuActionRoot;

        [Obsolete("Obsoleted in DotNetNuke 5.1.2. Replaced by Actions Property")]
        protected ModuleActionCollection m_menuActions;

        protected bool m_supportsIcons = true;

        [Obsolete("Obsoleted in DotNetNuke 5.1.2. No longer neccessary as there is no concept of an Admin Page")]
        protected bool m_tabPreview;

        protected ModuleActionCollection Actions
        {
            get
            {
                return ModuleContext.Actions;
            }
        }

        protected ModuleAction ActionRoot
        {
            get
            {
                if (_ActionRoot == null)
                {
                    _ActionRoot = new ModuleAction(ModuleContext.GetNextActionID(), " ", "", "", "action.gif");
                }
                return _ActionRoot;
            }
        }

        protected ModuleInstanceContext ModuleContext
        {
            get
            {
                return ModuleControl.ModuleContext;
            }
        }

        protected PortalSettings PortalSettings
        {
            get
            {
                PortalSettings _settings = ModuleControl.ModuleContext.PortalSettings;
                if (!_settings.ActiveTab.IsSuperTab)
                {
//still maintaining an obsolete type in public interface to maintain binary compatibility
#pragma warning disable 612,618
                    m_tabPreview = (_settings.UserMode == PortalSettings.Mode.View);
#pragma warning restore 612,618
                }
                return _settings;
            }
        }

        public bool EditMode
        {
            get
            {
                return ModuleContext.PortalSettings.UserMode != PortalSettings.Mode.View;
            }
        }

        public bool SupportsIcons
        {
            get
            {
                return m_supportsIcons;
            }
        }

        [Obsolete("Obsoleted in DotNetNuke 5.0. Use ModuleContext.Configuration")]
        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return ModuleContext.Configuration;
            }
        }

        [Obsolete("Obsoleted in DotNetNuke 5.0. Replaced by ModuleControl")]
        public PortalModuleBase PortalModule
        {
            get
            {
                return new PortalModuleBase();
            }
            set
            {
                ModuleControl = value;
            }
        }

        [Obsolete("Obsoleted in DotNetNuke 5.1.2. Replaced by Actions Property")]
        public ModuleActionCollection MenuActions
        {
            get
            {
                return Actions;
            }
        }

        #region IActionControl Members

        public event ActionEventHandler Action;

        public ActionManager ActionManager
        {
            get
            {
                if (_ActionManager == null)
                {
                    _ActionManager = new ActionManager(this);
                }
                return _ActionManager;
            }
        }

        public IModuleControl ModuleControl { get; set; }

        #endregion

        protected bool DisplayControl(DNNNodeCollection objNodes)
        {
            return ActionManager.DisplayControl(objNodes);
        }

        protected virtual void OnAction(ActionEventArgs e)
        {
            if (Action != null)
            {
                Action(this, e);
            }
        }

        protected void ProcessAction(string ActionID)
        {
            int output;
            if (Int32.TryParse(ActionID, out output))
            {
                ModuleAction action = Actions.GetActionByID(output);
                if (action != null)
                {
                    if (!ActionManager.ProcessAction(action))
                    {
                        OnAction(new ActionEventArgs(action, ModuleContext.Configuration));
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                ActionRoot.Actions.AddRange(Actions);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
