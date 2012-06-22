#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System;
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The module action menu object.
    /// </summary>
    public class ModuleActionMenu : WatiNBase
    {
        #region Constructors

        public ModuleActionMenu(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ModuleActionMenu(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links
        public Link DeleteActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Delete"))); }
        }
        public Link SettingsActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Settings"))); }
        }
        public Link CreateNewModuleActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Create New Module"))); }
        }
        public Link HelpActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Help"))); }
        }
        public Link OnlineHelpActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Online Help"))); }
        }
        public Link PrintActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Print"))); }
        }
        public Link ViewSourceActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("View Source"))); }
        }
        public Link RefreshActionMenuLink
        {
            get { return ActionMenuBody.Link(Find.ByText(s => s.Contains("Refresh"))); }
        }
        public Link MoveLeftPaneLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("To LeftPane"))); }
        }
        public Link MoveRightPaneLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("To RightPane"))); }
        }
        public Link MoveBottomPaneLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("To BottomPane"))); }
        }
        public Link MoveModuleUpLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Up"))); }
        }
        public Link MoveModuleDownLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Down"))); }
        }
        public Link MoveModuleTopLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Top"))); }
        }
        public Link MoveModuleBottomLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Bottom"))); }
        }
        #endregion

        #region Divs
        public Div ActionMenuManageDiv
        {
            get { return IEInstance.Div(Find.ByClass("dnnActionMenuTag")); }
        }
        /// <summary>
        /// The outer div containing elements in the action menu.
        /// </summary>
        public Div ActionMenuBody
        {
            get { return IEInstance.Div(Find.ByClass("dnnActionMenu ui-draggable")); }
        }
        #endregion

        #region Images
        /// <summary>
        /// The drop down image for the module action menu.
        /// </summary>
        [Obsolete("Element no longer exists.")]
        public Image ModuleActionMenuImage
        {
            get { return IEInstance.Image(Find.ById(new Regex(@"dnn_ctr\d\d\d_dnnACTIONS_ctldnnACTIONSicn\d", RegexOptions.IgnoreCase))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the action menu image for the module at the specified index.
        /// </summary>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The action menu image.</returns>
        public Image GetActionMenuImageForModule(int moduleNum)
        {
            return IEInstance.Divs.Filter(Find.ByClass("dnnActionMenuTag"))[moduleNum].Image(Find.Any);
        }

        /// <summary>
        /// Finds the action menu image for the first module within pane specified.
        /// </summary>
        /// <param name="paneId">The id of the pane div object.</param>
        /// <returns>The action menu image.</returns>
        public Image GetActionMenuImageForModuleInPane(string paneId)
        {
            return IEInstance.Div(Find.ById(paneId)).Div(Find.ByClass("dnnActionMenuTag")).Image(Find.Any);
        }

        /// <summary>
        /// Finds the move span within the module action menu of the first module within the pane specified.
        /// </summary>
        /// <param name="pane">The name of the pane. Ex. "Right" "Left" "Content" "Bottom" "Top".</param>
        /// <returns>The move span for the module.</returns>
        [Obsolete("No longer needed in 6.X")]    
        public Span GetMoveActionMenuSpanForPane(string pane)
        {
            if (pane.CompareTo("Right") == 0)
            {
                return RightPaneDiv.Table(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS"))).Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Left") == 0)
            {
                return LeftPaneDiv.Table(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS"))).Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Content") == 0)
            {
                return ContentPaneDiv.Table(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS"))).Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Bottom") == 0)
            {
                return BottomPaneDiv.Table(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS"))).Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Top") == 0)
            {
                return TopPaneDiv.Table(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS"))).Span(Find.ByText("Move"));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the move span within the module action menu of the module specified within the pane specified.
        /// </summary>
        /// <param name="pane">The name of the pane. Ex. "Right" "Left" "Content" "Bottom" "Top".</param>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The move span for the module.</returns>
        [Obsolete("No longer needed in 6.X")]
        public Span GetMoveActionMenuSpanForPaneAndModuleNumber(string pane, int moduleNum)
        {
            if (pane.CompareTo("Right") == 0)
            {
                return RightPaneDiv.Tables.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS")))[moduleNum].Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Left") == 0)
            {
                return LeftPaneDiv.Tables.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS")))[moduleNum].Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Content") == 0)
            {
                return ContentPaneDiv.Tables.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS")))[moduleNum].Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Bottom") == 0)
            {
                return BottomPaneDiv.Tables.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS")))[moduleNum].Span(Find.ByText("Move"));
            }
            else if (pane.CompareTo("Top") == 0)
            {
                return TopPaneDiv.Tables.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONS")))[moduleNum].Span(Find.ByText("Move"));
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}