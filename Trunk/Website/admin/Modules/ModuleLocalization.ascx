<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" CodeFile="ModuleLocalization.ascx.vb" Inherits="DotNetNuke.Admin.Modules.ModuleLocalization" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<dnn:DnnGrid ID="localizedModulesGrid" runat="server"  AutoGenerateColumns="false" AllowRowSelect="false" AllowMultiRowSelection="true">
    <MasterTableView DataKeyNames="ModuleId, TabId">
        <Columns>
            <dnn:DnnGridTemplateColumn UniqueName="CheckBoxTemplateColumn" HeaderStyle-Width="50px">
                <HeaderTemplate>
                    <asp:CheckBox ID="headerCheckBox" runat="server" 
                                OnCheckedChanged="ToggleSelectedState"
                                Visible = '<%# ShowHeaderCheckBox() %>' 
                                AutoPostBack="True" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="rowCheckBox" runat="server" OnCheckedChanged="ToggleRowSelection" AutoPostBack="True" />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn> 
            <dnn:DnnGridTemplateColumn UniqueName="Language" HeaderText="Language" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="200px" >
                <ItemTemplate>
                    <%# If(Eval("IsDefaultLanguage"), "", "&nbsp;&nbsp;&nbsp;&nbsp;")%>
                    <dnn:DnnLanguageLabel ID="moduleLanguageLabel" runat="server" Language='<%# Eval("CultureCode") %>'  />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridBoundColumn HeaderText="ModuleType" DataField="DesktopModule.FriendlyName" ItemStyle-Width="80px" ItemStyle-VerticalAlign="Middle" />
            <dnn:DnnGridBoundColumn HeaderText="ModuleTitle" DataField="ModuleTitle" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Middle"  />
            <dnn:DnnGridTemplateColumn UniqueName="Edit" HeaderText="Edit"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" >
                <ItemTemplate>
                    <a href='<%# NavigateURL(eval("TabId"), Null.NullBoolean, Me.PortalSettings, "Module", Eval("CultureCode"), "ModuleId=" + Eval("ModuleID").ToString()) %>' >
                        <asp:Image ID="editCultureImage" runat="server" ResourceKey="edit" ImageUrl="~/images/edit.gif" />
                    </a>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn UniqueName="IsLocalized" HeaderText="UnBound" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"  >
                <ItemTemplate>
                    <asp:Label ID="defaultLocalizedLabel" runat="server" resourcekey="NA" Visible='<%# eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="localizedImage" runat="server" ImageUrl="~/images/grant.gif" Visible='<%# eval("IsLocalized") And Not eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="notLocalizedImage" runat="server" ImageUrl="~/images/deny.gif" Visible='<%# Not eval("IsLocalized") And Not eval("IsDefaultLanguage")%>' />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
                <dnn:DnnGridTemplateColumn UniqueName="IsTranslated" HeaderText="Translated" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"  >
                <ItemTemplate>
                    <asp:Label ID="defaultTranslatedLabel" runat="server" resourcekey="NA" Visible='<%# eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="translatedImage" runat="server" ImageUrl="~/images/grant.gif" Visible='<%# eval("IsTranslated") And Not eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="notTranslatedImage" runat="server" ImageUrl="~/images/deny.gif" Visible='<%# Not eval("IsTranslated") And Not eval("IsDefaultLanguage")%>' />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
        </Columns>
    </MasterTableView>
</dnn:DnnGrid>
<br />
<asp:PlaceHolder ID="footerPlaceHolder" runat="server">
    <div>
    <table>
    <tr>
    <td><dnn:CommandButton ID="localizeModuleButton" resourcekey="unbindModule" runat="server" CssClass="CommandButton" ImageUrl="~/images/moduleunbind.gif" CausesValidation="False" />&nbsp;&nbsp;&nbsp;</td>
    <td><dnn:CommandButton ID="delocalizeModuleButton" resourcekey="bindModule" runat="server" CssClass="CommandButton" ImageUrl="~/images/modulebind.gif" CausesValidation="False" />&nbsp;&nbsp;&nbsp;</td>
    </tr>
    <tr>
    <td><dnn:CommandButton ID="markModuleTranslatedButton" resourcekey="markModuleTranslated" runat="server" CssClass="CommandButton" ImageUrl="~/images/translate.gif" CausesValidation="False" />&nbsp;&nbsp;&nbsp;</td>
    <td><dnn:CommandButton ID="markModuleUnTranslatedButton" resourcekey="markModuleUnTranslated" runat="server" CssClass="CommandButton" ImageUrl="~/images/untranslate.gif" CausesValidation="False" /></td>
    </tr>
    </table>  
    </div>
</asp:PlaceHolder>

