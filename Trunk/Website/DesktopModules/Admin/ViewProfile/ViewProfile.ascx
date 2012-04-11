<%@ Control language="C#" CodeFile="ViewProfile.ascx.cs" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfile" %>
<asp:literal id="profileOutput" runat="server" />
<asp:Label id="noPropertiesLabel" runat="server" resourcekey="NoProperties" Visible="false" />
<div id="buttonPanel" runat="server">
    <ul class="dnnActions dnnClear">
        <li><asp:HyperLink id="editLink" runat="server" resourcekey="Edit" CssClass="dnnPrimaryAction" /></li>
    </ul>
</div>
