<%@ Control Language="c#" AutoEventWireup="false" Codebehind="XSSearchInputSkinObject.ascx.cs" Inherits="DotNetNuke.Professional.SearchInput.XSSearchInputSkinObject" %>
<style>
.xsImgSearch { MARGIN-RIGHT: 2px }
.xsCmdSearch { BORDER-RIGHT: #c7cac9 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: #c7cac9 1px solid; PADDING-LEFT: 3px; FONT-SIZE: 8pt; BORDER-LEFT: #c7cac9 1px solid; MARGIN-RIGHT: 2px; BORDER-BOTTOM: #c7cac9 1px solid; BORDER-COLLAPSE: collapse; BACKGROUND-COLOR: #ffffff }
.xsTxtSearch { BORDER-RIGHT: #c7cac9 1px solid; BORDER-TOP: #c7cac9 1px solid; FONT-SIZE: 9pt; BORDER-LEFT: #c7cac9 1px solid; WIDTH: 90px; MARGIN-RIGHT: 2px; BORDER-BOTTOM: #c7cac9 1px solid; BORDER-COLLAPSE: collapse; BACKGROUND-COLOR: #ffffff }
</style>
<table cellSpacing="0" cellPadding="0" border="0">
	<tr>
		<td nowrap="nowrap">
			<asp:textbox id="txtSearch" runat="server" Wrap="False" Width="100px" maxlength="200" cssclass="NormalTextBox"></asp:textbox>
			<asp:imagebutton id="imgGo" runat="server" ImageAlign="AbsMiddle" CausesValidation="false"></asp:imagebutton><asp:Button id="cmdGo" runat="server" Text="Go" cssclass="CommandButton" CausesValidation="false"></asp:Button>
		</td>
	</tr>
</table>
