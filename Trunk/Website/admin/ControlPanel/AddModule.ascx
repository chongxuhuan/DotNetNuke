<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanel.AddModule" CodeFile="AddModule.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<asp:UpdatePanel ID="UpdateAddModule" runat="server" ChildrenAsTriggers="true"><ContentTemplate>
<table cellpadding="0" cellspacing="0" border="0">
<tr>
	<td class="AddMod1" valign="top">
		<dnn:DnnFieldLabel id="AddModule" runat="server" Text="AddModule" /><br />
		<dnn:DnnRadioButton ID="AddNewModule" runat="server" Text="AddNew" GroupName="AddModule" Checked="True" AutoPostBack="true" /><br />
		<dnn:DnnRadioButton ID="AddExistingModule" runat="server" Text="AddExisting" GroupName="AddModule" AutoPostBack="true" />
	</td>
	<td class="AddMod2" valign="top">
		<table cellpadding="0" cellspacing="0" border="0">
		<tr id="PageListTR" runat="server" visible="false">
			<td class="LabelCell"><dnn:DnnFieldLabel id="PageListLbl" runat="server" Text="Page" AssociatedControlID="PageLst" /></td>
			<td><dnn:DnnComboBox ID="PageLst" runat="server" Width="205px" MaxHeight="300px" AutoPostBack="true" /></td>
		</tr>
		<tr>
			<td class="LabelCell"><dnn:DnnFieldLabel id="ModuleLstLbl" runat="server" Text="Module" AssociatedControlID="ModuleLst" /></td>
			<td><dnn:DnnComboBox ID="ModuleLst" runat="server" Width="205px" MaxHeight="300px" AutoPostBack="true"/></td>
		</tr>
		<tr id="TitleTR" runat="server">
			<td class="LabelCell"><dnn:DnnFieldLabel id="TitleLbl" runat="server" Text="Title" AssociatedControlID="Title" /></td>
			<td><asp:TextBox ID="Title" runat="server" Width="199px" Height="14px" /></td>
		</tr>
		<tr>
			<td class="LabelCell"><dnn:DnnFieldLabel id="VisibilityLstLbl" runat="server" Text="Visibility" AssociatedControlID="VisibilityLst" /></td>
			<td><dnn:DnnComboBox ID="VisibilityLst" runat="server" Width="205px" /></td>
		</tr>
		<tr>
			<td></td>
			<td>
			    <dnn:DnnButton ID="cmdAddModule" runat="server" Text="AddModule" />
			    <asp:CheckBox ID="chkCopyModule" CssClass="SubHead dnnLabel" runat="server"  />
			</td>
		</tr>
		</table>
	</td>
	<td class="AddMod3" valign="top">
		<table cellpadding="0" cellspacing="0" border="0">
		<tr>
			<td class="LabelCell"><dnn:DnnFieldLabel id="PaneLstLbl" runat="server" Text="Pane" AssociatedControlID="PaneLst" /></td>
			<td><dnn:DnnComboBox ID="PaneLst" runat="server" Width="100px"  AutoPostBack="true" /></td>
		</tr>
		<tr>
			<td class="LabelCell"><dnn:DnnFieldLabel id="PositionLstLbl" runat="server" Text="Insert" AssociatedControlID="PositionLst" /></td>
			<td><dnn:DnnComboBox ID="PositionLst" runat="server" Width="100px" AutoPostBack="true" /></td>
		</tr>
		<tr>
			<td class="LabelCell"><dnn:DnnFieldLabel id="PaneModulesLstLbl" runat="server" Text="Module" AssociatedControlID="PaneModulesLst" /></td>
			<td><dnn:DnnComboBox ID="PaneModulesLst" runat="server" Width="100px" /></td>
		</tr>
		</table>
	</td>
</tr>
</table>
</ContentTemplate></asp:UpdatePanel>
