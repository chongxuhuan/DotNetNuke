<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewScheduleStatus" CodeFile="ViewScheduleStatus.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<div class="dnnForm dnnScheduleStatus dnnClear" id="dnnScheduleStatus">
    <div class="dnnssContent">
        <div class="dnnFormItem">
            <asp:Label ID="lblStatusLabel" resourcekey="lblStatusLabel" runat="server" />
            <asp:Label ID="lblStatus" runat="server" />
        </div>
        <div class="dnnFormItem">
            <asp:Label ID="lblMaxThreadsLabel" resourcekey="lblMaxThreadsLabel" runat="server" />
            <asp:Label ID="lblMaxThreads" runat="server" />
        </div>
         <div class="dnnFormItem">
            <asp:Label ID="lblActiveThreadsLabel" resourcekey="lblActiveThreadsLabel" runat="server" />
             <asp:Label ID="lblActiveThreads" runat="server" />
        </div>
         <div class="dnnFormItem">
            <asp:Label ID="lblFreeThreadsLabel" resourcekey="lblFreeThreadsLabel" runat="server" />
            <asp:Label ID="lblFreeThreads" runat="server" />
        </div>
        <asp:PlaceHolder ID="placeCommands" runat="server">
            <div class="dnnFormItem">
                <asp:Label ID="lblCommand" resourcekey="lblCommand" runat="server" />
                <asp:LinkButton ID="cmdStart" resourcekey="cmdStart" CssClass="dnnSecondaryAction" runat="server" />
                <asp:LinkButton ID="cmdStop" resourcekey="cmdStop" CssClass="dnnSecondaryAction" runat="server" />
            </div>
        </asp:PlaceHolder>
    </div>
    <asp:Panel ID="pnlScheduleProcessing" runat="server" CssClass="dnnScheduleProcessing">
        <h2><asp:Label ID="lblProcessing" runat="server" resourcekey="lblProcessing" /></h2>
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
    <asp:Panel ID="pnlScheduleQueue" runat="server" CssClass="dnnScheduleQueue">
        <h2><asp:Label ID="lblQueue" runat="server" resourcekey="lblQueue" /></h2>
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
        <li><asp:HyperLink id="cmdCancel" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdReturn" /></li>
    </ul>
</div>