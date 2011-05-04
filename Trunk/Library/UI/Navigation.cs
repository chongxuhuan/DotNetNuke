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
using System.Web.UI;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Containers;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI
{
    public class Navigation
    {
        #region NavNodeOptions enum

        public enum NavNodeOptions
        {
            IncludeSelf = 1,
            IncludeParent = 2,
            IncludeSiblings = 4,
            MarkPendingNodes = 8,
            IncludeHiddenNodes = 16
        }

        #endregion

        #region ToolTipSource enum

        public enum ToolTipSource
        {
            TabName,
            Title,
            Description,
            None
        }

        #endregion

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Recursive function to add module's actions to the DNNNodeCollection based off of passed in ModuleActions
        /// </summary>
        /// <param name="objParentAction">Parent action</param>
        /// <param name="objParentNode">Parent node</param>
        /// <param name="objActionControl">ActionControl to base actions off of</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/9/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddChildActions(ModuleAction objParentAction, DNNNode objParentNode, IActionControl objActionControl)
        {
            AddChildActions(objParentAction, objParentNode, objParentNode, objActionControl, -1);
        }

        private static void AddChildActions(ModuleAction objParentAction, DNNNode objParentNode, DNNNode objRootNode, IActionControl objActionControl, int intDepth)
        {
            bool blnPending;
            foreach (ModuleAction objAction in objParentAction.Actions)
            {
                blnPending = IsActionPending(objParentNode, objRootNode, intDepth);
                if (objAction.Title == "~")
                {
                    if (blnPending == false)
                    {
                        objParentNode.DNNNodes.AddBreak();
                    }
                }
                else
                {
                    if (objAction.Visible &&
                        (objAction.Secure != SecurityAccessLevel.Anonymous ||
                         ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objActionControl.ModuleControl.ModuleContext.Configuration)) &&
                        ModulePermissionController.HasModuleAccess(objAction.Secure, Null.NullString, objActionControl.ModuleControl.ModuleContext.Configuration))
                    {
                        if (blnPending)
                        {
                            objParentNode.HasNodes = true;
                        }
                        else
                        {
                            DNNNode objNode;
                            int i = objParentNode.DNNNodes.Add();
                            objNode = objParentNode.DNNNodes[i];
                            objNode.ID = objAction.ID.ToString();
                            objNode.Key = objAction.ID.ToString();
                            objNode.Text = objAction.Title;
                            if (!String.IsNullOrEmpty(objAction.ClientScript))
                            {
                                objNode.JSFunction = objAction.ClientScript;
                                objNode.ClickAction = eClickAction.None;
                            }
                            else
                            {
                                objNode.NavigateURL = objAction.Url;
                                if (objAction.UseActionEvent == false && !String.IsNullOrEmpty(objNode.NavigateURL))
                                {
                                    objNode.ClickAction = eClickAction.Navigate;
                                    if (objAction.NewWindow)
                                    {
                                        objNode.Target = "_blank";
                                    }
                                }
                                else
                                {
                                    objNode.ClickAction = eClickAction.PostBack;
                                }
                            }
                            objNode.Image = objAction.Icon;
                            if (objAction.HasChildren())
                            {
                                AddChildActions(objAction, objNode, objRootNode, objActionControl, intDepth);
                            }
                        }
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Assigns common properties from passed in tab to newly created DNNNode that is added to the passed in DNNNodeCollection
        /// </summary>
        /// <param name="objTab">Tab to base DNNNode off of</param>
        /// <param name="objNodes">Node collection to append new node to</param>
        /// <param name="objBreadCrumbs">Hashtable of breadcrumb IDs to efficiently determine node's BreadCrumb property</param>
        /// <param name="objPortalSettings">Portal settings object to determine if node is selected</param>
        /// <param name="eToolTips"></param>
        /// <remarks>
        /// Logic moved to separate sub to make GetNavigationNodes cleaner
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/9/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddNode(TabInfo objTab, DNNNodeCollection objNodes, Hashtable objBreadCrumbs, PortalSettings objPortalSettings, ToolTipSource eToolTips)
        {
            var objNode = new DNNNode();
            if (objTab.Title == "~")
            {
                objNodes.AddBreak();
            }
            else
            {
                if (objBreadCrumbs.Contains(objTab.TabID))
                {
                    objNode.BreadCrumb = true;
                    if (objTab.TabID == objPortalSettings.ActiveTab.TabID)
                    {
                        objNode.Selected = true;
                    }
                }
                if (objTab.DisableLink)
                {
                    objNode.Enabled = false;
                }
                objNode.ID = objTab.TabID.ToString();
                objNode.Key = objNode.ID;
                objNode.Text = objTab.LocalizedTabName;
                objNode.NavigateURL = objTab.FullUrl;
                objNode.ClickAction = eClickAction.Navigate;
                objNode.Image = objTab.IconFile;
                objNode.LargeImage = objTab.IconFileLarge;
                switch (eToolTips)
                {
                    case ToolTipSource.TabName:
                        objNode.ToolTip = objTab.LocalizedTabName;
                        break;
                    case ToolTipSource.Title:
                        objNode.ToolTip = objTab.Title;
                        break;
                    case ToolTipSource.Description:
                        objNode.ToolTip = objTab.Description;
                        break;
                }
                objNodes.Add(objNode);
            }
        }

        private static bool IsActionPending(DNNNode objParentNode, DNNNode objRootNode, int intDepth)
        {
            if (intDepth == -1)
            {
                return false;
            }
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
            {
                return false;
            }
            return true;
        }

        private static bool IsTabPending(TabInfo objTab, DNNNode objParentNode, DNNNode objRootNode, int intDepth, Hashtable objBreadCrumbs, int intLastBreadCrumbId, bool blnPOD)
        {
            if (intDepth == -1)
            {
                return false;
            }
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
            {
                return false;
            }
            if (blnPOD)
            {
                if (objBreadCrumbs.Contains(objTab.TabID))
                {
                    return false;
                }
                if (objBreadCrumbs.Contains(objTab.ParentId) && intLastBreadCrumbId != objTab.ParentId)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsTabSibling(TabInfo objTab, int intStartTabId, Hashtable objTabLookup)
        {
            if (intStartTabId == -1)
            {
                if (objTab.ParentId == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (objTab.ParentId == ((TabInfo) objTabLookup[intStartTabId]).ParentId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ProcessTab(DNNNode objRootNode, TabInfo objTab, Hashtable objTabLookup, Hashtable objBreadCrumbs, int intLastBreadCrumbId, ToolTipSource eToolTips, int intStartTabId,
                                       int intDepth, int intNavNodeOptions)
        {
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            DNNNodeCollection objRootNodes = objRootNode.DNNNodes;

            bool showHidden = (intNavNodeOptions & (int)NavNodeOptions.IncludeHiddenNodes) == (int)NavNodeOptions.IncludeHiddenNodes;

            if (CanShowTab(objTab, TabPermissionController.CanAdminPage(), true, showHidden))
            {
                DNNNodeCollection objParentNodes;
                DNNNode objParentNode = objRootNodes.FindNode(objTab.ParentId.ToString());
                bool blnParentFound = objParentNode != null;
                if (objParentNode == null)
                {
                    objParentNode = objRootNode;
                }
                objParentNodes = objParentNode.DNNNodes;
                if (objTab.TabID == intStartTabId)
                {
                    if ((intNavNodeOptions & (int) NavNodeOptions.IncludeParent) != 0)
                    {
                        if (objTabLookup[objTab.ParentId] != null)
                        {
                            AddNode((TabInfo) objTabLookup[objTab.ParentId], objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                            objParentNode = objRootNodes.FindNode(objTab.ParentId.ToString());
                            objParentNodes = objParentNode.DNNNodes;
                        }
                    }
                    if ((intNavNodeOptions & (int) NavNodeOptions.IncludeSelf) != 0)
                    {
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                    }
                }
                else if (((intNavNodeOptions & (int) NavNodeOptions.IncludeSiblings) != 0) && IsTabSibling(objTab, intStartTabId, objTabLookup))
                {
                    AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                }
                else
                {
                    if (blnParentFound)
                    {
                        if (((intNavNodeOptions & (int) NavNodeOptions.IncludeSiblings) != 0) || IsTabSibling(objTab, intStartTabId, objTabLookup) == false)
                        {
                            bool blnPOD = (intNavNodeOptions & (int) NavNodeOptions.MarkPendingNodes) != 0;
                            if (IsTabPending(objTab, objParentNode, objRootNode, intDepth, objBreadCrumbs, intLastBreadCrumbId, blnPOD))
                            {
                                if (blnPOD)
                                {
                                    objParentNode.HasNodes = true;
                                }
                            }
                            else
                            {
                                AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                            }
                        }
                    }
                    else if ((intNavNodeOptions & (int) NavNodeOptions.IncludeSelf) == 0 && objTab.ParentId == intStartTabId)
                    {
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                    }
                }
            }
        }

        public static bool CanShowTab(TabInfo objTab, bool isAdminMode, bool showDisabled)
        {
          bool showHidden = false;
          return CanShowTab(objTab, isAdminMode, showDisabled, showHidden);
        }

        public static bool CanShowTab(TabInfo objTab, bool isAdminMode, bool showDisabled, bool showHidden)
        {
            if ((objTab.IsVisible || showHidden) && !objTab.IsDeleted && (!objTab.DisableLink || showDisabled) &&
                (((objTab.StartDate < DateTime.Now || objTab.StartDate == Null.NullDate) && (objTab.EndDate > DateTime.Now || objTab.EndDate == Null.NullDate)) || isAdminMode) &&
                TabPermissionController.CanNavigateToPage(objTab))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Allows for DNNNode object to be easily obtained based off of passed in ID
        /// </summary>
        /// <param name="strID">NodeID to retrieve</param>
        /// <param name="strNamespace">Namespace for node collection (usually control's ClientID)</param>
        /// <param name="objActionRoot">Root Action object used in searching</param>
        /// <param name="objControl">ActionControl to base actions off of</param>
        /// <returns>DNNNode</returns>
        /// <remarks>
        /// Primary purpose of this is to obtain the DNNNode needed for the events exposed by the NavigationProvider
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static DNNNode GetActionNode(string strID, string strNamespace, ModuleAction objActionRoot, Control objControl)
        {
            DNNNodeCollection objNodes = GetActionNodes(objActionRoot, objControl, -1);
            DNNNode objNode = objNodes.FindNode(strID);
            var objReturnNodes = new DNNNodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This function provides a central location to obtain a generic node collection of the actions associated
        /// to a module based off of the current user's context
        /// </summary>
        /// <param name="objActionRoot">Root module action</param>
        /// <param name="objControl">ActionControl to base actions off of</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/9/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static DNNNodeCollection GetActionNodes(ModuleAction objActionRoot, Control objControl)
        {
            return GetActionNodes(objActionRoot, objControl, -1);
        }

        public static DNNNodeCollection GetActionNodes(ModuleAction objActionRoot, Control objControl, int intDepth)
        {
            var objCol = new DNNNodeCollection(objControl.ClientID);
            var objActionControl = objControl as IActionControl;
            if (objActionControl != null)
            {
                if (objActionRoot.Visible)
                {
                    objCol.Add();
                    DNNNode objRoot = objCol[0];
                    objRoot.ID = objActionRoot.ID.ToString();
                    objRoot.Key = objActionRoot.ID.ToString();
                    objRoot.Text = objActionRoot.Title;
                    objRoot.NavigateURL = objActionRoot.Url;
                    objRoot.Image = objActionRoot.Icon;
                    objRoot.Enabled = false;
                    AddChildActions(objActionRoot, objRoot, objRoot.ParentNode, objActionControl, intDepth);
                }
            }
            return objCol;
        }

        public static DNNNodeCollection GetActionNodes(ModuleAction objActionRoot, DNNNode objRootNode, Control objControl, int intDepth)
        {
            DNNNodeCollection objCol = objRootNode.ParentNode.DNNNodes;
            var objActionControl = objControl as IActionControl;
            if (objActionControl != null)
            {
                AddChildActions(objActionRoot, objRootNode, objRootNode, objActionControl, intDepth);
            }
            return objCol;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Allows for DNNNode object to be easily obtained based off of passed in ID
        /// </summary>
        /// <param name="strID">NodeID to retrieve</param>
        /// <param name="strNamespace">Namespace for node collection (usually control's ClientID)</param>
        /// <returns>DNNNode</returns>
        /// <remarks>
        /// Primary purpose of this is to obtain the DNNNode needed for the events exposed by the NavigationProvider
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/9/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static DNNNode GetNavigationNode(string strID, string strNamespace)
        {
            DNNNodeCollection objNodes = GetNavigationNodes(strNamespace);
            DNNNode objNode = objNodes.FindNode(strID);
            var objReturnNodes = new DNNNodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This function provides a central location to obtain a generic node collection of the pages/tabs included in
        /// the current context's (user) navigation hierarchy
        /// </summary>
        /// <param name="strNamespace">Namespace (typically control's ClientID) of node collection to create</param>
        /// <returns>Collection of DNNNodes</returns>
        /// <remarks>
        /// Returns all navigation nodes for a given user
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/9/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static DNNNodeCollection GetNavigationNodes(string strNamespace)
        {
            return GetNavigationNodes(strNamespace, ToolTipSource.None, -1, -1, 0);
        }

        public static DNNNodeCollection GetNavigationNodes(string strNamespace, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions)
        {
            var objCol = new DNNNodeCollection(strNamespace);
            return GetNavigationNodes(new DNNNode(objCol.XMLNode), eToolTips, intStartTabId, intDepth, intNavNodeOptions);
        }

        public static DNNNodeCollection GetNavigationNodes(DNNNode objRootNode, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions)
        {
            int i;
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            bool blnFoundStart = intStartTabId == -1;
            var objBreadCrumbs = new Hashtable();
            var objTabLookup = new Hashtable();
            DNNNodeCollection objRootNodes = objRootNode.DNNNodes;
            int intLastBreadCrumbId = 0;
            for (i = 0; i <= (objPortalSettings.ActiveTab.BreadCrumbs.Count - 1); i++)
            {
                objBreadCrumbs.Add(((TabInfo) objPortalSettings.ActiveTab.BreadCrumbs[i]).TabID, 1);
                intLastBreadCrumbId = ((TabInfo) objPortalSettings.ActiveTab.BreadCrumbs[i]).TabID;
            }
            var objTabController = new TabController();
            List<TabInfo> portalTabs = TabController.GetTabsBySortOrder(objPortalSettings.PortalId, objPortalSettings.CultureCode, true);
            List<TabInfo> hostTabs = TabController.GetTabsBySortOrder(Null.NullInteger, Localization.SystemLocale, true);
            foreach (TabInfo objTab in portalTabs)
            {
                objTabLookup.Add(objTab.TabID, objTab);
            }
            foreach (TabInfo objTab in hostTabs)
            {
                objTabLookup.Add(objTab.TabID, objTab);
            }
            foreach (TabInfo objTab in portalTabs)
            {
                try
                {
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            foreach (TabInfo objTab in hostTabs)
            {
                try
                {
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objRootNodes;
        }
    }
}
