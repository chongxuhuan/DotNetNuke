<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.TimeZoneEditor" CodeFile="TimeZoneEditor.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table id="Table1" cellspacing="1" cellpadding="1" border="0">
    <tr>
        <td class="SubHead" valign="top"><dnn:Label ID="lbLocales" runat="server" ControlName="cboLocales" Text="Available Locales" /></td>
        <td valign="top"><asp:DropDownList ID="cboLocales" runat="server" Width="300px" DataTextField="Text" DataValueField="Code" AutoPostBack="True" /></td>
    </tr>
</table>
<asp:Panel ID="pnlMissing" runat="server" Visible="False" Wrap="True">
    <asp:Label ID="lblMissing" runat="server">System Default resource file contains some entries not present in current localized file. This can lead to some values not being translated.</asp:Label>
    <br>
    <asp:LinkButton ID="cmdAddMissing" runat="server" resourcekey="cmdAddMissing" CssClass="CommandButton" CausesValidation="false" />
</asp:Panel>
<asp:DataGrid ID="dgEditor" runat="server" AutoGenerateColumns="False" CellPadding="3"
    CellSpacing="1" GridLines="None">
    <ItemStyle VerticalAlign="Top"></ItemStyle>
    <HeaderStyle Font-Bold="True" CssClass="subSubHead" BackColor="Silver"></HeaderStyle>
    <Columns>
        <asp:TemplateColumn HeaderText="Name">
            <ItemTemplate>
                <asp:TextBox ID="txtName" runat="server" Width="300px" Text='<%# DataBinder.Eval(Container, "DataItem.name") %>'>
                </asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" Display="Dynamic" ErrorMessage="<br>Required Field" resourcekey="RequiredField.ErrorMessage" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="key" ReadOnly="True" HeaderText="Offset">
            <ItemStyle HorizontalAlign="Right" CssClass="Normal"></ItemStyle>
        </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="DefaultValue">
            <ItemTemplate>
                <asp:Label ID="Label2" runat="server" CssClass="Normal"><%# Container.DataItem.Row.GetParentRow("defaultvalues").Item("defaultvalue") %></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<p align="center">
    <dnn:CommandButton ID="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate" ImageUrl="~/images/save.gif" />
    <dnn:CommandButton ID="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" CausesValidation="false" />
</p>
