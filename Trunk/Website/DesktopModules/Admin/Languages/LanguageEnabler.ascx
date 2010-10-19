<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEnabler"
    AutoEventWireup="false" Explicit="True" CodeFile="LanguageEnabler.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<dnn:DnnToolTipManager ID="toolTipManager" runat="server" Position="Center" RelativeTo="BrowserWindow"
    Width="500px" Height="200px" HideEvent="ManualClose" ShowEvent="OnClick" Modal="true"
    Skin="Default" RenderInPageRoot="true" AnimationDuration="200" ManualClose="true"
    ManualCloseButtonText="Close" />
<table cellpadding="0" cellspacing="0" border="0" width="800px">
    <tr>
        <td>
            <div class="le_label">
                <dnn:Label ID="systemDefaultLabel" runat="server" CssClass="SubHead" />
            </div>
            <div class="le_languages">
                <dnn:DnnLanguageLabel ID="systemDefaultLanguageLabel" runat="server" />
            </div>
        </td>
    </tr>
    <tr style="height: 10px">
        <td />
    </tr>
    <tr>
        <td>
            <div class="le_label">
                <dnn:Label ID="siteDefaultLabel" runat="server" CssClass="SubHead" />
            </div>
            <div class="le_languages">
                <dnn:DnnLanguageLabel ID="defaultLanguageLabel" runat="server" />
                <dnn:DnnLanguageComboBox ID="languagesComboBox" runat="server" LanguagesListType="Supported"
                    CssClass="NormalTextBox le_languages" Width="250px" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="le_label">
                <dnn:Label ID="detectBrowserLable" runat="server" CssClass="SubHead" />
            </div>
            <div class="le_languages">
                <asp:CheckBox ID="chkBrowser" runat="server" />
            </div>
        </td>
    </tr>
    <tr style="height: 10px">
        <td />
        <tr>
            <td>
                <dnn:CommandButton ID="updateButton" runat="server" ResourceKey="Update" ImageUrl="~/images/save.gif" />
            </td>
        </tr>
        <tr style="height: 25px">
            <td />
        </tr>
        <tr>
            <td>
                <div style="float: left">
                    <asp:PlaceHolder ID="enabledPublishedPlaceHolder" runat="server">
                        <asp:Label ID="enabledPublishedLabel" runat="server" CssClass="NormalRed" Text="*" />
                        <asp:Label ID="enabledPublishedMessage" runat="server" CssClass="Normal" ResourceKey="EnabledPublishedMessage" />
                        <br />
                    </asp:PlaceHolder>
                    <asp:Label ID="defaultPortalLabel" runat="server" CssClass="NormalRed" Text="**" />
                    <asp:Label ID="defaultPortalMessage" runat="server" CssClass="Normal" />
                </div>
                <div style="vertical-align: top; text-align: right">
                    <dnn:CommandButton ID="enableLocalizedContentButton" runat="server" CssClass="CommandButton"
                        ImageUrl="~/images/icon_language_16px.gif" ResourceKey="EnableLocalization" />
                </div>
            </td>
        </tr>
        <tr style="height: 10px">
            <td />
        </tr>
        <tr>
            <td>
                <dnn:DnnGrid ID="languagesGrid" runat="server" AutoGenerateColumns="false" Width="900px">
                    <MasterTableView>
                        <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" />
                        <AlternatingItemStyle VerticalAlign="Top" HorizontalAlign="Center" />
                        <HeaderStyle VerticalAlign="Bottom" HorizontalAlign="Center" Wrap="false" />
                        <Columns>
                            <dnn:DnnGridTemplateColumn ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                <HeaderTemplate>
                                    <%# Localization.GetString("Culture.Header", Me.LocalResourceFile)%>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <dnn:DnnLanguageLabel ID="translationStatusLabel" runat="server" Language='<%# Eval("Code") %>' />
                                    <asp:Label ID="defaultLanguageMessageLabel" runat="server" CssClass="NormalRed" Text="**"
                                        Visible='<%# IsDefaultLanguage(eval("Code")) %>' />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn ItemStyle-Width="80px">
                                <HeaderTemplate>
                                    <%# Localization.GetString("Enabled.Header", Me.LocalResourceFile)%>
                                    <asp:Label ID="enabledMessageLabel" runat="server" CssClass="NormalRed" Text="*"
                                        Visible='<%# PortalSettings.ContentLocalizationEnabled %>' />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <dnn:DnnCheckBox ID="enabledCheckbox" runat="server" AutoPostBack="True" CommandArgument='<%# eval("LanguageId") %>'
                                        Enabled='<%# CanEnableDisable(eval("Code")) %>' Checked='<%# IsLanguageEnabled(eval("Code"))%>'
                                        OnCheckedChanged="enabledCheckbox_CheckChanged" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn ItemStyle-Width="80px">
                                <HeaderTemplate>
                                    <%# Localization.GetString("Settings", Me.LocalResourceFile)%>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:HyperLink ID="editLink" runat="server" NavigateUrl='<%# GetEditUrl(Eval("LanguageId")) %>'>
                                        <asp:Image ID="editImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Edit" />
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderStyle-Width="194px">
                                <HeaderTemplate>
                                    <table cellspacing="0" class="DnnGridNestedTable" style="width: 180px;">
                                        <caption>
                                            <%# Localization.GetString("Static.Header", Me.LocalResourceFile) %></caption>
                                        <tbody>
                                            <tr>
                                                <% If UserInfo.IsSuperUser Then%>
                                                <td id="Td2" style="width: 60px;" runat="server">
                                                    <%# Localization.GetString("System", Me.LocalResourceFile)%>
                                                </td>
                                                <td id="Td1" style="width: 60px;" runat="server">
                                                    <%# Localization.GetString("Host", Me.LocalResourceFile)%>
                                                </td>
                                                <td style="width: 60px;">
                                                    <%# Localization.GetString("Portal", Me.LocalResourceFile)%>
                                                </td>
                                                <% Else%>
                                                <td style="width: 180px;">
                                                    <%# Localization.GetString("Portal", Me.LocalResourceFile)%>
                                                </td>
                                                <% End If%>
                                            </tr>
                                        </tbody>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <% If UserInfo.IsSuperUser Then%>
                                    <table cellspacing="0" class="DnnGridNestedTable" style="width: 180px;">
                                        <% Else%>
                                        <table cellspacing="0" class="DnnGridNestedTable" style="width: 60px;">
                                            <% End If%>
                                            <tbody>
                                                <tr>
                                                    <% If UserInfo.IsSuperUser Then%>
                                                    <td id="Td4" style="width: 60px; border-width: 0px">
                                                        <asp:HyperLink ID="editSystemLink" runat="server" NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "System") %>'>
                                                            <asp:Image ID="editSystemImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="System.Help" />
                                                        </asp:HyperLink>
                                                    </td>
                                                    <td id="Td3" style="width: 60px;" runat="server">
                                                        <asp:HyperLink ID="editHostLink" runat="server" NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "Host") %>'>
                                                            <asp:Image ID="editHostImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Host.Help" />
                                                        </asp:HyperLink>
                                                    </td>
                                                    <% End If%>
                                                    <td style="width: 60px;">
                                                        <asp:HyperLink ID="editPortalLink" runat="server" NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "Portal") %>'>
                                                            <asp:Image ID="editPortalImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Portal.Help" />
                                                        </asp:HyperLink>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn UniqueName="ContentLocalization" HeaderStyle-Width="204px">
                                <HeaderTemplate>
                                    <table cellspacing="0" class="DnnGridNestedTable" style="width: 190px;">
                                        <caption>
                                            <%# Localization.GetString("Content.Header", Me.LocalResourceFile)%></caption>
                                        <tr>
                                            <td style="width: 50px;">
                                                <asp:Image ID="pagesImage" runat="server" ImageUrl="~/images/icon_language_16px.gif"
                                                    resourcekey="LocalizePages" />
                                            </td>
                                            <td style="width: 50px;">
                                                <asp:Image ID="translatedImage" runat="server" ImageUrl="~/images/translated.gif"
                                                    resourcekey="TranslatedPages" />
                                            </td>
                                            <td style="width: 90px;">
                                                <%# Localization.GetString("Published.Header", Me.LocalResourceFile)%>
                                                <asp:Label ID="publishedMessageLabel" runat="server" CssClass="NormalRed" Text="*"
                                                    Visible='<%# PortalSettings.ContentLocalizationEnabled %>' />
                                            </td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellspacing="0" class="DnnGridNestedTable" style="width: 190px;">
                                        <tr>
                                            <td style="width: 50px; border-width: 0px">
                                                <asp:PlaceHolder ID="localizationStatus" runat="server" Visible='<%# IsLocalized(eval("Code")) %>'>
                                                    <span>
                                                        <%# GetLocalizedPages(Eval("Code"))%></span>
                                                    <br />
                                                    <span style="font-size: 0.8em">
                                                        <%# GetLocalizedStatus(eval("Code")) %></span> </asp:PlaceHolder>
                                                <asp:ImageButton ID="localizeButton" runat="server" ImageAlign="Middle" ImageUrl="~/images/icon_language_16px.gif"
                                                    CommandArgument='<%# eval("Code") %>' Visible='<%# Not IsLocalized(eval("Code")) AndAlso CanLocalize(eval("Code")) %>'
                                                    ResourceKey="CreateLocalizedPages" OnCommand="localizePages" />
                                            </td>
                                            <td style="width: 50px;">
                                                <span>
                                                    <%# GetTranslatedPages(Eval("Code"))%></span>
                                                <br />
                                                <span style="font-size: 0.8em">
                                                    <%# GetTranslatedStatus(Eval("Code"))%></span>
                                            </td>
                                            <td style="width: 90px;">
                                                <dnn:DnnCheckBox ID="publishedCheckbox" runat="server" AutoPostBack="True" CommandArgument='<%# eval("LanguageId") %>'
                                                    Enabled='<%# IsLanguageEnabled(eval("Code")) AndAlso Not IsDefaultLanguage(eval("Code")) %>'
                                                    Checked='<%# IsLanguagePublished(eval("Code")) %>' OnCheckedChanged="publishedCheckbox_CheckChanged"
                                                    Visible='<%# IsLocalized(eval("Code")) %>' />
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </dnn:DnnGrid>
            </td>
        </tr>
        <tr style="height: 25px">
            <td />
        </tr>
        <tr>
            <td>
                <dnn:CommandButton ID="addLanguageButton" runat="server" CssClass="CommandButton"
                    ImageUrl="~/images/add.gif" ResourceKey="AddLanguage" CommandName="Edit" />
                &nbsp;&nbsp;
                <dnn:CommandButton ID="installLanguagePackButton" runat="server" CssClass="CommandButton"
                    ImageUrl="~/images/rt.gif" ResourceKey="InstallLanguage" CommandName="InstallLanguage" />
                &nbsp;&nbsp;
                <dnn:CommandButton ID="createLanguagePackButton" runat="server" CssClass="CommandButton"
                    ImageUrl="~/images/rt.gif" ResourceKey="CreateLanguage" CommandName="PackageWriter" />
                <br />
                <dnn:CommandButton ID="verifyLanguageResourcesButton" runat="server" CssClass="CommandButton"
                    ImageUrl="~/images/icon_language_16px.gif" ResourceKey="Verify" CommandName="Verify" />
                &nbsp;&nbsp;
                <dnn:CommandButton ID="timeZonesButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_language_16px.gif"
                    ResourceKey="TimeZones" CommandName="TimeZone" />
            </td>
        </tr>
</table>
<br />
