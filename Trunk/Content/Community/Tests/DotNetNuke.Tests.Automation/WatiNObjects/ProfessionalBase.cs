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
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Professional Base object
    /// Contains some commonly used objects that a Professional installation will have.
    /// Including the Pro feature spans found in the host/admin menu, and the Pro feature Divs on the host/admin Console page.
    /// </summary>
    public class ProfessionalBase : WatiNBase
    {
        #region Constructors

        public ProfessionalBase(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ProfessionalBase(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Spans
        public Span ProfessionalFeaturesMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Professional Features")); }
        }
        public Span ManageWebServersMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Manage Web Servers")); }
        }
        public Span HealthMonitoringMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Health Monitoring")); }
        }
        public Span ApplicationIntegrityMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Application Integrity")); }
        }
        public Span SpecialOffersMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Special Offers")); }
        }
        public Span KnowledgeBaseMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Knowledge Base")); }
        }
        public Span SoftwareDocumentationMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Software and Documentation")); }
        }
        public Span TechnicalSupportMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Technical Support")); }
        }
        public Span MySupportTicketsMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("My Support Tickets")); }
        }
        public Span ActivateLicenseMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Activate your License")); }
        }
        public Span LicenseManagementMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("License Management")); }
        }
        public Span SecurityCenterMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Security Center")); }
        }
        public Span UserSwitcherMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("User Switcher")); }
        }
        public Span GoogleAnalyticsProMenuSpan
        {
            get { return IEInstance.Span(Find.ByText("Google Analytics Pro")); }
        }
        #endregion

        #region Divs
        public Div ManageWebServerConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Manage Web Servers"))); }
        }
        public Div HealthMonitoringConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Health Monitoring"))); }
        }
        public Div ApplicationIntegrityConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Application Integrity"))); }
        }
        public Div SpecialOffersConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Special Offers"))); }
        }
        public Div KnowledgeBaseConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Knowledge Base"))); }
        }
        public Div SoftwareAndDocumentationConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Software and Documentation"))); }
        }
        public Div TechincalSupportConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Technical Support"))); }
        }
        public Div MySupportTicketsConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("My Support Tickets"))); }
        }
        public Div ActivateYourLicenseConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(new Regex("Activate Your License",RegexOptions.IgnoreCase))); }
        }
        public Div LicenseManagementConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("License Management"))); }
        }
        public Div SearchCrawlerAdminConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Search Crawler Admin"))); }
        }
        public Div SecurityCenterConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Security Center"))); }
        }
        public Div UserSwitcherConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("User Switcher"))); }
        }
        public Div GoogleAnalyticsProConsoleDiv
        {
            get { return IEInstance.Div(Find.ByTitle(s => s.StartsWith("Google Analytics"))); }
        }
        #endregion
        
        #endregion

        #region Public Methods

        #endregion
    }
}