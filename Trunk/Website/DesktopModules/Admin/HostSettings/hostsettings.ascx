<%@ Control Inherits="DotNetNuke.Modules.Admin.Host.HostSettings" Language="vb" AutoEventWireup="false" Explicit="True" CodeFile="HostSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="FriendlyUrls" Src="~/DesktopModules/Admin/HostSettings/FriendlyUrls.ascx" %>
<%@ Register TagPrefix="dnn" TagName="RequestFilters" Src="~/DesktopModules/Admin/HostSettings/RequestFilters.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<!-- Settings Tables -->
<asp:ValidationSummary ID="valSummary" runat="server" CssClass="NormalRed" EnableClientScript="true" DisplayMode="BulletList" />
<table class="Settings" cellspacing="2" cellpadding="2" style="width: 100%" summary="Host Settings Design Table"
    border="0">
    <tr>
        <td style="vertical-align: top;">
            <dnn:SectionHead ID="dshBasic" runat="server" CssClass="Head" Text="Basic Settings"
                Section="tblBasic" ResourceKey="BasicSettings" IncludeRule="True" IsExpanded="True" />
            <table id="tblBasic" runat="server" cellspacing="0" cellpadding="2" style="width: 100%"
                summary="Basic Settings Design Table" border="0">
                <tr>
                    <td colspan="2"><asp:Label ID="lblBasicSettingsHelp" CssClass="Normal" runat="server" resourcekey="BasicSettingsHelp" EnableViewState="False" /></td>
                </tr>
                <tr>
                    <td width="25">
                    </td>
                    <td style="vertical-align: top;">
                        <dnn:SectionHead ID="dshConfiguration" runat="server" CssClass="Head" Text="Configuration"
                            Section="tblConfiguration" ResourceKey="Configuration" />
                        <table id="tblConfiguration" cellspacing="2" cellpadding="2" summary="Configuration Design Table"
                            border="0" runat="server">
							<tr>
								<td class="SubHead" width="150"><dnn:label id="plProduct" text="DotNetNuke Product:" controlname="lblProduct" runat="server" /></td>
								<td><asp:Label ID="lblProduct" Runat="server" CssClass="NormalBold" /></td>
							</tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plVersion" Text="DotNetNuke Version:" ControlName="lblVersion" runat="server" /></td>
                                <td><asp:Label ID="lblVersion" runat="server" CssClass="NormalBold" /></td>
                            </tr>
                            <tr id="trBeta" runat="server">
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plBetaNotice" Text="Display Beta Notice?" ControlName="chkBetaNotice" runat="server" /></td>
                                <td><asp:CheckBox ID="chkBetaNotice" runat="server" CssClass="NormalTextBox" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plUpgrade" Text="Check For Upgrades?" ControlName="chkUpgrade" runat="server" /></td>
                                <td><asp:CheckBox ID="chkUpgrade" runat="server" CssClass="NormalTextBox" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plAvailable" Text="Upgrade Available?" runat="server" /></td>
                                <td><asp:HyperLink ID="hypUpgrade" Target="_new" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plDataProvider" Text="Data Provider:" ControlName="lblDataProvider"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblDataProvider" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plFramework" Text=".NET Framework:" ControlName="lblFramework" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblFramework" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plIdentity" Text="ASP.NET Identity:" ControlName="lblIdentity" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblIdentity" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostName" Text="Host Name:" ControlName="lblHostName" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblHostName" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plIPAddress" Text="IP Address:" ControlName="lblIPAddress" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblIPAddress" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plPermissions" Text="Permissions:" ControlName="lblPermissions" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblPermissions" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plApplicationPath" Text="Relative Path:" ControlName="lblApplicationPath"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblApplicationPath" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plApplicationMapPath" Text="Physical Path:" ControlName="lblApplicationMapPath"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblApplicationMapPath" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plServerTime" Text="Server Time:" ControlName="lblServerTime" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblServerTime" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plGUID" Text="GUID:" ControlName="lblGUID" runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="lblGUID" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plWebFarm" Text="Web Farm Enabled?" ControlName="chkWebFarm" runat="server" />
                                </td>
                                <td>
                                    <input id="chkWebFarm" type="checkbox" runat="server" class="NormalBold" disabled="disabled" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshHost" runat="server" CssClass="Head" Text="Host Details"
                            Section="tblHost" ResourceKey="HostDetails" />
                        <table id="tblHost" cellspacing="2" cellpadding="2" summary="Site Details Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plHostPortal" ControlName="cboHostPortal" runat="server" /></td>
                                <td><asp:DropDownList ID="cboHostPortal" CssClass="NormalTextBox" DataTextField="PortalName" DataValueField="PortalID" Width="300" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plHostTitle" ControlName="txtHostTitle" runat="server" /></td>
                                <td><asp:TextBox ID="txtHostTitle" CssClass="NormalTextBox" runat="server" MaxLength="256" Width="300" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plHostURL" ControlName="txtHostURL" runat="server" /></td>
                                <td><asp:TextBox ID="txtHostURL" CssClass="NormalTextBox" runat="server" MaxLength="256" Width="300" /></td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px"><dnn:Label ID="plHostEmail" Text="Host Email:" ControlName="txtHostEmail" runat="server" /></td>
                                <td>
                                    <asp:TextBox ID="txtHostEmail" CssClass="NormalTextBox" runat="server" MaxLength="256" Width="300" />
                                    <asp:RegularExpressionValidator ID="valHostEmail" runat="server" ControlToValidate="txtHostEmail" Display="Dynamic" ResourceKey="HostEmail.Error" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" width="150">
                                    <dnn:Label ID="plHostDefaultDocType" Text="Default Doctype:" ControlName="cboHostDefaultDocType"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboHostDefaultDocType" CssClass="Normal" runat="server" Width="150">
                                        <asp:ListItem resourcekey="LegacyDoctype" Value="0">HTML 4 (legacy)</asp:ListItem>
                                        <asp:ListItem resourcekey="TransDoctype" Value="1">XHTML 1.0 transitional</asp:ListItem>
                                        <asp:ListItem resourcekey="StrictDoctype" Value="2">XHTML 1.0 strict</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" width="150">
                                    <dnn:Label ID="plRememberMe" Text="Enable Remember me on login controls?" ControlName="chkRemember"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkRemember" runat="server" CssClass="NormalBold" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshAppearance" runat="server" CssClass="Head" Text="Appearance"
                            Section="tblAppearance" ResourceKey="Appearance" />
                        <table id="tblAppearance" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plCopyright" Text="Show Copyright Credits?" ControlName="chkCopyright"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkCopyright" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUseCustomErrorMessages" Text="Use Custom Error Messages?" ControlName="chkUseCustomErrorMessages"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkUseCustomErrorMessages" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUseCustomModuleCssClass" ControlName="chkUseCustomModuleCssClass" runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkUseCustomModuleCssClass" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostSkin" Text="Host Skin:" ControlName="ctlHostSkin$cboSkin" runat="server" />
                                </td>
                                <td valign="top">
                                    <dnn:Skin ID="ctlHostSkin" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostContainer" Text="Host Container:" ControlName="ctlHostContainer$cboSkin"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <dnn:Skin ID="ctlHostContainer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plAdminSkin" Text="Admin Skin:" ControlName="ctlAdminSkin$cboSkin"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <dnn:Skin ID="ctlAdminSkin" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plAdminContainer" Text="Admin Container:" ControlName="ctlAdminContainer$cboSkin"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <dnn:Skin ID="ctlAdminContainer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <br />
                                    <dnn:CommandButton ID="cmdUploadSkin" ResourceKey="SkinUpload" runat="server" CssClass="CommandButton"
                                        ImageUrl="~/images/up.gif" CausesValidation="false" />&nbsp;&nbsp;
                                    <dnn:CommandButton ID="cmdUploadContainer" ResourceKey="ContainerUpload" runat="server"
                                        CssClass="CommandButton" ImageUrl="~/images/up.gif"  CausesValidation="false" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshPayment" runat="server" CssClass="Head" Text="Payment Settings"
                            IsExpanded="False" Section="tblPayment" ResourceKey="Payment" />
                        <table id="tblPayment" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plProcessor" Text="Payment Processor:" ControlName="cboProcessor"
                                        runat="server" />
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="cboProcessor" CssClass="NormalTextBox" DataTextField="value"
                                        DataValueField="text" Width="325" runat="server" />
                                    <br />
                                    <dnn:CommandButton ID="cmdProcessor" ResourceKey="ProcessorWebSite" runat="server"
                                        CssClass="CommandButton" ImageUrl="~/images/rt.gif" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUserId" Text="Payment UserId:" ControlName="txtUserId" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserId" runat="server" Width="300" MaxLength="50" CssClass="NormalTextBox" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plPassword" Text="Payment Password:" ControlName="txtPassword" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPassword" runat="server" Width="300" MaxLength="50" CssClass="NormalTextBox"
                                        TextMode="Password" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostFee" Text="Hosting Fee:" ControlName="txtHostFee" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHostFee" CssClass="NormalTextBox" runat="server" MaxLength="10"
                                        Width="100" />
                                    <asp:CompareValidator ID="valHostFee" runat="server" ControlToValidate="txtHostFee"
                                        CssClass="NormalRed" Display="Dynamic" ErrorMessage="Invalid fee, needs to be a currency value!"
                                        ResourceKey="valHostFee.Error" Operator="DataTypeCheck" Type="Currency" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostCurrency" Text="Hosting Currency:" ControlName="cboHostCurrency"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboHostCurrency" CssClass="NormalTextBox" DataValueField="value"
                                        DataTextField="text" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHostSpace" Text="Hosting Space (MB):" ControlName="txtHostSpace"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHostSpace" CssClass="NormalTextBox" runat="server" MaxLength="6"
                                        Width="100" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plPageQuota" Text="Page Quota:" ControlName="txtPageQuota" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPageQuota" CssClass="NormalTextBox" runat="server" MaxLength="6"
                                        Width="100" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUserQuota" Text="User Quota:" ControlName="txtUserQuota" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserQuota" CssClass="NormalTextBox" runat="server" MaxLength="6"
                                        Width="100" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plDemoPeriod" ControlName="txtDemoPeriod" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDemoPeriod" CssClass="NormalTextBox" runat="server" MaxLength="3"
                                        Width="50" />
                                    <asp:Label ID="lblDemoPeriod" runat="server" CssClass="Normal" resourcekey="Days" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plDemoSignup" Text="Anonymous Demo Signup:" ControlName="chkDemoSignup"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDemoSignup" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
            </table>
            <br />
            <dnn:SectionHead ID="dshAdvanced" runat="server" CssClass="Head" Text="Advanced Settings"
                Section="tblAdvanced" ResourceKey="AdvancedSettings" IsExpanded="True" IncludeRule="False" />
            <table id="tblAdvanced" cellspacing="0" cellpadding="2" style="width: 100%" summary="Basic Settings Design Table"
                border="0" runat="server">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblAdvancedSettingsHelp" CssClass="Normal" runat="server" resourcekey="AdvancedSettingsHelp"
                            EnableViewState="False" />
                    </td>
                </tr>
                <tr>
                    <td width="25">
                    </td>
                    <td style="vertical-align: top; ">
                        <dnn:SectionHead ID="dshFriendlyUrl" runat="server" CssClass="Head" Text="Friendly Url Settings"
                            IsExpanded="False" Section="tblFriendlyUrl" ResourceKey="FriendlyUrl" />
                        <table id="tblFriendlyUrl" runat="server" style="width: 100%" cellspacing="2" cellpadding="2"
                            summary="Appearance Design Table" border="0">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUseFriendlyUrls" Text="Use Friendly Urls?" ControlName="chkUseFriendlyUrls"
                                        runat="server" />
                                </td>
                                <td style="vertical-align: top">
                                    <asp:CheckBox ID="chkUseFriendlyUrls" AutoPostBack="true" CssClass="NormalTextBox"
                                        runat="server" />
                                </td>
                            </tr>
                            <tr id="rowFriendlyUrls" runat="server">
                                <td colspan="2" style="text-align: center">
                                    <dnn:FriendlyUrls ID="friendlyUrls" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshRequestFilter" runat="server" CssClass="Head" Text="Request Filter Settings"
                            IsExpanded="False" Section="tblRequestFilter" ResourceKey="RequestFilter" />
                        <table id="tblRequestFilter" runat="server" style="width: 100%" cellspacing="2" cellpadding="2"
                            summary="Appearance Design Table" border="0">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plEnableRequestFilters" Text="Enable Request Filters?" ControlName="chkEnableRequestFilters"
                                        runat="server" />
                                </td>
                                <td style="vertical-align: top">
                                    <asp:CheckBox ID="chkEnableRequestFilters" AutoPostBack="true" CssClass="NormalTextBox"
                                        runat="server" />
                                </td>
                            </tr>
                            <tr id="rowRequestFilters" runat="server">
                                <td colspan="2" style="text-align: center">
                                    <dnn:RequestFilters ID="requestFilters" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshProxy" runat="server" CssClass="Head" Text="Proxy Settings"
                            IsExpanded="False" Section="tblProxy" ResourceKey="Proxy" />
                        <table id="tblProxy" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plProxyServer" Text="Proxy Server:" ControlName="txtProxyServer" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProxyServer" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plProxyPort" Text="Proxy Port:" ControlName="txtProxyPort" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProxyPort" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plProxyUsername" Text="Proxy Username:" ControlName="txtProxyUsername"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProxyUsername" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plProxyPassword" Text="Proxy Password:" ControlName="txtProxyPassword"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProxyPassword" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" TextMode="Password" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plWebRequestTimeout" Text="Web Request Timeout:" ControlName="txtWebRequestTimeout"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWebRequestTimeout" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshSMTP" runat="server" CssClass="Head" Text="SMTP Server Settings"
                            IsExpanded="False" Section="tblSMTP" ResourceKey="SMTP" />
                        <table id="tblSMTP" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSMTPServer" Text="SMTP Server:" ControlName="txtSMTPServer" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtSMTPServer" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="225" />
                                    &nbsp;
                                    <asp:LinkButton ID="cmdEmail" resourcekey="EmailTest" runat="server" CssClass="CommandButton">Test</asp:LinkButton>
                                    <asp:Label ID="lblEmail" runat="server" CssClass="NormalRed" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSMTPAuthentication" Text="SMTP Authentication:" ControlName="optSMTPAuthentication"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="optSMTPAuthentication" CssClass="Normal" runat="server"
                                        RepeatDirection="Horizontal" AutoPostBack="true">
                                        <asp:ListItem Value="0" resourcekey="SMTPAnonymous">Anonymous</asp:ListItem>
                                        <asp:ListItem Value="1" resourcekey="SMTPBasic">Basic</asp:ListItem>
                                        <asp:ListItem Value="2" resourcekey="SMTPNTLM">NTLM</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSMTPEnableSSL" Text="SMTP Enable SSL:" ControlName="chkSMTPEnableSSL"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkSMTPEnableSSL" runat="server" />
                                </td>
                            </tr>
                            <tr id="trSMTPUserName" runat="server">
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSMTPUsername" Text="SMTP Username:" ControlName="txtSMTPUsername"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSMTPUsername" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                            <tr id="trSMTPPassword" runat="server">
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSMTPPassword" Text="SMTP Password:" ControlName="txtSMTPPassword"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSMTPPassword" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" TextMode="Password" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshPerformance" CssClass="Head" runat="server" Text="Performance Settings"
                            IsExpanded="False" Section="tblPerformance" ResourceKey="Performance" />
                        <table id="tblPerformance" cellspacing="2" cellpadding="2" summary="Performance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" width="200" valign="top">
                                <dnn:Label ID="plPageState" runat="server" ControlName="cboPageState" Text="Page State Persistence:">
                                    </dnn:Label>
                                </td>
                                <td>
                                    <asp:label ID="plPsWarning" runat="server" CssClass="NormalRed" resourcekey="plPsWarning">
                                    </asp:label>
                                    <asp:RadioButtonList ID="cboPageState" CssClass="Normal" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem resourcekey="Page" Value="P">Page</asp:ListItem>
                                        <asp:ListItem resourcekey="Memory" Value="M">Memory</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" valign="top" width="200">
                                    <dnn:Label ID="lblModuleCacheProvider" runat="server" Text="Module Cache Provider" ControlName="cboModuleCacheProvider" ResourceKey="ModuleCacheProvider" HelpKey="ModuleCacheProvider.Help"></dnn:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboModuleCacheProvider" runat="server" DataValueField="key" DataTextField="filteredkey" Style="width: 200px" />
                                </td>
                            </tr>
                            <tr id="PageCacheRow" runat="server" >
                                <td class="SubHead" valign="top" width="200">
                                    <dnn:Label ID="lblPageCacheProvider" runat="server" Text="Page Cache Provider" ControlName="cboPageCacheProvider" ResourceKey="PageCacheProvider" HelpKey="PageCacheProvider.Help"></dnn:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboPageCacheProvider" runat="server" DataValueField="key" DataTextField="filteredkey" Style="width: 200px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" width="200">
                                    <dnn:Label ID="plPerformance" Text="Performance Setting:" ControlName="cboPerformance"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboPerformance" runat="server" Style="width: 200px">
                                        <asp:ListItem resourcekey="NoCaching" Value="0">No Caching</asp:ListItem>
                                        <asp:ListItem resourcekey="LightCaching" Value="1">Light Caching</asp:ListItem>
                                        <asp:ListItem resourcekey="ModerateCaching" Value="3">Moderate Caching</asp:ListItem>
                                        <asp:ListItem resourcekey="HeavyCaching" Value="6">Heavy Caching</asp:ListItem>
                                    </asp:DropDownList>
                                    &nbsp;
                                    <asp:LinkButton ID="cmdCache" resourcekey="ClearCache" runat="server" CssClass="CommandButton" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" width="200">
                                    <dnn:Label ID="plCacheability" Text="Authenticated Cacheability:" ControlName="cboCacheability"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboCacheability" runat="server" Style="width: 200px" CssClass="Normal">
                                        <asp:ListItem resourcekey="NoCache" Value="0">NoCache</asp:ListItem>
                                        <asp:ListItem resourcekey="Private" Value="1">Private</asp:ListItem>
                                        <asp:ListItem resourcekey="Public" Value="2">Public</asp:ListItem>
                                        <asp:ListItem resourcekey="Server" Value="3">Server</asp:ListItem>
                                        <asp:ListItem resourcekey="ServerAndNoCache" Value="4">ServerAndNoCache</asp:ListItem>
                                        <asp:ListItem resourcekey="ServerAndPrivate" Value="5">ServerAndPrivate</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" width="200">
                                    <dnn:Label ID="plCompression" Text="Compression Setting:" ControlName="cboCompression"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboCompression" runat="server" Style="width: 200px" CssClass="NormalTextBox">
                                        <asp:ListItem resourcekey="NoCompression" Value="0">No Compression</asp:ListItem>
                                        <asp:ListItem resourcekey="Deflate" Value="2">Deflate Compression</asp:ListItem>
                                        <asp:ListItem resourcekey="GZip" Value="1">GZip Compression</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plWhitespace" Text="Use Whitespace Filter:" ControlName="chkWhitespace"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkWhitespace" runat="server" CssClass="NormalTextBox" />&nbsp;
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshCompression" CssClass="Head" runat="server" Text="Compression Settings"
                            IsExpanded="False" Section="tblCompression" ResourceKey="Compression" />
                        <table id="tblCompression" cellspacing="2" cellpadding="2" summary="Compression Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="vertical-align: top; width: 250px">
                                    <dnn:Label ID="plExcludedPaths" runat="server" ControlName="txtExcludedPaths" Text="Excluded Paths:">
                                    </dnn:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExcludedPaths" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" TextMode="MultiLine" Rows="3" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="vertical-align: top; width: 250px">
                                    <dnn:Label ID="plWhitespaceFilter" runat="server" ControlName="txtWhitespaceFilter"
                                        Text="Whitespace Filter:"></dnn:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWhitespaceFilter" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" TextMode="MultiLine" Rows="3" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center">
                                    <dnn:CommandButton ID="cmdUpdateCompression" ResourceKey="cmdUpdate" runat="server"
                                        CssClass="CommandButton" ImageUrl="~/images/save.gif" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <dnn:SectionHead ID="dshJQuery" CssClass="Head" runat="server" Text="jQuery Settings"
                            IsExpanded="False" Section="tblJQuery" ResourceKey="JQuery" />
                        <br />
                        <table id="tblJQuery" cellspacing="2" cellpadding="2" summary="jQuery Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plJQueryVersion" Text="Installed jQuery Version:" ControlName="txtDebugVersion"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:Label ID="jQueryVersion" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plJQueryDebugVersion" Text="Use jQuery Debug Version?" ControlName="chkJQueryDebugVersion"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkJQueryDebugVersion" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plJQueryUseHosted" Text="Use Hosted jQuery Version?" ControlName="chkJQueryUseHosted"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkJQueryUseHosted" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plJQueryHostUrl" Text="Hosted jQuery URL:" ControlName="txtJQueryHostedUrl"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtJQueryHostedUrl" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                       </table>
                        <dnn:SectionHead ID="dshOther" CssClass="Head" runat="server" Text="Other Settings"
                            IsExpanded="False" Section="tblOther" ResourceKey="Other" />
                        <table id="tblOther" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plControlPanel" Text="Control Panel:" ControlName="cboControlPanel"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:DropDownList ID="cboControlPanel" CssClass="NormalTextBox" Width="300" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSiteLogStorage" Text="Site Log Storage:" ControlName="optSiteLogStorage"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="optSiteLogStorage" CssClass="Normal" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="D" resourcekey="Database">Database</asp:ListItem>
                                        <asp:ListItem Value="F" resourcekey="FileSystem">File System</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSiteLogBuffer" ControlName="txtSiteLogBuffer" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSiteLogBuffer" CssClass="NormalTextBox" runat="server" MaxLength="4"
                                        Width="50" />
                                    <asp:Label ID="lblSiteLogBuffer" runat="server" CssClass="Normal" resourcekey="Items" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSiteLogHistory" ControlName="txtSiteLogHistory" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSiteLogHistory" CssClass="NormalTextBox" runat="server" MaxLength="3"
                                        Width="50" />
                                    <asp:Label ID="lblSiteLogHistory" runat="server" CssClass="Normal" resourcekey="Days" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUsersOnline" ControlName="chkUsersOnline" runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkUsersOnline" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plUsersOnlineTime" ControlName="txtUsersOnlineTime" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUsersOnlineTime" CssClass="NormalTextBox" runat="server" MaxLength="3"
                                        Width="50" />
                                    <asp:Label ID="lblUsersOnlineTime" runat="server" CssClass="Normal" resourcekey="Minutes" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plAutoAccountUnlock" ControlName="txtAutoAccountUnlock" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtAutoAccountUnlock" runat="server" CssClass="NormalTextBox" MaxLength="3"
                                        Width="50" />
                                    <asp:Label ID="lblAutoAccountUnlock" runat="server" CssClass="Normal" resourcekey="Minutes" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plFileExtensions" ControlName="txtFileExtensions" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtFileExtensions" CssClass="NormalTextBox" runat="server" MaxLength="256" Width="300" TextMode="MultiLine" Rows="3" />
                                    <asp:RegularExpressionValidator ID="valFileExtensions" CssClass="NormalRed" runat="server" ControlToValidate="txtFileExtensions" 
                                                EnableClientScript="true" ValidationExpression="[A-Za-z0-9,_]*" resourceKey="valFileExtensions.Error" Display="Dynamic" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plSchedulerMode" Text="Scheduler Mode:" ControlName="cboSchedulerMode"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboSchedulerMode" runat="server" Width="250px" CssClass="NormalTextBox">
                                        <asp:ListItem resourcekey="Disabled" Value="0">Disabled</asp:ListItem>
                                        <asp:ListItem resourcekey="TimerMethod" Value="1">Timer Method</asp:ListItem>
                                        <asp:ListItem resourcekey="RequestMethod" Value="2">Request Method</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plLogBuffer" Text="Enable Event Log Buffer?" ControlName="chkLogBuffer"
                                        runat="server" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkLogBuffer" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plHelpUrl" Text="Help Url:" ControlName="txtHelpURL" runat="server" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHelpURL" CssClass="NormalTextBox" runat="server" MaxLength="256"
                                        Width="300" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plEnableHelp" Text="Enable Module Help?" ControlName="chkEnableHelp"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkEnableHelp" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plAutoSync" Text="Enable File System Auto-Sync?" ControlName="chkAutoSync"
                                        runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkAutoSync" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead" style="width: 250px">
                                    <dnn:Label ID="plEnableContentLocalization" ControlName="chkEnableContentLocalization" runat="server" />
                                </td>
                                <td valign="top">
                                    <asp:CheckBox ID="chkEnableContentLocalization" CssClass="NormalTextBox" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="text-align: center">
            <dnn:CommandButton ID="cmdUpdate" ResourceKey="cmdUpdate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" />
            &nbsp;&nbsp;
            <dnn:CommandButton ID="cmdRestart" ResourceKey="cmdRestart" runat="server" CssClass="CommandButton" ImageUrl="~/images/reset.gif" CausesValidation="False" />
        </td>
    </tr>
</table>
<hr noshade="noshade" size="1" />
<br />
<table class="Settings" cellspacing="2" cellpadding="2" summary="Host Settings Design Table"
    border="0">
    <tr>
        <td class="SubHead" valign="bottom">
            <dnn:Label ID="plLog" Text="View Upgrade Log For Version:" ControlName="cboUpgrade"
                runat="server" />
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="cboVersion" runat="server" CssClass="NormalTextBox" />
            &nbsp;
            <dnn:CommandButton ID="cmdUpgrade" ResourceKey="cmdGo" runat="server" CssClass="CommandButton"
                ImageUrl="~/images/view.gif" CausesValidation="false" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label ID="lblUpgrade" runat="server" CssClass="Normal" />
        </td>
    </tr>
</table>
