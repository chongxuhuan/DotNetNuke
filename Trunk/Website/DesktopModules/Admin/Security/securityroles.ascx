<%@ Control Language="vb" CodeFile="SecurityRoles.ascx.vb" AutoEventWireup="false"
    Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.SecurityRoles" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Security Roles Design Table"
    border="0">
    <tr>
        <td width="760" valign="top">
            <asp:Panel ID="pnlRoles" runat="server" CssClass="WorkPanel" Visible="True">
                <table cellspacing="4" cellpadding="0" summary="Security Roles Design Table" border="0">
                    <tr>
                        <td colspan="7">
                            <asp:Label ID="lblTitle" runat="server" CssClass="Head"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td height="5">
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" valign="top" width="160">
                            <dnn:Label ID="plUsers" runat="server" Suffix="" ControlName="cboUsers"></dnn:Label>
                            <dnn:Label ID="plRoles" runat="server" Suffix="" ControlName="cboRoles"></dnn:Label>
                        </td>
                        <td width="10">
                        </td>
                        <td class="SubHead" valign="top" width="160">
                            <dnn:Label ID="plEffectiveDate" runat="server" Suffix="" ControlName="txtEffectiveDate">
                            </dnn:Label>
                        </td>
                        <td width="10">
                        </td>
                        <td class="SubHead" valign="top" width="160">
                            <dnn:Label ID="plExpiryDate" runat="server" Suffix="" ControlName="txtExpiryDate">
                            </dnn:Label>
                        </td>
                        <td width="10">
                        </td>
                        <td class="SubHead" valign="top" width="160">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="100%">
                            <asp:TextBox ID="txtUsers" runat="server" CssClass="NormalTextBox" Width="150" />
                            <asp:LinkButton ID="cmdValidate" runat="server" CssClass="CommandButton" resourceKey="cmdValidate" />
                            <asp:DropDownList ID="cboUsers" CssClass="NormalTextBox" runat="server" AutoPostBack="True"
                                Width="100%" />
                            <asp:DropDownList ID="cboRoles" CssClass="NormalTextBox" runat="server" AutoPostBack="True"
                                DataValueField="RoleID" DataTextField="RoleName" Width="100%" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="110" nowrap="nowrap">
                            <asp:TextBox ID="txtEffectiveDate" CssClass="NormalTextBox" runat="server" Width="80"></asp:TextBox>
                            <asp:HyperLink ID="cmdEffectiveCalendar" CssClass="CommandButton" runat="server" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="110" nowrap="nowrap">
                            <asp:TextBox ID="txtExpiryDate" CssClass="NormalTextBox" runat="server" Width="80"></asp:TextBox>
                            <asp:HyperLink ID="cmdExpiryCalendar" CssClass="CommandButton" runat="server" />
                        </td>
                        <td width="10">
                        </td>
                        <td valign="top" width="160" nowrap="nowrap">
                            <dnn:CommandButton ID="cmdAdd" CssClass="CommandButton" runat="server" ImageUrl="~/images/add.gif"
                                CausesValidation="true" />
                        </td>
                    </tr>
                </table>
                <asp:CompareValidator ID="valEffectiveDate" CssClass="NormalRed" runat="server" resourcekey="valEffectiveDate"
                    Display="Dynamic" Type="Date" Operator="DataTypeCheck" ErrorMessage="<br>Invalid effective date"
                    ControlToValidate="txtEffectiveDate"></asp:CompareValidator>
                <asp:CompareValidator ID="valExpiryDate" CssClass="NormalRed" runat="server" resourcekey="valExpiryDate"
                    Display="Dynamic" Type="Date" Operator="DataTypeCheck" ErrorMessage="<br>Invalid expiry date"
                    ControlToValidate="txtExpiryDate"></asp:CompareValidator>
                <asp:CompareValidator ID="valDates" CssClass="NormalRed" runat="server" resourcekey="valDates"
                    Display="Dynamic" Type="Date" Operator="GreaterThan" ErrorMessage="<br>Expiry Date must be Greater than Effective Date"
                    ControlToValidate="txtExpiryDate" ControlToCompare="txtEffectiveDate"></asp:CompareValidator>
            </asp:Panel>
            <asp:CheckBox ID="chkNotify" resourcekey="SendNotification" runat="server" CssClass="SubHead"
                Text="Send Notification?" TextAlign="Right" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td height="15">
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnlUserRoles" runat="server" CssClass="WorkPanel" Visible="True">
                <hr noshade size="1">
                <asp:DataGrid ID="grdUserRoles" runat="server" Width="100%" GridLines="None" BorderWidth="0px"
                    BorderStyle="None" DataKeyField="UserRoleID"
                    EnableViewState="false" AutoGenerateColumns="false" CellSpacing="0" CellPadding="4"
                    border="0" summary="Security Roles Design Table">
                    <HeaderStyle CssClass="NormalBold" />
                    <ItemStyle CssClass="Normal" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <!-- [DNN-4285] Hide the button if the user cannot be removed from the role -->
                                <asp:ImageButton ID="cmdDeleteUserRole" runat="server" AlternateText="Delete" CausesValidation="False"
                                    CommandName="Delete" ImageUrl="~/images/delete.gif" resourcekey="cmdDelete" Visible='<%# DeleteButtonVisible(DataBinder.Eval(Container.DataItem, "UserID"), DataBinder.Eval(Container.DataItem, "RoleID"))  %>' OnClick="cmdDeleteUserRole_click">
                                </asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="UserName">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatUser(DataBinder.Eval(Container.DataItem, "UserID"),DataBinder.Eval(Container.DataItem, "FullName")) %>'
                                    CssClass="Normal" ID="UserNameLabel" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="RoleName" HeaderText="SecurityRole" />
                        <asp:TemplateColumn HeaderText="EffectiveDate">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatDate(DataBinder.Eval(Container.DataItem, "EffectiveDate")) %>'
                                    CssClass="Normal" ID="Label2" name="Label1" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="ExpiryDate">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%#FormatDate(DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>'
                                    CssClass="Normal" ID="Label1" name="Label1" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <hr noshade size="1">
            </asp:Panel>
        </td>
    </tr>
</table>
