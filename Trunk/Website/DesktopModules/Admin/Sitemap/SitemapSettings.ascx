<%@ Control Language="vb" AutoEventWireup="false" CodeFile="SitemapSettings.ascx.vb"
    Inherits="DotNetNuke.Modules.Admin.Sitemap.SitemapSettings" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<br />
<table id="TableSiteMapUrl" cellspacing="2" cellpadding="2" border="0" runat="server"
    style="width: 100%">
    <tr>
        <td class="SubHead" style="width: 250px">
            <dnn:Label ID="lblSiteMap" runat="server" ControlName="txtSiteMap" />
        </td>
        <td>
            <asp:HyperLink ID="lnkSiteMapUrl" runat="server" Width="400" Target="_blank" />&nbsp;
        </td>
    </tr>
</table>
<br />
<dnn:DnnGrid ID="grdProviders" runat="Server" Width="100%" AutoGenerateColumns="false" AllowSorting="true" />
<br />
<dnn:SectionHead ID="secCore" CssClass="Head" runat="server" Section="tblCore" ResourceKey="SectionCoreSettings" IncludeRule="True" IsExpanded="true" />
<table id="tblCore" runat="server" cellspacing="2" cellpadding="2" style="width: 100%">
    <tr>
        <td colspan="3">
            <dnn:Label ID="lblSectionCoreSettingsHelp" CssClass="Normal" runat="server" ResourceKey="SectionCoreSettingsLbl" EnableViewState="False" />
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td style="width: 250px">
            <dnn:Label ID="lblLevelPriority" runat="server" CssClass="SubHead" ControlName="chkLevelPriority" />
        </td>
        <td>
            <asp:CheckBox ID="chkLevelPriority" runat="server" AutoPostBack="false" />
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td style="width: 250px">
            <dnn:Label ID="lblMinPagePriority" runat="server" CssClass="SubHead" ControlName="txtMinPagePriority" />
        </td>
        <td>
            <dnn:DnnTextBox ID="txtMinPagePriority" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="50" Text="0.1">
            </dnn:DnnTextBox>
            <asp:CompareValidator ID="val1" runat="server" ControlToValidate="txtMinPagePriority"
                Display="Dynamic" CssClass="NormalRed" resourcekey="valPriority" Operator="DataTypeCheck"
                Type="Double"></asp:CompareValidator>
            <asp:RequiredFieldValidator ID="val2" runat="server" ControlToValidate="txtMinPagePriority"
                Display="Dynamic" CssClass="NormalRed" resourcekey="valPriority"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td style="width: 250px">
            <dnn:Label ID="lblIncludeHidden" runat="server" CssClass="SubHead" ControlName="chkIncludeHidden" />
        </td>
        <td>
            <asp:CheckBox ID="chkIncludeHidden" runat="server" AutoPostBack="False" />
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td colspan="2">
            <asp:LinkButton ID="lnkRefresh" runat="server" CssClass="CommandButton" resourcekey="cmdRefresh"></asp:LinkButton>&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br />
<dnn:SectionHead ID="secGeneral" CssClass="Head" runat="server" Section="tblGeneral"
    ResourceKey="SectionGeneralSettings" IncludeRule="True" IsExpanded="true" />
<table id="tblGeneral" runat="server" cellspacing="2" cellpadding="2" style="width: 100%">
    <tr>
        <td colspan="3">
            <dnn:Label ID="lblSectionGeneralSettingsHelp" CssClass="Normal" runat="server" ResourceKey="SectionGeneralSettingsLbl" EnableViewState="False" />
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td class="SubHead" style="width: 250px">
            <dnn:Label ID="lblExcludePriority" runat="server" CssClass="SubHead" ControlName="txtExcludePriority" />
        </td>
        <td>
            <dnn:DnnTextBox ID="txtExcludePriority" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="50" Text="0">
            </dnn:DnnTextBox>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtExcludePriority"
                Display="Dynamic" CssClass="NormalRed" resourcekey="valPriority" Operator="DataTypeCheck"
                Type="Double"></asp:CompareValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtExcludePriority"
                Display="Dynamic" CssClass="NormalRed" resourcekey="valPriority"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td class="SubHead" style="width: 250px">
            <dnn:Label ID="lblCache" runat="server" CssClass="SubHead" ControlName="chkCache" />
        </td>
        <td>
            <dnn:DnnComboBox ID="cmbDaysToCache" runat="server" CssClass="NormalTextBox" Width="200px">
                <Items>
                    <telerik:RadComboBoxItem Text="Disable Caching" />
                    <telerik:RadComboBoxItem Text="1 Day" />
                    <telerik:RadComboBoxItem Text="2 Days" />
                    <telerik:RadComboBoxItem Text="3 Days" />
                    <telerik:RadComboBoxItem Text="4 Days" />
                    <telerik:RadComboBoxItem Text="5 Days" />
                    <telerik:RadComboBoxItem Text="6 Days" />
                    <telerik:RadComboBoxItem Text="7 Days" />
                </Items>
            </dnn:DnnComboBox>
            <asp:LinkButton ID="lnkResetCache" runat="server" CssClass="CommandButton" resourcekey="lnkResetCache"
                Text="ResetCache"/>
        </td>
    </tr>
</table>
<br />
<dnn:SectionHead ID="secSubmission" CssClass="Head" runat="server" Section="tblSubmission"
    ResourceKey="SectionSubmissionSettings" IncludeRule="true" IsExpanded="true" />
<table id="tblSubmission" cellspacing="2" cellpadding="2" border="0" runat="server"
    style="width: 100%">
    <tr>
        <td colspan="3">
            <dnn:Label ID="lblSectionSubmissionSettings" CssClass="Normal" runat="server" ResourceKey="SectionSubmissionSettingsLbl" EnableViewState="False" />
        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td class="SubHead" style="width: 250px">
            <dnn:Label ID="lblSearchEngine" runat="server" CssClass="SubHead" ControlName="cboSearchEngine" />
        </td>
        <td>
            <dnn:DnnComboBox ID="cboSearchEngine" runat="server" CssClass="NormalTextBox" Width="200px" AutoPostBack="true">
                <Items>
                    <telerik:RadComboBoxItem Text="Google" />
                    <telerik:RadComboBoxItem Text="Bing" />
                    <telerik:RadComboBoxItem Text="Yahoo!" />
                </Items>
            </dnn:DnnComboBox>
            
            <asp:HyperLink ID="cmdSubmitSitemap" Text="Submit" runat="server" Width="100" Target="_blank" CssClass="CommandButton" ResourceKey="submit" />&nbsp;

        </td>
    </tr>
    <tr>
        <td width="25"></td>
        <td class="SubHead" valign="top" style="width: 250px">
            <dnn:Label ID="lblVerification" runat="server" ControlName="txtVerification" />
        </td>
        <td>
            <dnn:DnnTextBox ID="txtVerification" runat="server" CssClass="NormalTextBox" Width="400" />&nbsp;
            <asp:LinkButton CssClass="CommandButton" ID="cmdVerification" resourcekey="cmdVerification"
                runat="server" Text="Create" />
        </td>
    </tr>
</table>
<br />
<p>
    <asp:LinkButton ID="lnkSaveAll" runat="server" CssClass="CommandButton" resourcekey="cmdSaveAll"></asp:LinkButton>&nbsp;&nbsp;
</p>
