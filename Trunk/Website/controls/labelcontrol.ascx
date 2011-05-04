<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.LabelControl" %>
<label id="label" runat="server">
    <asp:LinkButton ID="cmdHelp" TabIndex="-1" runat="server" CausesValidation="False" EnableViewState="False" CssClass="dnnFormHelp">
        <asp:Label ID="lblLabel" runat="server" EnableViewState="False" />
    </asp:LinkButton>
</label>
<asp:Panel ID="pnlHelp" runat="server" CssClass="dnnFormHelpContent dnnClear" EnableViewState="False"><asp:Label ID="lblHelp" runat="server" EnableViewState="False" /></asp:Panel>