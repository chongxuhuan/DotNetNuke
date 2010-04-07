<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanel.AddPage" CodeFile="AddPage.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<table cellpadding="0" cellspacing="0" border="0">
<tr>
	<td class="LabelCell"><dnn:DnnFieldLabel id="NameLbl" runat="server" Text="Name" AssociatedControlID="Name" /></td>
	<td><asp:TextBox ID="Name" runat="server" Width="263px" Height="14px" /></td>
</tr>
<tr>
	<td class="LabelCell"><dnn:DnnFieldLabel id="TemplateLbl" runat="server" Text="Template" AssociatedControlID="TemplateLst" /></td>
	<td><dnn:DnnComboBox ID="TemplateLst" runat="server" Width="137px" />
	<span style="padding-left:10px;"><dnn:DnnFieldLabel id="IncludeInMenuLbl" runat="server" Text="IncludeInMenu" AssociatedControlID="IncludeInMenu" /></span>
	<asp:CheckBox ID="IncludeInMenu" runat="server" Checked="true" />
	</td>
</tr>
<tr>
	<td class="LabelCell"><dnn:DnnFieldLabel id="LocationLbl" runat="server" Text="Location" AssociatedControlID="LocationLst" /></td>
	<td style="white-space:nowrap;"><dnn:DnnComboBox ID="LocationLst" runat="server" Width="67px" />&nbsp;<dnn:DnnComboBox ID="PageLst" runat="server" Width="198px" MaxHeight="300px"  /></td>
</tr>
<tr>
	<td></td>
	<td><dnn:DnnButton ID="cmdAddPage" runat="server" Text="AddButton" /></td>
</tr>
</table>
