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
using TechTalk.SpecFlow;
using WatiN.Core;
using System.IO;

using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using TableRow = WatiN.Core.TableRow;

namespace DotNetNuke.Tests.UI.WatiN.Common
{
    public class WatiNBase
    {
        #region Private Members

        string siteUrl;
        string dbName;
        bool popUpsEnabled;
        private bool set;
        #endregion

        #region Constructors

        public WatiNBase(IE ieInstance, string siteUrl, string dbName)
        {
            ScenarioContext.Current["browser"] = ieInstance;
            this.siteUrl = siteUrl;
            this.dbName = dbName;
        }

        #endregion

        #region Protected Properties
        public bool PopUpsEnabled
        {
            set { popUpsEnabled = value; }
            get { return popUpsEnabled; }
        }
        public bool Set
        {
            set { set = value; }
            get { return set; }
        }
        public IE IEInstance
        {
            get
            {
                var key = "browser";
                if (!ScenarioContext.Current.ContainsKey(key))
                {
                    ScenarioContext.Current[key] = new IE();
                }
                return ScenarioContext.Current[key] as IE;
            }
        }

        public string SiteUrl
        {
            get
            {
                return siteUrl;
            }
        }
        public string DBName
        {
            get { return dbName; }
        }

        #endregion

        #region Public Properties
        public Int32 siteVersion 
        { 
            get
            {
                string regexString = @"(?<=localhost/DNN_)\d*";
                Match match = Regex.Match(@siteUrl, regexString);
                if(match.Success)
                {
                    return Convert.ToInt32(match.Value);
                }
                return 000;
            }
        }

        #region Common Page Elements

        #region Links
        public Link RegisterLink { get { return IEInstance.Link(Find.ById(new Regex(@"(dnn_dnnUser_registerLink)|(dnn_USER\d_registerLink)|(dnn_dnnUser_enhancedRegisterLink)", RegexOptions.IgnoreCase))); } }
        public Link LoginLink { get { return IEInstance.Link(Find.ById(new Regex("(dnn_dnnLogin_loginLink)|(dnn_LOGIN1_loginLink)|(dnn_dnnLogin_enhancedLoginLink)", RegexOptions.IgnoreCase))); } }
        public Link TermsLink { get { return IEInstance.Link(Find.ById("dnn_dnnTERMS_hypTerms")); } }
        public Link PrivacyLink { get { return IEInstance.Link(Find.ById("dnn_dnnPRIVACY_hypPrivacy")); } }
        public Link LogoLink { get { return IEInstance.Link(Find.ById(new Regex("dnn_dnnLogo_hypLogo", RegexOptions.IgnoreCase))); } }
        public Link UpdateLink
        {
            get
            {
                if (PopUpFrame != null && PopUpFrame.Link(Find.ByTitle("Update")).Exists)
                {
                    //The update link is in a pop up frame and has the title update
                    return PopUpFrame.Link(Find.ByTitle("Update"));
                }
                if (PopUpFrame != null)
                {
                    //The update link is in a pop up frame and does not have a title, but has the inner text Update
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Update")));
                }
                if (IEInstance.Link(Find.ByTitle("Update")).Exists)
                {
                    //The update link is not in a pop up frame and has the title update
                    return IEInstance.Link(Find.ByTitle("Update"));
                }
                //The update link is not in a pop up frame and does not have a title, but has the inner text Update
                return IEInstance.Link(Find.ByText(s => s.Contains("Update")));
            }
        }
        #endregion

        #region Spans
        public Span CopyrightSpan { get { return IEInstance.Span(Find.ById("dnn_dnnCopyright_lblCopyright")); } }
        public Span MessageSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Span(Find.ById(s => s.EndsWith("lblMessage")));
                }
                return IEInstance.Span(Find.ById(s => s.EndsWith("lblMessage")));
            }
        }
        public Span ModuleMessageSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(s => s.EndsWith("ModuleContent"))).Span(Find.ById(s => s.EndsWith("lblMessage")));
                }
                return IEInstance.Div(Find.ById(s => s.EndsWith("ModuleContent"))).Span(Find.ById(s => s.EndsWith("lblMessage")));
            }
        }
        public Span SecondMessageSpan { get { return ContentPaneDiv.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage")))[1]; } }
        public SpanCollection AllMessageSpans { get { return ContentPaneDiv.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage"))); } }
        public Span ModuleTitleSpan { get { return IEInstance.Span(Find.ById(s => s.EndsWith("dnnTITLE_titleLabel"))); } }
        public Span PopUpTitleSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return IEInstance.Span(Find.ById("ui-dialog-title-iPopUp"));
                }
                return ModuleTitleSpan;
            }
        }
        public SpanCollection ModuleTitleSpans { get { return IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("dnnTITLE_titleLabel"))); } }
        public Span ErrorTextLabelSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Span(Find.ById(s => s.EndsWith("errorTextLabel")));
                }
                return IEInstance.Span(Find.ById(s => s.EndsWith("errorTextLabel")));
            }
        }
        public SpanCollection MessageHeadingSpans { get { return ContentPaneDiv.Spans.Filter(Find.ById(s => s.EndsWith("lblHeading"))); } }
        #endregion

        #region Images
        public Image LogoImage { get { return IEInstance.Image(Find.ById(new Regex("dnn_dnnLogo_imgLogo", RegexOptions.IgnoreCase))); } }
        public Image ErrorImage { get { return PageContentDiv.Image(Find.ByTitle("Error")); } }
        public ImageCollection ErrorImages { get { return PageContentDiv.Images.Filter(Find.ByTitle("Error")); } }
        #endregion

        #region Divs
        public Div ConfirmationPopUpDiv
        {
            get { return IEInstance.Div(Find.ByClass(s => s.Contains("ui-dialog"))); }
        }
        public DivCollection ModuleDivs
        {
            get { return IEInstance.Divs.Filter(Find.ByClass(s => s.Contains("DnnModule"))); }
        }
        #endregion

        #region Button
        public Button YesConfirmationButton
        {
            get
            {
                return ConfirmationPopUpDiv.Button(Find.ByText("Yes"));
            }
        }
        public Button NoConfirmationButton
        {
            get
            {
                return ConfirmationPopUpDiv.Button(Find.ByText("No"));
            }
        }
        #endregion

        public Frame PopUpFrame
        {
            get
            {
                try
                {
                    return IEInstance.Frame(Find.ById("iPopUp"));
                }
                catch (FrameNotFoundException)
                {
                    return null;
                }
            }
        }
        public Element Body { get { return IEInstance.Element(Find.ById("Body")); } }
        public Element Header { get { return IEInstance.Element(Find.ById("Head")); } }
        
        #endregion

        #region Page Layout Elements
        public Div PageContentDiv
        {
            get
            {
                if (IEInstance.Div(Find.ById("Content")).Exists)
                {
                    return IEInstance.Div(Find.ById("Content"));
                }
                return IEInstance.Div(Find.ByClass("content_pad"));
            }
        }
        public Div LeftPaneDiv { get { return IEInstance.Div(Find.ById("dnn_LeftPane")); } }
        public Div ContentPaneDiv
        {
            get
            {
                if (PopUpFrame != null && PopUpFrame.Div(Find.ById("dnn_ContentPane")).Exists)
                {
                    return PopUpFrame.Div(Find.ById("dnn_ContentPane"));
                }
                if (IEInstance.Div(Find.ById("dnn_ContentPane")).Exists)
                {
                    return IEInstance.Div(Find.ById("dnn_ContentPane"));
                }
                return PageContentDiv;
            }
        }
        public Div RightPaneDiv { get { return IEInstance.Div(Find.ById("dnn_RightPane")); } }
        public Div BottomPaneDiv { get { return IEInstance.Div(Find.ById("dnn_BottomPane")); } }
        public Div TopPaneDiv { get { return IEInstance.Div(Find.ById("dnn_TopPane")); } }
        public Div FooterDiv { get { return IEInstance.Div(Find.ById("Footer")); } }
        public Div DefaultModuleContainer { get { return IEInstance.Div(Find.ByClass("c_container c_head_grey")); } }
        public Element FckEditorScriptElement
        {
            get { return IEInstance.Element(Find.BySrc(s => s.Contains("/HtmlEditorProviders/Fck/fckeditor/editor"))); }
        }
        #endregion

        #region Control Panel and Menu Elements

        #region Divs
        public Div RBAdminMenuDiv { get { return IEInstance.Div(Find.ById(s => s.EndsWith("RibbonBar_adminMenus"))); } }
        public Div ControlPanel { get { return IEInstance.Div(Find.ById(new Regex("(dnnCPWrapRight)|(ControlPanelWrapper)|(dnn_ControlPanel)"))); } }
        public Div RibbonBarDiv
        {
            get
            {
                if (ControlPanel.Div(Find.ById(new Regex("dnn_(cp_)?RibbonBar_ControlPanel"))).Exists)
                {
                    return ControlPanel.Div(Find.ById(new Regex("dnn_(cp_)?RibbonBar_ControlPanel")));
                }
                return null;
            }
        }
        #endregion

        #region Spans
        public Span MenuNavSpan { get { return IEInstance.Span(Find.ById("dnn_dnnNAV_ctldnnNAVt")); } }
        public Span MenuHomeSpan { get { return IEInstance.Span(Find.ByTitle("Home")); } }
        #endregion 

        #region Links
        public Link HostConsoleLink { get { return RBAdminMenuDiv.Link(Find.ByText(s => s.Contains("Host"))); } }
        public Link AdminConsoleLink { get { return RBAdminMenuDiv.Link(Find.ByText(s => s.Contains("Admin"))); } }
        #endregion

        #region Radio Buttons
        public RadioButton EditRadioButton { get { return ControlPanel.RadioButton(Find.ByValue("EDIT")); } }
        public RadioButton ViewRadioButton { get { return ControlPanel.RadioButton(Find.ByValue("VIEW")); } }
        public RadioButton LayoutRadioButton { get { return ControlPanel.RadioButton(Find.ByValue("LAYOUT")); } }
        #endregion

        #region Images
        public Image MaximiseControlPanelImage { get { return ControlPanel.Image(Find.ByTitle("Maximize")); } }
        public Image MinimiseControlPanelImage { get { return ControlPanel.Image(Find.ByTitle("Minimize")); } }
        #endregion

        public TableRow ControlPanelAlt { get { return IEInstance.TableRow(Find.ById("dnn_IconBar.ascx_rowControlPanel")); } }
        public SelectList ControlPanelModeSelectlist { get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("RibbonBar_ddlMode"))); } }
        public Element ControlPanelElement { get { return ControlPanel.Element(Find.ById(new Regex(@"(dnn_(cp_)?IconBar.ascx_tblControlPanel|dnn_(cp_)?RibbonBar_ControlPanel)"))); } }
        public String RibbonBarID { get { return "_RibbonBar_ControlPanel"; } }
        public String ControlPanelID { get { return "_IconBar.ascx_tblControlPanel"; } }
        #endregion

        //IE Security Page
        public Link SecurityOveride { get { return IEInstance.Link(Find.ById("overridelink")); } }

        #endregion


        #region Certificate Functions
        public void IfSecurityErrorThenBypass()
        {
            if (!IEInstance.Title.Contains("Certificate Error")) return;
            if (SecurityOveride.Exists) SecurityOveride.Click();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns true if a module with the title and content specified exists on a page.
        /// Returns false if either the title or content is not on the page. 
        /// </summary>
        /// <param name="moduleName">The module title.</param>
        /// <param name="moduleContent">The content in the module.</param>
        /// <returns></returns>
        public bool HtmlModuleExistsOnPage(string moduleName, string moduleContent)
        {
            bool exists = false;
            bool contentPresent = false;
            foreach (Div module in ModuleDivs)
            {
                if (module.Span(Find.ById(s => s.EndsWith("dnnTITLE_titleLabel"))).InnerHtml.Contains(moduleName))
                {
                    exists = true;
                    if (module.Div(Find.ById(s => s.EndsWith("HtmlModule_lblContent"))).InnerHtml.Contains(moduleContent))
                    {
                        contentPresent = true;
                    }
                }
            }
            bool result = exists && contentPresent;
            return result;
        }

        /// <summary>
        /// Returns true if the current page contains an error image and the text error in a message span.
        /// Returns false otherwise
        /// </summary>
        /// <returns></returns>
        public bool PageContainsErrors()
        {
            bool errors = false;
            if (ErrorImage.Exists)
            {
                foreach (Span msgHeaderSpan in MessageHeadingSpans)
                {
                    if (msgHeaderSpan.InnerHtml.Contains("Error") || msgHeaderSpan.InnerHtml.Contains("error"))
                    {
                        errors = true;
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// Returns the selected item in the given selectlist.
        /// Use when selectList.SelectedItem is not returning the right value.
        /// </summary>
        /// <param name="selectList">The select list to search through.</param>
        /// <returns>The selected option in the select list.</returns>
        public Option GetSelectedItemFromSelecList(SelectList selectList)
        {
            
            OptionCollection options = selectList.Options;
            Option selectedOption = null;
            foreach(Option option in options)
            {
                if (option.GetAttributeValue("selected").Contains("selected") || option.GetAttributeValue("selected").Contains("true"))
                {
                    selectedOption = option;
                }
            }
            return selectedOption;
        }

        /// <summary>
        /// Returns a collection of all the module container divs that are on the current page.
        /// </summary>
        /// <returns></returns>
        public DivCollection GetAllModuleContainers()
        {
            return IEInstance.Divs.Filter(Find.ByClass("c_container c_head_grey"));
        }

        /// <summary>
        /// Deletes all of the files in the directory at the given path. 
        /// Used to delete irrelevant email files in the email folder.
        /// </summary>
        /// <param name="emailPath">The full path to the email folder.</param>
        public void DeleteFilesInTestEmails(string emailPath)
        {
            foreach (var file in Directory.GetFiles(emailPath))
            {
                File.Delete(file);
            }
        }

        public TElement FindElement<TElement>(Constraint findBy) where TElement : Element
        {
            if(PopUpFrame != null)
            {
                return PopUpFrame.ElementOfType<TElement>(findBy);
            }

            return IEInstance.ElementOfType<TElement>(findBy);
        }

        #endregion

    }
}