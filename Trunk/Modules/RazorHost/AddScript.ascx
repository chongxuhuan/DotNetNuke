<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddScript.ascx.cs" Inherits="DotNetNuke.Modules.RazorHost.AddScript" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table>
	<tr>
		<td colspan="2"><h2><asp:Label ID="headerLabel"  runat="server" resourceKey="header" /></h2></td>
	</tr>
	<tr>
		<td colspan="2" class="Normal"><asp:Label ID="createNewScriptLabel" runat="server" resourceKey="createNewScript" /></td>
	</tr>    
	<tr>
		<td style="width:150px;" class="SubHead"><dnn:Label id="fileTypeLabel" runat="Server"/></td>
		<td style="width:350px;" class="NormalTextBox">
			<asp:RadioButtonList ID="scriptFileType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
				<asp:ListItem Value="CSHTML" ResourceKey="CSHTML" Selected="True" />
				<asp:ListItem Value="VBHTML" ResourceKey="VBHTML" />
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td style="width:150px;" class="SubHead"><dnn:Label id="fileNameLabel" runat="Server"/></td>
		<td style="width:350px;" class="NormalTextBox">
			_
			<asp:TextBox ID="fileName" runat="server" width="150px" />
			<asp:Label ID="fileExtension" runat="server" />
		</td>
	</tr>
</table>
<p style="text-align:center">
	<dnn:commandbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" ImageUrl="~/images/save.gif"/>&nbsp;
	<dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>