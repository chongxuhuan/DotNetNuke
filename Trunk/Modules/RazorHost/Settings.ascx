<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuke.Modules.RazorHost.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="550" cellspacing="0" cellpadding="4" border="0" width=100%>
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="scriptListLabel" controlname="scriptList" runat="server" /></td>
		<td><asp:DropDownList ID="scriptList" runat="server" /></td>
	</tr> 
</table>