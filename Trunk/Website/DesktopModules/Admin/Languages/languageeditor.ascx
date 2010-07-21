<%@ Control Language="vb" AutoEventWireup="false" CodeFile="LanguageEditor.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<table cellspacing="5" border="0">
    <tr>
        <td valign="top" nowrap="nowrap" width="250px">
            <dnn:Label ID="plResources" runat="server" ControlName="DNNTree" />
            <br />
            <dnn:DnnTreeView ID="resourceFiles" runat="server">
            </dnn:DnnTreeView>
        </td>
        <td style="vertical-align: top; width: 600px">
            <table border="0">
                <tr id="rowMode" runat="server">
                    <td class="SubHead" style="width:200px">
                        <dnn:Label ID="plMode" runat="server" Text="Available Locales" ControlName="cboLocales" />
                    </td>
                    <td style="width:400px">
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
                        <asp:CheckBox ID="chkHighlight" runat="server" resourcekey="Highlight" AutoPostBack="True" TextAlign="Left" />
                    </td>
                </tr>
                <tr height="20px">
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td class="SubHead" style="width:200px">
                        <dnn:Label ID="plEditingLanguage" runat="server" ControlName="lblEditingLanguage" />
                    </td>
                    <td valign="top" style="width:400px">
                        <dnn:DnnLanguageLabel ID="languageLabel" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" style="width:200px">
                        <dnn:Label ID="plFolder" runat="server" ControlName="lblFolder" />
                    </td>
                    <td valign="top" style="width:400px">
                        <asp:Label ID="lblFolder" runat="server" CssClass="NormalBold" />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" style="width:200px">
                        <dnn:Label ID="plSelected" runat="server" ControlName="lblResourceFile" />
                    </td>
                    <td valign="top" style="width:400px">
                        <asp:Label ID="lblResourceFile" runat="server" CssClass="NormalBold" />
                    </td>
                </tr>
            </table>
            <br />
            <dnn:DnnGrid ID="resourcesGrid" runat="server" AutoGenerateColumns="false" 
                Width="600px">
                <MasterTableView>
                    <Columns>
                        <dnn:DnnGridTemplateColumn>
                            <HeaderTemplate>
                                <table cellpadding="0" cellspacing="2" style="width:100%">
                                    <tr>
                                        <td style="width:50%">
                                            <asp:Label ID="Label4" runat="server" CssClass="NormalBold" resourcekey="Value" />
                                        </td>
                                        <td style="width:50%">
                                            <asp:Label ID="Label5" runat="server" CssClass="NormalBold" resourcekey="DefaultValue" />
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="2" style="width:100%">
                                    <tr>
                                        <td colspan="2" style="border-width:0px;background-color:#f7f7f7">
                                            <asp:Label ID="resourceKeyLabel" runat="server" CssClass="NormalBold" resourcekey="ResourceName" />
                                            <asp:Label ID="resourceKey" runat="server" CssClass="Normal" Text='<%# Eval("key") %>' />
                                       </td>
                                    </tr>
                                    <tr>
                                        <td style="border-width:0px;width:50%">
                                            <asp:TextBox ID="txtValue" runat="server" Width="90%" />
                                            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="CommandButton" NavigateUrl='<%# OpenFullEditor(DataBinder.Eval(Container, "DataItem.key")) %>'>
                                                <asp:Image runat="server" AlternateText="Edit" ID="imgEdit" ImageUrl="~/images/edit.gif"
                                                    resourcekey="cmdEdit" Style="vertical-align: top"></asp:Image>
                                            </asp:HyperLink>
                                        </td>
                                        <td style="width:50%">
                                            <asp:TextBox ID="txtDefault" runat="server" Width="90%" Enabled="false" />
                                            <asp:Image runat="server" AlternateText="View" ID="imgView" ImageUrl="~/images/view.gif"
                                                resourcekey="cmdView" Style="vertical-align: top" Visible="false"></asp:Image>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric" />
            </dnn:DnnGrid>
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
