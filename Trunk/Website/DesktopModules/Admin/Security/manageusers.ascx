<%@ Control language="vb" CodeFile="ManageUsers.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.ManageUsers" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Membership" Src="~/DesktopModules/Admin/Security/Membership.ascx" %>
<%@ Register TagPrefix="dnn" TagName="User" Src="~/DesktopModules/Admin/Security/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Roles" Src="~/DesktopModules/Admin/Security/SecurityRoles.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Password" Src="~/DesktopModules/Admin/Security/Password.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Profile" Src="~/DesktopModules/Admin/Security/Profile.ascx" %>
<%@ Register TagPrefix="dnn" TagName="MemberServices" Src="~/DesktopModules/Admin/Security/MemberServices.ascx" %>
<asp:panel id="pnlTabs" runat="server">
	<table width="450">
		<tr>
			<td nowrap valign="bottom">
				<dnn:commandbutton id="cmdUser" runat="server" 
					resourcekey="cmdUser" imageurl="~/images/icon_users_16px.gif" 
					causesvalidation="False" />
			</td>
			<td width="10"></td>
			<td nowrap valign="bottom">
				<dnn:commandbutton id="cmdRoles" runat="server" 
					resourcekey="cmdRoles" imageurl="~/images/icon_securityroles_16px.gif" 
					causesvalidation="False"/>
			</td>
			<td width="10"></td>
			<td nowrap valign="bottom">
				<dnn:commandbutton id="cmdPassword" runat="server" 
					resourcekey="cmdPassword" imageurl="~/images/password.gif" 
					causesvalidation="False" />
			</td>
			<td width="10"></td>
			<td nowrap valign="bottom">
				<dnn:commandbutton id="cmdProfile" runat="server" 
					resourcekey="cmdProfile" imageurl="~/images/icon_users_16px.gif" 
					causesvalidation="False" />
			</td>
			<td width="10"></td>
			<td nowrap valign="bottom">
				<dnn:commandbutton id="cmdServices" runat="server" 
					resourcekey="cmdServices" imageurl="~/images/icon_securityroles_16px.gif" 
					causesvalidation="False" />
			</td>
			<td width="*"></td>
		</tr>
			<tr><td colspan="7" height="10"></td></tr>
	</table>
</asp:panel>
<asp:panel id="pnlUser" runat="server">
	<table cellspacing="0" cellpadding="0" summary="User Design Table" border="0">
		<tr id="trTitle" runat="server">
			<td colspan="3" valign="bottom">
				<asp:label id="lblTitle" cssclass="Head" runat="server"></asp:label>
				<asp:image id="imgLockedOut" imageurl="~/images/icon_securityroles_16px.gif" runat="server" visible="False" />
				<asp:image id="imgOnline" imageurl="~/images/userOnline.gif" runat="server" visible="False" />
			</td>
		</tr>
		<tr id="trHelp" runat="server" visible="false"><td colspan="3" valign="bottom"><asp:label id="lblUserHelp" cssclass="Normal" runat="server"></asp:label></td></tr>
		<tr><td colspan="3" height="10"></td></tr>
		<tr id="UserRow" runat="server">
			<td valign="top"><dnn:user id="ctlUser" runat="Server" /></td>
			<td width="10" rowspan="5"></td>
			<td valign="top"><dnn:membership id="ctlMembership" runat="Server" /></td>
		</tr>
		<tr><td height="10"></td></tr>
	</table>
</asp:panel>
<asp:panel id="pnlRoles" runat="server" visible="false">
	<dnn:roles id="ctlRoles" runat="server"></dnn:roles>
</asp:panel>
<asp:panel id="pnlPassword" runat="server" visible="false">
	<dnn:Password id="ctlPassword" runat="server"></dnn:Password>
</asp:panel>
<asp:panel id="pnlProfile" runat="server" visible="false" style="text-align:left">
	<dnn:Profile id="ctlProfile" runat="server"></dnn:Profile>
</asp:panel>
<asp:panel id="pnlServices" runat="server" visible="false">
	<dnn:MemberServices id="ctlServices" runat="server"></dnn:MemberServices>
</asp:panel>
<asp:Panel ID="pnlCaptcha" runat="server" Visible="false" style="text-align:left">
    <table cellpadding="0" cellspacing="0" border="0">
		<tr id="trCaptcha" runat="server">
			<td class="SubHead" style="width:175px; vertical-align:top"><dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" text="Password:"></dnn:label></td>
			<td style="width:175px">
				<dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" ErrorStyle-CssClass="NormalRed" runat="server"  textboxstyle-cssclass="NormalTextBox" />
			</td>
		</tr>
    </table>
</asp:Panel>
<asp:panel ID="pnlRegister" runat="server" visible="false">
	<dnn:commandbutton id="cmdRegister" runat="server" 
		imageurl="~/images/save.gif" 
		causesvalidation="True" />
	<dnn:commandbutton id="cmdCancel" runat="server" 
	    resourcekey="cmdCancel" 
		imageurl="~/images/lt.gif" 
		causesvalidation="false" />
	<dnn:commandbutton id="cmdLogin" runat="server"
	    resourcekey="cmdLogin" 
		imageurl="~/images/rt.gif" 
		causesvalidation="False"
		visible="false" />
</asp:panel>
