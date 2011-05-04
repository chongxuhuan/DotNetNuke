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
using System.Text.RegularExpressions;
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.UI.Containers
    /// Class	 : ActionsMenu
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ActionsMenu provides a menu for a collection of actions.
    /// </summary>
    /// <remarks>
    /// ActionsMenu inherits from CompositeControl, and implements the IActionControl
    /// Interface. It uses the Navigation Providers to implement the Menu.
    /// </remarks>
    /// <history>
    /// 	[cnurse]	12/24/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ActionsMenu : Control, IActionControl
    {
        private ActionManager _ActionManager;
        private ModuleAction _ActionRoot;
        private int _ExpandDepth = -1;
        private NavigationProvider _ProviderControl;
        private string _ProviderName = "DNNMenuNavigationProvider";

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ActionRoot
        /// </summary>
        /// <returns>A ModuleActionCollection</returns>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected ModuleAction ActionRoot
        {
            get
            {
                if (_ActionRoot == null)
                {
                    _ActionRoot = new ModuleAction(ModuleControl.ModuleContext.GetNextActionID(), " ", "", "", "action.gif");
                }
                return _ActionRoot;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Provider Control
        /// </summary>
        /// <returns>A NavigationProvider</returns>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected NavigationProvider ProviderControl
        {
            get
            {
                return _ProviderControl;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Expansion Depth for the Control
        /// </summary>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int ExpandDepth
        {
            get
            {
                if (PopulateNodesFromClient == false || ProviderControl.SupportsPopulateOnDemand == false)
                {
                    return -1;
                }
                return _ExpandDepth;
            }
            set
            {
                _ExpandDepth = value;
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets and Sets the Path to the Script Library for the provider
 /// </summary>
 /// <returns>A String</returns>
 /// <history>
 /// 	[cnurse]	12/24/2007  created
 /// </history>
 /// -----------------------------------------------------------------------------
        public string PathSystemScript { get; set; }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets and Sets whether the Menu should be populated from the client
 /// </summary>
 /// <returns>A Boolean</returns>
 /// <history>
 /// 	[cnurse]	12/24/2007  created
 /// </history>
 /// -----------------------------------------------------------------------------
        public bool PopulateNodesFromClient { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Name of the provider to use
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ProviderName
        {
            get
            {
                return _ProviderName;
            }
            set
            {
                _ProviderName = value;
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// BindMenu binds the Navigation Provider to the Node Collection
        /// </summary>
        /// <param name="objNodes">The Nodes collection to bind</param>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void BindMenu(DNNNodeCollection objNodes)
        {
            Visible = ActionManager.DisplayControl(objNodes);
            if (Visible)
            {
                ProviderControl.ClearNodes();
                foreach (DNNNode objNode in objNodes)
                {
                    ProcessNodes(objNode);
                }
                ProviderControl.Bind(objNodes);
            }
        }

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ProcessNodes proceses a single node and its children
		/// </summary>
		/// <param name="objParent">The Node to process</param>
		/// <history>
		/// 	[cnurse]	12/24/2007  created
		/// </history>
		/// -----------------------------------------------------------------------------

        private void ProcessNodes(DNNNode objParent)
        {
            if (!String.IsNullOrEmpty(objParent.JSFunction))
            {
                objParent.JSFunction = string.Format("if({0}){{{1}}};", objParent.JSFunction, Page.ClientScript.GetPostBackEventReference(ProviderControl.NavigationControl, objParent.ID));
            }
            foreach (DNNNode objNode in objParent.DNNNodes)
            {
                ProcessNodes(objNode);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SetMenuDefaults sets up the default values
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void SetMenuDefaults()
        {
            try
            {
                ProviderControl.StyleIconWidth = 15;
                ProviderControl.MouseOutHideDelay = 500;
                ProviderControl.MouseOverAction = NavigationProvider.HoverAction.Expand;
                ProviderControl.MouseOverDisplay = NavigationProvider.HoverDisplay.None;
                ProviderControl.CSSControl = "ModuleTitle_MenuBar";
                ProviderControl.CSSContainerRoot = "ModuleTitle_MenuContainer";
                ProviderControl.CSSNode = "ModuleTitle_MenuItem";
                ProviderControl.CSSIcon = "ModuleTitle_MenuIcon";
                ProviderControl.CSSContainerSub = "ModuleTitle_SubMenu";
                ProviderControl.CSSBreak = "ModuleTitle_MenuBreak";
                ProviderControl.CSSNodeHover = "ModuleTitle_MenuItemSel";
                ProviderControl.CSSIndicateChildSub = "ModuleTitle_MenuArrow";
                ProviderControl.CSSIndicateChildRoot = "ModuleTitle_RootMenuArrow";
                if (String.IsNullOrEmpty(ProviderControl.PathSystemScript))
                {
                    ProviderControl.PathSystemScript = Globals.ApplicationPath + "/Controls/SolpartMenu/";
                }
                ProviderControl.PathImage = Globals.ApplicationPath + "/Images/";
                ProviderControl.PathSystemImage = Globals.ApplicationPath + "/Images/";
                ProviderControl.IndicateChildImageSub = "action_right.gif";
                ProviderControl.IndicateChildren = true;
                ProviderControl.StyleRoot = "background-color: Transparent; font-size: 1pt;";
                ProviderControl.NodeClick += MenuItem_Click;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// BindMenu binds the Navigation Provider to the Node Collection
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void BindMenu()
        {
            BindMenu(Navigation.GetActionNodes(ActionRoot, this, ExpandDepth));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// OnAction raises the Action Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/23/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected virtual void OnAction(ActionEventArgs e)
        {
            if (Action != null)
            {
                Action(this, e);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            _ProviderControl = NavigationProvider.Instance(ProviderName);
            ProviderControl.PopulateOnDemand += ProviderControl_PopulateOnDemand;
            base.OnInit(e);
            ProviderControl.ControlID = "ctl" + ID;
            ProviderControl.Initialize();
            Controls.Add(ProviderControl.NavigationControl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActionRoot.Actions.AddRange(ModuleControl.ModuleContext.Actions);
            SetMenuDefaults();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindMenu();
        }

        private void MenuItem_Click(NavigationEventArgs args)
        {
            if (Regex.IsMatch(args.ID, "^\\d+$"))
            {
                ModuleAction action = ModuleControl.ModuleContext.Actions.GetActionByID(Convert.ToInt32(args.ID));
                if (!ActionManager.ProcessAction(action))
                {
                    OnAction(new ActionEventArgs(action, ModuleControl.ModuleContext.Configuration));
                }
            }
        }

        private void ProviderControl_PopulateOnDemand(NavigationEventArgs args)
        {
            SetMenuDefaults();
            ActionRoot.Actions.AddRange(ModuleControl.ModuleContext.Actions);
            ModuleAction objAction = ActionRoot;
            if (ActionRoot.ID != Convert.ToInt32(args.ID))
            {
                objAction = ModuleControl.ModuleContext.Actions.GetActionByID(Convert.ToInt32(args.ID));
            }
            if (args.Node == null)
            {
                args.Node = Navigation.GetActionNode(args.ID, ProviderControl.ID, objAction, this);
            }
            ProviderControl.ClearNodes();
            BindMenu(Navigation.GetActionNodes(objAction, args.Node, this, ExpandDepth));
        }
    }
}
