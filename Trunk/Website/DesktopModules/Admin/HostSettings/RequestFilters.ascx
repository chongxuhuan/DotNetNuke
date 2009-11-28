<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RequestFilters.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Host.RequestFilters" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div id="lblErr" class="NormalRed" style="text-align: left; font-size=0.7em;" runat="server" visible="false"></div>
<asp:DataList ID="rptRules" runat="server">
    <HeaderTemplate><hr /></HeaderTemplate>
    <FooterTemplate><hr /></FooterTemplate>
    <ItemTemplate>
    <table style="border-collapse:collapse;" width="100%">
        <tr>
            <td rowspan="5" valign="top">
                <asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" ImageUrl="~/images/edit.gif" />
                <asp:ImageButton ID="cmdDelete" runat="server" CommandName="Delete" ImageUrl="~/images/delete.gif" />&nbsp;&nbsp;&nbsp;
            </td>
            <td class="SubHead" width="120">
                <dnn:label id="plServerVar" runat="server" controlname="lblServerVar" suffix=":"></dnn:label>
            </td>
            <td >
                <asp:label runat="server" Text='<%#Eval("ServerVariable") %>' CssClass="Normal" ID="lblServerVar" Width="250px"/>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plOperator" runat="server" controlname="lblOperator" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:label runat="server" Text='<%#Eval("Operator") %>' CssClass="Normal" ID="lblOperator" Width="250px"/>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plValue" runat="server" controlname="lblValue" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:label runat="server" Text='<%#Eval("RawValue") %>' CssClass="Normal" ID="lblValue" Width="250px"/>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plAction" runat="server" controlname="lblAction" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:label runat="server" Text='<%#Eval("Action") %>' CssClass="Normal" ID="lblAction" Width="250px"/>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plLocation" runat="server" controlname="lblLocation" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:label runat="server" Text='<%#Eval("Location") %>' CssClass="Normal" ID="lblLocation" Width="250px"/>
            </td>
        </tr>
    </table>
    </ItemTemplate>
    <EditItemTemplate>
    <table style="border-collapse:collapse;" width="100%">
        <tr>
            <td colspan="3" style="padding-bottom: 15px;">
                <asp:Image ID="imgWarning" runat="server" ImageUrl="~/images/icon_viewstats_16px.gif"/>&nbsp; <asp:Label ID="lblWarning" runat="server" Text="Simple warning" resourcekey="lblWarning"></asp:Label>
            </td>
        </tr>
        <tr>
            <td rowspan="5" valign="top" nowrap="nowrap">
                <asp:ImageButton ID="cmdSave" runat="server" CommandName="Update" ImageUrl="~/images/save.gif" />
                <asp:ImageButton ID="cmdDelete" runat="server" CommandName="Cancel" ImageUrl="~/images/delete.gif" />&nbsp;&nbsp;&nbsp;
            </td>
            <td class="SubHead" width="120" nowrap="nowrap" valign="top">
                <dnn:label id="plServerVar" runat="server" controlname="txtServerVar" suffix=":"></dnn:label>
            </td>
            <td  style="padding-bottom: 10px;">
                <asp:TextBox ID="txtServerVar" runat="server" Text='<%#Eval("ServerVariable") %>' CssClass="Normal" Width="250px" />
                <br />
                <asp:Label ID="lblServerVarLink" class="small" runat="server" text="Simple Link" resourcekey="lblServerVarLink"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plOperator" runat="server" controlname="ddlOperator" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:DropDownList ID="ddlOperator" runat="server">
                    <asp:ListItem>Equal</asp:ListItem>
                    <asp:ListItem>NotEqual</asp:ListItem>
                    <asp:ListItem>Regex</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plValue" runat="server" controlname="txtValue" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:TextBox ID="txtValue" runat="server" Text='<%#Eval("RawValue") %>' CssClass="Normal" Width="250px" />
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plAction" runat="server" controlname="ddlAction" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:DropDownList ID="ddlAction" runat="server">
                    <asp:ListItem>Redirect</asp:ListItem>
                    <asp:ListItem>PermanentRedirect</asp:ListItem>
                    <asp:ListItem>NotFound</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="SubHead" valign="top">
                <dnn:label id="plLocation" runat="server" controlname="txtLocation" suffix=":"></dnn:label>
            </td>
            <td valign="top">
                <asp:TextBox ID="txtLocation" runat="server" Text='<%#Eval("Location") %>' CssClass="Normal" Width="250px" />
            </td>
        </tr>
    </table>
    </EditItemTemplate>
    <SeparatorTemplate>
        <hr />
    </SeparatorTemplate>
</asp:DataList>
<dnn:CommandButton ID="cmdAddRule" runat="server" ResourceKey="cmdAdd" ImageUrl="~/images/add.gif" />
