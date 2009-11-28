<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Dashboard.DashboardControls" CodeFile="DashboardControls.ascx.vb" language="vb" AutoEventWireup="false" Explicit="True" %>
<div style="text-align:left;">
    <asp:Label ID="lblDashboardHelp" runat="Server" class="normal" resourcekey="DashboardControlsHelp" />
</div>
<br />
<asp:datagrid id="grdDashboardControls" AutoGenerateColumns="false" width="100%" CellPadding="4"
	GridLines="None" cssclass="DataGrid_Container" Runat="server">
	<headerstyle cssclass="NormalBold" verticalalign="Top" horizontalalign="Center" />
	<itemstyle cssclass="DataGrid_Item" horizontalalign="Left" />
	<alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	<edititemstyle cssclass="NormalTextBox" />
	<selecteditemstyle cssclass="NormalRed" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
	<columns>
		<dnn:imagecommandcolumn CommandName="Delete" Text="Delete" ImageUrl="~/images/delete.gif" HeaderText="Del" KeyField="DashboardControlID" />
		<dnn:imagecommandcolumn commandname="MoveDown" imageurl="~/images/dn.gif" headertext="Dn" keyfield="DashboardControlID" />
		<dnn:imagecommandcolumn commandname="MoveUp" imageurl="~/images/up.gif" headertext="Up" keyfield="DashboardControlID" />
		<dnn:textcolumn DataField="DashboardControlKey" HeaderText="DashboardControlKey" Width="100px" />
		<dnn:textcolumn DataField="DashboardControlSrc" HeaderText="DashboardControlSrc" Width="500px" />
		<dnn:checkboxcolumn DataField="IsEnabled" HeaderText="IsEnabled" AutoPostBack="True" />
	</columns>
</asp:datagrid>
<br>
<br>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdInstall" imageUrl="~/images/add.gif" resourcekey="cmdInstall" runat="server" />&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" imageUrl="~/images/save.gif" resourcekey="cmdApply" runat="server" />&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdRefresh" imageUrl="~/images/refresh.gif" resourcekey="cmdRefresh" runat="server" />
	<dnn:commandbutton class="CommandButton" id="cmdCancel" imageUrl="~/images/lt.gif" resourcekey="cmdCancel" runat="server" />&nbsp;
</p>
