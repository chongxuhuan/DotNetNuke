<%@ Control Inherits="DotNetNuke.Modules.Admin.Portals.SiteSettings" Language="vb" AutoEventWireup="false" Explicit="True" EnableViewState="True" CodeFile="SiteSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="PortalAliases" Src="~/DesktopModules/Admin/Portals/PortalAliases.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<!-- Settings Tables -->
<table class="Settings" cellspacing="2" cellpadding="2" width="760" summary="Site Settings Design Table" border="0">
    <tr>
        <td style="vertical-align:top; width:725px">
            <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" CssClass="Head" runat="server" Text="Basic Settings" Section="tblBasic" ResourceKey="BasicSettings" IncludeRule="True" />
                <table id="tblBasic" cellspacing="0" cellpadding="2" style="width:100%" summary="Basic Settings Design Table" border="0" runat="server">
                    <tr>
                        <td colspan="2"><asp:Label ID="lblBasicSettingsHelp" CssClass="Normal" runat="server" resourcekey="BasicSettingsHelp" EnableViewState="False" /></td>
                    </tr>
                    <tr>
                        <td width="25"></td>
                        <td style="vertical-align:top; width:700px">
                            <dnn:SectionHead ID="dshSite" CssClass="Head" runat="server" Text="Site Details" Section="tblSite" ResourceKey="SiteDetails"/>
                            <table id="tblSite" cellspacing="2" cellpadding="2" summary="Site Details Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPortalName" runat="server" Text="Title:" ControlName="txtPortalName" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
                                        <asp:TextBox ID="txtPortalName" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="128" />
                                        <asp:RequiredFieldValidator ID="valPortalName" CssClass="NormalRed" runat="server" resourcekey="valPortalName.ErrorMessage" Display="Dynamic" ControlToValidate="txtPortalName"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plDescription" runat="server" Text="Description:" ControlName="txtDescription" /></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" style="width:450px" xLength="475" Rows="3" TextMode="MultiLine"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plKeyWords" runat="server" Text="Key Words:" ControlName="txtKeyWords"/></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:TextBox ID="txtKeyWords" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="475" Rows="3" TextMode="MultiLine"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plFooterText" runat="server" Text="Copyright:" ControlName="txtFooterText" /></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:TextBox ID="txtFooterText" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="100"/></td>
                                </tr>
							    <tr>
								    <td class="SubHead" style="width:250px"><dnn:label id="plGUID" text="GUID:" controlname="lblGUID" runat="server" /></td>
								    <td><asp:Label ID="lblGUID" Runat="server" CssClass="NormalBold"/></td>
							    </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshMarketing" CssClass="Head" runat="server" Text="Marketing" Section="tblMarketing" ResourceKey="Marketing" IsExpanded="True" />
                            <table id="tblMarketing" cellspacing="2" cellpadding="2" summary="Marketing Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plSearchEngine" runat="server" ControlName="cboSearchEngine"/></td>
                                    <td style="width:450px">
                                        <asp:DropDownList ID="cboSearchEngine" runat="server" CssClass="NormalTextBox" Width="250">
                                            <asp:ListItem>Google</asp:ListItem>
                                            <asp:ListItem>Yahoo</asp:ListItem>
                                            <asp:ListItem>Microsoft</asp:ListItem>
                                        </asp:DropDownList>&nbsp;
                                        <asp:LinkButton CssClass="CommandButton" ID="cmdSearchEngine" resourcekey="cmdSearchEngine" runat="server" Text="Submit"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plSiteMap" runat="server" ControlName="txtSiteMap" /></td>
                                    <td style="width:450px">
                                        <asp:TextBox ID="txtSiteMap" runat="server" CssClass="NormalTextBox" ReadOnly="true" Width="250"/>&nbsp;
                                        <asp:LinkButton CssClass="CommandButton" ID="cmdSiteMap" resourcekey="cmdSiteMap" runat="server" Text="Submit"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plVerification" runat="server" ControlName="txtVerification" /></td>
                                    <td style="width:450px">
                                        <asp:TextBox ID="txtVerification" runat="server" CssClass="NormalTextBox" Width="250"/>&nbsp;
                                        <asp:LinkButton CssClass="CommandButton" ID="cmdVerification" resourcekey="cmdVerification" runat="server" Text="Create" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plBanners" runat="server" ControlName="optBanners" /></td>
                                    <td class="NormalTextBox" style="width:450px" nowrap>
                                        <asp:RadioButtonList ID="optBanners" CssClass="Normal" runat="server" EnableViewState="False" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0" resourcekey="None">None</asp:ListItem>
                                            <asp:ListItem Value="1" resourcekey="Site">Site</asp:ListItem>
                                            <asp:ListItem Value="2" resourcekey="Host">Host</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:Label ID="lblBanners" runat="server" resourcekey="lblBanners" CssClass="normal" />
                                    </td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshAppearance" CssClass="Head" runat="server" Text="Appearance" Section="tblAppearance" ResourceKey="Appearance" IsExpanded="False"/>
                            <table id="tblAppearance" cellspacing="2" cellpadding="2" summary="Appearance Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plLogo" runat="server" ControlName="ctlLogo" /></td>
                                    <td style="width:450px"><dnn:DnnFilePicker ID="ctlLogo" runat="server" style="width:450px" Required="False" ShowSecure="True" ShowDatabase="False" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plBackground" runat="server" Text="Body Background:" ControlName="cboBackground"/></td>
                                    <td style="width:450px"><dnn:DnnFilePicker ID="ctlBackground" runat="server" style="width:450px" Required="False" ShowSecure="True" ShowDatabase="False"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSkinWidgestEnabled" runat="server" ControlName="chkSkinWidgestEnabled" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:CheckBox ID="chkSkinWidgestEnabled" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPortalSkin" runat="server" Text="Portal Skin:" ControlName="ctlPortalSkin" /></td>
                                    <td valign="top" style="width:450px"><dnn:Skin ID="ctlPortalSkin" runat="server" DefaultKey="Application" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPortalContainer" runat="server" Text="Portal Container:" ControlName="ctlPortalContainer" /></td>
                                    <td valign="top" style="width:450px"><dnn:Skin ID="ctlPortalContainer" runat="server" DefaultKey="Application" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plAdminSkin" runat="server" Text="Admin Skin:" ControlName="ctlAdminSkin"/></td>
                                    <td valign="top" style="width:450px"><dnn:Skin ID="ctlAdminSkin" runat="server" DefaultKey="Application" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plAdminContainer" runat="server" Text="Admin Container:" ControlName="ctlAdminContainer"/></td>
                                    <td valign="top" style="width:450px"><dnn:Skin ID="ctlAdminContainer" runat="server" DefaultKey="Application" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br />
                <dnn:SectionHead ID="dshAdvanced" CssClass="Head" runat="server" Text="Advanced Settings" Section="tblAdvanced" ResourceKey="AdvancedSettings" IncludeRule="True" IsExpanded="False" />
                <table id="tblAdvanced" cellspacing="0" cellpadding="2" style="width:100%" summary="Advanced Settings Design Table" border="0" runat="server">
                    <tr>
                        <td colspan="2"><asp:Label ID="lblAdvancedSettingsHelp" CssClass="Normal" runat="server" resourcekey="AdvancedSettingsHelp" EnableViewState="False" /></td>
                    </tr>
                    <tr>
                        <td width="25"></td>
                        <td style="vertical-align:top; width:600px">
                            <dnn:SectionHead ID="dshSecurity" CssClass="Head" runat="server" Text="Security Settings" Section="tblSecurity" ResourceKey="SecuritySettings" />
                            <table id="tblSecurity" cellspacing="2" cellpadding="2" summary="Security Settings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plUserRegistration" runat="server" ControlName="optUserRegistration" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
                                        <asp:RadioButtonList ID="optUserRegistration" CssClass="Normal" runat="server" EnableViewState="False" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="0" resourcekey="None" />
                                            <asp:ListItem Value="1" resourcekey="Private" />
                                            <asp:ListItem Value="2" resourcekey="Public" />
                                            <asp:ListItem Value="3" resourcekey="Verified" />
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshPages" CssClass="Head" runat="server" Text="Page Management" Section="tblPages" ResourceKey="Pages" />
                            <table id="tblPages" cellspacing="2" cellpadding="2" summary="Page Management Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSplashTabId" runat="server" ControlName="cboSplashTabId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboSplashTabId" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="IndentedTabName" DataValueField="TabId" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plHomeTabId" runat="server" ControlName="cboHomeTabId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboHomeTabId" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="IndentedTabName" DataValueField="TabId" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plLoginTabId" runat="server" ControlName="cboLoginTabId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboLoginTabId" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="IndentedTabName" DataValueField="TabId"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plRegisterTabId" runat="server" ControlName="cboRegisterTabId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboRegisterTabId" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="IndentedTabName" DataValueField="TabId" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plUserTabId" runat="server" ControlName="cboUserTabId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboUserTabId" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="IndentedTabName" DataValueField="TabId" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plHomeDirectory" runat="server" ControlName="lblHomeDirectory" /></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:Label ID="lblHomeDirectory" CssClass="NormalTextBox" runat="server" /></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshPayment" CssClass="Head" runat="server" Text="Payment Settings" Section="tblPayment" ResourceKey="Payment" IsExpanded="False" />
                            <table id="tblPayment" cellspacing="2" cellpadding="2" summary="Payment Setttings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plCurrency" runat="server" ControlName="cboCurrency" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:DropDownList ID="cboCurrency" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="text" DataValueField="value" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px"><dnn:Label ID="plProcessor" runat="server" ControlName="cboProcessor" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
                                        <asp:DropDownList ID="cboProcessor" CssClass="NormalTextBox" runat="server" style="width:450px" DataTextField="value" DataValueField="text" />
                                        <br />
                                        <asp:LinkButton ID="cmdProcessor" CssClass="CommandButton" runat="server" resourcekey="ProcessorWebSite" EnableViewState="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plUserId" runat="server" ControlName="txtUserId" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:TextBox ID="txtUserId" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="50" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPassword" runat="server" ControlName="txtPassword" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:TextBox ID="txtPassword" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="50" TextMode="Password" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPayPaylReturnURL" runat="server" ControlName="txtPayPaylReturnURL" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:TextBox ID="txtPayPalReturnURL" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="255" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPayPaylCancelURL" runat="server" ControlName="txtPayPaylCancelURL" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:TextBox ID="txtPayPalCancelURL" CssClass="NormalTextBox" runat="server" style="width:450px" MaxLength="255" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPayPalSandboxEnabled" runat="server" ControlName="chkPayPalSandboxEnabled" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:CheckBox ID="chkPayPalSandboxEnabled" runat="server" /></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshUsability" CssClass="Head" runat="server" Text="Usability Settings" Section="tblUsability" ResourceKey="Usability" IsExpanded="False" />
                            <table id="tblUsability" cellspacing="2" cellpadding="2" summary="Usability Setttings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plInlineEditor" runat="server" ControlName="chkInlineEditor" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:CheckBox ID="chkInlineEditor" runat="server" CssClass="Normal" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plHideSystemFolders" runat="server" ControlName="chkHideSystemFolders" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px"><asp:CheckBox ID="chkHideSystemFolders" runat="server" CssClass="Normal" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plControlPanelMode" runat="server" ControlName="optControlPanelMode" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
						                <asp:radiobuttonlist id="optControlPanelMode" cssclass="Normal" runat="server" repeatdirection="Horizontal" repeatlayout="Flow">
							                <asp:listitem value="VIEW" resourcekey="ControlPanelModeView" />
							                <asp:listitem value="EDIT" resourcekey="ControlPanelModeEdit" />
						                </asp:radiobuttonlist>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plControlPanelVisibility" runat="server" ControlName="optControlPanelVisibility" /></td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
						                <asp:radiobuttonlist id="optControlPanelVisibility" cssclass="Normal" runat="server" repeatdirection="Horizontal" repeatlayout="Flow">
							                <asp:listitem value="MIN" resourcekey="ControlPanelVisibilityMinimized" />
							                <asp:listitem value="MAX" resourcekey="ControlPanelVisibilityMaximized" />
						                </asp:radiobuttonlist>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plControlPanelSecurity" runat="server" ControlName="optControlPanelSecurity" />
                                    </td>
                                    <td class="NormalTextBox" valign="top" style="width:450px">
						                <asp:radiobuttonlist id="optControlPanelSecurity" cssclass="Normal" runat="server" repeatdirection="Horizontal" repeatlayout="Flow">
							                <asp:listitem value="TAB" resourcekey="ControlPanelSecurityTab" />
							                <asp:listitem value="MODULE" resourcekey="ControlPanelSecurityModule" />
						                </asp:radiobuttonlist>
			                        </td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshOther" CssClass="Head" runat="server" Text="Other Settings" Section="tblOther" ResourceKey="Other" IsExpanded="False" />
                            <table id="tblOther" cellspacing="2" cellpadding="2" summary="Other Setttings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plAdministrator" runat="server" ControlName="cboAdministratorId" /></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:DropDownList ID="cboAdministratorId" CssClass="NormalTextBox" runat="server" Width="300" DataTextField="FullName" DataValueField="UserId" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plDefaultLanguage" runat="server" ControlName="cboDefaultLanguage" /></td>
                                    <td style="width:450px"><asp:DropDownList ID="cboDefaultLanguage" CssClass="NormalTextBox" runat="server" Width="300" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plTimeZone" runat="server" ControlName="cboTimeZone" /></td>
                                    <td style="width:450px"><asp:DropDownList ID="cboTimeZone" CssClass="NormalTextBox" runat="server" Width="300" /></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshAliases" CssClass="Head" runat="server" Text="Portal Aliases" Section="tblAliases" ResourceKey="PortalAliases" IsExpanded="False" />
                            <table id="tblAliases" cellspacing="2" cellpadding="2" summary="Portal Aliases Setttings Design Table" border="0" runat="server">
                                <tr>
                                    <td style="width:50px"></td>
                                    <td><dnn:PortalAliases id="portalAliases" runat="server" /></td>
                                </tr>
                            </table>
                            <br/>
                            <dnn:SectionHead ID="dshSSL" CssClass="Head" runat="server" Text="SSL Settings" Section="tblSSL" ResourceKey="SSLSettings" IsExpanded="False" />
                            <table id="tblSSL" cellspacing="2" cellpadding="2" summary="SSL Settings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSSLEnabled" runat="server" ControlName="chkSSLEnabled" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:CheckBox ID="chkSSLEnabled" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSSLEnforced" runat="server" ControlName="chkSSLEnforced" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:CheckBox ID="chkSSLEnforced" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSSLURL" runat="server" ControlName="txtSSLURL" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:TextBox ID="txtSSLURL" CssClass="NormalTextBox" runat="server" style="width:450px" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSTDURL" runat="server" ControlName="txtSTDURL" /></td>
                                    <td class="NormalTextBox" valign="top"><asp:TextBox ID="txtSTDURL" CssClass="NormalTextBox" runat="server" style="width:450px" /></td>
                                </tr>
                           </table>
                            <br/>
                            <dnn:SectionHead ID="dshHost" CssClass="Head" runat="server" Text="Host Settings" Section="tblHost" ResourceKey="HostSettings" IsExpanded="False" />
                            <table id="tblHost" cellspacing="2" cellpadding="2" summary="Host Settings Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plExpiryDate" runat="server" ControlName="txtExpiryDate" /></td>
                                    <td class="NormalTextBox" style="width:450px">
                                        <asp:TextBox ID="txtExpiryDate" CssClass="NormalTextBox" runat="server" style="width:250px" MaxLength="150" />
                                        <asp:HyperLink ID="cmdExpiryCalendar" CssClass="CommandButton" runat="server" resourcekey="Calendar" />
                                        <asp:CompareValidator ID="valExpiryDate" CssClass="NormalRed" runat="server" ControlToValidate="txtExpiryDate" ErrorMessage="<br>Invalid expiry date!" Operator="DataTypeCheck" Type="Date" Display="Dynamic" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plHostFee" runat="server" Text="Hosting Fee:" ControlName="txtHostFee" /></td>
                                    <td class="NormalTextBox" style="width:450px">
								        <asp:textbox id="txtHostFee" cssclass="NormalTextBox" runat="server" maxlength="10" width="100" />
                                        <asp:CompareValidator ID="valHostFee" runat="server" ControlToValidate="txtHostFee" CssClass="NormalRed" Display="Dynamic" ResourceKey="valHostFee.Error" Operator="DataTypeCheck" Type="Currency" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plHostSpace" runat="server" ControlName="txtHostSpace"/></td>
                                    <td class="NormalTextBox" width="100"><asp:TextBox ID="txtHostSpace" CssClass="NormalTextBox" runat="server" MaxLength="6" Width="300" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plPageQuota" runat="server" ControlName="txtPageQuota"/></td>
                                    <td class="NormalTextBox" width="100"><asp:TextBox ID="txtPageQuota" CssClass="NormalTextBox" runat="server" MaxLength="6" Width="300" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plUserQuota" runat="server" ControlName="txtUserQuota"/></td>
                                    <td class="NormalTextBox" width="100"><asp:TextBox ID="txtUserQuota" CssClass="NormalTextBox" runat="server" MaxLength="6" Width="300" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="width:250px"><dnn:Label ID="plSiteLogHistory" runat="server" ControlName="txtSiteLogHistory"/></td>
                                    <td class="NormalTextBox" style="width:450px"><asp:TextBox ID="txtSiteLogHistory" CssClass="NormalTextBox" runat="server" Width="300" MaxLength="3" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" style="width:250px">
                                        <dnn:Label ID="plDesktopModules" runat="server" Text="Premium Modules:" ControlName="ctlDesktopModules"/>
                                    </td>
                                    <td class="NormalTextBox" style="width:450px">
                                        <dnn:DualListBox id="ctlDesktopModules" runat="server" DataValueField="DesktopModuleID" DataTextField="FriendlyName" 
                                            AddKey="AddModule" RemoveKey="RemoveModule" AddAllKey="AddAllModules" RemoveAllKey="RemoveAllModules"
                                            AddImageURL="~/images/rt.gif" AddAllImageURL="~/images/ffwd.gif" RemoveImageURL="~/images/lt.gif" 
                                            RemoveAllImageURL="~/images/frev.gif" >
                                            <AvailableListBoxStyle CssClass="NormalTextBox" Height="130px" Width="130px" />
                                            <HeaderStyle CssClass="NormalBold" />
                                            <SelectedListBoxStyle CssClass="NormalTextBox" Height="130px" Width="130px"  />
                                        </dnn:DualListBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshStylesheet" CssClass="Head" runat="server" Text="Stylesheet Editor"
                    Section="tblStylesheet" ResourceKey="StylesheetEditor" IncludeRule="True" IsExpanded="False">
                </dnn:SectionHead>
                <table id="tblStylesheet" cellspacing="0" cellpadding="2" style="width:100%" summary="Stylesheet Editor Design Table"
                    border="0" runat="server">
                    <tr>
                        <td style="vertical-align:top; width:600px">
                            <asp:TextBox ID="txtStyleSheet" CssClass="NormalTextBox" runat="server" Rows="20"
                                TextMode="MultiLine" Wrap="False" Columns="100"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="cmdSave" CssClass="CommandButton" runat="server" resourcekey="SaveStyleSheet"
                                EnableViewState="False">Save Style Sheet</asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton ID="cmdRestore" CssClass="CommandButton" runat="server" resourcekey="RestoreDefaultStyleSheet"
                                EnableViewState="False">Restore Default Style Sheet</asp:LinkButton></td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<p>
	<dnn:commandbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate" ImageUrl="~/images/save.gif" />&nbsp;
	<dnn:commandbutton id="cmdDelete" runat="server" CssClass="CommandButton" resourcekey="cmdDelete" ImageUrl="~/images/delete.gif" Causesvalidation="False" />&nbsp;
    <dnn:commandbutton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel" ImageUrl="~/images/lt.gif" CausesValidation="False" />
</p>
        <dnn:audit id="ctlAudit" runat="server" />