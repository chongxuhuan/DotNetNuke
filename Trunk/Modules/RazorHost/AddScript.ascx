<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddScript.ascx.cs" Inherits="DotNetNuke.Modules.RazorHost.AddScript" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnRazorHostAddScript dnnClear">
	<h2><asp:Label ID="headerLabel"  runat="server" resourceKey="header" /></h2>
	<p><asp:Label ID="createNewScriptLabel" runat="server" resourceKey="createNewScript" /></p>
	<div class="dnnFormItem">
        <dnn:Label id="fileTypeLabel" runat="Server" />
        <asp:RadioButtonList ID="scriptFileType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CssClass="dnnFormRadioButtons">
            <asp:ListItem Value="CSHTML" ResourceKey="CSHTML" Selected="True" />
            <asp:ListItem Value="VBHTML" ResourceKey="VBHTML" />
        </asp:RadioButtonList>
	</div>
    <div class="dnnFormItem">
		<dnn:Label id="fileNameLabel" runat="Server"/>
        <asp:TextBox ID="fileName" runat="server" />
        <asp:Label ID="fileExtension" runat="server" />
	</div>
    <ul class="dnnActions dnnClear">
        <li><dnn:commandbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="dnnPrimaryAction" ImageUrl="~/images/save.gif" /></li>
        <li><dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="dnnSecondaryAction" ImageUrl="~/images/lt.gif" causesvalidation="False" /></li>
    </ul>
</div>