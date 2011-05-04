<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="VocabulariesList.ascx.cs" Inherits="DotNetNuke.Modules.Taxonomy.Views.VocabularyList" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<br />
<h3><asp:Label ID="titleLabel" runat="server" resourceKey="Title" /></h3>
<dnn:DnnGrid id = "vocabulariesGrid" runat="server" DataSource="<%# Model.Vocabularies %>"  AutoGenerateColumns="false">
    <MasterTableView>
        <Columns>
            <dnn:DnnGridHyperlinkColumn Text="Edit" DataNavigateUrlFields="VocabularyId" />
            <dnn:DnnGridBoundColumn DataField="Name" HeaderText="Name" />
            <dnn:DnnGridBoundColumn DataField="Description" HeaderText="Description" />
            <dnn:DnnGridBoundColumn DataField="Type" HeaderText="Type" />
            <dnn:DnnGridBoundColumn DataField="ScopeType" HeaderText="Scope" />
        </Columns>
    </MasterTableView>
</dnn:DnnGrid>
<br />
<dnn:DnnButton ID="addVocabularyButton" runat="server" Text="Create" />
