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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

using DNNControls = DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Common.Lists
{

    public partial class ListEditor : PortalModuleBase
    {

        protected string Mode
        {
            get
            {
                return lstEntries.Mode;
            }
            set
            {
                lstEntries.Mode = value;
            }
        }

        private void BindList(string key)
        {
            lstEntries.SelectedKey = key;
            if (PortalSettings.ActiveTab.IsSuperTab)
            {
                lstEntries.ListPortalID = Null.NullInteger;
            }
            else
            {
                lstEntries.ListPortalID = PortalId;
            }
            lstEntries.ShowDelete = true;
            lstEntries.DataBind();
        }

        private void BindTree()
        {
            var ctlLists = new ListController();
            var colLists = ctlLists.GetListInfoCollection();
            var indexLookup = new Hashtable();
            DNNtree.TreeNodes.Clear();
            foreach (ListInfo list in colLists)
            {
                var node = new TreeNode(list.DisplayName);
                {
                    node.Key = list.Key;
                    node.ToolTip = list.EntryCount + " entries";
                    node.ImageIndex = (int) eImageType.Folder;
                }
                if (list.Level == 0)
                {
                    DNNtree.TreeNodes.Add(node);
                }
                else
                {
                    if (indexLookup[list.ParentList] != null)
                    {
                        var parentNode = (TreeNode) indexLookup[list.ParentList];
                        parentNode.TreeNodes.Add(node);
                    }
                }
                if (indexLookup[list.Key] == null)
                {
                    indexLookup.Add(list.Key, node);
                }
            }
        }

        private TreeNode GetParentNode(string ParentKey)
        {
            int i;
            for (i = 0; i <= DNNtree.TreeNodes.Count - 1; i++)
            {
                if (DNNtree.TreeNodes[i].Key == ParentKey)
                {
                    return DNNtree.TreeNodes[i];
                }
            }
            return null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lstEntries.ID = "ListEntries";
            
            //ensure that module context is forwarded from parent module to child module
            lstEntries.ModuleContext.Configuration = ModuleContext.Configuration;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DNNtree.NodeClick += DNNTree_NodeClick;
            cmdAddList.Click += cmdAddList_Click;

            try
            {
                if (!Page.IsPostBack)
                {
                    DNNtree.ImageList.Add(ResolveUrl("~/images/folder.gif"));
                    DNNtree.ImageList.Add(ResolveUrl("~/images/file.gif"));
                    DNNtree.IndentWidth = 10;
                    DNNtree.CollapsedNodeImage = ResolveUrl("~/images/max.gif");
                    DNNtree.ExpandedNodeImage = ResolveUrl("~/images/min.gif");
                    if (Request.QueryString["Key"] != null)
                    {
                        Mode = "ListEntries";
                        BindList(Request.QueryString["Key"]);
                    }
                    else
                    {
                        Mode = "NoList";
                    }
                    BindTree();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Mode == "NoList")
            {
                divDetails.Visible = false;
            }
            else
            {
                divDetails.Visible = true;
            }
        }

        private void DNNTree_NodeClick(object source, DNNTreeNodeClickEventArgs e)
        {
            Mode = "ListEntries";
            BindList(e.Node.Key);
        }

        private void cmdAddList_Click(object sender, EventArgs e)
        {
            Mode = "AddList";
            BindList("");
        }

        #region Nested type: eImageType

        private enum eImageType
        {
            Folder = 0,
            Page = 1
        }

        #endregion
    }
}