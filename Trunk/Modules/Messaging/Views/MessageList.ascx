<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MessageList.ascx.cs" Inherits="DotNetNuke.Modules.Messaging.Views.MessageList" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<br />
<h3>
    <asp:Label ID="titleLabel" runat="server" resourceKey="Title" /></h3>
<dnn:DnnGrid ID="messagesGrid" runat="server" AutoGenerateColumns="false" Width="100%"
    DataSource="<%# Model.Messages %>"
    PagerStyle-AlwaysVisible="true" AllowPaging="true" AllowCustomPaging="true" CellPadding="2"
    GridLines="None" CssClass="DataGrid_Container" ClientSettings-Selecting-AllowRowSelect="true"
    AllowMultiRowSelection="true">
    <HeaderStyle CssClass="NormalBold" VerticalAlign="Top" />
    <ItemStyle CssClass="Normal" HorizontalAlign="Left" />
    <%--    <AlternatingItemStyle CssClass="Normal" />--%>
    <EditItemStyle CssClass="NormalTextBox" />
    <SelectedItemStyle CssClass="NormalRed" />
    <FooterStyle CssClass="DataGrid_Footer" />
    <PagerStyle CssClass="DataGrid_Pager" />
    <MasterTableView DataKeyNames="MessageID">
        <Columns>
            <dnn:DnnGridClientSelectColumn HeaderStyle-Width="0" />
            <dnn:DnnGridBoundColumn DataField="FromUserName" HeaderText="From" HeaderStyle-Width="100"
                ItemStyle-Wrap="false" />
            <dnn:DnnGridHyperlinkColumn HeaderText="Subject" />
            <dnn:DnnGridBoundColumn DataField="MessageDate" HeaderText="Date" HeaderStyle-Width="0"
                ItemStyle-Wrap="false" />
            <dnn:DnnGridBoundColumn DataField="Status" HeaderText="Status" HeaderStyle-Width="0" />
        </Columns>
    </MasterTableView>
</dnn:DnnGrid>
<br />
<asp:Button ID="addMessageButton" runat="server" resourceKey="addMessage" />
<asp:LinkButton ID="markAsRead" runat="server" resourceKey="markAsRead" />
<asp:LinkButton ID="markAsUnread" runat="server" resourceKey="markAsUnread" />
<asp:LinkButton ID="delete" runat="server" resourceKey="markDeleted" />
