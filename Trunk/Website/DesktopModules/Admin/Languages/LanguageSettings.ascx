<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LanguageSettings.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table cellspacing="1" cellpadding="1" border="0">
    <tr>
        <td width="300" class="SubHead"><dnn:Label ID="plBrowser" runat="server" ControlName="chkBrowser"/></td>
        <td width="350"><asp:CheckBox ID="chkBrowser" runat="server" /></td>
    </tr>
    <tr>
        <td width="300" class="SubHead"><dnn:Label ID="plUrl" runat="server" ControlName="chkUrl"/></td>
        <td width="350"><asp:CheckBox ID="chkUrl" runat="server" /></td>
    </tr>
    <tr>
        <td width="300" class="SubHead"><dnn:Label ID="plUsePaging" runat="server" ControlName="chkUsePaging"/></td>
        <td width="350"><asp:CheckBox ID="chkUsePaging" runat="server" /></td>
    </tr>
    <tr>
        <td width="300" class="SubHead"><dnn:Label ID="plPageSize" runat="server" ControlName="txtPageSize"/></td>
        <td width="350">
            <asp:TextBox ID="txtPageSize" runat="server" />
            <asp:RequiredFieldValidator ID="valPageSize2" runat="server" ControlToValidate="txtPageSize" resourcekey="PageSize.Error" Display="Dynamic" CssClass="NormalRed" />
            <asp:RangeValidator ID="valPageSize" runat="server" ControlToValidate="txtPageSize" resourcekey="PageSize.Error" Type="Integer" Display="Dynamic" MinimumValue="0" CssClass="NormalRed" />
        </td>
    </tr>
</table>
<p align="center">
    <dnn:CommandButton ID="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate" ImageUrl="~/images/save.gif" />
    <dnn:CommandButton ID="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" CausesValidation="false" />
</p>
