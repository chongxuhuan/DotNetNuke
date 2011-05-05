<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Tabs.Export" CodeFile="Export.ascx.cs" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table style="width:560px;" cellspacing="1" cellpadding="1" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plFolder" runat="server" controlname="cboFolders" /></td>
        <td><asp:DropDownList ID="cboFolders" Runat="server" CssClass="NormalTextBox" Width="300" /></td>
    </tr>
    <tr>
        <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plFile" runat="server" controlname="txtFile" /></td>
        <td>
            <asp:textbox id="txtFile" cssclass="NormalTextBox" runat="server" maxlength="200" width="300" />
			<br/>
			<asp:requiredfieldvalidator id="valFileName" runat="server" controltovalidate="txtFile" display="Dynamic" resourcekey="valFileName.ErrorMessage"/>
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plDescription" runat="server" controlname="txtDescription" /></td>
        <td>
			<asp:textbox id="txtDescription" runat="server" width="300px" enableviewstate="False" TextMode="MultiLine" Height="150px" />
			<br/>
			<asp:requiredfieldvalidator id="valDescription" runat="server" controltovalidate="txtDescription" display="Dynamic" resourcekey="valDescription.ErrorMessage" />
        </td>
    </tr>
	<tr>
		<td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plContent" runat="server" controlname="chkContent" /></td>
		<td><asp:CheckBox id="chkContent" runat="server" /></td>
	</tr>
</table>
<p>
    <dnn:commandbutton id="cmdExport" resourcekey="cmdExport" runat="server" cssclass="CommandButton" ImageUrl="~/images/save.gif"/>&nbsp;
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
<asp:label id="lblMessage" runat="server" enableviewstate="False" CssClass="Normal" />
