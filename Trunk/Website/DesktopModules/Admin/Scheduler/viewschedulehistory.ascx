<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewScheduleHistory"
    CodeFile="ViewScheduleHistory.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<asp:DataGrid ID="dgScheduleHistory" AutoGenerateColumns="false" width="100%" 
	CellPadding="2" GridLines="None" cssclass="DataGrid_Container" Runat="server"
	EnableViewState="false">
	<headerstyle cssclass="NormalBold" verticalalign="Top"/>
	<itemstyle cssclass="Normal" horizontalalign="Left" />
	<alternatingitemstyle cssclass="Normal" />
	<edititemstyle cssclass="NormalTextBox" />
	<selecteditemstyle cssclass="NormalRed" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
    <Columns>
        <asp:TemplateColumn HeaderText="Description" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <table border="0" width="100%">
                    <tr>
                        <td nowrap class="Normal">
                            <i>
                                <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
                            </i>
                        </td>
                    </tr>
                </table>
                <asp:Label runat="server" Visible='<%# DataBinder.Eval(Container.DataItem,"LogNotes")<>""%>'
                    Text='<%# DataBinder.Eval(Container.DataItem,"LogNotes")%>' />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="Server" HeaderText="Server" ItemStyle-VerticalAlign="Top"/>
        <asp:BoundColumn DataField="ElapsedTime" HeaderText="Duration" ItemStyle-VerticalAlign="Top"/>
        <asp:BoundColumn DataField="Succeeded" HeaderText="Succeeded" ItemStyle-VerticalAlign="Top"/>
        <asp:TemplateColumn HeaderText="Start/End/Next Start">
            <ItemStyle Wrap="False" VerticalAlign="Top"></ItemStyle>
            <ItemTemplate>
                S:&nbsp;<%# DataBinder.Eval(Container.DataItem,"StartDate")%>
                <br>
                E:&nbsp;<%# DataBinder.Eval(Container.DataItem,"EndDate")%>
                <br>
                N:&nbsp;<%# DataBinder.Eval(Container.DataItem,"NextStart")%>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br />
<p>
    <dnn:commandbutton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton" imageurl="~/images/lt.gif" />
</p>
