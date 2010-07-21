<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Languages.EditLanguage" CodeFile="EditLanguage.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td class="SubHead" style="width:200px"><dnn:Label ID="languageLabel" runat="server" ControlName="languageComboBox" /></td>
        <td class="NormalTextBox" style="width:325px">
            <dnn:DnnLanguageLabel ID="languageLanguageLabel" runat="server"  />
            <dnn:DnnLanguageComboBox ID="languageComboBox" runat="server" LanguagesListType="All" CssClass="NormalTextBox le_languages" Width="250px" ShowModeButtons="false" />
        </td>
    </tr>
    <tr style="height:10px">
        <td colspan="2"></td>
    </tr>
    <tr>
        <td class="SubHead" style="width:200px"><dnn:Label ID="fallBackLabel" runat="server" ControlName="fallBackComboBox" /></td>
        <td class="NormalTextBox" style="width:325px">
            <dnn:DnnLanguageLabel ID="fallbackLanguageLabel" runat="server"  />
            <dnn:DnnLanguageComboBox ID="fallBackComboBox" runat="server" LanguagesListType="Supported" CssClass="NormalTextBox le_languages" Width="250px" />
        </td>
    </tr>
    <tr id="translatorsRow" runat="server">
        <td class="SubHead" style="vertical-align:top;width:200px"><dnn:Label ID="translatorsLabel" runat="server" ControlName="translatorRoles" /></td>
        <td class="NormalTextBox" style="width:325px"><dnn:RolesSelectionGrid  runat="server" ID="translatorRoles" /></td>
    </tr>
    <tr style="height:10px">
        <td colspan="2"></td>
    </tr>
    <tr id="localizationEnabled" runat="server">
        <td class="SubHead" colspan="2" style="vertical-align:top;width:200px"><asp:Label ID="localizationNotEnabledLabel" runat="server" CssClass="NormalRed" resourcekey="localizationNotEnabled" /></td>
    </tr>
</table>
<p style="text-align:center">
    <dnn:commandbutton id="cmdUpdate" runat="server" class="CommandButton" ImageUrl="~/images/save.gif"  ResourceKey="cmdUpdate" />
    <dnn:commandbutton id="cmdDelete" runat="server" class="CommandButton" ImageUrl="~/images/delete.gif"  ResourceKey="cmdDelete" />
    <dnn:commandbutton id="cmdCancel" runat="server" class="CommandButton" ImageUrl="~/images/lt.gif"  ResourceKey="cmdCancel" />
</p>
