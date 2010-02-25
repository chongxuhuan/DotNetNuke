<%@ Control Language="vb" AutoEventWireup="false" CodeFile="LanguageEditor.ascx.vb"
    Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<table cellspacing="5" border="0">
    <tr>
        <td valign="top" nowrap="nowrap" width="200px">
            <dnn:Label ID="plResources" runat="server" ControlName="DNNTree" />
            <br />
            <dnn:DnnTree ID="DNNTree" runat="server" DefaultNodeCssClassOver="Normal" CssClass="Normal"
                DefaultNodeCssClass="Normal" />
        </td>
        <td style="vertical-align: top; width: 99%">
            <table border="0">
                <tr id="rowMode" runat="server">
                    <td class="SubHead">
                        <dnn:Label ID="plMode" runat="server" Text="Available Locales" ControlName="cboLocales" />
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rbMode" runat="server" CssClass="Normal" AutoPostBack="True"
                            RepeatColumns="3" RepeatDirection="Horizontal">
                            <asp:ListItem resourcekey="ModeSystem" Value="System" Selected="True" />
                            <asp:ListItem resourcekey="ModeHost" Value="Host" />
                            <asp:ListItem resourcekey="ModePortal" Value="Portal" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" colspan="2">
                        <asp:CheckBox ID="chkHighlight" runat="server" resourcekey="Highlight" AutoPostBack="True"
                            TextAlign="Left" />
                    </td>
                </tr>
                <tr height="20px">
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <dnn:Label ID="plEditingLanguage" runat="server" ControlName="lblEditingLanguage" />
                    </td>
                    <td valign="top">
                        <asp:Label ID="lblEditingLanguage" runat="server"  CssClass="NormalBold" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <dnn:Label ID="plSelected" runat="server" ControlName="lblResourceFile" />
                    </td>
                    <td valign="top">
                        <asp:Label ID="lblResourceFile" runat="server" CssClass="NormalBold" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:DataGrid ID="dgEditor" runat="server" CssClass="Normal" GridLines="None" CellPadding="2"
                CellSpacing="0" AutoGenerateColumns="False" AllowPaging="true" AllowCustomPaging="false"
                ShowHeader="true" ShowFooter="false" PagerStyle-Visible="false" Width="100%">
                <HeaderStyle Wrap="False" CssClass="NormalBold" BackColor="#f1f6f9" Height="25px" />
                <ItemStyle VerticalAlign="Top" />
                <Columns>
                    <asp:BoundColumn Visible="False" DataField="key"></asp:BoundColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate>
                            <asp:Label ID="Label4" runat="server" CssClass="NormalBold" resourcekey="Value" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="txtValue" Width="90%" runat="server" TextMode="MultiLine" Height="30px"></asp:TextBox>
                            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="CommandButton" NavigateUrl='<%# OpenFullEditor(DataBinder.Eval(Container, "DataItem.key")) %>'>
                                <asp:Image runat="server" AlternateText="Edit" ID="imgEdit" ImageUrl="~/images/edit.gif"
                                    resourcekey="cmdEdit" Style="vertical-align: top"></asp:Image>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate>
                            <asp:Label ID="Label5" runat="server" CssClass="NormalBold" resourcekey="DefaultValue"
                                Font-Bold="True" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefault" Width="90%" runat="server" TextMode="MultiLine" Height="30px"
                                ReadOnly="true" BorderColor="#f7f7f7" BorderStyle="solid" BorderWidth="1px"></asp:TextBox>
                            <asp:Image runat="server" AlternateText="View" ID="imgView" ImageUrl="~/images/view.gif"
                                resourcekey="cmdView" Style="vertical-align: top" Visible="false"></asp:Image>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <dnn:PagingControl ID="ctlPagingControl" runat="server" Mode="PostBack" />
            <br />
            <br />
            <p style="text-align: center">
                <dnn:CommandButton ID="cmdUpdate" runat="server" CssClass="CommandButton" ResourceKey="cmdUpdate"
                    ImageUrl="~/images/save.gif" />
                &nbsp;&nbsp;
                <dnn:CommandButton ID="cmdDelete" runat="server" CssClass="CommandButton" ResourceKey="cmdDelete"
                    ImageUrl="~/images/delete.gif" CausesValidation="false" />
                &nbsp;&nbsp;
                <dnn:CommandButton ID="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" ResourceKey="cmdCancel" CausesValidation="false" />
            </p>
            <br />
        </td>
    </tr>
</table>
