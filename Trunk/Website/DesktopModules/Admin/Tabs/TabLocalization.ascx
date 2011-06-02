<%@ Control Language="C#" AutoEventWireup="false" Explicit="True" CodeFile="TabLocalization.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Tabs.TabLocalization" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnForm dnnTabLocalization dnnClear">
    <dnnweb:DnnGrid ID="localizedTabsGrid" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="100%" CssClass="dnnTabLocalizationGrid">
        <ClientSettings >
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <MasterTableView DataKeyNames="CultureCode">
            <Columns>
                <dnnweb:DnnGridClientSelectColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" />
                <dnnweb:DnnGridTemplateColumn UniqueName="Language" HeaderText="Language" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="200px" >
                    <ItemTemplate>
                        <dnnweb:DnnLanguageLabel ID="languageLanguageLabel" runat="server" Language='<%# Eval("CultureCode") %>'  />
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridBoundColumn HeaderText="TabName" DataField="TabName" ItemStyle-Width="200px"  />
                <dnnweb:DnnGridTemplateColumn UniqueName="View" HeaderText="View" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" >
                    <ItemTemplate>
                        <asp:PlaceHolder ID="viewPlaceHolder" runat="server" Visible='<%# CanView(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString()) %>'>
                            <a href='<%# DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(Eval("TabId")), Null.NullBoolean, PortalSettings, "", Eval("CultureCode").ToString(), new string[]{}) %>' >
                                <asp:Image ID="viewCultureImage" runat="server" ResourceKey="view" ImageUrl="~/images/view.gif" />
                            </a>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn UniqueName="Edit" HeaderText="Edit" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="editPlaceHolder" runat="server" Visible='<%# CanEdit(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString()) %>'>
                            <a href='<%# DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(Eval("TabId")), Null.NullBoolean, PortalSettings, "Tab", Eval("CultureCode").ToString(), new []{"action=edit"}) %>' >
                                <asp:Image ID="editCultureImage" runat="server" ResourceKey="edit" ImageUrl="~/images/edit.gif" />
                            </a>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn UniqueName="IsTranslated" HeaderText="Translated">
                    <ItemStyle VerticalAlign="Middle" Width="60px" HorizontalAlign="Center"/>
                    <ItemTemplate>
                        <dnnweb:DnnImage ID="translatedImage" runat="server" IconKey="Grant" Visible='<%# Eval("IsTranslated")%>' />
                        <dnnweb:DnnImage ID="notTranslatedImage" runat="server" IconKey="Deny" Visible='<%# !Convert.ToBoolean(Eval("IsTranslated"))%>' />
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                    <HeaderTemplate>
                        <asp:Image ID="totalModulesImage" runat="server" ImageUrl="~/images/total.gif" resourceKey="TotalModules" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span><%# GetTotalModules(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                    <HeaderTemplate>
                        <asp:Image ID="sharedModulesImage" runat="server" ImageUrl="~/images/shared.gif" resourceKey="SharedModules" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span><%# GetSharedModules(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn  HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                    <HeaderTemplate>
                        <asp:Image ID="localizedModulesImage" runat="server" ImageUrl="~/images/moduleUnbind.gif" resourceKey="LocalizedModules" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span><%# GetLocalizedModules(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                        <br />
                        <span><%# GetLocalizedStatus(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
                <dnnweb:DnnGridTemplateColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                    <HeaderTemplate>
                        <asp:Image ID="translatedModulesImage" runat="server" ImageUrl="~/images/translated.gif" resourceKey="TranslatedModules" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span><%# GetTranslatedModules(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                        <br />
                        <span><%# GetTranslatedStatus(Convert.ToInt32(Eval("TabId")), Eval("CultureCode").ToString())%></span>
                    </ItemTemplate>
                </dnnweb:DnnGridTemplateColumn>
            </Columns>
        </MasterTableView>
    </dnnweb:DnnGrid>
    <asp:PlaceHolder ID="footerPlaceHolder" runat="server">
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton ID="markTabTranslatedButton" resourcekey="markTabTranslated" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
            <li><asp:LinkButton ID="markTabUnTranslatedButton" resourcekey="markTabUnTranslated" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
        </ul>
    </asp:PlaceHolder>
</div>