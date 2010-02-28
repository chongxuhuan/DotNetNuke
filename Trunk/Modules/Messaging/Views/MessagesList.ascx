<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MessagesList.ascx.vb" Inherits="DotNetNuke.Modules.Messaging.Views.MessagesList" %>
<br />
<h3><asp:Label ID="titleLabel" runat="server" resourceKey="Title" /></h3>
<asp:DataGrid ID="messagesGrid" runat="server" AutoGenerateColumns="false" width="100%" 
	CellPadding="2" GridLines="None" cssclass="DataGrid_Container">
	<headerstyle cssclass="NormalBold" verticalalign="Top"/>
	<itemstyle cssclass="Normal" horizontalalign="Left" />
	<alternatingitemstyle cssclass="Normal" />
	<edititemstyle cssclass="NormalTextBox" />
	<selecteditemstyle cssclass="NormalRed" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
    <Columns>
        <asp:HyperLinkColumn HeaderText="" />
        <asp:BoundColumn DataField="Subject" HeaderText="Subject" />
        <asp:BoundColumn DataField="FromDisplayName" HeaderText="From" />
        <asp:BoundColumn DataField="MessageDate" HeaderText="Date" />
        <asp:BoundColumn DataField="Status" HeaderText="Status" />
    </Columns>                
</asp:DataGrid>
<br />
<asp:Button ID="addMessageButton" runat="server" resourceKey="addMessage" />
