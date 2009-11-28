<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.SiteLog.SiteLog" language="vb" AutoEventWireup="false" Explicit="True" CodeFile="SiteLog.ascx.vb" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Site Log Design Table" border="0" width="450">
	<tr vAlign="top">
    <td class="SubHead" width="150"><dnn:label id="plReportType" runat="server" controlname="cboReportType" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:DropDownList ID="cboReportType" Runat="server" DataValueField="value" DataTextField="text" CssClass="NormalTextBox"></asp:DropDownList>
		</td>
	</tr>
	<tr>
    <td class="SubHead" width="150"><dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:TextBox id="txtStartDate" CssClass="NormalTextBox" runat="server" width="120" Columns="20"></asp:TextBox>&nbsp;
			<asp:HyperLink id="cmdStartCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton">Calendar</asp:HyperLink>
		</td>
	</tr>
	<tr>
    <td class="SubHead" width="150"><dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":"></dnn:label></td>
		<td class="NormalBold" align="left" width="325">
			<asp:TextBox id="txtEndDate" CssClass="NormalTextBox" runat="server" width="120" Columns="20"></asp:TextBox>&nbsp;
			<asp:HyperLink id="cmdEndCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton">Calendar</asp:HyperLink>
		</td>
	</tr>
	<tr>
		<td class="NormalBold" vAlign="top" align="center" colspan="2">
			<asp:LinkButton id="cmdDisplay" resourcekey="cmdDisplay" cssclass="CommandButton" Text="Display" runat="server"  />&nbsp;&nbsp;
			<asp:LinkButton id="cmdCancel" resourcekey="cmdCancel" CssClass="CommandButton" runat="server">Cancel</asp:LinkButton>
		</td>
	</tr>
</table>
<asp:comparevalidator id="valStartDate" cssclass="NormalRed" runat="server" resourcekey="valStartDate" display="Dynamic" type="Date" operator="DataTypeCheck" controltovalidate="txtStartDate"></asp:comparevalidator>
<asp:comparevalidator id="valEndDate" cssclass="NormalRed" runat="server" resourcekey="valEndDate" display="Dynamic" type="Date" operator="DataTypeCheck" controltovalidate="txtEndDate"></asp:comparevalidator>
<asp:comparevalidator id="valDates" cssclass="NormalRed" runat="server" resourcekey="valDates" display="Dynamic" type="Date" operator="GreaterThan" controltovalidate="txtEndDate" controltocompare="txtStartDate"></asp:comparevalidator>
<br/>
<asp:Label ID="lblMessage" runat="server" Visible="false" resourcekey="NoRecords" CssClass="NormalRed" />
<asp:datagrid id="grdLog" Width="750" Runat="server" Border="0" CellPadding="4" CellSpacing="4" 
    AutoGenerateColumns="true" HeaderStyle-CssClass="NormalBold" 
    ItemStyle-CssClass="Normal" HeaderStyle-HorizontalAlign="Center" 
    ItemStyle-HorizontalAlign="Center" BorderStyle="None" 
    BorderWidth="0px" GridLines="None">
</asp:datagrid>
