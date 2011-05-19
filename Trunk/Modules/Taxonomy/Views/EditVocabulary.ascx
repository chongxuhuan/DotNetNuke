<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditVocabulary.ascx.cs" Inherits="DotNetNuke.Modules.Taxonomy.Views.EditVocabulary" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="EditVocabularyControl" Src="Controls/EditVocabularyControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="EditTermControl" Src="Controls/EditTermControl.ascx" %>
<div class="dnnForm dnnEditVocab dnnClear">
    <asp:Panel ID="pnlVocabTerms" runat="server" class="dnnForm">
        <dnn:EditVocabularyControl ID="editVocabularyControl" runat="server" IsAddMode="false" />
        <div class="dnnFormItem">
            <dnn:DnnFieldLabel id="termsLabel" runat="server" Text="Terms.Text" ToolTip="Terms.ToolTip" />
            <dnn:TermsList id="termsList" runat="server" Height="200px" Width="200px" />
        </div>
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton ID="saveVocabulary" runat="server" resourcekey="SaveVocabulary" CssClass="dnnPrimaryAction" /></li>
            <li><asp:LinkButton ID="deleteVocabulary" runat="server" resourceKey="DeleteVocabulary" CausesValidation="false" CssClass="dnnSecondaryAction" /></li>
            <li><asp:HyperLink ID="cancelEdit" runat="server" resourceKey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
            <li><asp:LinkButton ID="addTermButton" runat="server" resourceKey="AddTerm" CssClass="dnnSecondaryAction" /></li>
        </ul>
    </asp:Panel>
    <asp:Panel ID="pnlTermEditor" runat="server" Visible="false">
        <fieldset>
            <legend><asp:Label ID="termLabel" runat="server" /></legend>
            <dnn:EditTermControl ID="editTermControl" runat="server" />
            <ul class="dnnActions dnnClear">
                <li><asp:LinkButton ID="saveTermButton" runat="server" CssClass="dnnPrimaryAction" resourcekey="saveTermButton" /></li>
                <li><asp:LinkButton ID="deleteTermButton" runat="server" resourceKey="DeleteTerm" CausesValidation="false" CssClass="dnnSecondaryAction" /></li>
                <li><asp:LinkButton ID="cancelTermButton" runat="server" resourceKey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
            </ul>
        </fieldset>
    </asp:Panel>
</div>