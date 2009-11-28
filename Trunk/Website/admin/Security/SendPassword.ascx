<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Control language="vb" Inherits="DotNetNuke.Modules.Admin.Security.SendPassword" CodeFile="SendPassword.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<div style="text-align:left;">
    <asp:Label ID="lblHelp" runat="Server" class="normal" />
</div>
<br />
<table id="tblSendPassword" runat="server" cellspacing="0" cellpadding="3" border="0" summary="SignIn Design Table" >
	<tr>
		<td width="160" class="SubHead"><dnn:label id="plUsername" controlname="txtUsername" runat="server" text="UserName:"></dnn:label></td>
	</tr>
	<tr>
		<td width="160"><asp:textbox id="txtUsername" columns="9" width="130" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
	<tr id="rowEmailLabel" runat="Server">
		<td width="160" class="SubHead"><dnn:label id="plEmail" controlname="txtEmail" runat="server" text="Email Address:"></dnn:label></td>
	</tr>
	<tr id="rowEmailText" runat="Server">
		<td width="160"><asp:textbox id="txtEmail" columns="9" width="130" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
	<tr id="trCaptcha1" runat="server">
		<td colspan="2" class="SubHead"><dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" text="Security Code:"></dnn:label></td>
	</tr>
	<tr id="trCaptcha2" runat="server">
		<td colspan="2"><dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" /></td>
	</tr>
	<tr>
		<td>
			<table id="tblQA" runat="server" visible="false">
				<tr height="25">
					<td class="SubHead"><dnn:label id="plQuestion" runat="server" controlname="lblQuestion" text="Password Question:"></dnn:label></td>
				</tr>
				<tr>
					<td><asp:label id = "lblQuestion" runat="server" cssclass="Normal"></asp:label></td>
				</tr>
				<tr height="25">
					<td class="SubHead" width="175"><dnn:label id="plAnswer" runat="server" controlname="txtAnswer" text="Password Answer:"></dnn:label></td>
				</tr>
				<tr>
					<td><asp:textbox id="txtAnswer" runat="server" cssclass="NormalTextBox" size="25" maxlength="20"></asp:textbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
		    <dnn:commandbutton id="cmdSendPassword" cssclass="CommandButton" runat="server"/>&nbsp;
	        <dnn:commandbutton id="cmdLogin" runat="server" resourcekey="cmdLogin"  imageurl="~/images/rt.gif" causesvalidation="False" cssclass="CommandButton" visible="false" />
		</td>
	</tr>
</table>
