<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.LocaleSelectorControl" %>
<asp:RadioButtonList ID="rbViewType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True"/>
<asp:DropDownList ID="ddlPortalDefaultLanguage" runat="server" CssClass="le_languages" AutoPostBack="true" />
<asp:Literal ID="litStatus" runat="server" />