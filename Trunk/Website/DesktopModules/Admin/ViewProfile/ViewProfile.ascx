<%@ Control language="C#" CodeFile="ViewProfile.ascx.cs" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfile" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<asp:literal id="profileOutput" runat="server" />
<asp:Label id="noPropertiesLabel" runat="server" resourcekey="NoProperties" Visible="false" />
<ul class="dnnActions dnnClear">
    <li><asp:HyperLink id="editLink" runat="server" resourcekey="Edit" CssClass="dnnPrimaryAction" /></li>
</ul>