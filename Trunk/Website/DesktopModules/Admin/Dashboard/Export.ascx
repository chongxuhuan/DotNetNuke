<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Dashboard.Export" CodeFile="Export.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<br/>
<table width="560" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" valign="top" width="150"><dnn:label id="plFileName" runat="server" controlname="txtFileName" /></td>
        <td><asp:textbox id="txtFileName" cssclass="NormalTextBox" runat="server" maxlength="200" width="300" /></td>
    </tr>
</table>
<p>
    <dnn:commandbutton id="cmdSave" resourcekey="cmdSave" runat="server" cssclass="CommandButton" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
