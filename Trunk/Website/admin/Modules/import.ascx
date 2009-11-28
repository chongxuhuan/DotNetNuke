<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Modules.Import" CodeFile="Import.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<br>
<table width="560" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" valign="top" width="150"><dnn:label id="plFolder" runat="server" controlname="cboFolders" suffix=":"></dnn:label></td>
        <td><asp:DropDownList ID="cboFolders" Runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="true" /></td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150"><dnn:label id="plFile" runat="server" controlname="cboFiles" suffix=":"></dnn:label></td>
        <td><asp:DropDownList ID="cboFiles" Runat="server" CssClass="NormalTextBox" Width="300"></asp:DropDownList></td>
    </tr>
</table>
<p>
    <dnn:commandbutton id="cmdImport" resourcekey="cmdImport" runat="server" cssclass="CommandButton" ImageUrl="~/images/rt.gif"/>&nbsp;
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
