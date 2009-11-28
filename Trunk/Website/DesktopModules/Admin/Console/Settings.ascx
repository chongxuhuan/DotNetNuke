<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Settings.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Console.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table cellpadding="2" cellspacing="0" border="0">
<tr>
	<td>
	    <dnn:label id="lblParentTab" runat="server" ControlName="ParentTab" ResourceKey="ParentTab" Suffix=":" />
	</td>
	<td>
	    <asp:DropDownList ID="ParentTab" runat="server" />
	</td>
</tr>
<tr><td>&nbsp;</td></tr>
<tr>
	<td>
	    <dnn:label id="lblDefaultSize" runat="server" ControlName="DefaultSize" ResourceKey="DefaultSize" Suffix=":" />
	</td>
	<td>
	   <asp:DropDownList ID="DefaultSize" runat="server" />
	</td>
</tr>
<tr>
	<td>
		<dnn:label id="lblAllowResize" runat="server" ControlName="AllowResize" ResourceKey="AllowResize" Suffix=":" />
	</td>
	<td>
	    <asp:Checkbox ID="AllowResize" runat="server" Checked="true" />
	</td>
</tr>
<tr><td>&nbsp;</td></tr>
<tr>
	<td>
	    <dnn:label id="lblDefaultView" runat="server" ControlName="DefaultView" ResourceKey="DefaultView" Suffix=":" />
	</td>
	<td>
		<asp:DropDownList ID="DefaultView" runat="server" />
	</td>
</tr>
<tr>
	<td>
		<dnn:label id="lblAllowViewChange" runat="server" ControlName="AllowViewChange" ResourceKey="AllowViewChange" Suffix=":" />
	</td>
	<td>
		<asp:Checkbox ID="AllowViewChange" runat="server" Checked="true" />
	</td>
</tr>
<tr>
	<td>
		<dnn:label id="lblShowTooltip" runat="server" ControlName="ShowTooltip" ResourceKey="ShowTooltip" Suffix=":" />
	</td>
	<td>
		<asp:Checkbox ID="ShowTooltip" runat="server" Checked="true" />
	</td>
</tr>
<tr>
	<td>
		<dnn:label id="lblConsoleWidth" runat="server" ControlName="ConsoleWidth" ResourceKey="ConsoleWidth" Suffix=":" />
	</td>
	<td>
		<asp:TextBox ID="ConsoleWidth" runat="server" Text="" />
	</td>
</tr>
</table>
