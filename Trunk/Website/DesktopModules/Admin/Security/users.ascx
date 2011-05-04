<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.UserAccounts" Language="C#" AutoEventWireup="false" CodeFile="Users.ascx.cs" %>
<div class="dnnForm dnnUsers dnnClear" id="dnnUsers">
    <div class="uContent dnnClear">
        <div class="dnnFormItem">
            <asp:textbox id="txtSearch" Runat="server" />
            <asp:dropdownlist id="ddlSearchType" Runat="server" />
            <asp:LinkButton ID="cmdSearch" runat="server" resourcekey="Search" />
        </div>
        <div class="uLetterSearch">
            <asp:Repeater id="rptLetterSearch" Runat="server">
		        <ItemTemplate>
			        <asp:HyperLink ID="hlLetter" runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL((string)Container.DataItem,"1") %>' Text='<%# Container.DataItem %>'>
			        </asp:HyperLink>&nbsp;&nbsp;
		        </ItemTemplate>
	        </asp:Repeater>
        </div>
        <div>
            <asp:datagrid id="grdUsers" AutoGenerateColumns="false" width="100%" CellPadding="2" GridLines="None" cssclass="dnnSecurityRolesGrid" Runat="server">
                <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	            <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	            <alternatingitemstyle cssclass="dnnGridAltItem" />
	            <edititemstyle cssclass="dnnFormInput" />
	            <selecteditemstyle cssclass="dnnFormError" />
	            <footerstyle cssclass="dnnGridFooter" />
	            <pagerstyle cssclass="dnnGridPager" />
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
			            <itemTemplate>
				            <asp:Label ID="lblAddress" Runat="server" Text='<%# DisplayAddress(((UserInfo)Container.DataItem).Profile.Unit,((UserInfo)Container.DataItem).Profile.Street, ((UserInfo)Container.DataItem).Profile.City, ((UserInfo)Container.DataItem).Profile.Region, ((UserInfo)Container.DataItem).Profile.Country, ((UserInfo)Container.DataItem).Profile.PostalCode) %>'>
				            </asp:Label>
			            </itemTemplate>
		            </asp:TemplateColumn>
		            <asp:TemplateColumn HeaderText="Telephone">
			            <itemtemplate>
				            <asp:Label ID="Label4" Runat="server" Text='<%# DisplayEmail(((UserInfo)Container.DataItem).Profile.Telephone) %>'>
				            </asp:Label>
			            </ItemTemplate>
		            </asp:TemplateColumn>
		            <asp:TemplateColumn HeaderText="Email">
			            <itemtemplate>
				            <asp:Label ID="lblEmail" Runat="server" Text='<%# DisplayEmail(((UserInfo)Container.DataItem).Email) %>'>
				            </asp:Label>
			            </ItemTemplate>
		            </asp:TemplateColumn>
		            <asp:TemplateColumn HeaderText="CreatedDate">
			            <itemtemplate>
				            <asp:Label ID="lblLastLogin" Runat="server" Text='<%# DisplayDate(((UserInfo)Container.DataItem).Membership.CreatedDate) %>'>
				            </asp:Label>
			            </ItemTemplate>
		            </asp:TemplateColumn>
		            <asp:TemplateColumn HeaderText="LastLogin">
			            <itemtemplate>
				            <asp:Label ID="Label7" Runat="server" Text='<%# DisplayDate(((UserInfo)Container.DataItem).Membership.LastLoginDate) %>'>
				            </asp:Label>
			            </ItemTemplate>
		            </asp:TemplateColumn>
		            <asp:TemplateColumn HeaderText="Authorized">
			            <itemtemplate>
				            <asp:Image Runat="server" ID="imgApproved" ImageUrl="~/images/checked.gif" Visible="False" />
				            <asp:Image Runat="server" ID="imgNotApproved" ImageUrl="~/images/unchecked.gif" Visible="False" />
                            <asp:Image Runat="server" ID="imgApprovedDeleted" ImageUrl="~/images/checked-disabled.gif" Visible="False" />
                            <asp:Image Runat="server" ID="imgNotApprovedDeleted" ImageUrl="~/images/unchecked-disabled.gif" Visible="False" />
			            </ItemTemplate>
		            </asp:TemplateColumn>
	            </columns>
            </asp:datagrid>
            <dnn:pagingcontrol id="ctlPagingControl" runat="server"></dnn:pagingcontrol>
        </div>
    </div>
</div>