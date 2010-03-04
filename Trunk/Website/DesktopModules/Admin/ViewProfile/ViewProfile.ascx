<%@ Control language="vb" CodeFile="ViewProfile.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfile" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>

<asp:literal id="ProfileOutput" runat="server" />
<asp:Label id="lblNoProperties" runat="server" resourcekey="NoProperties" Visible="false" />
<dnn:CommandButton id="cmdEdit" runat="server" resourcekey="Edit" imageurl="~/images/edit.gif" />