<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.ProfileDefinitions" CodeFile="ProfileDefinitions.ascx.vb" language="vb" AutoEventWireup="false" Explicit="True" %>
<div style="text-align:left;">
    <asp:Label ID="lblProfileHelp" runat="Server" class="normal" resourcekey="ProfilePropertiesHelp" />
</div>
<br />
<asp:datagrid id="grdProfileProperties" AutoGenerateColumns="false" width="100%" CellPadding="4"
	GridLines="None" cssclass="DataGrid_Container" Runat="server">
	<headerstyle cssclass="NormalBold" verticalalign="Top" horizontalalign="Center" />
	<itemstyle cssclass="DataGrid_Item" horizontalalign="Left" />
	<alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	<edititemstyle cssclass="NormalTextBox" />
	<selecteditemstyle cssclass="NormalRed" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
	<columns>
		<dnn:imagecommandcolumn CommandName="Edit" Text="Edit" ImageUrl="~/images/edit.gif" HeaderText="Edit" KeyField="PropertyDefinitionID" EditMode="URL" />
		<dnn:imagecommandcolumn CommandName="Delete" Text="Delete" ImageUrl="~/images/delete.gif" HeaderText="Del" KeyField="PropertyDefinitionID" />
		<dnn:imagecommandcolumn commandname="MoveDown" imageurl="~/images/dn.gif" headertext="Dn" keyfield="PropertyDefinitionID" />
		<dnn:imagecommandcolumn commandname="MoveUp" imageurl="~/images/up.gif" headertext="Up" keyfield="PropertyDefinitionID" />
		<dnn:textcolumn DataField="PropertyName" HeaderText="Name" Width="100px" />
		<dnn:textcolumn DataField="PropertyCategory" HeaderText="Category" Width="100px" />
		<asp:TemplateColumn HeaderText="DataType">
			<ItemStyle Width="100px"></ItemStyle>
			<ItemTemplate>
				<asp:label id="lblDataType" runat="server" Text='<%# DisplayDataType(CType(Container.DataItem,DotNetNuke.Entities.Profile.ProfilePropertyDefinition) ) %>'></asp:label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<dnn:textcolumn DataField="Length" HeaderText="Length" />
		<dnn:textcolumn DataField="DefaultValue" HeaderText="DefaultValue" Width="100px" />
		<dnn:textcolumn DataField="ValidationExpression" HeaderText="ValidationExpression" Width="100px" />
		<dnn:textcolumn DataField="DefaultVisibility" HeaderText="DefaultVisibility" Width="100px" />
		<dnn:checkboxcolumn DataField="Required" HeaderText="Required" AutoPostBack="True" />
		<dnn:checkboxcolumn DataField="Visible" HeaderText="Visible" AutoPostBack="True" />
	</columns>
</asp:datagrid>
<br>
<br>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" imageUrl="~/images/save.gif" resourcekey="cmdApply"
		runat="server" text="Apply Changes" />&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdRefresh" imageUrl="~/images/refresh.gif" resourcekey="cmdRefresh"
		runat="server" text="Refresh" />&nbsp;
</p>
