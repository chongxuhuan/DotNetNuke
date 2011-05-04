<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Scheduler.ViewScheduleHistory" CodeFile="ViewScheduleHistory.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<div class="dnnForm dnnScheduleHistory dnnClear" id="dnnScheduleHistory">
    <asp:DataGrid ID="dgScheduleHistory" AutoGenerateColumns="false" width="100%" CellPadding="2" GridLines="None" cssclass="dnnScheduleGrid" Runat="server" EnableViewState="false">
    <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	<itemstyle cssclass="dnnGridItem" horizontalalign="Left" />
	<alternatingitemstyle cssclass="dnnGridAltItem" />
	<edititemstyle cssclass="dnnFormInput" />
	<selecteditemstyle cssclass="dnnFormError" />
	<footerstyle cssclass="dnnGridFooter" />
	<pagerstyle cssclass="dnnGridPager" />
        <Columns>
            <asp:TemplateColumn HeaderText="Description">
                 <ItemTemplate>
                    <table border="0" width="100%">
                        <tr>
                            <td>
                                <i>
                                    <asp:Literal ID="litName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>' />
                                </i>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="Label1" runat="server" Visible='<%# DataBinder.Eval(Container.DataItem,"LogNotes").ToString() != ""%>'
                        Text='<%# DataBinder.Eval(Container.DataItem,"LogNotes")%>' width="500px" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="Server" HeaderText="Server" />
            <asp:BoundColumn DataField="ElapsedTime" HeaderText="Duration" />
            <asp:BoundColumn DataField="Succeeded" HeaderText="Succeeded" />
            <asp:TemplateColumn HeaderText="Start/End/Next Start" ItemStyle-Width="325px">
                <ItemTemplate>
                    S:&nbsp;<asp:Literal ID="litStartDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"StartDate")%>' />
                    <br />
                    E:&nbsp;<asp:Literal ID="litEndDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"EndDate")%>' />
                    <br />
                    N:&nbsp;<asp:Literal ID="litNextStart" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"NextStart")%>' />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdCancel" /></li>
    </ul>
</div>