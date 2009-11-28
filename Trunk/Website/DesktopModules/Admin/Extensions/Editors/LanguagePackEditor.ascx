<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Features.LanguagePackEditor" CodeFile="LanguagePackEditor.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td class="SubHead" style="width:200px"><dnn:Label ID="plPackageLanguage" runat="server" ControlName="cboLanguage" /></td>
        <td class="NormalTextBox" style="width:325px"><asp:DropDownList ID="cboLanguage" runat="server" DataTextField="Text" DataValueField="LanguageID"/></td>
    </tr>
    <tr id="trPackage" runat="server">
        <td class="SubHead" style="width:200px"><dnn:Label ID="plPackage" runat="server" ControlName="cboPackage" /></td>
        <td class="NormalTextBox" style="width:325px"><asp:DropDownList ID="cboPackage" runat="server" DataTextField="FriendlyName" DataValueField="PackageID"/></td>
    </tr>
</table>
<p style="text-align:center">
    <dnn:commandbutton id="cmdEdit" runat="server" class="CommandButton" ImageUrl="~/images/edit.gif"  ResourceKey="cmdEdit" />
</p>
