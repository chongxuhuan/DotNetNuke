<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Admin.Authentication.Login" AutoEventWireup="false" CodeFile="Login.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table cellspacing="0" cellpadding="3" border="0" summary="SignIn Design Table" width="160">
	<tr>
		<td class="SubHead" align="left"><dnn:label id="plUsername" controlname="txtUsername" runat="server" resourcekey="Username" /></td>
	</tr>
	<tr>
		<td><asp:textbox id="txtUsername" columns="9" width="150" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" align="left"><dnn:label id="plPassword" controlname="txtPassword" runat="server" resourcekey="Password" /></td>
	</tr>
	<tr>
		<td><asp:textbox id="txtPassword" columns="9" width="150" textmode="Password" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
	<tr id="rowVerification1" runat="server" visible="false">
		<td class="SubHead" align="left"><dnn:label id="plVerification" controlname="txtVerification" runat="server"/></td>
	</tr>
	<tr id="rowVerification2" runat="server" visible="false">
		<td><asp:textbox id="txtVerification" columns="9" width="150" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
    <tr id="trCaptcha1" runat="server">
	    <td class="SubHead" align="left"><dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" resourcekey="Captcha" /></td>
    </tr>
    <tr id="trCaptcha2" runat="server">
	    <td><dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" textboxstyle-cssclass="NormalTextBox" /></td>
    </tr>
	<tr>
		<td><asp:button id="cmdLogin" resourcekey="cmdLogin" cssclass="StandardButton" text="Login" runat="server" /></td>
	</tr>
</table>
