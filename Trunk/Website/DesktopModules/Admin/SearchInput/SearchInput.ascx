<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.SearchInput.SearchInput" CodeFile="SearchInput.ascx.vb" %>
<table cellspacing="0" cellpadding="4" summary="Search Input Table" border="0">
    <tr>
        <td nowrap="nowrap">
            <dnn:label id="plSearch" runat="server" controlname="cboModule" />
            <asp:image id="imgSearch" runat="server" />
        </td>
        <td><asp:textbox id="txtSearch" runat="server" Wrap="False" Width="150px" columns="35" maxlength="200" cssclass="NormalTextBox"/></td>
        <td><asp:imagebutton id="imgGo" runat="server"/><asp:Button id="cmdGo" runat="server" Text="Go"  ResourceKey="cmdGo" CssClass="StandardButton"/></td>
    </tr>
</table>
