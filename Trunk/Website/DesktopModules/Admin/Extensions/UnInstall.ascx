<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.UnInstall" CodeFile="UnInstall.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnUnInstallExtension dnnClear" id="dnnUnInstallExtension">
    <fieldset>
        <legend></legend>
        <dnn:DnnFormEditor id="packageForm" runat="Server" FormMode="Short">
            <Items>
                <dnn:DnnFormLiteralItem ID="name" runat="server" DataField = "Name" />
                <dnn:DnnFormLiteralItem ID="packageType" runat="server" DataField = "PackageType" />
                <dnn:DnnFormLiteralItem ID="friendlyName" runat="server" DataField = "FriendlyName"/>
                <dnn:DnnFormLiteralItem ID="description" runat="server" DataField = "Description" />
                <dnn:DnnFormLiteralItem ID="version" runat="server" DataField = "Version" />
                <dnn:DnnFormLiteralItem ID="license" runat="server" DataField = "License" />
            </Items>
        </dnn:DnnFormEditor>   
        <div id="deleteRow" runat="server" class="dnnFormItem">
            <dnn:Label ID="plDelete" runat="server" ControlName="rbPackageType" />
            <asp:CheckBox ID="chkDelete" runat="server" />
        </div>    
    </fieldset>
        <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdUninstall" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUninstall" /></li>
        <li><asp:HyperLink id="cmdReturn1" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdReturn" Causesvalidation="False" /></li>
    </ul>
</div>

<asp:Label ID="lblMessage" runat="server" CssClass="Normal" Width="500px" EnableViewState="False" />

<table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table"
    runat="server" visible="False">
    <tr>
        <td>
            <asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle" CssClass="Head" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:PlaceHolder ID="phPaLogs" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <ul class="dnnActions dnnClear">
    	        <li><asp:HyperLink ID="cmdReturn2" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdReturn" /></li>
            </ul>
        </td>
    </tr>
</table>
