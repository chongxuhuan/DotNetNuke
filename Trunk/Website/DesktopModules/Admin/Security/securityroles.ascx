<%@ Control language="vb" CodeFile="SecurityRoles.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.SecurityRoles" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Security Roles Design Table" border="0">
	<tr>
		<td width="760" valign="top">
			<asp:panel id="pnlRoles" runat="server" cssclass="WorkPanel" visible="True">
				<table cellspacing="4" cellpadding="0" summary="Security Roles Design Table" border="0">
					<tr>
						<td colspan="7"><asp:label id="lblTitle" runat="server" cssclass="Head"></asp:label></td></tr>
					<tr>
						<td height="5"></td></tr>
					<tr>
						<td class="SubHead" valign="top" width="160"><dnn:label id="plUsers" runat="server" suffix="" controlname="cboUsers"></dnn:label><dnn:label id="plRoles" runat="server" suffix="" controlname="cboRoles"></dnn:label></td>
						<td width="10"></td>
						<td class="SubHead" valign="top" width="160"><dnn:label id="plEffectiveDate" runat="server" suffix="" controlname="txtEffectiveDate"></dnn:label></td>
						<td width="10"></td>
						<td class="SubHead" valign="top" width="160"><dnn:label id="plExpiryDate" runat="server" suffix="" controlname="txtExpiryDate"></dnn:label></td>
						<td width="10"></td>
						<td class="SubHead" valign="top" width="160">&nbsp;</td>
					</tr>
					<tr>
						<td valign="top" width="100%">
							<asp:TextBox ID="txtUsers" Runat="server" cssclass="NormalTextBox" width="150" />
							<asp:LinkButton ID="cmdValidate" Runat="server" CssClass="CommandButton" resourceKey="cmdValidate"/>
							<asp:dropdownlist id="cboUsers" cssclass="NormalTextBox" runat="server" autopostback="True" Width="100%" />
							<asp:dropdownlist id="cboRoles" cssclass="NormalTextBox" runat="server" autopostback="True" datavaluefield="RoleID" datatextfield="RoleName" width="100%"/>
						</td>
						<td width="10"></td>
						<td valign="top" width="110" nowrap="nowrap">
							<asp:textbox id="txtEffectiveDate" cssclass="NormalTextBox" runat="server" width="80"></asp:textbox>
							<asp:hyperlink id="cmdEffectiveCalendar" cssclass="CommandButton" runat="server" />
						</td>
						<td width="10"></td>
						<td valign="top" width="110" nowrap="nowrap">
							<asp:textbox id="txtExpiryDate" cssclass="NormalTextBox" runat="server" width="80"></asp:textbox>
							<asp:hyperlink id="cmdExpiryCalendar" cssclass="CommandButton" runat="server"/>
						</td>
						<td width="10"></td>
						<td valign="top" width="160" nowrap="nowrap">
							<dnn:commandbutton id="cmdAdd" cssclass="CommandButton" runat="server" ImageUrl="~/images/add.gif" CausesValidation="true" />
						</td>
					</tr>
				</table>
				<asp:comparevalidator id="valEffectiveDate" cssclass="NormalRed" runat="server" resourcekey="valEffectiveDate" display="Dynamic" type="Date" operator="DataTypeCheck" errormessage="<br>Invalid effective date" controltovalidate="txtEffectiveDate"></asp:comparevalidator>
				<asp:comparevalidator id="valExpiryDate" cssclass="NormalRed" runat="server" resourcekey="valExpiryDate" display="Dynamic" type="Date" operator="DataTypeCheck" errormessage="<br>Invalid expiry date" controltovalidate="txtExpiryDate"></asp:comparevalidator>
				<asp:comparevalidator id="valDates" cssclass="NormalRed" runat="server" resourcekey="valDates" display="Dynamic" type="Date" operator="GreaterThan" errormessage="<br>Expiry Date must be Greater than Effective Date" controltovalidate="txtExpiryDate" controltocompare="txtEffectiveDate"></asp:comparevalidator>
			</asp:panel>
			<asp:checkbox id="chkNotify" resourcekey="SendNotification" runat="server" cssclass="SubHead" text="Send Notification?" textalign="Right" Checked="True"></asp:checkbox>
		</td>
	</tr>
	<tr><td height="15"></td></tr>
	<tr>
		<td>
			<asp:panel id="pnlUserRoles" runat="server" cssclass="WorkPanel" visible="True">
    			<hr noshade size="1">
			    <asp:datagrid id="grdUserRoles" runat="server" width="100%" gridlines="None" borderwidth="0px" borderstyle="None" ondeletecommand="grdUserRoles_Delete" datakeyfield="UserRoleID" enableviewstate="false" autogeneratecolumns="false" cellspacing="0" cellpadding="4" border="0" summary="Security Roles Design Table">
					<headerstyle cssclass="NormalBold" />
					<itemstyle cssclass="Normal" />
					<columns>
						<asp:templatecolumn>
							<itemtemplate>
							    <!-- [DNN-4285] Hide the button if the user cannot be removed from the role --> 
								<asp:imagebutton id="cmdDeleteUserRole" 
								    runat="server" alternatetext="Delete" 
								    causesvalidation="False" commandname="Delete" 
								    imageurl="~/images/delete.gif" 
								    resourcekey="cmdDelete"
								    visible='<%# DeleteButtonVisible(DataBinder.Eval(Container.DataItem, "UserID"), DataBinder.Eval(Container.DataItem, "RoleID")) %>'></asp:imagebutton>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn headertext="UserName">
							<itemtemplate>
								<asp:label runat="server" text='<%#FormatUser(DataBinder.Eval(Container.DataItem, "UserID"),DataBinder.Eval(Container.DataItem, "FullName")) %>' cssclass="Normal" id="Label3" name="Label1"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:boundcolumn datafield="RoleName" headertext="SecurityRole" />
						<asp:templatecolumn headertext="EffectiveDate">
							<itemtemplate>
								<asp:label runat="server" text='<%#FormatDate(DataBinder.Eval(Container.DataItem, "EffectiveDate")) %>' cssclass="Normal" id="Label2" name="Label1"/>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn headertext="ExpiryDate">
							<itemtemplate>
								<asp:label runat="server" text='<%#FormatDate(DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>' cssclass="Normal" id="Label1" name="Label1"/>
							</itemtemplate>
						</asp:templatecolumn>
					</columns>
				</asp:datagrid>
				<hr noshade size="1">
			</asp:panel>
		</td>
	</tr>
</table>