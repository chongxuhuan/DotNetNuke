<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Admin.AppGallery.AppGalleryDownload" AutoEventWireup="false" CodeFile="AppGalleryDownload.ascx.cs" %>
<asp:Button ID="btnDownload" runat="server" Text="Download" Visible="false" 
    onclick="btnDownload_Click" />
<asp:Button ID="btnDeploy" runat="server" Text="Deploy" Visible="false" 
    onclick="btnDeploy_Click" />
  <asp:HyperLink id="cmdInstall" runat="server" CssClass="dnnPrimaryAction" ResourceKey="installExtension" Text="install" visible="false"/>

