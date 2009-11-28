<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.UnInstall" CodeFile="UnInstall.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Web Upload Design Table">
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlUpload" Visible="True" CssClass="WorkPanel" runat="server">
                <dnn:propertyeditorcontrol id="ctlPackage" runat="Server"
                    AutoGenerate="false"
                    SortMode="SortOrderAttribute"
                    editcontrolstyle-cssclass="NormalTextBox" 
                    editcontrolwidth="350px" 
                    ErrorStyle-cssclass="NormalRed"
                    helpstyle-cssclass="Help" 
                    labelstyle-cssclass="SubHead" 
                    labelwidth="150px" 
                    width="500px">
                        <Fields>
                            <dnn:FieldEditorControl ID="fldName" runat="server" DataField="Name" />
                            <dnn:FieldEditorControl ID="fldPackageType" runat="server" DataField="PackageType" />
                            <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" />
                            <dnn:FieldEditorControl ID="fldDescription" runat="server" DataField="Description" />
                            <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" EditorTypeName="DotNetNuke.UI.WebControls.VersionEditControl" />
                            <dnn:FieldEditorControl ID="fldLicense" runat="server" DataField="License" EditorTypeName="DotNetNuke.UI.WebControls.DNNRichTextEditControl" />
                        </Fields>
                </dnn:propertyeditorcontrol>
                <br />
                <p id="pButtons" style="text-align:center" runat="server">
                    <dnn:CommandButton ID="cmdReturn1" runat="server" CssClass="CommandButton" imageurl="~/images/lt.gif" resourcekey="cmdReturn"/>
                    <dnn:CommandButton ID="cmdUninstall" runat="server" CssClass="CommandButton" imageurl="~/images/delete.gif" resourcekey="cmdUninstall" />
                    <asp:checkbox id="chkDelete" resourcekey="chkDelete" runat="server" Text="Delete Files?" TextAlign="Right" CssClass="SubHead" />
                </p>
                <asp:Label ID="lblMessage" runat="server" CssClass="Normal" Width="500px" EnableViewState="False" />
                <br />
                <br />
                <table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table" runat="server" visible="False">
                    <tr><td><asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle" CssClass="Head" /></td></tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr><td><asp:PlaceHolder ID="phPaLogs" runat="server" /></td></tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr><td><dnn:CommandButton ID="cmdReturn2" runat="server" CssClass="CommandButton" imageurl="~/images/lt.gif" resourcekey="cmdReturn"/></td></tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
