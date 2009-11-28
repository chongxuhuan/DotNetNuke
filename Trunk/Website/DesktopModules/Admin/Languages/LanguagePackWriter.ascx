<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.LanguagePackWriter" CodeFile="LanguagePackWriter.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table>
	<tr>
		<td class="SubHead" valign="middle" width="200px"><dnn:label id="lbLocale" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></td>
		<td valign="top"><asp:dropdownlist id="cboLanguage" runat="server"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td class="SubHead" valign="middle" width="200px"><dnn:label id="lblType" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></td>
		<td valign="top"><asp:radiobuttonlist id="rbPackType" runat="server" AutoPostBack="true" CellSpacing="0" CellPadding="0"
				Repeatdirection="Horizontal" CssClass="Normal">
				<asp:ListItem resourcekey="Core.LangPackType" Value="Core" Selected="true">Core</asp:ListItem>
				<asp:ListItem resourcekey="Module.LangPackType" Value="Module">Module</asp:ListItem>
				<asp:ListItem resourcekey="Provider.LangPackType" Value="Provider">Provider</asp:ListItem>
				<asp:ListItem resourcekey="AuthSystem.LangPackType" Value="AuthSystem">Authentication System</asp:ListItem>
				<asp:ListItem resourcekey="Full.LangPackType" Value="Full">Full</asp:ListItem>
			</asp:radiobuttonlist></td>
	</tr>
	<tr id="rowitems" runat="server">
		<td valign="middle" width="200px"></td>
		<td valign="top">
		    <asp:label id="lblItems" runat="server" CssClass="SubHead"></asp:label><br/>
			<asp:checkboxlist id="lstItems" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
		</td>
	</tr>
	<tr id="rowFileName" runat="server">
		<td class="SubHead" valign="middle" width="200px"><dnn:label id="lblName" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label></td>
		<td valign="top">
			<asp:Label id="Label2" runat="server" CssClass="Normal">ResourcePack.</asp:Label><asp:textbox id="txtFileName" runat="server" Width="200px">Core</asp:textbox>
			<asp:Label id="lblFilenameFix" runat="server" CssClass="Normal">.&lt;version&gt;.&lt;locale&gt;.zip</asp:Label></td>
	</tr>
	<tr>
		<td class="SubHead" valign="middle" width="200px"></td>
		<td valign="top">
	</tr>
</table>
<p align="center">
    <dnn:CommandButton ID="cmdCreate" runat="server" CssClass="CommandButton" resourcekey="cmdCreate" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:CommandButton ID="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" CausesValidation="false" />&nbsp;
</p>
<asp:panel id="pnlLogs" runat="server" Visible="False">
	<dnn:sectionhead id="dshBasic" runat="server" text="Language Pack Log" resourcekey="LogTitle" cssclass="Head"
		includerule="true" section="divLog"></dnn:sectionhead>
	<DIV id="divLog" runat="server">
		<asp:HyperLink id="hypLink" runat="server" CssClass="CommandButton"></asp:HyperLink>
		<HR>
		<asp:Label id="lblMessage" runat="server"></asp:Label></DIV>
</asp:panel>