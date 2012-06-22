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
    /// The Telerik Editor object from the HTML module and the Messaging module.
    /// </summary>
    public class TelerikEditor : ModulePage
    {
        #region Constructors

        public TelerikEditor(WatiNBase watinBase) : this(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public TelerikEditor(IE ieInstance, string siteUrl, string dbName)
            : base(ieInstance, siteUrl, dbName)
        {
        }

        #endregion

        #region Public Properties

        #region TableCells
        /// <summary>
        /// The table cell containing the toolbar for the editor in the messaging module.
        /// </summary>
        public TableCell MessageEditorToolbar
        {
            get { return ContentPaneDiv.TableCell(Find.ById(s => s.EndsWith("EditMessage_messageEditor_messageEditorTop"))); }
        }
        /// <summary>
        /// The table cell containing the toolbar for the editor in the html module.
        /// </summary>
        public TableCell ContentEditorToolbar
        {
            get { return ContentPaneDiv.TableCell((Find.ById(s => s.EndsWith("EditHTML_txtContent_txtContentTop")))); }
        }
        #endregion

        #region Tables
        /// <summary>
        /// The table containing the contents of the current folder in the Document manager pop up.
        /// </summary>
        public Table FolderContentsTable
        {
            get { return TelerikPopUpFrame.Div(Find.ById("RadFileExplorer1_grid_GridData")).Table(Find.ById(s => s.StartsWith("RadFileExplorer1_grid"))); }
        }
        #endregion

        #region Frames
        public Frame TelerikPopUpFrame
        {
            get
            {
				if (PopUpFrame != null)
				{
					return PopUpFrame.Frame(Find.ByName("Window"));
				}

            	return IEInstance.Frame(Find.ByName("Window"));
            }
        }
        #endregion

        #region Links
        public Link HyperLinkManagerLink
        {
            get
            {
				if (MessageEditorToolbar.Exists)
				{
					return MessageEditorToolbar.Link(Find.ByTitle("Hyperlink Manager (CTRL+K)"));
				}

            	return ContentEditorToolbar.Link(Find.ByTitle("Hyperlink Manager (CTRL+K)"));
            }
        }
        /// <summary>
        /// The link to expand the Page drop down in the hyperlink manager.
        /// </summary>
        public Link HyperlinkPageSelectLink
        {
            get { return DialogDiv.Link(Find.ById("PageOnSite_Arrow")); }
        }
        public Link TelerikNewFolderLink
        {
            get { return TelerikPopUpFrame.Div(Find.ById("dialogControl")).Div(Find.ByClass("rtbInner")).Link(Find.ByTitle("New Folder")); }
        }
        public Link ContentImageManagerLink
        {
            get { return ContentEditorToolbar.Link(Find.ByTitle("Insert Media")); }
        }
        public Link UploadImageLink
        {
            get { return TelerikPopUpFrame.Link(Find.ByTitle("Upload")); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The outer div in the telerik pop up frame.
        /// </summary>
        public Div DialogDiv
        {
            get { return TelerikPopUpFrame.Div(Find.ById("dialogControl")); }
        }
        /// <summary>
        /// The div containing the drop down elements for the page select list.
        /// </summary>
        public Div HyperlinkPageComboBox
        {
            get { return DialogDiv.Div(Find.ById("PageOnSite_DropDown")); }
        }
        public Div UploadImagePopUp
        {
            get { return TelerikPopUpFrame.Div(Find.ById(s => s.EndsWith("windowManagerfileExplorerUpload"))); }
        }
        #endregion

        #region Elements
        /// <summary>
        /// The ul element containing all of the options for the page select list in the hyperlink manager.
        /// </summary>
        public Element HyperlinkPageSelectList
        {
            get { return TelerikPopUpFrame.Element(Find.ByClass("rcbList")); }
        }
        #endregion

        #region TextFields
        public TextField TelerikNewFolderNameField
        {
            get { return TelerikPopUpFrame.TableCell(Find.ByClass("rwWindowContent")).TextField(Find.ByValue("NewFolder")); }
        }
        public TextField HyperLinkTextField
        {
            get { return DialogDiv.TextField(Find.ById("LinkText")); }
        }
        #endregion

        #region Spans
        public Span NewFolderOKLink
        {
            get { return TelerikPopUpFrame.TableCell(Find.ByClass("rwWindowContent")).Span(Find.ByText("OK")); }
        }
        #endregion

        #region Buttons
        public Button TelerikCancelButton
        {
            get { return TelerikPopUpFrame.Button(Find.ById("CancelButton")); }
        }
        /// <summary>
        /// The Upload File Button from the upload pop up in the document/image manager. 
        /// </summary>
        public Button UploadImageButton
        {
            get { return UploadImagePopUp.Button(Find.ByValue("Upload")); }
        }
        public Button HyperlinkOKButton
        {
            get { return DialogDiv.Button(Find.ByTitle("OK")); }
        }
        #endregion

        #region FileUploads
        /// <summary>
        /// The first file upload in the Upload pop up in the document/image manager.
        /// </summary>
        public FileUpload DocumentFileUpload
        {
            get { return UploadImagePopUp.FileUpload(Find.ById("RadFileExplorer1_upload1file0")); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox HyperLinkTrackLinkCheckBox
        {
            get { return DialogDiv.CheckBox(Find.ById("TrackLink")); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the Hyperlink manager link in the editor toolbar
        /// </summary>
        /// <param name="toolbarType">The type of toolbar. Either "Message" (for the messaging module), or "HTML" (for the HTML module).</param>
        /// <returns>The hyperlink manager link.</returns>
        public Link GetHyperLinkManagerLink(string toolbarType)
        {
            Link hyperLink = null;
            TableCell toolBar = null;
            if(toolbarType.CompareTo("Message") == 0)
            {
                toolBar = MessageEditorToolbar; 
            }
            else if (toolbarType.CompareTo("HTML") == 0)
            {
                toolBar = ContentEditorToolbar; 
            }
            hyperLink = toolBar.Link(Find.ByTitle("Hyperlink Manager (CTRL+K)"));
            return hyperLink;
        }

        /// <summary>
        /// Finds the Document manager link in the editor toolbar
        /// </summary>
        /// <param name="toolbarType">The type of toolbar. Either "Message" (for the messaging module), or "HTML" (for the HTML module).</param>
        /// <returns>The document manager link.</returns>
        public Link GetDocumentManagerLink(string toolbarType)
        {
            Link documentManager;
            TableCell toolBar = null;
            if (toolbarType.CompareTo("Message") == 0)
            {
                toolBar = MessageEditorToolbar;
            }
            else if (toolbarType.CompareTo("HTML") == 0)
            {
                toolBar = ContentEditorToolbar;
            }
            documentManager = toolBar.Link(Find.ByTitle("Document Manager"));
            return documentManager;
        }

        /// <summary>
        /// Finds the Folder span within the document/image/etc manager pop up.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <returns>The span for the folder.</returns>
        public Span FindPopUpFolderSpanByFolderName(string folderName)
        {
            return TelerikPopUpFrame.Span(Find.ByText(folderName));
        }
        
        /// <summary>
        /// Finds and selects the page element within the page select list in the Hyperlink manager.
        /// </summary>
        /// <param name="itemText">The page name to select.</param>
        public void SelectPageForHyperLink(string itemText)
        {
            //Click Drop down
            //This select list defaults to nothing selected, therefor all page options will have the class rcbItem 
            string selectListClass = "rcbItem ";
            HyperlinkPageComboBox.Click();
            System.Threading.Thread.Sleep(3000);
            //Find Item to Select
            //Find all List Item elements.
            ElementCollection selectListElements = DialogDiv.Div(Find.ById("PageOnSite_DropDown")).Elements.Filter(Find.ByClass(selectListClass));
            Element result = null;
            //Search for the desired Element
            foreach (Element e in selectListElements)
            {
                if (itemText.CompareTo(e.Text) == 0)
                {
                    //Select the Element
                    result = e;
                    break;
                }
                continue;
            }
            //Select the item
            result.FireEvent("onmouseover");
            result.Click();
        }

        public Frame GetDialog(string dialogName)
        {
            if(PopUpFrame != null)
            {
                return PopUpFrame.Frame(Find.BySrc(s => s.Contains("DialogName=" + dialogName)));
            }

            return IEInstance.Frame(Find.BySrc(s => s.Contains("DialogName=" + dialogName)));
        }
        
        #endregion
    }
}


