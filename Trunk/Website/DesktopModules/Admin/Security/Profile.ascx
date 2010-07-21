<%@ Control language="vb" CodeFile="Profile.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.Profile" %>
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
				editcontrolstyle-cssclass="NormalTextBox"
				enableClientValidation = "true"
				errorstyle-cssclass="NormalRed" 
				groupHeaderStyle-cssclass="Head" 
				groupHeaderIncludeRule="True" 
				helpstyle-cssclass="Help"
				labelstyle-cssclass="SubHead" 
				visibilitystyle-cssclass="Normal" 
				editcontrolwidth="700px" 
				labelwidth="150px"
				width="850px" />
		</td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
</table>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" imageurl="~/images/save.gif" />
</p>
