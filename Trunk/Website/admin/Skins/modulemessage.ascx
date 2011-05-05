<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Controls.ModuleMessage" %>
<div class="ModuleMessage">
    <asp:Panel id="dnnSkinMessage" runat="server">
        <h3><asp:label id="lblHeading" runat="server" visible="False" enableviewstate="False"/></h3>
        <asp:label id="lblMessage" runat="server" enableviewstate="False" />
    </asp:Panel>
</div>