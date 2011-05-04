<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Users.DNNProfile" CodeFile="Profile.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Sectionhead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table cellspacing="0" cellpadding="0" summary="Profile Design Table" border="0">
	<tr id="trTitle" runat="server">
		<td valign="bottom"><asp:label id="lblTitle" cssclass="Head" runat="server"></asp:label></td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td valign="top">
			<dnn:ProfileEditorControl id="ProfileProperties" runat="Server"
				enableClientValidation = "true"
				errorstyle-cssclass="NormalRed" 
				helpstyle-cssclass="dnnFormHelpContent dnnClear"
				visibilitystyle-cssclass="Normal" />
		</td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
</table>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" imageurl="~/images/save.gif" />
</p>
