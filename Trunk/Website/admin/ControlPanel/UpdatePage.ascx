<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanel.UpdatePage" CodeFile="UpdatePage.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<table cellpadding="0" cellspacing="0" border="0">
<tr><td valign="top" class="EditPage1">
	<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td class="LabelCell"><dnn:DnnFieldLabel id="NameLbl" runat="server" Text="Name" AssociatedControlID="Name" /></td>
		<td><asp:TextBox ID="Name" runat="server" Width="262px" Height="14px" /></td>
	</tr>
	<tr>
		<td class="LabelCell"><dnn:DnnFieldLabel id="LocationLbl" runat="server" Text="Location" AssociatedControlID="LocationLst" /></td>
		<td><dnn:DnnComboBox ID="LocationLst" runat="server" Width="69px" />&nbsp;<dnn:DnnComboBox ID="PageLst" runat="server" Width="195px" MaxHeight="300px" /></td>
	</tr>
	<tr>
		<td class="LabelCell"><dnn:DnnFieldLabel id="SkinLbl" runat="server" Text="Skin" AssociatedControlID="SkinLst" /></td>
		<td><dnn:DnnComboBox ID="SkinLst" runat="server" Width="268px" MaxHeight="300px"  /></td>
	</tr>
	<tr>
		<td></td>
		<td><dnn:DnnButton ID="cmdUpdate" runat="server" Text="UpdateButton" /></td>
	</tr>
	</table>
</td><td valign="top" class="EditPage2">
	<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td class="LabelCell"><dnn:DnnFieldLabel id="IncludeInMenuLbl" runat="server" Text="IncludeInMenu" AssociatedControlID="IncludeInMenu" /></td>
		<td><asp:CheckBox ID="IncludeInMenu" runat="server" Checked="true" /></td>
	</tr>
	<tr>
		<td class="LabelCell"><dnn:DnnFieldLabel id="DisabledLbl" runat="server" Text="Disabled" AssociatedControlID="IsDisabled" /></td>
		<td><asp:CheckBox ID="IsDisabled" runat="server" Checked="false" /></td>
	</tr>
	<tr id="TRSSL" runat="server">
		<td class="LabelCell"><dnn:DnnFieldLabel id="IsSecureLbl" runat="server" Text="Secured" AssociatedControlID="IsSecure" /></td>
		<td><asp:CheckBox ID="IsSecure" runat="server" Checked="false" /></td>
	</tr>
	</table>
</td></tr>
</table>
