<%@ Control Inherits="DotNetNuke.Modules.Admin.Security.Roles" Language="C#" AutoEventWireup="false" CodeFile="Roles.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnSecurityRoles">
    <div runat="server" id="divGroups">
        <div class="dnnFormItem">
            <dnn:label id="plRoleGroups" runat="server" suffix="" controlname="cboRoleGroups" />
            <asp:dropdownlist id="cboRoleGroups" Runat="server" AutoPostBack="True" />
			<asp:hyperlink ID="lnkEditGroup" runat="server">
				<asp:image ID="imgEditGroup" ImageUrl="~/images/edit.gif" AlternateText="Edit" runat="server" resourcekey="Edit" />
			</asp:hyperlink>
			<asp:imagebutton ID="cmdDelete" Runat="server" ImageUrl="~/images/delete.gif" />
        </div>
    </div>
	<asp:datagrid id="grdRoles" Width="98%" AutoGenerateColumns="false" EnableViewState="false" runat="server" BorderStyle="None" GridLines="None" CssClass="dnnSecurityRolesGrid">
        <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	    <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	    <alternatingitemstyle cssclass="dnnGridAltItem" />
	    <edititemstyle cssclass="dnnFormInput" />
	    <selecteditemstyle cssclass="dnnFormError" />
	    <footerstyle cssclass="dnnGridFooter" />
	    <pagerstyle cssclass="dnnGridPager" />
		<columns>
			<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif" editmode="URL" keyfield="RoleID" />
			<dnn:imagecommandcolumn commandname="UserRoles" imageurl="~/images/icon_users_16px.gif" editmode="URL" keyfield="RoleID" />
			<asp:boundcolumn DataField="RoleName" HeaderText="Name">
			</asp:boundcolumn>
			<asp:boundcolumn DataField="Description" HeaderText="Description">
			</asp:boundcolumn>
			<asp:templatecolumn HeaderText="Fee">
				<itemtemplate>
					<asp:label runat="server" Text='<%#FormatPrice((float)DataBinder.Eval(Container.DataItem, "ServiceFee")) %>' ID="Label1" />
				</ItemTemplate>
			</asp:templatecolumn>
			<asp:templatecolumn HeaderText="Every">
				<itemtemplate>
					<asp:label runat="server" Text='<%#FormatPeriod((int)DataBinder.Eval(Container.DataItem, "BillingPeriod")) %>' ID="Label2" />
				</ItemTemplate>
			</asp:templatecolumn>
			<asp:boundcolumn DataField="BillingFrequency" HeaderText="Period" />
			<asp:templatecolumn HeaderText="Trial">
				<itemtemplate>
					<asp:label runat="server" Text='<%#FormatPrice((float)DataBinder.Eval(Container.DataItem, "TrialFee")) %>' ID="Label3" />
				</ItemTemplate>
			</asp:templatecolumn>
			<asp:templatecolumn HeaderText="Every">
				<itemtemplate>
					<asp:label runat="server" Text='<%#FormatPeriod((int)DataBinder.Eval(Container.DataItem, "TrialPeriod")) %>' ID="Label4" />
				</ItemTemplate>
			</asp:templatecolumn>
			<asp:boundcolumn DataField="TrialFrequency" HeaderText="Period" />
			<asp:templatecolumn HeaderText="Public">
				<itemtemplate>
					<asp:image Runat="server" ID="imgApproved" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"IsPublic") %>' />
					<asp:image Runat="server" ID="imgNotApproved" ImageUrl="~/images/unchecked.gif" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem,"IsPublic")%>' />
				</ItemTemplate>
			</asp:templatecolumn>
			<asp:templatecolumn HeaderText="Auto">
				<itemtemplate>
					<asp:image Runat="server" ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"AutoAssignment") %>' />
					<asp:image Runat="server" ID="Image2" ImageUrl="~/images/unchecked.gif" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem,"AutoAssignment") %>' />
				</ItemTemplate>
			</asp:templatecolumn>
		</columns>
	</asp:datagrid>
</div>
<script language="javascript" type="text/javascript">
    function setupDnnRoles() {
        $('#<%= cmdDelete.ClientID %>').dnnConfirm({
            text: '<%= LocalizeString("DeleteItem") %>',
            yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
            noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
            title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
        });
    }

    $(document).ready(function () {
        setupDnnRoles();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setupDnnSiteSettingssetupDnnRoles();
        });
    });
</script>