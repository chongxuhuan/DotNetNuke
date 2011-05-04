<%@ Control Language="C#" AutoEventWireup="false" CodeFile="Settings.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Users.ViewProfileSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table cellspacing="0" cellpadding="2" border="0" summary="Edit Links Design Table" style="width: 650px">
    <tr>
        <td class="SubHead" valign="top" style="width: 200px; vertical-align: top;">
            <dnn:Label ID="plTemplate" runat="server" ControlName="txtTemplate" Suffix=":"></dnn:Label>
        </td>
        <td valign="top" style="vertical-align: top">
            <asp:TextBox ID="txtTemplate" CssClass="NormalTextBox" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
        </td>
        <td style="white-space: nowrap; vertical-align: bottom; width: 84px;">
            <asp:LinkButton ID="cmdLoadDefault" runat="server" CausesValidation="False" CssClass="commandButton" resourcekey="LoadDefault" >Load Default</asp:LinkButton></td>
    </tr>
</table>