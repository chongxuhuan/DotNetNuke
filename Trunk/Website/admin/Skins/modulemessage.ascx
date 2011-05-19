<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Controls.ModuleMessage" %>
<asp:Panel id="dnnSkinMessage" runat="server" CssClass="dnnModuleMessage">
    <asp:label id="lblHeading" runat="server" visible="False" enableviewstate="False" CssClass="dnnModMessageHeading" />
    <asp:label id="lblMessage" runat="server" enableviewstate="False" />
</asp:Panel>