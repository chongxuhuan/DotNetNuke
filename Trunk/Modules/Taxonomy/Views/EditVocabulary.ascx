<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditVocabulary.ascx.vb" Inherits="DotNetNuke.Modules.Taxonomy.Views.EditVocabulary" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Modules.Taxonomy" Namespace="DotNetNuke.Modules.Taxonomy.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="EditVocabularyControl" Src="Controls/EditVocabularyControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="EditTermControl" Src="Controls/EditTermControl.ascx" %>
<table cellspacing="2" cellpadding="2">
    <tr>
        <td style="vertical-align:top">
            <h3><asp:Label ID="titleLabel" runat="server" resourcekey="Title" /></h3>
        </td>
        <td rowspan="2" style="width:40px"></td>
        <td rowspan="2" style="vertical-align:top">
            <h3><asp:Label ID="termsLabel" runat="server" resourceKey="Terms" /></h3>
            <dnn:TermsList id="termsList" runat="server" Height="200px" Width="200px" />
            <p>
                <asp:Button ID="addTerm" runat="server" resourceKey="AddTerm" />&nbsp;&nbsp;&nbsp;&nbsp;
            </p>
       </td>
        <td rowspan="2" style="width:40px"></td>
        <td rowspan="2" style="vertical-align:top">
            <asp:PlaceHolder ID="termEditor" runat="server" Visible="false">
                <h3><asp:Label ID="termLabel" runat="server" /></h3>
                <dnn:EditTermControl ID="editTermControl" runat="server" />
                <br />
                <p>
                    <asp:Button ID="saveTerm" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:PlaceHolder ID="deleteTermPlaceHolder" runat="server">
                        <asp:LinkButton ID="deleteTerm" runat="server" resourceKey="DeleteTerm" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
                    </asp:PlaceHolder>
                    <asp:LinkButton ID="cancelTerm" runat="server" resourceKey="CancelTerm" CausesValidation="false" />
                </p>
            </asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <td style="vertical-align:top">
            <dnn:EditVocabularyControl ID="editVocabularyControl" runat="server" IsAddMode="false" />
            <br />
            <p>
                <asp:Button ID="saveVocabulary" runat="server" resourceKey="SaveVocabulary" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="deleteVocabulary" runat="server" resourceKey="DeleteVocabulary" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="cancelEdit" runat="server" resourceKey="CancelEdit" CausesValidation="false" />
            </p>
        </td>
    </tr>
</table>
