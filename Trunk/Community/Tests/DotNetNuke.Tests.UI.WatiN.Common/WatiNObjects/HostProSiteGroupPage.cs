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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
	/// <summary>
	/// The host settings page object.
	/// </summary>
	public class HostProSiteGroupPage : WatiNBase
	{
		#region Constructors

		public HostProSiteGroupPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

		public HostProSiteGroupPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

		#endregion

		#region Public Properties

		public Link CreateSiteGroupButton
		{
			get
			{
				return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("PortalGroupListView_createButton")));
			}
		}

		public Link DoCreateButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ById(s => s.EndsWith("EditPortalGroupView_createButton")));
				}

				return ContentPaneDiv.Link((Find.ById(s => s.EndsWith("EditPortalGroupView_createButton"))));
			}
		}

		public TextField GroupNameField
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditPortalGroupView_nameTextBox")));
				}

				return ContentPaneDiv.TextField((Find.ById(s => s.EndsWith("EditPortalGroupView_nameTextBox"))));
			}
		}


		public TextField GroupDescriptionField
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditPortalGroupView_descriptionTextBox")));
				}

				return ContentPaneDiv.TextField((Find.ById(s => s.EndsWith("EditPortalGroupView_descriptionTextBox"))));
			}
		}

		public SelectList PortalSelectList
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("EditPortalGroupView_availablePortalsList")));
				}

				return ContentPaneDiv.SelectList((Find.ById(s => s.EndsWith("EditPortalGroupView_availablePortalsList"))));
			}
		}

		public TextField AuthDomainField
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditPortalGroupView_domainTextBox")));
				}

				return ContentPaneDiv.TextField((Find.ById(s => s.EndsWith("EditPortalGroupView_domainTextBox"))));
			}
		}

		public SelectList AvailablePortalSelectList
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("portalsDualListBox_Available")));
				}

				return ContentPaneDiv.SelectList((Find.ByName(s => s.EndsWith("portalsDualListBox_Available"))));
			}
		}

		public SelectList SelectedPortalSelectList
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("portalsDualListBox_Selected")));
				}

				return ContentPaneDiv.SelectList((Find.ByName(s => s.EndsWith("portalsDualListBox_Selected"))));
			}
		}

		public Link AddSiteToGroupButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ByTitle("Add Site to Group"));
				}

				return ContentPaneDiv.Link(Find.ByTitle("Add Site to Group"));
			}
		}

		public Link AddAllSiteToGroupButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ByTitle("Add All Sites to Group"));
				}

				return ContentPaneDiv.Link(Find.ByTitle("Add All Sites to Group"));
			}
		}

		public Link RemoveSiteToGroupButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ByTitle("Remove Site from Group"));
				}

				return ContentPaneDiv.Link(Find.ByTitle("Remove Site from Group"));
			}
		}

		public CheckBox CopyUserCheckBox
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.CheckBox(Find.ByName(s => s.EndsWith("EditPortalGroupView_copyUsers")));
				}

				return ContentPaneDiv.CheckBox((Find.ByName(s => s.EndsWith("EditPortalGroupView_copyUsers"))));
			}
		}

		public Link UpdateButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ByName(s => s.EndsWith("EditPortalGroupView_updateButton")));
				}

				return ContentPaneDiv.Link((Find.ByName(s => s.EndsWith("EditPortalGroupView_updateButton"))));
			}
		}

		public Link DeleteButton
		{
			get
			{
				if (PopUpFrame != null)
				{
					return PopUpFrame.Link(Find.ByName(s => s.EndsWith("EditPortalGroupView_deleteButton")));
				}

				return ContentPaneDiv.Link((Find.ByName(s => s.EndsWith("EditPortalGroupView_deleteButton"))));
			}
		}


		#endregion
	}
}
