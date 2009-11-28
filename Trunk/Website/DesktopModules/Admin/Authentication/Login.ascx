<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Login.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Authentication.Login" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNN" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Profile" Src="~/DesktopModules/Admin/Security/Profile.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Password" Src="~/DesktopModules/Admin/Security/Password.ascx" %>
<%@ Register TagPrefix="dnn" TagName="User" Src="~/DesktopModules/Admin/Security/User.ascx" %>

<asp:panel id="pnlLogin" runat="server" Visible="false">
    <table cellspacing="0" cellpadding="3" border="0" summary="SignIn Design Table" width="160">
	    <tr>
		    <td colspan="2" class="SubHead" align="center">
                <DNN:DNNTabStrip 
                    ID="tsLogin" 
                    runat="server" 
                    TabRenderMode="All"
                    CssTabContainer="LoginTabGroup"
                    CssContentContainer="LoginContainerGroup" 
                    DefaultContainerCssClass="LoginContainer"
                    DefaultLabel-CssClass="LoginTab"
                    DefaultLabel-CssClassHover="LoginTabHover"
                    DefaultLabel-CssClassSelected="LoginTabSelected" 
                    visible="false" />
                <asp:Panel ID="pnlLoginContainer" runat="server" style="text-align:left" CssClass="LoginPanel" Visible="false" />
            </td>
	    </tr>
	    <tr>
		    <td colspan="2" align="left"><asp:checkbox id="chkCookie" class="Normal" resourcekey="Remember" text="Remember Login" runat="server" /></td>
	    </tr>
	    <tr style="height:5px;"><td colspan="2"></td></tr>
	    <tr>
		    <td id="tdRegister" runat="server" colspan="2" align="left"><asp:Linkbutton id="cmdRegister" resourcekey="cmdRegister" cssclass="CommandButton" text="Register" runat="server"/></td>
	    </tr>
	    <tr>
		    <td id="tdPassword" runat="server" colspan="2" align="left"><asp:Linkbutton id="cmdPassword" resourcekey="cmdForgotPassword" cssclass="CommandButton" text="Forgot Password?" runat="server" /></td>
	    </tr>
	    <tr>
		    <td colspan="2" align="left"><asp:label id="lblLogin" cssclass="Normal" runat="server" /></td>
	    </tr>
    </table>
</asp:panel>
<asp:Panel ID="pnlAssociate" runat="Server" Visible="false">
    <table cellspacing="0" cellpadding="3" border="0" summary="SignIn Design Table" width="350">
		<tr><td colspan="2" valign="bottom"><asp:label id="lblAuthenticatedTitle" cssclass="Head" runat="server" resourcekey="AuthenticatedTitle" /></td></tr>
		<tr><td colspan="2" valign="bottom"><asp:label id="lblAuthenticatedHelp" cssclass="Normal" runat="server" resourcekey="AuthenticatedHelp" /></td></tr>
		<tr><td colspan="2" height="10"></td></tr>
		<tr>
		    <td width="150" class="SubHead" align="left"><dnn:label id="plType" controlname="lblType" runat="server" /></td>
		    <td width="200" align="left"><asp:label id="lblType" cssclass="Normal" runat="server" /></td>
		</tr>
		<tr>
		    <td width="150" class="SubHead" align="left"><dnn:label id="plToken" controlname="lblToken" runat="server" /></td>
		    <td width="200" align="left"><asp:label id="lblToken" cssclass="Normal" runat="server" /></td>
		</tr>
		<tr><td colspan="2" height="10"></td></tr>
		<tr><td colspan="2" valign="bottom"><asp:label id="lblAssociateTitle" cssclass="Head" runat="server" resourcekey="AssociateTitle" /></td></tr>
		<tr><td colspan="2" valign="bottom"><asp:label id="lblAssociateHelp" cssclass="Normal" runat="server" resourcekey="AssociateHelp" /></td></tr>
		<tr><td colspan="2" height="10"></td></tr>
	    <tr>
		    <td width="150" class="SubHead" align="left"><dnn:label id="plUsername" controlname="txtUsername" runat="server" resourcekey="Username" /></td>
		    <td width="200" align="left"><asp:textbox id="txtUsername" columns="9" width="120" cssclass="NormalTextBox" runat="server" /></td>
	    </tr>
        <tr id="trCaptcha" runat="server">
	        <td width="150" class="SubHead" align="left"><dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" resourcekey="Captcha" /></td>
	        <td width="200" align="left"><dnn:captchacontrol id="ctlCaptcha" captchawidth="120" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" textboxstyle-cssclass="NormalTextBox" /></td>
        </tr>
	    <tr>
		    <td width="150" class="SubHead" align="left"><dnn:label id="plPassword" controlname="txtPassword" runat="server" resourcekey="Password" /></td>
		    <td width="200" align="left" valign="middle"><asp:textbox id="txtPassword" columns="9" width="80" textmode="password" cssclass="NormalTextBox" runat="server" /></td>
	    </tr>
	    <tr>
    	    <td colspan="2" align="center">
		        <dnn:commandbutton id="cmdAssociate" resourcekey="cmdAssociate" cssclass="CommandButtonButton" imageurl="~/images/save.gif" runat="server" CausesValidation="false" />
	        </td>
	    </tr>
    </table>
</asp:Panel>
<br />
<asp:Panel ID="pnlRegister" runat="Server" Visible="false">
	<table cellspacing="0" cellpadding="0" summary="User Design Table" border="0" width="350">
		<tr><td width="350" valign="bottom"><asp:label id="lblRegisterTitle" cssclass="Head" runat="server" resourcekey="RegisterTitle" /></td></tr>
		<tr><td width="350" valign="bottom"><asp:label id="lblRegisterHelp" cssclass="Normal" runat="server" /></td></tr>
		<tr><td width="350" height="10"></td></tr>
		<tr><td width="350" valign="top"><dnn:user id="ctlUser" runat="Server" /></td></tr>
		<tr><td width="350" height="10"></td></tr>
	</table>
	<dnn:commandbutton id="cmdCreateUser" runat="server" 
		imageurl="~/images/save.gif"
		causesvalidation="True" />
</asp:Panel>
<asp:panel id="pnlPassword" runat="server" visible="false">
	<dnn:password id="ctlPassword" runat="server" />
	<asp:panel ID="pnlProceed" runat="Server" Visible="false">
	    <hr width="95%" />
	    <dnn:commandbutton cssClass="CommandButton" id="cmdProceed" runat="server" resourcekey="cmdProceed" imageurl="~/images/rt.gif" />
	</asp:panel>
</asp:panel>
<asp:panel id="pnlProfile" runat="server" visible="false">
	<dnn:profile id="ctlProfile" runat="server" />
</asp:panel>