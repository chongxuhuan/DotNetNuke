<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewScheduleStatus"
    CodeFile="ViewScheduleStatus.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table border="0" cellspacing="1" cellpadding="3">
    <tr>
        <td class="SubHead">
            <asp:Label ID="lblStatusLabel" resourcekey="lblStatusLabel" runat="server">Current Status:</asp:Label>
        </td>
        <td class="Normal">
            <asp:Label CssClass="NormalBold" ID="lblStatus" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead">
            <asp:Label ID="lblMaxThreadsLabel" resourcekey="lblMaxThreadsLabel" runat="server">Max Threads:</asp:Label>
        </td>
        <td class="Normal">
            <asp:Label CssClass="NormalBold" ID="lblMaxThreads" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead">
            <asp:Label ID="lblActiveThreadsLabel" resourcekey="lblActiveThreadsLabel" runat="server">Active Threads:</asp:Label>
        </td>
        <td class="Normal">
            <asp:Label CssClass="NormalBold" ID="lblActiveThreads" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead">
            <asp:Label ID="lblFreeThreadsLabel" resourcekey="lblFreeThreadsLabel" runat="server">Free Threads:</asp:Label>
        </td>
        <td class="Normal">
            <asp:Label CssClass="NormalBold" ID="lblFreeThreads" runat="server" />
        </td>
    </tr>
    <asp:PlaceHolder ID="placeCommands" runat="server">
    <tr>
        <td class="SubHead">
            <asp:Label ID="lblCommand" resourcekey="lblCommand" runat="server">Command:</asp:Label>
        </td>
        <td class="Normal">
            <asp:LinkButton ID="cmdStart" resourcekey="cmdStart" CssClass="CommandButton" runat="server">Start</asp:LinkButton>
            &nbsp;&nbsp;
            <asp:LinkButton ID="cmdStop" resourcekey="cmdStop" CssClass="CommandButton" runat="server">Stop</asp:LinkButton>
        </td>
    </tr>
    </asp:PlaceHolder>
</table>
<br>
<asp:Panel ID="pnlScheduleProcessing" runat="server">
    <asp:Label ID="lblProcessing" runat="server" resourcekey="lblProcessing" CssClass="SubHead">Items Processing</asp:Label>
    <hr noshade="noshade" size="1">
    <asp:DataGrid ID="dgScheduleProcessing" runat="server" AutoGenerateColumns="false"
        CellPadding="4" DataKeyField="ScheduleID" EnableViewState="false" border="1"
        summary="This table shows the scheduled tasks that are currently running." AlternatingItemStyle-BackColor="#CFCFCF"
        BorderStyle="None" BorderWidth="0px" GridLines="None">
        <AlternatingItemStyle BackColor="#CFCFCF"></AlternatingItemStyle>
        <Columns>
            <asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="TypeFullName" HeaderText="Type">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="StartDate" HeaderText="Started">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ElapsedTime" HeaderText="Duration&lt;br&gt;(seconds)">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ThreadID" HeaderText="Thread">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Servers">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem,"Servers") %>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</asp:Panel>
<br>
<br>
<asp:Panel ID="pnlScheduleQueue" runat="server">
    <asp:Label ID="lblQueue" runat="server" resourcekey="lblQueue" CssClass="SubHead">Items Processing</asp:Label>
    <hr noshade="noshade" size="1">
    <asp:DataGrid ID="dgScheduleQueue" runat="server" AutoGenerateColumns="false" CellPadding="4"
        DataKeyField="ScheduleID" EnableViewState="false" border="1" summary="This table shows the tasks that are queued up in the schedule."
        AlternatingItemStyle-BackColor="#CFCFCF" BorderStyle="None" BorderWidth="0px"
        GridLines="None">
        <AlternatingItemStyle BackColor="#CFCFCF"></AlternatingItemStyle>
        <Columns>
            <asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="FriendlyName" HeaderText="Name">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="NextStart" HeaderText="NextStart">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Overdue">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
                <ItemTemplate>
                    <%# GetOverdueText(DataBinder.Eval(Container.DataItem,"OverdueBy")) %>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="RemainingTime" HeaderText="TimeRemaining">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Servers">
                <HeaderStyle CssClass="NormalBold"></HeaderStyle>
                <ItemStyle CssClass="Normal"></ItemStyle>
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem,"Servers") %>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</asp:Panel>
<br />
<p>
    <dnn:commandbutton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton" imageurl="~/images/lt.gif" />
</p>
