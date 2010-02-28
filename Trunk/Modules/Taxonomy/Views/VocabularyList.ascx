<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VocabulariesList.ascx.vb" Inherits="DotNetNuke.Modules.Taxonomy.Views.VocabularyList" %>
<br />
<h3><asp:Label ID="titleLabel" runat="server" resourceKey="Title" /></h3>
<asp:DataGrid ID="vocabulariesGrid" runat="server" AutoGenerateColumns="false" width="100%" 
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
        <asp:BoundColumn DataField="Name" HeaderText="Name" />
        <asp:BoundColumn DataField="Description" HeaderText="Description" />
        <asp:BoundColumn DataField="Type" HeaderText="Type" />
        <asp:BoundColumn DataField="ScopeType" HeaderText="Scope" />
    </Columns>                
</asp:DataGrid>
<br />
<asp:Button ID="addVocabularyButton" runat="server" resourceKey="addVocabulary" />
