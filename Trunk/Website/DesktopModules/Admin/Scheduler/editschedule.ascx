<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduler.EditSchedule" CodeFile="EditSchedule.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<asp:Panel ID="pnlScheduleItem" runat="server">
    <table cellspacing="0" cellpadding="3" width="750" summary="Edit Schedule" border="0">
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plFriendlyName" runat="server" ControlName="txtFriendlyName" Text="Friendly Name:"/></td>
            <td class="Normal"><asp:TextBox ID="txtFriendlyName" Width="450" runat="server" CssClass="NormalTextBox" /></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plType" runat="server" ControlName="txtType" Text="Full Class Name and Assembly:"/></td>
            <td class="Normal"><asp:TextBox ID="txtType" Width="450" runat="server" CssClass="NormalTextBox" /></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plEnabled" runat="server" ControlName="chkEnabled" Text="Schedule Enabled:" /></td>
            <td class="Normal"><asp:CheckBox ID="chkEnabled" runat="server" Text="Yes" resourcekey="Yes" CssClass="NormalTextBox" AutoPostBack="true" /></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plTimeLapse" runat="server" ControlName="txtTimeLapse" Text="Time Lapse:"/></td>
            <td class="Normal">
                <asp:TextBox ID="txtTimeLapse" runat="server" MaxLength="10" Width="50" CssClass="NormalTextBox"/>
                <asp:DropDownList ID="ddlTimeLapseMeasurement" runat="server" CssClass="NormalTextBox">
                    <asp:ListItem resourcekey="Seconds" Value="s">Seconds</asp:ListItem>
                    <asp:ListItem resourcekey="Minutes" Value="m">Minutes</asp:ListItem>
                    <asp:ListItem resourcekey="Hours" Value="h">Hours</asp:ListItem>
                    <asp:ListItem resourcekey="Days" Value="d">Days</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plRetryTimeLapse" runat="server" ControlName="txtRetryTimeLapse" Text="Retry Frequency:"/></td>
            <td class="Normal">
                <asp:TextBox ID="txtRetryTimeLapse" runat="server" MaxLength="10" Width="50" CssClass="NormalTextBox"/>
                <asp:DropDownList ID="ddlRetryTimeLapseMeasurement" runat="server" CssClass="NormalTextBox">
                    <asp:ListItem resourcekey="Seconds" Value="s">Seconds</asp:ListItem>
                    <asp:ListItem resourcekey="Minutes" Value="m">Minutes</asp:ListItem>
                    <asp:ListItem resourcekey="Hours" Value="h">Hours</asp:ListItem>
                    <asp:ListItem resourcekey="Days" Value="d">Days</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plRetainHistoryNum" runat="server" ControlName="ddlRetainHistoryNum" Text="Retain Schedule History:"/></td>
            <td class="Normal">
                <asp:DropDownList ID="ddlRetainHistoryNum" runat="server" CssClass="NormalTextBox">
                    <asp:ListItem Value="0">None</asp:ListItem>
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                    <asp:ListItem Value="100">100</asp:ListItem>
                    <asp:ListItem Value="250">250</asp:ListItem>
                    <asp:ListItem Value="500">500</asp:ListItem>
                    <asp:ListItem Value="-1">All</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plAttachToEvent" runat="server" ControlName="ddlAttachToEvent" Text="Run on Event:"/></td>
            <td class="Normal">
                <asp:DropDownList ID="ddlAttachToEvent" runat="server" CssClass="NormalTextBox">
                    <asp:ListItem resourcekey="None" Value="">None</asp:ListItem>
                    <asp:ListItem resourcekey="APPLICATION_START" Value="APPLICATION_START">Application Start</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plCatchUpEnabled" runat="server" ControlName="chkCatchUpEnabled" Text="Catch Up Enabled:"/></td>
            <td class="Normal"><asp:CheckBox ID="chkCatchUpEnabled" runat="server" Text="Yes" resourcekey="Yes" CssClass="NormalTextBox"/></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plObjectDependencies" runat="server" ControlName="txtObjectDependencies" Text="Object Dependencies:"/></td>
            <td class="Normal"><asp:TextBox ID="txtObjectDependencies" runat="server" MaxLength="150" Width="390" CssClass="NormalTextBox"/></td>
        </tr>
        <tr valign="top">
            <td class="SubHead" width="250"><dnn:Label ID="plServers" runat="server" ControlName="txtServers" Text="Run on Servers:"/></td>
            <td class="Normal">
                <asp:CheckBoxList ID="lstServers" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <p>
        <dnn:commandbutton ID="cmdRun" runat="server" resourcekey="cmdRun" CssClass="CommandButton" imageurl="~/images/icon_scheduler_16px.gif" />&nbsp;
        <dnn:commandbutton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="CommandButton" imageurl="~/images/save.gif" />&nbsp;
        <dnn:commandbutton ID="cmdDelete" runat="server" resourcekey="cmdDelete" CssClass="CommandButton" imageurl="~/images/delete.gif" />&nbsp;
        <dnn:commandbutton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton" imageurl="~/images/lt.gif" />
    </p>
</asp:Panel>
