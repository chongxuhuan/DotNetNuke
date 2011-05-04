<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.ResourceVerifier" CodeFile="ResourceVerifier.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<p>
    <asp:placeholder id="PlaceHolder1" runat="server"></asp:placeholder>
</p>
<p>
	<dnn:CommandButton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" />
</p>