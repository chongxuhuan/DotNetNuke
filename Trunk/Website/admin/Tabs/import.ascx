<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Tabs.Import" CodeFile="Import.ascx.cs" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table style="width:560px;" cellspacing="2" cellpadding="2" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plFolder" runat="server" controlname="cboFolders" /></td>
        <td><asp:DropDownList ID="cboFolders" Runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="true" /></td>
    </tr>
	<tr>
		<td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plTemplate" runat="server" controlname="cboTemplate" /></td>
		<td style="vertical-align:top;">
		    <asp:dropdownlist id="cboTemplate" cssclass="NormalTextBox" runat="server" width="300" AutoPostBack="True" />
		    <br />
			<asp:RequiredFieldValidator id="valTemplate" runat="server" EnableClientScript="false" Display="Dynamic" ControlToValidate="cboTemplate" InitialValue="-1" resourcekey="valTemplate.ErrorMessage"/>
			<asp:Label id="lblTemplateDescription" runat="server" CssClass="Normal" />
			<br/>
		</td>
	</tr>
	<tr>
		<td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plMode" runat="server" controlname="optMode" /></td>
		<td>
			<asp:radiobuttonlist id="optMode" cssclass="SubHead" runat="server" repeatdirection="Horizontal" repeatlayout="Flow" autopostback="True">
				<asp:listitem value="ADD" resourcekey="ModeAdd" Selected="True" />
				<asp:listitem value="REPLACE" resourcekey="ModeReplace" />
			</asp:radiobuttonlist>
		</td>
	</tr>
	<tr id="trTabName" runat="server" visible="false">
		<td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plTabName" runat="server" controlname="txtTabName" /></td>
		<td><asp:TextBox id="txtTabName" cssclass="NormalTextBox" runat="server" maxlength="50" width="300" /></td>
	</tr>
    <tr id="trParentTabName" runat="server" visible="false">
        <td class="SubHead" width="200">
            <dnn:Label ID="plParentTab" runat="server" ControlName="cboParentTab"></dnn:Label>
        </td>
        <td width="325">
            <asp:DropDownList ID="cboParentTab" CssClass="NormalTextBox" runat="server" Width="300" DataTextField="IndentedTabName" DataValueField="TabId" />
        </td>
    </tr>
    <tr id="trInsertPositionRow" runat="server">
        <td class="SubHead" width="200"><dnn:Label ID="plInsertPosition" runat="server" ResourceKey="InsertPosition" ControlName="cboPositionTab" /></td>
        <td width="325">
            <asp:RadioButtonList ID="rbInsertPosition" runat="server" CssClass="Normal" RepeatDirection="Horizontal" AutoPostBack="true"/>
            <asp:DropDownList ID="cboPositionTab" CssClass="NormalTextBox" runat="server" Width="300" DataTextField="LocalizedTabName" DataValueField="TabId"/>
        </td>
    </tr>
	<tr>
		<td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plRedirect" runat="server" controlname="optRedirect" /></td>
		<td>
			<asp:radiobuttonlist id="optRedirect" cssclass="SubHead" runat="server" repeatdirection="Horizontal" repeatlayout="Flow">
				<asp:listitem value="VIEW" resourcekey="ModeView" Selected="True" />
				<asp:listitem value="SETTINGS" resourcekey="ModeSettings" />
			</asp:radiobuttonlist>
		</td>
	</tr>
</table>
<p>
    <dnn:commandbutton id="cmdImport" resourcekey="cmdImport" runat="server" cssclass="CommandButton" ImageUrl="~/images/rt.gif"/>&nbsp;
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
