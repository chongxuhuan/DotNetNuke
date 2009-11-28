<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.EditAffiliate" CodeFile="EditAffiliate.ascx.vb" %>
<table cellspacing="2" cellpadding="0" width="560">
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":"></dnn:label></td>
		<td width="325">
			<asp:textbox id="txtStartDate" runat="server" cssclass="NormalTextBox" width="120" columns="30"
				maxlength="11"></asp:textbox>&nbsp;
			<asp:hyperlink id="cmdStartCalendar" resourcekey="Calendar" cssclass="CommandButton" runat="server">Calendar</asp:hyperlink>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":"></dnn:label></td>
		<td width="325">
			<asp:textbox id="txtEndDate" runat="server" cssclass="NormalTextBox" width="120" columns="30"
				maxlength="11"></asp:textbox>&nbsp;
			<asp:hyperlink id="cmdEndCalendar" resourcekey="Calendar" cssclass="CommandButton" runat="server">Calendar</asp:hyperlink>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plCPC" runat="server" controlname="txtCPC" suffix=":"></dnn:label></td>
		<td width="325">
			<asp:textbox id="txtCPC" runat="server" maxlength="7" columns="30" width="100" cssclass="NormalTextBox"></asp:textbox>
			<asp:requiredfieldvalidator id="valCPC1" resourcekey="CPC.ErrorMessage" runat="server" controltovalidate="txtCPC"
				errormessage="You Must Enter a Valid CPC" display="Dynamic" cssclass="NormalRed"></asp:requiredfieldvalidator>
			<asp:comparevalidator id="valCPC2" resourcekey="CPC.ErrorMessage" runat="server" controltovalidate="txtCPC"
				errormessage="You Must Enter a Valid CPC" display="Dynamic" type="Double" operator="DataTypeCheck"
				cssclass="NormalRed"></asp:comparevalidator>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="200"><dnn:label id="plCPA" runat="server" controlname="txtCPA" suffix=":"></dnn:label></td>
		<td width="325">
			<asp:textbox id="txtCPA" runat="server" maxlength="7" columns="30" width="100" cssclass="NormalTextBox"></asp:textbox>
			<asp:requiredfieldvalidator id="valCPA1" resourcekey="CPA.ErrorMessage" runat="server" controltovalidate="txtCPA"
				errormessage="You Must Enter a Valid CPA" display="Dynamic" cssclass="NormalRed"></asp:requiredfieldvalidator>
			<asp:comparevalidator id="valCPA2" resourcekey="CPA.ErrorMessage" runat="server" controltovalidate="txtCPA"
				errormessage="You Must Enter a Valid CPA" display="Dynamic" type="Double" operator="DataTypeCheck"
				cssclass="NormalRed"></asp:comparevalidator>
		</td>
	</tr>
</table>
<asp:label id="lblOptional" resourcekey="Optional" class="SubHead" runat="server">* = Optional</asp:label>
<p>
	<asp:linkbutton class="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"
		borderstyle="none"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" text="Delete"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>
	<asp:linkbutton class="CommandButton" id="cmdSend" resourcekey="cmdSend" runat="server" text="Send Notification"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>
</p>
