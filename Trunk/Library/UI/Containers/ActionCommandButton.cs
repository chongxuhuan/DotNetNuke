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

using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Containers
    /// Class	 : ActionCommandButton
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ActionCommandButton provides a button for a single action.
    /// </summary>
    /// <remarks>
    /// ActionBase inherits from CommandButton, and implements the IActionControl Interface.
    /// </remarks>
    /// <history>
    /// 	[cnurse]	12/23/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ActionCommandButton : CommandButton, IActionControl
    {
        private ActionManager _ActionManager;
        private ModuleAction _ModuleAction;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the ModuleAction for this Action control
        /// </summary>
        /// <returns>A ModuleAction object</returns>
        /// <history>
        /// 	[cnurse]	12/23/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleAction ModuleAction
        {
            get
            {
                if (_ModuleAction == null)
                {
                    _ModuleAction = ModuleControl.ModuleContext.Actions.GetActionByCommandName(CommandName);
                }
                return _ModuleAction;
            }
            set
            {
                _ModuleAction = value;
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

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CausesValidation = false;
            EnableViewState = false;
        }

        protected virtual void OnAction(ActionEventArgs e)
        {
            if (Action != null)
            {
                Action(this, e);
            }
        }

        protected override void OnButtonClick(EventArgs e)
        {
            base.OnButtonClick(e);
            if (!ActionManager.ProcessAction(ModuleAction))
            {
                OnAction(new ActionEventArgs(ModuleAction, ModuleControl.ModuleContext.Configuration));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (ModuleAction != null && ActionManager.IsVisible(ModuleAction))
            {
                Text = ModuleAction.Title;
                CommandArgument = ModuleAction.ID.ToString();
                if (DisplayIcon && (!string.IsNullOrEmpty(ModuleAction.Icon) || !string.IsNullOrEmpty(ImageUrl)))
                {
                    if (!string.IsNullOrEmpty(ImageUrl))
                    {
                        ImageUrl = ModuleControl.ModuleContext.Configuration.ContainerPath.Substring(0, ModuleControl.ModuleContext.Configuration.ContainerPath.LastIndexOf("/") + 1) + ImageUrl;
                    }
                    else
                    {
                        if (ModuleAction.Icon.IndexOf("/") > 0)
                        {
                            ImageUrl = ModuleAction.Icon;
                        }
                        else
                        {
                            ImageUrl = "~/images/" + ModuleAction.Icon;
                        }
                    }
                }
                ActionManager.GetClientScriptURL(ModuleAction, this);
            }
            else
            {
                Visible = false;
            }
        }
    }
}
