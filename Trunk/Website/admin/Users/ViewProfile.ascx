<%@ Control language="vb" CodeFile="ViewProfile.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfile" %>
<%@ Register TagPrefix="dnn" TagName="Profile" Src="~/DesktopModules/Admin/Security/Profile.ascx" %>
<asp:Label id="lblNoProperties" runat="server" resourcekey="NoProperties" Visible="false" />
<dnn:Profile id="ctlProfile" runat="server" 
	EditorMode="View" 
	ShowUpdate="False"/>
