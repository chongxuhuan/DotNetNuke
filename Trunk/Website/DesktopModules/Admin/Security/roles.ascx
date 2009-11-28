<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Security.Roles" CodeFile="Roles.ascx.vb" language="vb" AutoEventWireup="false" Explicit="True" %>
<table align="left" cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr id="trGroups" runat="server">
		<td width="*">&nbsp;</td>
		<td width="150" class="SubHead"><dnn:label id="plRoleGroups" runat="server" suffix="" controlname="cboRoleGroups" /></td>
		<td width="200" class="Normal">
			<asp:dropdownlist id="cboRoleGroups" Runat="server" AutoPostBack="True" />
			<asp:hyperlink ID="lnkEditGroup" runat="server">
				<asp:image ID="imgEditGroup" ImageUrl="~/images/edit.gif" AlternateText="Edit" runat="server" resourcekey="Edit"/>
			</asp:hyperlink>
			<asp:imagebutton ID="cmdDelete" Runat="server" ImageUrl="~/images/delete.gif" />
		</td>
		<td width="*">&nbsp;</td>
	</tr>
	<tr height="20">
	    <td colspan="4"></td>
	</tr>
	<tr>
		<td colspan="4">
			<asp:datagrid id="grdRoles" Border="0" CellPadding="0" CellSpacing="0" Width="98%" 
				AutoGenerateColumns="false" EnableViewState="false" runat="server" 
				summary="Roles Design Table" BorderStyle="None" BorderWidth="0px"
				GridLines="None">
				<headerstyle cssclass="NormalBold" verticalalign="Top"/>
				<itemstyle cssclass="Normal" horizontalalign="Left" />
				<alternatingitemstyle cssclass="Normal" />
				<edititemstyle cssclass="NormalTextBox" />
				<selecteditemstyle cssclass="NormalRed" />
				<footerstyle cssclass="DataGrid_Footer" />
				<pagerstyle cssclass="DataGrid_Pager" />
				<columns>
					<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif" editmode="URL" keyfield="RoleID" />
					<dnn:imagecommandcolumn commandname="UserRoles" imageurl="~/images/icon_users_16px.gif" editmode="URL" keyfield="RoleID" />
					<asp:boundcolumn DataField="RoleName" HeaderText="Name">
					</asp:boundcolumn>
					<asp:boundcolumn DataField="Description" HeaderText="Description">
					</asp:boundcolumn>
					<asp:templatecolumn HeaderText="Fee">
						<itemtemplate>
							<asp:label runat="server" Text='<%#FormatPrice(DataBinder.Eval(Container.DataItem, "ServiceFee")) %>' CssClass="Normal" ID="Label1"/>
						</ItemTemplate>
					</asp:templatecolumn>
					<asp:templatecolumn HeaderText="Every">
						<itemtemplate>
							<asp:label runat="server" Text='<%#FormatPeriod(DataBinder.Eval(Container.DataItem, "BillingPeriod")) %>' CssClass="Normal" ID="Label2"/>
						</ItemTemplate>
					</asp:templatecolumn>
					<asp:boundcolumn DataField="BillingFrequency" HeaderText="Period">
						<itemstyle cssclass="Normal"></ItemStyle>
					</asp:boundcolumn>
					<asp:templatecolumn HeaderText="Trial">
						<itemtemplate>
							<asp:label runat="server" Text='<%#FormatPrice(DataBinder.Eval(Container.DataItem, "TrialFee")) %>' CssClass="Normal" ID="Label3"/>
						</ItemTemplate>
					</asp:templatecolumn>
					<asp:templatecolumn HeaderText="Every">
						<itemtemplate>
							<asp:label runat="server" Text='<%#FormatPeriod(DataBinder.Eval(Container.DataItem, "TrialPeriod")) %>' CssClass="Normal" ID="Label4"/>
						</ItemTemplate>
					</asp:templatecolumn>
					<asp:boundcolumn DataField="TrialFrequency" HeaderText="Period">
						<itemstyle cssclass="Normal"></ItemStyle>
					</asp:boundcolumn>
					<asp:templatecolumn HeaderText="Public">
						<itemtemplate>
							<asp:image Runat="server" ID="imgApproved" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"IsPublic")="true" %>'/>
							<asp:image Runat="server" ID="imgNotApproved" ImageUrl="~/images/unchecked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"IsPublic")="false" %>'/>
						</ItemTemplate>
					</asp:templatecolumn>
					<asp:templatecolumn HeaderText="Auto">
						<itemtemplate>
							<asp:image Runat="server" ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"AutoAssignment")="true" %>'/>
							<asp:image Runat="server" ID="Image2" ImageUrl="~/images/unchecked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"AutoAssignment")="false" %>'/>
						</ItemTemplate>
					</asp:templatecolumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
</table>
