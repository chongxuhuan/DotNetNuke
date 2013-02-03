<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskList.ascx.cs" Inherits="DotNetNuke.Modules.TaskList.TaskList" %>

<asp:DataGrid ID="tasksGrid" runat="server" AutoGenerateColumns="False" >
    <Columns>
        <asp:HyperLinkColumn Text="Edit"  DataNavigateUrlField="TaskID" />
        <asp:BoundColumn DataField="Title" HeaderText="Title" />
        <asp:BoundColumn DataField="Description" HeaderText="Description" />
        <asp:BoundColumn DataField="IsComplete" HeaderText="IsComplete" />
    </Columns>
</asp:DataGrid>
<ul class="dnnActions dnnClear">
	<li><asp:HyperLink id="addButton" runat="server" class="dnnPrimaryAction" resourcekey="Add" /></li>
</ul>

