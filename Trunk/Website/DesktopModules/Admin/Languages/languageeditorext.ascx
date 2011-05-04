<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEditorExt" CodeFile="LanguageEditorExt.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="2" cellpadding="2" border="0">
	<tr>
		<td class="SubHead"><dnn:Label id="plFile" runat="server" ControlName="lblFile" /></td>
		<td class="Normal"><asp:Label id="lblFile" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><dnn:Label id="plName" runat="server" ControlName="lblName" /></td>
		<td class="Normal"><asp:Label id="lblName" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead"><dnn:Label id="plDefault" runat="server" ControlName="lblDefault" /></td>
		<td class="Normal"><asp:Label id="lblDefault" runat="server" /></td>
	</tr>
	<tr height="10"><td></td></tr>
	<tr valign="top">
		<td colspan="2" class="SubHead"><dnn:texteditor id="teContent" runat="server" height="400" width="600" /></td>
	</tr>
</table>
<p>
    <dnn:CommandButton ID="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:CommandButton ID="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" CausesValidation="false" />
</p>
