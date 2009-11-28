<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WhatsNew.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Host.WhatsNew" %>
<p class="Normal" id="header" runat="server"></p>
<asp:Repeater ID="WhatsNewList" runat="server">
    <ItemTemplate>
        <span class="Head"><%#Eval("Version")%></span>
        <hr />
        <div class="Normal"><%#Eval("Notes")%></div>
        <p><hr style="color:#DDD;" /></p>
    </ItemTemplate>
</asp:Repeater>
<p class="NormalBold" id="footer" runat="server"></p>