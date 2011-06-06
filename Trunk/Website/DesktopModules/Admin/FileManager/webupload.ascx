<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.FileManager.WebUpload" CodeFile="WebUpload.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnFileUpload dnnClear" id="dnnFileUpload">
    <fieldset>
        <legend></legend>
        <div id="rootRow" runat="server" visible="false" class="dnnFormItem">
            <dnn:Label ID="lblRootType" runat="server" />
            <asp:Label ID="lblRootFolder" runat="server"  />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plBrowse" runat="server" ControlName="cmdBrowse" />
            <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server" />&nbsp;&nbsp;
            <asp:LinkButton ID="cmdAdd" runat="server" CssClass="dnnSecondaryAction"  />
        </div>
        <div id="foldersRow" runat="server" visible="false" class="dnnFormItem">
            <dnn:Label ID="plFolder" runat="server" ControlName="ddlFolders" />
            <asp:DropDownList ID="ddlFolders" runat="server" />
        </div>
        <div id="unzipRow" runat="server" visible="false" class="dnnFormItem">
            <dnn:Label ID="Label1" runat="server" ControlName="chkUnzip" resourcekey="Decompress" />
            <asp:CheckBox ID="chkUnzip" runat="server"  />
        </div>
        <div class="dnnFormItem">
            <asp:Label ID="lblMessage" runat="server" EnableViewState="False" />
        </div>
    </fieldset>
    <div class="dnnFormMessage dnnFormInfo">
        <asp:Label id="maxSizeWarningLabel" runat="server" />
    </div>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdReturn1" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdReturn" /></li>
    </ul>
</div>
<table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table"
    runat="server" visible="False">
    <tr><td><asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle" /></td></tr>
    <tr><td>&nbsp;</td></tr>
    <tr><td><asp:PlaceHolder ID="phPaLogs" runat="server" /></td></tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td>
            <ul class="dnnActions dnnClear">
    	        <li><asp:LinkButton id="cmdReturn2" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdReturn" /></li>
            </ul>
        </td>
    </tr>
</table>
