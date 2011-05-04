<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewSchedule" CodeFile="ViewSchedule.ascx.cs" %>
<asp:DataGrid ID="dgSchedule" runat="server" AutoGenerateColumns="false" CellPadding="4" DataKeyField="ScheduleID" EnableViewState="false" border="0" CssClass="dnnScheduleGrid" GridLines="None" Width="100%">
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
                <asp:HyperLink ID="editLink" NavigateUrl='<%# EditUrl("ScheduleID",DataBinder.Eval(Container.DataItem,"ScheduleID").ToString()) %>'
                    Visible="<%# IsEditable %>" runat="server">
                    <asp:Image ID="editLinkImage" ImageUrl="~/images/edit.gif" Visible="<%# IsEditable %>"
                        AlternateText="Edit" runat="server" resourcekey="Edit" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="FriendlyName" HeaderText="Name" />
        <asp:TemplateColumn HeaderText="Enabled">
            <ItemTemplate>
                <asp:Image ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# Convert.ToBoolean(Eval("Enabled"))%>'
                    AlternateText="Enabled" runat="server" resourcekey="Enabled.Header" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Frequency">
            <ItemTemplate>
                <%# GetTimeLapse((int)DataBinder.Eval(Container.DataItem,"TimeLapse"), (string)DataBinder.Eval(Container.DataItem,"TimeLapseMeasurement")) %>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="RetryTimeLapse">
            <ItemTemplate>
                <%# GetTimeLapse((int)DataBinder.Eval(Container.DataItem, "RetryTimeLapse"), (string)DataBinder.Eval(Container.DataItem, "RetryTimeLapseMeasurement"))%>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="NextStart">
            <ItemTemplate>
                <asp:Label CssClass="Normal" ID="lblNextStart" runat="server" Visible='<%# Convert.ToBoolean(Eval("Enabled"))%>'><%# DataBinder.Eval(Container.DataItem,"NextStart")%></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink CssClass="dnnSecondaryAction" ID="lnkStatus" runat="server"
                    NavigateUrl='<%# EditUrl("ScheduleID",DataBinder.Eval(Container.DataItem,"ScheduleID").ToString(), "Status") %>'>
                    <asp:Image ID=Image2 runat="server" ImageUrl="~/images/icon_scheduler_16px.gif" resourcekey="Status" />
                </asp:HyperLink>
                <asp:HyperLink CssClass="dnnSecondaryAction" ID="lnkHistory" runat="server"
                    NavigateUrl='<%# EditUrl("ScheduleID",DataBinder.Eval(Container.DataItem,"ScheduleID").ToString(), "History") %>'>
                    <asp:Image ID=imgHistory runat="server" ImageUrl="~/images/icon_viewScheduleHistory_16px.gif" resourcekey="History" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>