<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.FileManager.WebUpload" CodeFile="WebUpload.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Web Upload Design Table">
    <tr>
        <td valign="top">
            <asp:Panel ID="pnlUpload" Visible="True" CssClass="WorkPanel" runat="server">
                <table class="Settings" id="tblUpload" cellspacing="2" cellpadding="2" summary="Web Upload Design Table" runat="server">
                    <tr id="trRoot" runat="server" visible="false">
                        <td width="100"><asp:Label ID="lblRootType" runat="server" CssClass="SubHead" /></td>
                        <td width="550"><asp:Label ID="lblRootFolder" runat="server" CssClass="Normal" /></td>
                    </tr>
                    <tr><td colspan="2">&nbsp;</td></tr>
                    <tr>
                        <td align="left" colspan="2">
                            <label style="display: none" for="<%=cmdBrowse.ClientID%>">Browse Files</label>
                            <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server">&nbsp;&nbsp;
                            <dnn:CommandButton ID="cmdAdd" runat="server" CssClass="CommandButton" imageurl="~/images/save.gif" />
                        </td>
                    </tr>
                    <tr id="trFolders" runat="server" visible="false">
                        <td align="left" colspan="2">
                            <asp:DropDownList ID="ddlFolders" runat="server" Width="525px">
                                <asp:ListItem Value="Root">Root</asp:ListItem>
                                <asp:ListItem Value="Files\">Files</asp:ListItem>
                                <asp:ListItem Value="Files\SubFolder\">Files\SubFolder</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="trUnzip" runat="server" visible="false">
                        <td colspan="2"><asp:CheckBox ID="chkUnzip" runat="server" CssClass="Normal" TextAlign="Right" resourcekey="Decompress" /></td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2"><asp:Label ID="lblMessage" runat="server" CssClass="Normal" Width="500px" EnableViewState="False" /></td>
                    </tr>
                </table>
                <br />
                <dnn:CommandButton ID="cmdReturn1" runat="server" CssClass="CommandButton" imageurl="~/images/lt.gif" resourcekey="cmdReturn"/>
                <br />
                <table id="tblLogs" cellspacing="0" cellpadding="0" summary="Resource Upload Logs Table"
                    runat="server" visible="False">
                    <tr><td><asp:Label ID="lblLogTitle" runat="server" resourcekey="LogTitle" /></td></tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr><td><asp:PlaceHolder ID="phPaLogs" runat="server" /></td></tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr><td><dnn:CommandButton ID="cmdReturn2" runat="server" CssClass="CommandButton" imageurl="~/images/lt.gif" resourcekey="cmdReturn"/></td></tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
