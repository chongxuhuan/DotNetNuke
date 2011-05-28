<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.LabelControl" %>

<div class="dnnTooltip">
    <label id="label" runat="server">
        <asp:LinkButton ID="cmdHelp" TabIndex="-1" runat="server" CausesValidation="False" EnableViewState="False" CssClass="dnnFormHelp">
            <asp:Label ID="lblLabel" runat="server" EnableViewState="False" />
        </asp:LinkButton>
    </label>

    <asp:Panel ID="pnlHelp" runat="server" CssClass="dnnFormHelpContent dnnClear" EnableViewState="False" style="display:none;">
        <asp:Label ID="lblHelp" runat="server" EnableViewState="False" class="dnnHelpText" />
        <a href="#" class="pinHelp">Pin</a> <!-- todo, we don't want to localize this crap so just make it an image -->
    </asp:Panel>
</div>