<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Host.RequestFilters" CodeFile="RequestFilters.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div id="lblErr" class="NormalRed" runat="server" visible="false"></div>
<asp:DataList ID="rptRules" runat="server" CssClass="dnnRequestFilter">
	<HeaderStyle CssClass="" />
    <FooterStyle CssClass="" />
    <ItemTemplate>
        <table width="100%">
            <tr>
                <td rowspan="5" valign="top">
                    <asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" ImageUrl="~/images/edit.gif" />
                    <asp:ImageButton ID="cmdDelete" runat="server" CommandName="Delete" ImageUrl="~/images/delete.gif" />
                </td>
                <td width="188"><dnn:label id="plServerVar" runat="server" controlname="lblServerVar" suffix=":"></dnn:label></td>
                <td ><asp:label runat="server" Text='<%#Eval("ServerVariable") %>' ID="lblServerVar" Width="250px" /></td>
            </tr>
            <tr>
                <td><dnn:label id="plOperator" runat="server" controlname="lblOperator" suffix=":"></dnn:label></td>
                <td><asp:label runat="server" Text='<%#Eval("Operator") %>' ID="lblOperator" Width="250px" /></td>
            </tr>
            <tr>
                <td><dnn:label id="plValue" runat="server" controlname="lblValue" suffix=":"></dnn:label></td>
                <td><asp:label runat="server" Text='<%#Eval("RawValue") %>' ID="lblValue" Width="250px" /></td>
            </tr>
            <tr>
                <td><dnn:label id="plAction" runat="server" controlname="lblAction" suffix=":"></dnn:label></td>
                <td><asp:label runat="server" Text='<%#Eval("Action") %>' ID="lblAction" Width="250px" /></td>
            </tr>
            <tr>
                <td><dnn:label id="plLocation" runat="server" controlname="lblLocation" suffix=":"></dnn:label></td>
                <td><asp:label runat="server" Text='<%#Eval("Location") %>' ID="lblLocation" Width="250px" /></td>
            </tr>
        </table>
    </ItemTemplate>
    <EditItemTemplate>
        <table width="100%">
            <tr>
                <td colspan="3"><div class="dnnFormMessage dnnFormWarning"><asp:Label ID="lblWarning" runat="server" Text="Simple warning" resourcekey="lblWarning"></asp:Label></div></td>
            </tr>
            <tr>
                <td rowspan="5" valign="top">
                    <asp:ImageButton ID="cmdSave" runat="server" CommandName="Update" ImageUrl="~/images/save.gif" CssClass="dnnPrimaryAction" />
                    <asp:ImageButton ID="cmdDelete" runat="server" CommandName="Cancel" ImageUrl="~/images/delete.gif" CssClass="dnnSecondaryAction" />
                </td>
                <td width="188"><dnn:label id="plServerVar" runat="server" controlname="txtServerVar" suffix=":"></dnn:label></td>
                <td>
                    <asp:TextBox ID="txtServerVar" runat="server" Text='<%#Eval("ServerVariable") %>' Width="250px" />
                    <asp:Label ID="lblServerVarLink" runat="server" text="Simple Link" resourcekey="lblServerVarLink"></asp:Label>
                </td>
            </tr>
            <tr>
                <td><dnn:label id="plOperator" runat="server" controlname="ddlOperator" suffix=":"></dnn:label></td>
                <td>
                    <asp:DropDownList ID="ddlOperator" runat="server">
                        <asp:ListItem>Equal</asp:ListItem>
                        <asp:ListItem>NotEqual</asp:ListItem>
                        <asp:ListItem>Regex</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td><dnn:label id="plValue" runat="server" controlname="txtValue" suffix=":"></dnn:label></td>
                <td><asp:TextBox ID="txtValue" runat="server" Text='<%#Eval("RawValue") %>' Width="250px" /></td>
            </tr>
            <tr>
                <td><dnn:label id="plAction" runat="server" controlname="ddlAction" suffix=":"></dnn:label></td>
                <td>
                    <asp:DropDownList ID="ddlAction" runat="server">
                        <asp:ListItem>Redirect</asp:ListItem>
                        <asp:ListItem>PermanentRedirect</asp:ListItem>
                        <asp:ListItem>NotFound</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td><dnn:label id="plLocation" runat="server" controlname="txtLocation" suffix=":"></dnn:label></td>
                <td><asp:TextBox ID="txtLocation" runat="server" Text='<%#Eval("Location") %>' Width="250px" /></td>
            </tr>
        </table>
    </EditItemTemplate>
    <SeparatorTemplate><div></div></SeparatorTemplate>
</asp:DataList>
<ul class="dnnActions rfAddRule dnnClear"><li><asp:LinkButton ID="cmdAddRule" runat="server" resourcekey="cmdAdd" CssClass="dnnPrimaryAction" /></li></ul>