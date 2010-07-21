<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" CodeFile="TabLocalization.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Tabs.TabLocalization" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<dnn:DnnGrid ID="localizedTabsGrid" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="100%">
    <ClientSettings >
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <MasterTableView DataKeyNames="CultureCode">
        <Columns>
            <dnn:DnnGridClientSelectColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" />
            <dnn:DnnGridTemplateColumn UniqueName="Language" HeaderText="Language" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="200px" >
                <ItemTemplate>
                    <dnn:DnnLanguageLabel ID="languageLanguageLabel" runat="server" Language='<%# Eval("CultureCode") %>'  />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridBoundColumn HeaderText="TabName" DataField="TabName" ItemStyle-Width="200px"  />
            <dnn:DnnGridTemplateColumn UniqueName="View" HeaderText="View" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" >
                <ItemTemplate>
                    <asp:PlaceHolder ID="viewPlaceHolder" runat="server" Visible='<%# CanView(Eval("TabId"), Eval("CultureCode")) %>'>
                        <a href='<%# NavigateURL(Eval("TabId"), Null.NullBoolean, Me.PortalSettings, "", Eval("CultureCode"), Nothing) %>' >
                            <asp:Image ID="viewCultureImage" runat="server" ResourceKey="view" ImageUrl="~/images/view.gif" />
                        </a>
                    </asp:PlaceHolder>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn UniqueName="Edit" HeaderText="Edit" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:PlaceHolder ID="editPlaceHolder" runat="server" Visible='<%# CanEdit(Eval("TabId"), Eval("CultureCode")) %>'>
                        <a href='<%# NavigateURL(Eval("TabId"), Null.NullBoolean, Me.PortalSettings, "Tab", Eval("CultureCode"), "action=edit") %>' >
                            <asp:Image ID="editCultureImage" runat="server" ResourceKey="edit" ImageUrl="~/images/edit.gif" />
                        </a>
                    </asp:PlaceHolder>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn UniqueName="IsTranslated" HeaderText="Translated">
                <ItemStyle VerticalAlign="Middle" Width="60px" HorizontalAlign="Center"/>
                <ItemTemplate>
                    <asp:Image ID="translatedImage" runat="server" ImageUrl="~/images/grant.gif" Visible='<%# eval("IsTranslated")%>' />
                    <asp:Image ID="notTranslatedImage" runat="server" ImageUrl="~/images/deny.gif" Visible='<%# Not eval("IsTranslated")%>' />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="totalModulesImage" runat="server" ImageUrl="~/images/total.gif" resourceKey="TotalModules" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span><%# GetTotalModules(Eval("TabId"), Eval("CultureCode"))%></span>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="sharedModulesImage" runat="server" ImageUrl="~/images/shared.gif" resourceKey="SharedModules" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span><%# GetSharedModules(Eval("TabId"), Eval("CultureCode"))%></span>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="localizedModulesImage" runat="server" ImageUrl="~/images/moduleUnbind.gif" resourceKey="LocalizedModules" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span><%# GetLocalizedModules(Eval("TabId"), Eval("CultureCode"))%></span>
                    <br />
                    <span><%# GetLocalizedStatus(Eval("TabId"), Eval("CultureCode"))%></span>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="translatedModulesImage" runat="server" ImageUrl="~/images/translated.gif" resourceKey="TranslatedModules" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span><%# GetTranslatedModules(Eval("TabId"), Eval("CultureCode"))%></span>
                    <br />
                    <span><%# GetTranslatedStatus(Eval("TabId"), Eval("CultureCode"))%></span>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
        </Columns>
    </MasterTableView>
</dnn:DnnGrid>
<br />
<asp:PlaceHolder ID="footerPlaceHolder" runat="server">
    <div>
        <dnn:CommandButton ID="markTabTranslatedButton" resourcekey="markTabTranslated" runat="server" CssClass="CommandButton" ImageUrl="~/images/grant.gif" CausesValidation="False" />&nbsp;&nbsp;&nbsp;
        <dnn:CommandButton ID="markTabUnTranslatedButton" resourcekey="markTabUnTranslated" runat="server" CssClass="CommandButton" ImageUrl="~/images/deny.gif" CausesValidation="False" />
    </div>
</asp:PlaceHolder>
