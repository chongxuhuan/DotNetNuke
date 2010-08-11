<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EnableLocalizedContent.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Languages.EnableLocalizedContent" Explicit="True"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<style type="text/css">
    .RadUploadProgressArea .ruProgress { padding-top: 15px; }
    .RadUploadProgressArea_Default .ruProgress { background-image: none; }
    .RadUploadProgressArea_Simple .ruProgress { background-image: none; }
</style>
<table>
    <tr>
        <td colspan="2"><h2><asp:Label ID="headerLabel"  runat="server" resourceKey="header" /></h2></td>
    </tr>
    <tr>
        <td colspan="2" class="Normal"><asp:Label ID="enableLocalizationLabel" runat="server" resourceKey="enableLocalization" /></td>
    </tr>
    <tr>
        <td><asp:Label ID="siteDefaultLabel" runat="server" CssClass="SubHead" resourceKey="siteDefaultLabel" /></td>
        <td><dnn:DnnLanguageLabel ID="defaultLanguageLabel" runat="server"  /></td>
    </tr>
    <tr>
        <td colspan="2"><asp:Label ID="siteDefaultDescriptionLabel" runat="server" resourcekey="siteDefaultDescription" /></td>
    </tr>
    <tr style="height:10px"><td colspan="2"></td></tr>
</table>
<div style="text-align:center">
    <dnn:CommandButton ID="updateButton" runat="server" ResourceKey="Update" ImageUrl="~/images/save.gif" />
    <dnn:CommandButton ID="cancelButton" runat="server" ResourceKey="Cancel" ImageUrl="~/images/lt.gif" />
</div>
<br />
<br />
<div style="text-align:center">
    <dnn:DnnProgressManager id="progressManager" runat="server" />
    <dnn:DnnProgressArea id="pageCreationProgressArea" runat="server"  Width="100%" TimeElapsed="true"  />
</div>
        