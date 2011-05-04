<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewScheduleStatus" CodeFile="ViewScheduleStatus.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<div class="dnnForm dnnScheduleStatus dnnClear" id="dnnScheduleStatus">
    <table border="0" cellspacing="1" cellpadding="3">
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblStatusLabel" resourcekey="lblStatusLabel" runat="server" />
            </td>
            <td class="Normal">
                <asp:Label CssClass="NormalBold" ID="lblStatus" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblMaxThreadsLabel" resourcekey="lblMaxThreadsLabel" runat="server" />
            </td>
            <td class="Normal">
                <asp:Label CssClass="NormalBold" ID="lblMaxThreads" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblActiveThreadsLabel" resourcekey="lblActiveThreadsLabel" runat="server" />
            </td>
            <td class="Normal">
                <asp:Label CssClass="NormalBold" ID="lblActiveThreads" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblFreeThreadsLabel" resourcekey="lblFreeThreadsLabel" runat="server" />
            </td>
            <td class="Normal">
                <asp:Label CssClass="NormalBold" ID="lblFreeThreads" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder ID="placeCommands" runat="server">
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblCommand" resourcekey="lblCommand" runat="server" />
            </td>
            <td class="Normal">
                <asp:LinkButton ID="cmdStart" resourcekey="cmdStart" CssClass="CommandButton" runat="server" />
                &nbsp;&nbsp;
                <asp:LinkButton ID="cmdStop" resourcekey="cmdStop" CssClass="CommandButton" runat="server" />
            </td>
        </tr>
        </asp:PlaceHolder>
    </table>
    <br />
    <asp:Panel ID="pnlScheduleProcessing" runat="server">
        <asp:Label ID="lblProcessing" runat="server" resourcekey="lblProcessing" CssClass="SubHead" />
        <hr noshade="noshade" size="1" />
        <asp:DataGrid ID="dgScheduleProcessing" runat="server" AutoGenerateColumns="false" CellPadding="4" DataKeyField="ScheduleID" EnableViewState="false" CssClass="dnnScheduleGrid"
            GridLines="None">
            <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	        <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	        <alternatingitemstyle cssclass="dnnGridAltItem" />
	        <edititemstyle cssclass="dnnFormInput" />
	        <selecteditemstyle cssclass="dnnFormError" />
	        <footerstyle cssclass="dnnGridFooter" />
	        <pagerstyle cssclass="dnnGridPager" />
            <Columns>
                <asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID" />
                <asp:BoundColumn DataField="TypeFullName" HeaderText="Type" />
                <asp:BoundColumn DataField="StartDate" HeaderText="Started" />
                <asp:BoundColumn DataField="ElapsedTime" HeaderText="Duration&lt;br&gt;(seconds)" />
                <asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies" />
                <asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy" />
                <asp:BoundColumn DataField="ThreadID" HeaderText="Thread" />
                <asp:TemplateColumn HeaderText="Servers">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem,"Servers") %>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlScheduleQueue" runat="server">
        <asp:Label ID="lblQueue" runat="server" resourcekey="lblQueue" CssClass="SubHead" />
        <hr noshade="noshade" size="1" />
        <asp:DataGrid ID="dgScheduleQueue" runat="server" AutoGenerateColumns="false" CellPadding="4" DataKeyField="ScheduleID" EnableViewState="false" CssClass="dnnScheduleGrid"
            GridLines="None">
            <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	        <itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	        <alternatingitemstyle cssclass="dnnGridAltItem" />
	        <edititemstyle cssclass="dnnFormInput" />
	        <selecteditemstyle cssclass="dnnFormError" />
	        <footerstyle cssclass="dnnGridFooter" />
	        <pagerstyle cssclass="dnnGridPager" />
            <Columns>
                <asp:BoundColumn DataField="ScheduleID" HeaderText="ScheduleID" />
                <asp:BoundColumn DataField="FriendlyName" HeaderText="Name" />
                <asp:BoundColumn DataField="NextStart" HeaderText="NextStart" />
                <asp:TemplateColumn HeaderText="Overdue">
                    <ItemTemplate>
                        <%# GetOverdueText((double)DataBinder.Eval(Container.DataItem,"OverdueBy")) %>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="RemainingTime" HeaderText="TimeRemaining" />
                <asp:BoundColumn DataField="ObjectDependencies" HeaderText="ObjectDependencies" />
                <asp:BoundColumn DataField="ScheduleSource" HeaderText="TriggeredBy" />
                <asp:TemplateColumn HeaderText="Servers">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem,"Servers") %>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </asp:Panel>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdCancel" /></li>
    </ul>
</div>