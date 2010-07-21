<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEnabler" AutoEventWireup="false" Explicit="True" CodeFile="LanguageEnabler.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<dnn:DnnToolTipManager 
        ID="toolTipManager" runat="server" Position="Center"
        RelativeTo="BrowserWindow" Width="500px" Height="200px" 
        HideEvent="ManualClose" ShowEvent="OnClick" Modal="true"
        Skin="Default"  RenderInPageRoot="true" AnimationDuration="200" 
        ManualClose="true" ManualCloseButtonText="Close" />
<table cellpadding="0" cellspacing="0" border="0" width="800px">
    <tr>
        <td>
            <div class="le_label">
                <dnn:Label ID="systemDefaultLabel" runat="server" CssClass="SubHead" />
            </div>
            <div class="le_languages">
                <dnn:DnnLanguageLabel ID="systemDefaultLanguageLabel" runat="server"  />
            </div>
        </td>
    </tr>
    <tr style="height:10px">
        <td />
    </tr>
    <tr>
        <td>
            <div class="le_label">
                <dnn:Label ID="siteDefaultLabel" runat="server" CssClass="SubHead" />
            </div>
            <div class="le_languages">
                <dnn:DnnLanguageLabel ID="defaultLanguageLabel" runat="server"  />
                <dnn:DnnLanguageComboBox ID="languagesComboBox" runat="server" LanguagesListType="Supported" CssClass="NormalTextBox le_languages" Width="250px" />&nbsp;
                <dnn:CommandButton ID="updateButton" runat="server" ResourceKey="Update" ImageUrl="~/images/save.gif" />
            </div>
        </td>
    </tr>
    <tr style="height:25px">
        <td />
    </tr>
    <tr>
        <td>
            <div style="float:left">
                <asp:PlaceHolder ID="enabledPublishedPlaceHolder" runat="server">
                    <asp:Label ID="enabledPublishedLabel" runat="server" CssClass="NormalRed" Text="*" />
                    <asp:Label ID="enabledPublishedMessage" runat="server" CssClass="Normal" ResourceKey="EnabledPublishedMessage"/>
                    <br />
                </asp:PlaceHolder>
                <asp:Label ID="defaultPortalLabel" runat="server" CssClass="NormalRed" Text="**" />
                <asp:Label ID="defaultPortalMessage" runat="server" CssClass="Normal"/>
            </div>
            <div style="vertical-align:top; text-align:right">
                 <dnn:CommandButton ID="enableLocalizedContentButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_language_16px.gif" ResourceKey="EnableLocalization" />
            </div>
            </td>
    </tr>
    <tr style="height:10px">
        <td />
    </tr>
    <tr>
        <td>
            <dnn:DnnGrid id = "languagesGrid" runat="server" AutoGenerateColumns="false" Width="100%" >
                <MasterTableView>
                    <ItemStyle VerticalAlign="Top" />
                    <Columns>
                        <dnn:DnnGridTemplateColumn HeaderText="Culture" ItemStyle-VerticalAlign="Top" ItemStyle-Width="200px" >
                            <ItemTemplate>
                                <dnn:DnnLanguageLabel ID="translationStatusLabel" runat="server" 
                                        Language='<%# Eval("Code") %>' />
                                <asp:Label ID="defaultLanguageMessageLabel" runat="server"
                                        CssClass="NormalRed"
                                        Text="**"
                                        Visible='<%# IsDefaultLanguage(eval("Code")) %>' />
                           </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                        <dnn:DnnGridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="60px">
                            <HeaderTemplate>
                                <%# Localization.GetString("Enabled.Header", Me.LocalResourceFile)%>
                                <asp:Label ID="enabledMessageLabel" runat="server"
                                        CssClass="NormalRed"
                                        Text="*"
                                        Visible='<%# PortalSettings.ContentLocalizationEnabled %>' />
                            </HeaderTemplate>
                            <ItemTemplate>
                                 <dnn:DnnCheckBox ID="enabledCheckbox" runat="server" 
                                    AutoPostBack="True"
                                    CommandArgument='<%# eval("LanguageId") %>'
                                    Enabled='<%# CanEnableDisable(eval("Code")) %>'
                                    Checked='<%# IsLanguageEnabled(eval("Code"))%>'
                                    OnCheckedChanged="enabledCheckbox_CheckChanged" />
                           </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                        <dnn:DnnGridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                            <HeaderTemplate>
                                <%# Localization.GetString("Settings", Me.LocalResourceFile)%>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink ID="editLink" runat="server" 
                                                NavigateUrl='<%# GetEditUrl(Eval("LanguageId")) %>' >
                                    <asp:Image ID="editImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Edit" />
                                </asp:HyperLink>
                            </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                        <dnn:DnnGridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="200px">
                            <HeaderTemplate>
                                 <table cellspacing="0" width="100%" cellpadding="0" border="0">
                                    <tr>
                                        <td colspan="3" style="text-align:center">
                                            <%# Localization.GetString("Static.Header", Me.LocalResourceFile)%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="Td2" style="text-align:center; width: 33%" runat="server" Visible='<%# UserInfo.IsSuperUser %>'>
                                            <%# Localization.GetString("System", Me.LocalResourceFile)%>
                                        </td>
                                        <td id="Td1" style="text-align:center; width: 33%" runat="server" Visible='<%# UserInfo.IsSuperUser %>'>
                                            <%# Localization.GetString("Host", Me.LocalResourceFile)%>
                                        </td>
                                        <td style="text-align:center;">
                                            <%# Localization.GetString("Portal", Me.LocalResourceFile)%>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table cellspacing="0" width="100%" cellpadding="0" style="border:0px">
                                    <tr>
                                        <td id="Td4" style="text-align:center; width: 33%;border-width:0px" runat="server" Visible='<%# UserInfo.IsSuperUser %>'>
                                             <asp:HyperLink ID="editSystemLink" runat="server" 
                                                            NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "System") %>' >
                                                <asp:Image ID="editSystemImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="System.Help" />
                                            </asp:HyperLink>
                                        </td>
                                        <td id="Td3" style="text-align:center; width: 33%;border-width:0px" runat="server" Visible='<%# UserInfo.IsSuperUser %>'>
                                            <asp:HyperLink ID="editHostLink" runat="server" 
                                                            NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "Host") %>' >
                                                <asp:Image ID="editHostImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Host.Help" />
                                            </asp:HyperLink>
                                        </td>
                                        <td style="text-align:center; width: 33%;border-width:0px">
                                            <asp:HyperLink ID="editPortalLink" runat="server" 
                                                            NavigateUrl='<%# GetEditKeysUrl(Eval("Code"), "Portal") %>'>
                                                <asp:Image ID="editPortalImage" runat="server" ImageUrl="~/images/edit.gif" resourcekey="Portal.Help" />
                                            </asp:HyperLink>
                                        </td>
                                    </tr>
                                </table>
                             </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                        <dnn:DnnGridTemplateColumn UniqueName="ContentLocalization" ItemStyle-VerticalAlign="Top" ItemStyle-Width="250px">
                            <HeaderTemplate>
                                 <table cellspacing="0" width="100%" cellpadding="0" border="0">
                                    <tr>
                                        <td colspan="4" style="text-align:center">
                                            <%# Localization.GetString("Content.Header", Me.LocalResourceFile)%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:center; width: 20%">
                                            <asp:Image ID="pagesImage" runat="server" ImageUrl="~/images/icon_language_16px.gif" resourcekey="LocalizePages" />
                                        </td>
                                        <td style="text-align:center; width: 20%">
                                            <asp:Image ID="translatedImage" runat="server" ImageUrl="~/images/translated.gif" resourcekey="TranslatedPages" />
                                        </td>
                                        <td style="text-align:center; width: 20%">
                                            <asp:Image ID="summaryImage" runat="server" ImageUrl="~/images/icon_lists_16px.gif" resourcekey="TranslationSummary" />
                                        </td>
                                        <td style="text-align:center; width: 40%">
                                            <%# Localization.GetString("Published.Header", Me.LocalResourceFile)%>
                                            <asp:Label ID="publishedMessageLabel" runat="server"
                                                    CssClass="NormalRed"
                                                    Text="*"
                                                    Visible='<%# PortalSettings.ContentLocalizationEnabled %>' />
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                 <table cellspacing="0" width="100%" cellpadding="0" border="0">
                                    <tr>
                                        <td style="border-left:0px; text-align:center; vertical-align:top; width: 20%">
                                            <asp:PlaceHolder ID="localizationStatus" runat="server"
                                                 Visible = '<%# IsLocalized(eval("Code")) %>'>
                                                    <span><%# GetLocalizedPages(Eval("Code"))%></span>
                                                    <br />
                                                    <span><%# GetLocalizedStatus(eval("Code")) %></span>
                                                 </asp:PlaceHolder>
                                            <asp:ImageButton ID="localizeButton" runat="server"
                                                ImageAlign="Middle"
                                                ImageUrl="~/images/icon_language_16px.gif"
                                                CommandArgument='<%# eval("Code") %>' 
                                                Visible='<%# Not IsLocalized(eval("Code")) AndAlso CanLocalize(eval("Code")) %>'
                                                ResourceKey="CreateLocalizedPages"
                                                OnCommand="localizePages" />
                                        </td>
                                        <td style="text-align:center; vertical-align:top; width: 20%">
                                            <span><%# GetTranslatedPages(Eval("Code"))%></span>
                                            <br />
                                            <span><%# GetTranslatedStatus(Eval("Code"))%></span>
                                        </td>
                                        <td style="text-align:center; vertical-align:top; width: 20%">
                                            <asp:HyperLink ID="statusLink" runat="server"
                                                           Visible='<%# IsLanguageEnabled(eval("Code")) AndAlso Not IsDefaultLanguage(eval("Code")) AndAlso IsLocalized(eval("Code")) %>' 
                                                           NavigateUrl='<%# NavigateURL(TabId, "TranslationStatus", "mid=" & ModuleId, "locale=" & Eval("Code")) %>' >
                                                <asp:Image ID="statusLinkImage" runat="server" ImageUrl="~/images/icon_lists_16px.gif" resourcekey="TranslationStatus" />
                                            </asp:HyperLink>
 
                                        </td>
                                        <td style="text-align:center; vertical-align:top; width: 40%">
                                             <dnn:DnnCheckBox ID="publishedCheckbox" runat="server" 
                                                AutoPostBack="True"
                                                CommandArgument='<%# eval("LanguageId") %>'
                                                Enabled='<%# IsLanguageEnabled(eval("Code")) AndAlso Not IsDefaultLanguage(eval("Code")) %>'
                                                Checked='<%# IsLanguagePublished(eval("Code")) %>'
                                                OnCheckedChanged="publishedCheckbox_CheckChanged" 
                                                Visible='<%# IsLocalized(eval("Code")) %>'/>
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
    <tr style="height:25px">
        <td />
    </tr>
    <tr>
        <td>
            <dnn:CommandButton ID="languageSettingsButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/settings.gif" ResourceKey="LanguageSettings" CommandName="LanguageSettings" />&nbsp;&nbsp;
            <dnn:CommandButton ID="timeZonesButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_language_16px.gif" ResourceKey="TimeZones" CommandName="TimeZone" />&nbsp;&nbsp;
            <br />
            <dnn:CommandButton ID="addLanguageButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/add.gif" ResourceKey="AddLanguage" CommandName="Edit" />&nbsp;&nbsp;
            <dnn:CommandButton ID="installLanguagePackButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/rt.gif" ResourceKey="InstallLanguage" CommandName="InstallLanguage" />&nbsp;&nbsp;
            <dnn:CommandButton ID="createLanguagePackButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/rt.gif" ResourceKey="CreateLanguage" CommandName="PackageWriter" />&nbsp;&nbsp;
            <dnn:CommandButton ID="verifyLanguageResourcesButton" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_language_16px.gif" ResourceKey="Verify" CommandName="Verify"/>
        </td>
    </tr>
</table>
<br />

