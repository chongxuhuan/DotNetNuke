<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.SQL.SQL"
    CodeFile="SQL.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="430" cellspacing="2" cellpadding="2" summary="Load Script Design Table" border="0">
    <tr>
        <td class="SubHead" width="110" valign="top">
            <dnn:Label ID="plSqlScript" runat="server" ControlName="uplSqlScript" Suffix=""></dnn:Label>
        </td>
        <td valign="top">
            <asp:FileUpload ID="uplSqlScript" runat="server" />
        </td>
        <td valign="top">
            <asp:LinkButton ID="cmdUpload" resourcekey="cmdUpload" EnableViewState="False" CssClass="CommandButton"
                runat="server" ToolTip="Load the selected file.">Load</asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="110" valign="top">
            <dnn:Label ID="plConnection" runat="server" ControlName="cboConnection" Text="Connection" Suffix=":"></dnn:Label>
        </td>
        <td colspan="2">
            <asp:DropDownList ID="cboConnection" runat="server" CssClass="NormalTextBox" Width="300" />
        </td>
    </tr>
</table>
<asp:TextBox ID="txtQuery" runat="server" TextMode="MultiLine" Columns="50" Rows="10"
    EnableViewState="False"></asp:TextBox>
<br>
<asp:LinkButton ID="cmdExecute" resourcekey="cmdExecute" EnableViewState="False"
    CssClass="CommandButton" runat="server" ToolTip="can include {directives} and /*comments*/">Execute</asp:LinkButton>&nbsp;&nbsp;
<asp:CheckBox ID="chkRunAsScript" resourcekey="chkRunAsScript" CssClass="SubHead"
    runat="server" Text="Run as Script" TextAlign="Left" ToolTip="include 'GO' directives; for testing &amp; update scripts">
</asp:CheckBox>
<br>
<br>
<asp:Label ID="lblMessage" runat="server" CssClass="NormalRed" EnableViewState="False"></asp:Label>
<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="True" EnableViewState="False">
    <RowStyle CssClass="Normal"></RowStyle>
    <HeaderStyle CssClass="SubHead"></HeaderStyle>
    <EmptyDataTemplate>
        <asp:Label ID="Label1" runat="server" resourcekey="NoDataReturned" />
    </EmptyDataTemplate>
</asp:GridView>
