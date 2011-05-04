<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Security.SecurityRoles" CodeFile="SecurityRoles.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Security Roles Design Table" border="0">
    <tr>
        <td width="900" valign="top">
            <asp:Panel ID="pnlRoles" runat="server" Visible="True">
                <table cellspacing="4" cellpadding="0" border="0" class="dnnForm dnnSecurityRoles dnnClear">
                    <tr>
                        <td colspan="7">
                            <asp:Label ID="lblTitle" runat="server" CssClass="Head" />
                        </td>
                    </tr>
                    <tr>
                        <td height="10">
                        </td>
                    </tr>
                    <tr class="dnnFormItem">
                        <td valign="top" width="300">
                            <dnn:Label ID="plUsers" runat="server" ControlName="cboUsers" />
                            <dnn:Label ID="plRoles" runat="server" ControlName="cboRoles" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150">
                            <dnn:Label ID="plEffectiveDate" runat="server" ControlName="txtEffectiveDate" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150">
                            <dnn:Label ID="plExpiryDate" runat="server" ControlName="txtExpiryDate" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="320">
                            <asp:TextBox ID="txtUsers" runat="server" Width="150" />
                            <asp:LinkButton ID="cmdValidate" runat="server" CssClass="CommandButton" resourceKey="cmdValidate" />
                            <asp:DropDownList ID="cboUsers" runat="server" AutoPostBack="True" Width="100%" />
                            <asp:DropDownList ID="cboRoles" runat="server" AutoPostBack="True" DataValueField="RoleID" DataTextField="RoleName" Width="100%" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150" nowrap="nowrap" align="right">
                            <asp:TextBox ID="txtEffectiveDate" runat="server" Width="80" />
                            <asp:HyperLink ID="cmdEffectiveCalendar" CssClass="CommandButton" runat="server" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150" nowrap="nowrap" align="right">
                            <asp:TextBox ID="txtExpiryDate" runat="server" Width="80" />
                            <asp:HyperLink ID="cmdExpiryCalendar" CssClass="CommandButton" runat="server" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="150" nowrap="nowrap">
                            <asp:LinkButton ID="cmdAdd" CssClass="dnnSecondaryAction" runat="server"  CausesValidation="true" />
                        </td>
                    </tr>
                </table>
                <asp:CompareValidator ID="valEffectiveDate" CssClass="NormalRed" runat="server" resourcekey="valEffectiveDate"
                    Display="Dynamic" Type="Date" Operator="DataTypeCheck" ErrorMessage="<br>Invalid effective date"
                    ControlToValidate="txtEffectiveDate" />
                <asp:CompareValidator ID="valExpiryDate" CssClass="NormalRed" runat="server" resourcekey="valExpiryDate"
                    Display="Dynamic" Type="Date" Operator="DataTypeCheck" ErrorMessage="<br>Invalid expiry date"
                    ControlToValidate="txtExpiryDate" />
                <asp:CompareValidator ID="valDates" CssClass="NormalRed" runat="server" resourcekey="valDates"
                    Display="Dynamic" Type="Date" Operator="GreaterThan" ErrorMessage="<br>Expiry Date must be Greater than Effective Date"
                    ControlToValidate="txtExpiryDate" ControlToCompare="txtEffectiveDate" />
            </asp:Panel>
            <asp:CheckBox ID="chkNotify" resourcekey="SendNotification" runat="server" Checked="True" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnlUserRoles" runat="server" CssClass="WorkPanel" Visible="True">
                <hr noshade="noshade" size="1" />
                <asp:DataGrid ID="grdUserRoles" runat="server" Width="100%" GridLines="None" BorderStyle="None" DataKeyField="UserRoleID" EnableViewState="false" AutoGenerateColumns="false" CellSpacing="0" CellPadding="4">
                    <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	                <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	                <alternatingitemstyle cssclass="dnnGridAltItem" />
	                <edititemstyle cssclass="dnnFormInput" />
	                <selecteditemstyle cssclass="dnnFormError" />
	                <footerstyle cssclass="dnnGridFooter" />
	                <pagerstyle cssclass="dnnGridPager" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <!-- [DNN-4285] Hide the button if the user cannot be removed from the role -->
                                <asp:ImageButton ID="cmdDeleteUserRole" runat="server" AlternateText="Delete" CausesValidation="False"
                                    CommandName="Delete" ImageUrl="~/images/delete.gif" resourcekey="cmdDelete" 
                                    Visible='<%# DeleteButtonVisible(Convert.ToInt32(Eval("UserID")), Convert.ToInt32(Eval("RoleID")))  %>' 
                                    OnClick="cmdDeleteUserRole_click">
                                </asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="UserName">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatUser(Convert.ToInt32(Eval("UserID")),Eval("FullName").ToString()) %>'
                                    CssClass="Normal" ID="UserNameLabel" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="RoleName" HeaderText="SecurityRole" />
                        <asp:TemplateColumn HeaderText="EffectiveDate">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatDate(Convert.ToDateTime(Eval("EffectiveDate"))) %>'
                                    CssClass="Normal" ID="Label2" name="Label1" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="ExpiryDate">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatDate(Convert.ToDateTime(Eval("ExpiryDate"))) %>'
                                    CssClass="Normal" ID="Label1" name="Label1" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <hr noshade="noshade" size="1" />
            </asp:Panel>
        </td>
    </tr>
</table>