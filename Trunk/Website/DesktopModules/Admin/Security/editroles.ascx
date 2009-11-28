<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.EditRoles" CodeFile="EditRoles.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Url" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Roles Design Table"
    border="0">
    <tr>
        <td width="560" valign="top">
            <asp:Panel ID="pnlBasic" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" CssClass="Head" runat="server" Text="Basic Settings" Section="tblBasic" ResourceKey="BasicSettings" IncludeRule="True" />
                <table id="tblBasic" cellspacing="0" cellpadding="2" width="525" summary="Basic Settings Design Table" border="0" runat="server">
                    <tr>
                        <td colspan="2"><asp:Label ID="lblBasicSettingsHelp" CssClass="Normal" runat="server" resourcekey="BasicSettingsDescription" EnableViewState="False" /></td>
                    </tr>
                    <tr><td colspan="2" height="10"></td></tr>
                    <tr valign="top">
                        <td class="SubHead" width="150"><dnn:Label ID="plRoleName" runat="server" ResourceKey="RoleName" Suffix=":" ControlName="txtRoleName" /></td>
                        <td align="left" width="325">
                            <asp:TextBox ID="txtRoleName" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="325" />
                            <asp:Label ID="lblRoleName" Visible="False" runat="server" CssClass="Normal" />
                            <asp:RequiredFieldValidator ID="valRoleName" CssClass="NormalRed" runat="server" resourcekey="valRoleName" ControlToValidate="txtRoleName" Display="Dynamic"/>
                            <asp:RegularExpressionValidator ID="valRoleName2" CssClass="NormalRed" runat="server" resourcekey="valRoleName2" ControlToValidate="txtRoleName" Display="Dynamic" ValidationExpression="[A-Za-z0-9\.\s_-]*"/>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150"><dnn:Label ID="plDescription" runat="server" ResourceKey="Description" Suffix=":" ControlName="txtDescription" /></td>
                        <td width="325"><asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" MaxLength="1000" Columns="30" Width="325" TextMode="MultiLine" Height="84px" /></td>
                    </tr>
                    <tr>
                        <td class="SubHead" width="150"><dnn:Label ID="plRoleGroups" runat="server" Suffix="" ControlName="cboRoleGroups" /></td>
                        <td width="325"><asp:DropDownList ID="cboRoleGroups" CssClass="NormalTextBox" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="SubHead" width="150"><dnn:Label ID="plIsPublic" runat="server" ResourceKey="PublicRole" ControlName="chkIsPublic" /></td>
                        <td width="325"><asp:CheckBox ID="chkIsPublic" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="SubHead" width="150"><dnn:Label ID="plAutoAssignment" runat="server" ResourceKey="AutoAssignment" ControlName="chkAutoAssignment" /></td>
                        <td width="325"><asp:CheckBox ID="chkAutoAssignment" runat="server" /></td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshAdvanced" CssClass="Head" runat="server" Text="Advanced Settings" Section="tblAdvanced" ResourceKey="AdvancedSettings" IncludeRule="True" IsExpanded="False" />
                <table id="tblAdvanced" cellspacing="0" cellpadding="2" width="525" summary="Advanced Settings Design Table" border="0" runat="server">
                    <tr>
                        <td colspan="2"><asp:Label ID="lblAdvancedSettingsHelp" CssClass="Normal" runat="server" resourcekey="AdvancedSettingsHelp" EnableViewState="False" /></td>
                    </tr>
                    <tr height="10"><td colspan="2"></td></tr>
                    <tr>
                        <td colspan="2"><asp:Label ID="lblProcessorWarning" visible="false" CssClass="NormalRed" runat="server" resourcekey="ProcessorWarning" EnableViewState="False" /></td>
                    </tr>
                    <tr height="10"><td colspan="2"></td></tr>
                    <tr id="trServiceFee" runat="server" valign="top" visible="false">
                        <td class="SubHead" width="150"><dnn:Label ID="plServiceFee" runat="server" ResourceKey="ServiceFee" Suffix=":" ControlName="txtServiceFee" /></td>
                        <td width="325">
                            <asp:TextBox ID="txtServiceFee" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="100" />
                            <asp:CompareValidator ID="valServiceFee1" CssClass="NormalRed" runat="server" resourcekey="valServiceFee1" ControlToValidate="txtServiceFee" Display="Dynamic" Type="Currency" Operator="DataTypeCheck" />
                            <asp:CompareValidator ID="valServiceFee2" CssClass="NormalRed" runat="server" resourcekey="valServiceFee2" ControlToValidate="txtServiceFee" Display="Dynamic" Operator="GreaterThanEqual" ValueToCompare="0" />
                        </td>
                    </tr>
                    <tr id="trBillingPeriod" valign="top" runat="server" visible="false">
                        <td class="SubHead" width="150"><dnn:Label ID="plBillingPeriod" runat="server" ResourceKey="BillingPeriod" Suffix=":" ControlName="txtBillingPeriod" /></td>
                        <td width="325">
                            <asp:TextBox ID="txtBillingPeriod" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="100" />
                            <asp:DropDownList ID="cboBillingFrequency" CssClass="NormalTextBox" runat="server" Width="100px" DataValueField="value" DataTextField="text" AutoPostBack="true" />
                            <asp:CompareValidator ID="valBillingPeriod1" CssClass="NormalRed" runat="server" resourcekey="valBillingPeriod1" ControlToValidate="txtBillingPeriod" Display="Dynamic" Type="Integer" Operator="DataTypeCheck" />
                            <asp:CompareValidator ID="valBillingPeriod2" CssClass="NormalRed" runat="server" resourcekey="valBillingPeriod2" ControlToValidate="txtBillingPeriod" Display="Dynamic" Operator="GreaterThan" ValueToCompare="0" />
                        </td>
                    </tr>
                    <tr id="trTrialFee" valign="top" runat="server" visible="false">
                        <td class="SubHead" width="150"><dnn:Label ID="plTrialFee" runat="server" ResourceKey="TrialFee" Suffix=":" ControlName="txtTrialFee" /></td>
                        <td width="325">
                            <asp:TextBox ID="txtTrialFee" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="100" />
                            <asp:CompareValidator ID="valTrialFee1" CssClass="NormalRed" runat="server" resourcekey="valTrialFee1" ControlToValidate="txtTrialFee" Display="Dynamic" Type="Currency" Operator="DataTypeCheck" />
                            <asp:CompareValidator ID="valTrialFee2" CssClass="NormalRed" runat="server" resourcekey="valTrialFee2" ControlToValidate="txtTrialFee" Display="Dynamic" Operator="GreaterThanEqual" ValueToCompare="0" />
                        </td>
                    </tr>
                    <tr id="trTrialPeriod" valign="top" runat="server" visible="false">
                        <td class="SubHead" width="150"><dnn:Label ID="plTrialPeriod" runat="server" ResourceKey="TrialPeriod" Suffix=":" ControlName="txtTrialPeriod" /></td>
                        <td width="325">
                            <asp:TextBox ID="txtTrialPeriod" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="100" />
                            <asp:DropDownList ID="cboTrialFrequency" CssClass="NormalTextBox" runat="server" Width="100px" DataValueField="value" DataTextField="text" AutoPostBack="true" />
                            <asp:CompareValidator ID="valTrialPeriod1" CssClass="NormalRed" runat="server" resourcekey="valTrialPeriod1" ControlToValidate="txtTrialPeriod" Display="Dynamic" Type="Integer" Operator="DataTypeCheck" />
                            <asp:CompareValidator ID="valTrialPeriod2" CssClass="NormalRed" runat="server" resourcekey="valTrialPeriod2" ControlToValidate="txtTrialPeriod" Display="Dynamic" Operator="GreaterThan" ValueToCompare="0" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150"><dnn:Label ID="plRSVPCode" runat="server" ControlName="txtRSVPCode" /></td>
                        <td width="325"><asp:TextBox ID="txtRSVPCode" CssClass="NormalTextBox" runat="server" MaxLength="50" Columns="30" Width="100" AutoPostBack="true" /></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="150"><dnn:Label ID="plRSVPLink" runat="server" ControlName="txtRSVPLink" /></td>
                        <td width="325"><asp:Label ID="lblRSVPLink" CssClass="Normal" runat="server" Width="325"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" width="150" valign="top"><dnn:Label ID="plIcon" Text="Icon:" runat="server" ControlName="ctlIcon" />
                        </td>
                        <td width="325"><dnn:Url ID="ctlIcon" runat="server" Width="325" ShowUrls="False" ShowTabs="False" ShowLog="False" ShowTrack="False" Required="False" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<p>
    <dnn:CommandButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:CommandButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="CommandButton" ImageUrl="~/images/delete.gif" CausesValidation="False" />&nbsp;
    <dnn:CommandButton ID="cmdManage" resourcekey="cmdManage" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_users_16px.gif" CausesValidation="False" />&nbsp;
    <dnn:CommandButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" CausesValidation="False" />
</p>
