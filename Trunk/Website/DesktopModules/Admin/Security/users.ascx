<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.UserAccounts" CodeFile="Users.ascx.vb" language="vb" AutoEventWireup="false" Explicit="True" %>
<table width="475" border="0">
	<tr>
		<td align="left" width="75"><asp:label id="lblSearch" cssclass="SubHead" resourcekey="Search" runat="server">Search:</asp:label></td>
		<td class="Normal" align="left" width="*">
			<asp:textbox id="txtSearch" Runat="server"></asp:textbox>
			<asp:dropdownlist id="ddlSearchType" Runat="server" />
			<asp:imagebutton id="btnSearch" Runat="server" ImageUrl="~/images/icon_search_16px.gif"></asp:imagebutton>
		</td>
	</tr>
	<tr style="height:15px"><td colspan="3"></td></tr>
</table>
<asp:panel id="plLetterSearch" Runat="server" HorizontalAlign="Center">
    <asp:Repeater id="rptLetterSearch" Runat="server">
		<itemtemplate>
			<asp:HyperLink runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL(Container.DataItem,"1") %>' Text='<%# Container.DataItem %>'>
			</asp:HyperLink>&nbsp;&nbsp;
		</ItemTemplate>
	</asp:Repeater>
</asp:panel>
<br />
<asp:datagrid id="grdUsers" AutoGenerateColumns="false" width="100%" 
	CellPadding="2" GridLines="None" cssclass="DataGrid_Container" Runat="server">
	<headerstyle cssclass="NormalBold" verticalalign="Top"/>
	<itemstyle cssclass="Normal" horizontalalign="Left" />
	<alternatingitemstyle cssclass="Normal" />
	<edititemstyle cssclass="NormalTextBox" />
	<selecteditemstyle cssclass="NormalRed" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
	<columns>
		<dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="UserID" />
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/action_delete.gif" keyfield="UserID" />
		<dnn:imagecommandcolumn CommandName="UserRoles" ImageUrl="~/images/icon_securityroles_16px.gif" EditMode="URL" KeyField="UserID" />
        <dnn:imagecommandcolumn commandname="Restore" imageurl="~/images/restore.gif" keyfield="UserID" />
        <dnn:imagecommandcolumn commandname="Remove" imageurl="~/images/delete.gif" keyfield="UserID"  />
		<asp:templatecolumn>
			<itemtemplate>
				<asp:image id="imgOnline" runat="Server" imageurl="~/images/userOnline.gif" />		
			</itemtemplate>
		</asp:templatecolumn>
		<dnn:textcolumn datafield="UserName" headertext="Username"/>
		<dnn:textcolumn datafield="FirstName" headertext="FirstName"/>
		<dnn:textcolumn datafield="LastName" headertext="LastName"/>
		<dnn:textcolumn datafield="DisplayName" headertext="DisplayName"/>
		<asp:TemplateColumn HeaderText="Address">
			<itemtemplate>
				<asp:Label ID="lblAddress" Runat="server" Text='<%# DisplayAddress(CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.Unit,CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.Street, CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.City, CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.Region, CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.Country, CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.PostalCode) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Telephone">
			<itemtemplate>
				<asp:Label ID="Label4" Runat="server" Text='<%# DisplayEmail(CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Profile.Telephone) %>'>
				</asp:Label>
			</itemtemplate>            
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Email">
			<itemtemplate>
				<asp:Label ID="lblEmail" Runat="server" Text='<%# DisplayEmail(CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Email) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="CreatedDate">
			<itemtemplate>
				<asp:Label ID="lblLastLogin" Runat="server" Text='<%# DisplayDate(CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Membership.CreatedDate) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="LastLogin">
			<itemtemplate>
				<asp:Label ID="Label7" Runat="server" Text='<%# DisplayDate(CType(Container.DataItem, DotNetNuke.Entities.Users.UserInfo).Membership.LastLoginDate) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Authorized">
			<itemtemplate>
				<asp:Image Runat="server" ID="imgApproved" ImageUrl="~/images/checked.gif" Visible="False"/>
				<asp:Image Runat="server" ID="imgNotApproved" ImageUrl="~/images/unchecked.gif" Visible="False"/>
                <asp:Image Runat="server" ID="imgApprovedDeleted" ImageUrl="~/images/checked-disabled.gif" Visible="False"/>
                <asp:Image Runat="server" ID="imgNotApprovedDeleted" ImageUrl="~/images/unchecked-disabled.gif" Visible="False"/>
			</ItemTemplate>
		</asp:TemplateColumn>
	</columns>
</asp:datagrid>
<br/><br/>
<dnn:pagingcontrol id="ctlPagingControl" runat="server"></dnn:pagingcontrol>
