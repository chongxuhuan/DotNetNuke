<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Portals.Signup" CodeFile="Signup.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table class="Settings" cellSpacing="2" cellPadding="2" width="560" summary="Host Settings Design Table"
	border="0">
	<tr>
		<td vAlign="top" width="560">
		    <dnn:sectionhead id="dshPortal" includerule="True" resourcekey="PortalSetup" section="tblPortal" text="Portal Setup" cssclass="Head" runat="server"></dnn:sectionhead>
			<table id="tblPortal" cellSpacing="0" cellPadding="2" width="525" summary="Basic Settings Design Table" border="0" runat="server">
				<tr>
					<td class="Normal" colSpan="2"><asp:label id="lblInstructions" cssclass="Normal" runat="server"/></td>
				</tr>
				<tr>
					<td align="center" colSpan="2">
					    <asp:label id="lblMessage" cssclass="NormalRed" runat="server"></asp:label><br>
						<asp:datalist id="lstResults" runat="server" cellpadding="4" cellspacing="0" borderwidth="0" visible="False" width="100%">
							<headertemplate><asp:label id="lblValidationResults" cssclass="NormalBold" runat="server" resourcekey="ValidationResults"/></headertemplate>
							<itemtemplate><span class="Normal"><%# Container.DataItem %></span></itemtemplate>
						</asp:datalist>
					</td>
				</tr>
				<tr id="rowType" runat="server">
					<td class="SubHead" width="150"><dnn:label id="plType" text="Portal Type:" runat="server" controlname="optType"></dnn:label></td>
					<td class="SubHead">
					    <asp:radiobuttonlist id="optType" cssclass="Normal" runat="server" repeatdirection="Horizontal" AutoPostBack="True">
							<asp:listitem resourcekey="Parent" value="P">Parent</asp:listitem>
							<asp:listitem resourcekey="Child" value="C">Child</asp:listitem>
						</asp:radiobuttonlist>
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plPortalAlias" runat="server" controlname="txtPortalName"/></td>
					<td>
					    <asp:textbox id="txtPortalName" cssclass="NormalTextBox" runat="server" width="300" maxlength="128"/>
					    <asp:requiredfieldvalidator id="valPortalName" resourcekey="valPortalName.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtPortalName" display="Dynamic" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" valign="top" width="150"><dnn:label id="plHomeDirectory" text="Home Directory:" runat="server" controlname="txtHomeDirectory"/></td>
					<td class="NormalTextBox" width="325" nowrap>
						<asp:textbox id="txtHomeDirectory" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"/>
						<asp:LinkButton CausesValidation="False" ID="btnCustomizeHomeDir" Runat="server" resourcekey="Customize" CssClass="CommandButton"/></td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plTitle" runat="server" controlname="txtTitle"/></td>
					<td><asp:textbox id="txtTitle" cssclass="NormalTextBox" runat="server" width="300" maxlength="128"/></td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plDescription" runat="server" controlname="txtDescription"/></td>
					<td><asp:textbox id="txtDescription" cssclass="NormalTextBox" runat="server" width="300" maxlength="500" textmode="MultiLine" rows="3"/></td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plKeyWords" text="KeyWords:" runat="server" controlname="txtKeyWords"/></td>
					<td><asp:textbox id="txtKeyWords" cssclass="NormalTextBox" runat="server" width="300" maxlength="500" textmode="MultiLine" rows="3"/></td>
				</tr>
				<tr>
					<td class="SubHead" width="150" vAlign="top"><dnn:label id="plTemplate" text="Template:" runat="server" controlname="cboTemplate"/></td>
					<td vAlign="top">
					    <asp:dropdownlist id="cboTemplate" cssclass="NormalTextBox" runat="server" width="300" AutoPostBack="True"/>
						<asp:RequiredFieldValidator id="valTemplate" runat="server" Display="Dynamic" ControlToValidate="cboTemplate" InitialValue="-1" resourcekey="valTemplate.ErrorMessage"/>
						<br/>
						<asp:Label id="lblTemplateDescription" runat="server" CssClass="Normal"/>
					</td>
				</tr>
			</table>
			<br>
			<dnn:sectionhead id="dshSecurity" includerule="True" resourcekey="SecuritySettings" section="tblSecurity" text="Security Settings" cssclass="Head" runat="server"/>
			<table id="tblSecurity" cellSpacing="0" cellPadding="2" width="525" summary="Basic Settings Design Table" border="0" runat="server">
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plUsername" runat="server" controlname="txtUsername"/></td>
					<td>
					    <asp:textbox id="txtUsername" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"/>
					    <asp:requiredfieldvalidator id="valUsername" resourcekey="valUsername.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtUsername" display="Dynamic"/>
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plFirstName" runat="server" controlname="txtFirstName"/></td>
					<td>
					    <asp:textbox id="txtFirstName" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"/>
					    <asp:requiredfieldvalidator id="valFirstName" resourcekey="valFirstName.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtFirstName" display="Dynamic"/>
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plLastName" runat="server" controlname="txtLastName"/></td>
					<td>
					    <asp:textbox id="txtLastName" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"/>
					    <asp:requiredfieldvalidator id="valLastName" cssclass="Normal" runat="server" controltovalidate="txtLastName" errormessage="Last Name Is Required." display="Dynamic"/>
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plEmail" runat="server" controlname="txtEmail"></dnn:label></td>
					<td>
					    <asp:textbox id="txtEmail" cssclass="NormalTextBox" runat="server" width="300" maxlength="100"/>
					    <asp:requiredfieldvalidator id="valEmail" resourcekey="valEmail.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtEmail" display="Dynamic"/>
					    <asp:RegularExpressionValidator ID="valEmail2" runat="server" resourcekey="valEmail2.ErrorMessage" CssClass="Normal" ControlToValidate="txtEmail" Display="Dynamic" />
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plPassword" runat="server" controlname="txtPassword"/></td>
					<td>
					    <asp:textbox id="txtPassword" cssclass="NormalTextBox" runat="server" width="300" maxlength="20" textmode="password"/>
					    <asp:requiredfieldvalidator id="valPassword" resourcekey="valPassword.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtPassword" display="Dynamic"/>
					</td>
				</tr>
				<tr>
					<td class="SubHead" width="150"><dnn:label id="plConfirm" runat="server" controlname="txtConfirm"/></td>
					<td>
					    <asp:textbox id="txtConfirm" cssclass="NormalTextBox" runat="server" width="300" maxlength="20" textmode="password"/>
					    <asp:requiredfieldvalidator id="valConfirm" resourcekey="valConfirm.ErrorMessage" cssclass="Normal" runat="server" controltovalidate="txtConfirm" display="Dynamic"/>
					</td>
				</tr>
		        <tr id="trQuestion" runat="server" height="25" visible="false">
			        <td class="SubHead" width="175"><dnn:label id="plQuestion" runat="server" controlname="lblQuestion"/></td>
			        <td><asp:textbox id="txtQuestion" runat="server" cssclass="NormalTextBox" width="300" maxlength="100" /></td>
		        </tr>
		        <tr id="trAnswer" runat="server" height="25" visible="false">
			        <td class="SubHead" width="175"><dnn:label id="plAnswer" runat="server" controlname="txtAnswer"/></td>
			        <td><asp:textbox id="txtAnswer" runat="server" cssclass="NormalTextBox" width="300" maxlength="100" /></td>
		        </tr>
			</table>
		</td>
	</tr>
</table>
<p>
    <dnn:commandButton class="CommandButton" id="cmdUpdate" ImageUrl="~/images/save.gif" resourcekey="cmdUpdate" runat="server" />&nbsp;&nbsp;
	<dnn:commandButton class="CommandButton" id="cmdCancel" ImageUrl="~/images/lt.gif" resourcekey="cmdCancel" runat="server" causesvalidation="False"/>
</p>
<asp:label id="lblNote" resourcekey="Note" cssclass="Normal" runat="server" />
