<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditVocabulary.ascx.vb" Inherits="DotNetNuke.Modules.Taxonomy.Views.CreateVocabulary" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="EditVocabularyControl" Src="Controls/EditVocabularyControl.ascx" %>
<dnn:EditVocabularyControl ID="editVocabularyControl" runat="server" IsAddMode="true" />
<br />
<p>
    <asp:Button ID="saveVocabulary" runat="server" resourceKey="saveVocabulary" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="cancelCreate" runat="server" resourceKey="cancelCreate" CausesValidation="false" />
</p>
