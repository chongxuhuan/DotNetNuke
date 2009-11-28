<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control language="vb" CodeFile="EditGroups.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.EditGroups" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Edit Roles Design Table" border="0">
	<tr>
		<td width="560" valign="top">
			<table id="tblBasic" cellspacing="0" cellpadding="2" width="525" summary="Basic Settings Design Table" border="0" runat="server">
				<tr valign="top">
					<td class="SubHead" width="150"><dnn:label id="plRoleGroupName" runat="server" controlname="txtRoleGroupName"></dnn:label></td>
					<td align="left" width="325">
						<asp:textbox id="txtRoleGroupName" cssclass="NormalTextBox" runat="server" maxlength="50" columns="30" width="325"></asp:textbox>
						<asp:requiredfieldvalidator id="valRoleGroupName" cssclass="NormalRed" runat="server" resourcekey="valRoleGroupName" controltovalidate="txtRoleGroupName" errormessage="<br>You Must Enter a Valid Name" display="Dynamic"></asp:requiredfieldvalidator>
					</td>
				</tr>
				<tr valign="top">
					<td class="SubHead" width="150"><dnn:label id="plDescription" runat="server" controlname="txtDescription"></dnn:label></td>
					<td width="325"><asp:textbox id="txtDescription" cssclass="NormalTextBox" runat="server" maxlength="1000" columns="30" width="325" textmode="MultiLine" height="84px"></asp:textbox></td></tr>
			</table>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" text="Update" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" text="Cancel" causesvalidation="False" borderstyle="none" />
	&nbsp;
	<asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" runat="server" cssclass="CommandButton" text="Delete" causesvalidation="False" borderstyle="none" />
</p>
