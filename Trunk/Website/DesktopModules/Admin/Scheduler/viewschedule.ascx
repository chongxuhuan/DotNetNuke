<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewSchedule" CodeFile="ViewSchedule.ascx.vb" %>
<asp:DataGrid ID="dgSchedule" runat="server" AutoGenerateColumns="false" CellPadding="4"
    DataKeyField="ScheduleID" EnableViewState="false" border="0" summary="This table shows the schedule of events for the portal."
    BorderStyle="None" BorderWidth="0px" GridLines="None" Width="750px">
    <Columns>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink ID="editLink" NavigateUrl='<%# EditURL("ScheduleID",DataBinder.Eval(Container.DataItem,"ScheduleID").ToString()) %>'
                    Visible="<%# IsEditable %>" runat="server">
                    <asp:Image ID="editLinkImage" ImageUrl="~/images/edit.gif" Visible="<%# IsEditable %>"
                        AlternateText="Edit" runat="server" resourcekey="Edit" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="FriendlyName" HeaderText="Name">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="Enabled">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center"></ItemStyle>
            <ItemTemplate>
                <asp:Image ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"Enabled")="true" %>'
                    AlternateText="Enabled" runat="server" resourcekey="Enabled.Header" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Frequency">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <%# GetTimeLapse(DataBinder.Eval(Container.DataItem,"TimeLapse"), DataBinder.Eval(Container.DataItem,"TimeLapseMeasurement")) %>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="RetryTimeLapse">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <%# GetTimeLapse(DataBinder.Eval(Container.DataItem,"RetryTimeLapse"), DataBinder.Eval(Container.DataItem,"RetryTimeLapseMeasurement")) %>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="NextStart">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center"></ItemStyle>
            <ItemTemplate>
                <asp:Label CssClass="Normal" ID="lblNextStart" runat="server" Visible='<%# DataBinder.Eval(Container.DataItem,"Enabled")="true" %>'><%# DataBinder.Eval(Container.DataItem,"NextStart")%></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink CssClass="CommandButton" ID="lnkHistory" runat="server"
                    NavigateUrl='<%# EditURL("ScheduleID",DataBinder.Eval(Container.DataItem,"ScheduleID").ToString(), "History") %>'>
                    <asp:Image ID="historyLinkImage" ImageUrl="~/images/icon_profile_16px.gif" 
                        AlternateText="History" runat="server" resourcekey="Edit" />
                    <asp:Label ID="lblHistory" runat="server" resourcekey="lnkHistory" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br />
