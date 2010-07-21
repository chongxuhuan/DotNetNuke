<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" CodeFile="ManageTabs.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Tabs.ManageTabs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="Portal" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ModuleLocalization" Src="~/Admin/Modules/ModuleLocalization.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TabLocalization" Src="~/DesktopModules/Admin/Tabs/TabLocalization.ascx" %>
<table ID="tblManageTabs" runat="server" cellspacing="2" cellpadding="2" summary="Manage Tabs Design Table" border="0">
    <tr>
        <td width="850" valign="top">
            <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" CssClass="Head" runat="server" Text="Basic Settings"
                    Section="tblBasic" ResourceKey="BasicSettings" IncludeRule="True"/>
                <table id="tblBasic" cellspacing="0" cellpadding="2" width="825" summary="Basic Settings Design Table"
                    border="0" runat="server">
                    <tr>
                        <td colspan="2"><asp:Label ID="lblBasicSettingsHelp" CssClass="Normal" runat="server" resourcekey="BasicSettingsHelp" EnableViewState="False"/></td>
                    </tr>
                    <tr>
                        <td width="25"></td>
                        <td valign="top" width="825">
                            <dnn:SectionHead ID="dshPage" CssClass="Head" runat="server" Text="Page Details"
                                Section="tblPage" ResourceKey="PageDetails"/>
                            <table id="tblPage" cellspacing="2" cellpadding="2" summary="Site Details Design Table" border="0" runat="server">
                                <tr>
                                    <td class="SubHead" width="200">
                                        <dnn:Label ID="plTabName" runat="server" ResourceKey="TabName" Suffix=":" HelpKey="TabNameHelp" ControlName="txtTabName"/>
                                    </td>
                                    <td width="625">
                                        <asp:TextBox ID="txtTabName" CssClass="NormalTextBox" runat="server" MaxLength="50" Width="625"/>
                                        <asp:RequiredFieldValidator ID="valTabName" CssClass="NormalRed" runat="server" resourcekey="valTabName.ErrorMessage" Display="Dynamic" ControlToValidate="txtTabName"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" width="200"><dnn:Label ID="plTitle" runat="server" ResourceKey="Title" Suffix=":" HelpKey="TitleHelp" ControlName="txtTitle"/></td>
                                    <td><asp:TextBox ID="txtTitle" CssClass="NormalTextBox" runat="server" MaxLength="200" Width="625"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="vertical-align:top; width:200px"><dnn:Label ID="plDescription" runat="server" ResourceKey="Description" Suffix=":" HelpKey="DescriptionHelp" ControlName="txtDescription"/></td>
                                    <td class="NormalTextBox" width="625"><asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" MaxLength="625" Width="625" TextMode="MultiLine" Rows="2"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" style="vertical-align:top; width:200px"><dnn:Label ID="plKeywords" runat="server" ResourceKey="KeyWords" Suffix=":" HelpKey="KeyWordsHelp" ControlName="txtKeyWords"/></td>
                                    <td class="NormalTextBox" width="625"><asp:TextBox ID="txtKeyWords" CssClass="NormalTextBox" runat="server" MaxLength="500" Width="625" TextMode="MultiLine" Rows="2"/></td>
                                </tr>
                                <tr>
                                    <td><dnn:Label ID="plTags" runat="server" cssClass="SubHead" ControlName="termsSelector"/></td>
                                    <td><dnn:TermsSelector ID="termsSelector" runat="server" Height="250" Width="625"/></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" width="200"><dnn:Label ID="plParentTab" runat="server" ResourceKey="ParentTab" ControlName="cboParentTab"/></td>
                                    <td width="625"><asp:DropDownList ID="cboParentTab" CssClass="NormalTextBox" runat="server" Width="300" DataTextField="IndentedTabName" DataValueField="TabId"/></td>
                                </tr>
                                <tr id="trInsertPositionRow" runat="server">
                                    <td class="SubHead" width="200"><dnn:Label ID="plInsertPosition" runat="server" ResourceKey="InsertPosition" ControlName="cboPositionTab" /></td>
                                    <td width="625">
                                        <asp:RadioButtonList ID="rbInsertPosition" runat="server" CssClass="Normal" RepeatDirection="Horizontal" AutoPostBack="true"/>
                                        <asp:DropDownList ID="cboPositionTab" CssClass="NormalTextBox" runat="server" Width="300" DataTextField="LocalizedTabName" DataValueField="TabId"/>
                                    </td>
                                </tr>
                                <tr id="rowTemplate1" runat="Server" visible="false">
                                    <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plFolder" runat="server" controlname="cboFolders" /></td>
                                    <td><asp:DropDownList ID="cboFolders" Runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="true" /></td>
                                </tr>
	                            <tr id="rowTemplate2" runat="Server" visible="false">
		                            <td class="SubHead" style="vertical-align:top; width:150px;"><dnn:label id="plTemplate" runat="server" controlname="cboTemplate" /></td>
		                            <td style="vertical-align:top;"><asp:dropdownlist id="cboTemplate" cssclass="NormalTextBox" runat="server" width="300" /></td>
	                            </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200"><dnn:Label ID="plMenu" runat="server" ResourceKey="Menu" Suffix="?" HelpKey="MenuHelp" ControlName="chkMenu"/></td>
                                    <td width="625"><asp:CheckBox ID="chkMenu" runat="server" Font-Size="8pt" Font-Names="Verdana,Arial"/></td>
                                </tr>
                                <tr id="rowPerm" runat="server">
                                    <td colspan="2">
                                        <dnn:Label ID="plPermissions" runat="server" cssClass="SubHead"  ResourceKey="Permissions" Suffix=":" HelpKey="PermissionsHelp" ControlName="dgPermissions"/>
                                        <div style="text-align:left">
											<Portal:TabPermissionsGrid ID="dgPermissions" runat="server"/>
										</div>
										<br />
                                    </td>
                                </tr>
                                <tr id="rowCopyPerm" runat="server">
                                    <td class="SubHead"><dnn:Label ID="plCopyPerm" runat="server" ResourceKey="plCopyPerm" /></td>
                                    <td><asp:LinkButton ID="cmdCopyPerm" runat="server" CssClass="CommandButton" resourcekey="cmdCopyPerm"/></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshCopy" CssClass="Head" runat="server" Text="Copy Page" Section="tblCopy"
                    ResourceKey="Copy" IncludeRule="True"></dnn:SectionHead>
                <table id="tblCopy" cellspacing="0" cellpadding="2" width="825" summary="Copy Tab Design Table"
                    border="0" runat="server">
                    <tr>
                        <td width="25">
                        </td>
                        <td class="SubHead" width="200">
                            <dnn:Label ID="plCopyPage" runat="server" ResourceKey="CopyModules" Suffix=":" HelpKey="CopyModulesHelp"
                                ControlName="cboCopyPage"></dnn:Label>
                        </td>
                        <td width="625">
                            <asp:DropDownList ID="cboCopyPage" CssClass="NormalTextBox" runat="server" Width="300"
                                DataTextField="IndentedTabName" DataValueField="TabId" AutoPostBack="True">
                            </asp:DropDownList></td>
                    </tr>
                    <tr id="rowModules" runat="server">
                        <td width="25">
                        </td>
                        <td class="SubHead" colspan="2">
                            <dnn:Label ID="plModules" runat="server" ResourceKey="CopyContent" Suffix=":" HelpKey="CopyContentHelp"
                                ControlName="grdModules"></dnn:Label>
                            <br/>
                            <asp:DataGrid ID="grdModules" runat="server" DataKeyField="ModuleID" ShowHeader="False"
                                ItemStyle-CssClass="Normal" GridLines="None" BorderWidth="0px" BorderStyle="None"
                                AutoGenerateColumns="False" CellSpacing="0" CellPadding="0" Width="100%">
                                <Columns>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkModule" runat="server" CssClass="NormalTextBox" Checked="True" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtCopyTitle" width="200" runat="server" CssClass="NormalTextBox"
                                                Text='<%# Databinder.eval(Container.Dataitem,"ModuleTitle")%>'>
                                            </asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn runat="server" DataField="PaneName" />
                                    <asp:TemplateColumn>
                                        <ItemTemplate>
                                            <asp:RadioButton ID="optNew" runat="server" CssClass="NormalBold" GroupName="Copy"
                                                Text="New" resourcekey="ModuleNew" Checked="True"></asp:RadioButton>
                                            <asp:RadioButton ID="optCopy" runat="server" CssClass="NormalBold" GroupName="Copy"
                                                Text="Copy" resourcekey="ModuleCopy" Enabled='<%# DataBinder.Eval(Container.DataItem, "IsPortable") %>'>
                                            </asp:RadioButton>
                                            <asp:RadioButton ID="optReference" runat="server" CssClass="NormalBold" GroupName="Copy"
                                                Text="Reference" resourcekey="ModuleReference" Enabled='<%# DataBinder.Eval(Container.DataItem, "ModuleID") <> -1  %>'>
                                            </asp:RadioButton>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshLocalization" CssClass="Head" runat="server" Text="Localization"
                    Section="tblLocalization" ResourceKey="Localization" IncludeRule="True" IsExpanded="False" Visible="false"></dnn:SectionHead>
                <table id="tblLocalization" cellspacing="0" cellpadding="2" width="825" summary="Localization Table"
                    border="0" runat="server" Visible="false">
                    <tr id="cultureTypeRow" runat="server" visible="false">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;vertical-align:top">
                             <dnn:Label ID="cultureTypeLabel" runat="server" ControlName="cultureTypeList"></dnn:Label>
                       </td>
                       <td colspan="2">
                            <asp:RadioButtonList ID="cultureTypeList" runat="server" RepeatDirection="Vertical">
                                <asp:ListItem Value="Neutral" resourcekey="Neutral" Selected="True" />
                                <asp:ListItem Value="Culture" resourcekey="Culture" />
                                <asp:ListItem Value="Localized" resourcekey="Localized" />
                            </asp:RadioButtonList>
                       </td>
                   </tr>
                    <tr id="cultureRow" runat="server">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;">
                             <dnn:Label ID="cultureLabel" runat="server" ControlName="cultureName"></dnn:Label>
                       </td>
                       <td colspan="2">
                            <dnn:DnnLanguageLabel ID="cultureLanguageLabel" runat="server"  />&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="defaultCultureMessageLabel" runat="server" CssClass="NormalRed" Text="**" />
                            <asp:Label ID="defaultCultureMessage" runat="server" CssClass="Normal" resourcekey="DefaultCulture"/>
                      </td>
                     </tr>
                    <tr id="translatedRow" runat="server">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;vertical-align:top">
                             <dnn:Label ID="translatedLabel" runat="server" ControlName="translatedCheckbox" resourcekey="IsTranslated"></dnn:Label>
                       </td>
                       <td colspan="2">
                            <asp:Checkbox ID="translatedCheckbox" runat="server" cssclass="SubHead" />
                            <asp:Button ID="localizePagesButton" runat="server" resourcekey="CreateLocalizedPages" Visible ="false" />
                       </td>
                    </tr>
                    <tr id="defaultCultureRow" runat="server">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;">
                             <dnn:Label ID="defaultCultureLabel" runat="server" ControlName="defaultCultureLink"></dnn:Label>
                       </td>
                       <td style="width:300px;">
                            <dnn:DnnLanguageLabel ID="defaultCultureLanguageLabel" runat="server"  />
                       </td>
                        <td style="width:325px;">
                            <dnn:CommandButton ID="viewDefaultCultureButton" runat="server" ImageUrl="~/images/view.gif" ResourceKey="view" />&nbsp;&nbsp;
                            <dnn:CommandButton ID="editDefaultCultureButton" runat="server" ImageUrl="~/images/edit.gif" ResourceKey="edit" />
                        </td>
                    </tr>
                    <tr id="publishRow" runat="server" visible="false">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;">
                             <dnn:Label ID="publishPageLabel" runat="server" ControlName="publishPageButton"></dnn:Label>
                       </td>
                        <td colspan="2">
                            <dnn:CommandButton ID="publishPageButton" runat="server" 
                                            ImageUrl="~/images/save.gif" 
                                            DisplayLink = "false"
                                            ResourceKey="PublishPage" />
                       </td>
                    </tr>
                    <tr id="readyForTranslationRow" runat="server" visible="false">
                        <td style="width:25px;">
                        </td>
                        <td class="SubHead" style="width:200px;">
                             <dnn:Label ID="readyToTranslateLabel" runat="server" ControlName="readyForTranslationButton"></dnn:Label>
                       </td>
                       <td colspan="2">
                            <dnn:CommandButton ID="readyForTranslationButton" runat="server" 
                                        ImageUrl="~/images/icon_language_16px.gif" 
                                        DisplayLink = "false"
                                        ResourceKey="ReadyForTranslation" />
                       </td>
                    </tr>
                    <tr id="localizedTabsRow" runat="server">
                        <td style="width:25px;">
                        </td>
                        <td colspan="3">
                            <br />
                            <asp:Label ID="localizedTabsLabel" CssClass="SubHead" runat="server" resourcekey="LocalizedTabs" EnableViewState="False" />
                            <br />
                            <br />
                            <dnn:TabLocalization id="tabLocalization" runat="server" />
                        </td>
                    </tr>
                    <tr id="localizedModulesRow" runat="server">
                        <td style="width:25px;">
                        </td>
                        <td valign="top" colspan="3">
                            <br />
                            <br />
                            <asp:Label ID="localizedModulesLabel" CssClass="SubHead" runat="server" resourcekey="LocalizedModules" EnableViewState="False" />
                            <br />
                            <br />
                            <dnn:ModuleLocalization id="moduleLocalization" runat="server" />
                        </td>
                    </tr>
                </table>
                <br/>
                <dnn:SectionHead ID="dshAdvanced" CssClass="Head" runat="server" Text="Advanced Settings"
                    Section="tblAdvanced" ResourceKey="AdvancedSettings" IncludeRule="True" IsExpanded="False"></dnn:SectionHead>
                <table id="tblAdvanced" cellspacing="0" cellpadding="2" width="825" summary="Advanced Settings Design Table"
                    border="0" runat="server">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAdvancedSettingsHelp" CssClass="Normal" runat="server" resourcekey="AdvancedSettingsHelp"
                                EnableViewState="False"></asp:Label></td>
                    </tr>
                    <tr>
                        <td width="25">
                        </td>
                        <td valign="top" width="825">
                            <dnn:SectionHead ID="dhsAppearance" CssClass="Head" runat="server" Text="Appearance"
                                Section="tblAppearance" ResourceKey="Appearance"></dnn:SectionHead>
                            <table id="tblAppearance" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                                border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plIcon" runat="server" ResourceKey="Icon" Suffix=":" HelpKey="IconHelp"
                                            ControlName="ctlIcon"></dnn:Label>
                                    </td>
                                    <td width="325">
                                        <dnn:URL ID="ctlIcon" runat="server" Width="300" ShowLog="False"></dnn:URL>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plIconLarge" runat="server" ResourceKey="IconLarge" Suffix=":" HelpKey="IconLargeHelp"
                                            ControlName="ctlIconLarge"></dnn:Label>
                                    </td>
                                    <td width="325">
                                        <dnn:URL ID="ctlIconLarge" runat="server" Width="300" ShowLog="False"></dnn:URL>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plSkin" runat="server" ResourceKey="TabSkin" Suffix=":" HelpKey="TabSkinHelp"
                                            ControlName="ctlSkin"></dnn:Label>
                                    </td>
                                    <td valign="top" width="625">
                                        <dnn:Skin ID="ctlSkin" runat="server" DefaultKey="Portal" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plContainer" runat="server" ResourceKey="TabContainer" Suffix=":"
                                            HelpKey="TabContainerHelp" ControlName="ctlContainer" />
                                    </td>
                                    <td valign="top" width="625">
                                        <dnn:Skin ID="ctlContainer" runat="server" DefaultKey="Portal" />
                                    </td>
                                </tr>
                                <tr id="rowCopySkin" runat="server">
                                    <td class="SubHead">
                                        <dnn:Label ID="plCopySkin" runat="server" ResourceKey="plCopySkin" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="cmdCopySkin" runat="server" CssClass="CommandButton" resourcekey="cmdCopySkin"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plDisable" runat="server" ResourceKey="Disabled" Suffix=":" HelpKey="DisabledHelp"
                                            ControlName="chkDisableLink"></dnn:Label>
                                    </td>
                                    <td width="625">
                                        <asp:CheckBox ID="chkDisableLink" runat="server" Font-Size="8pt" Font-Names="Verdana,Arial">
                                        </asp:CheckBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" width="200">
                                        <dnn:Label ID="plRefreshInterval" runat="server" ResourceKey="RefreshInterval" Suffix=":"
                                            HelpKey="RefreshInterval.Help" ControlName="cboRefreshInterval" columns="10"></dnn:Label>
                                    </td>
                                    <td width="625">
                                        <asp:TextBox ID="txtRefreshInterval" CssClass="NormalTextBox" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plPageHeadText" runat="server" ResourceKey="PageHeadText" Suffix=":"
                                            HelpKey="PageHeadText.Help" ControlName="txtPageHeadText"></dnn:Label>
                                    </td>
                                    <td class="NormalTextBox" width="625">
                                        <asp:TextBox ID="txtPageHeadText" CssClass="NormalTextBox" runat="server" TextMode="MultiLine" MaxLength="500"
                                            Rows="4" Columns="50"></asp:TextBox></td>
                                </tr>
                            </table>
                            <br>
                            <dnn:SectionHead ID="dshCaching" CssClass="Head" runat="server" Text="Cache Settings"
                                Section="tblCaching" ResourceKey="CacheSettings"></dnn:SectionHead>
                                <table id="tblCaching" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                                border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheProvider" runat="server" Text="Cache Provider" ControlName="cboCacheProvider" ResourceKey="CacheProvider" HelpKey="CacheProvider.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cboCacheProvider" runat="server" AutoPostBack="true" DataValueField="Key" DataTextField="Key" />
                                    </td>
                                </tr>
                                 <tr id="trCacheStatus" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheStatus" runat="server" Text="Cache Status" ResourceKey="CacheStatus" HelpKey="CacheStatus.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblCachedItemCount" runat="server" /><br />
                                        <asp:LinkButton ID="cmdClearPageCache" runat="server" CssClass="CommandButton" resourcekey="cmdClearPageCache"/>&nbsp;|&nbsp;<asp:LinkButton ID="cmdClearAllPageCache" runat="server" CssClass="CommandButton" resourcekey="cmdClearAllPageCache"/>
                                    </td>
                                </tr>
                                <tr id="trCacheDuration" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheDuration" runat="server" Text="Cache Duration (seconds)" ControlName="txtCacheDuration" ResourceKey="CacheDuration" HelpKey="CacheDuration.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCacheDuration" runat="server" />
                                        <asp:CompareValidator ID="valCacheTime" ControlToValidate="txtCacheDuration" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valCacheTime.ErrorMessage" />
                                        <dnn:Label ID="lblCacheDurationInfo" runat="server" ResourceKey="CacheDurationInfo.Text"></dnn:Label>
                                    </td>
                                </tr>
                                <tr id="trCacheIncludeExclude" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheIncludeExclude" runat="server" Text="Include/Exclue Params by Default" ControlName="rblCacheIncludeExclude" ResourceKey="CacheIncludeExclude" HelpKey="CacheIncludeExclude.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblCacheIncludeExclude" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="Exclude" Value="0" />
                                            <asp:ListItem Text="Include" Value="1" />
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr id="trIncludeVaryBy" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblIncludeVaryBy" runat="server" Text="Include Params in Cache Variation" ControlName="txtIncludeVaryBy" ResourceKey="IncludeVaryBy" HelpKey="IncludeVaryBy.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtIncludeVaryBy" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trExcludeVaryBy" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblExcludeVaryBy" runat="server" Text="Exclude Params in Cache Variation" ControlName="txtExcludeVaryBy" ResourceKey="ExcludeVaryBy" HelpKey="ExcludeVaryBy.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExcludeVaryBy" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trMaxVaryByCount" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblMaxVaryByCount" runat="server" Text="Vary By Limit" ControlName="txtMaxVaryByCount" ResourceKey="CacheMaxVaryByCount" HelpKey="CacheMaxVaryByCount.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMaxVaryByCount" runat="server" />
                                        <asp:CompareValidator ID="valMaxVaryByCount" ControlToValidate="txtMaxVaryByCount" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valCacheTime.ErrorMessage" />
                                    </td>
                                </tr>
                                </table>
                                <br />
                            <dnn:SectionHead ID="dshOther" CssClass="Head" runat="server" Text="Other Settings"
                                Section="tblOther" ResourceKey="OtherSettings"></dnn:SectionHead>
                            <table id="tblOther" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                                border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plSecure" runat="server" Text="Secure?" ControlName="chkSecure"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkSecure" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plPriority" runat="server" ControlName="txtPriority"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPriority" CssClass="NormalTextBox" runat="server" MaxLength="11" Width="120"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plStartDate" runat="server" Text="Start Date:" ControlName="txtStartDate"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStartDate" CssClass="NormalTextBox" runat="server" MaxLength="11" Width="120" Columns="30"/>&nbsp;
                                        <asp:HyperLink ID="cmdStartCalendar" CssClass="CommandButton" runat="server" resourcekey="Calendar"/>
                                        <asp:CompareValidator ID="valtxtStartDate" resourcekey="valStartDate.ErrorMessage" Operator="DataTypeCheck" Type="Date" runat="server" Display="Dynamic" ControlToValidate="txtStartDate"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plEndDate" runat="server" Text="End Date:" ControlName="txtEndDate"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEndDate" CssClass="NormalTextBox" runat="server" MaxLength="11" Width="120" Columns="30"/>&nbsp;
                                        <asp:HyperLink ID="cmdEndCalendar" CssClass="CommandButton" runat="server" resourcekey="Calendar"/>
                                        <asp:CompareValidator ID="valtxtEndDate" resourcekey="valEndDate.ErrorMessage" Operator="DataTypeCheck" Type="Date" runat="server" Display="Dynamic" ControlToValidate="txtEndDate"/>
									    <asp:CompareValidator ID="val2txtEndDate" ControlToValidate="txtEndDate" ControlToCompare="txtStartdate" Operator="GreaterThanEqual" Type="Date" Runat="server" Display="Dynamic" resourcekey="valEndDate2.ErrorMessage" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="plURL" runat="server" ResourceKey="Url" Suffix=":" HelpKey="UrlHelp"
                                            ControlName="ctlURL"></dnn:Label>
                                    </td>
                                    <td class="NormalTextBox" width="325">
                                        <dnn:URL ID="ctlURL" runat="server" Width="300" ShowLog="False" ShowNone="True" ShowTrack="False"/>
                                    </td>
                                </tr>
                                <tr id="trRedirect" runat="server">
                                    <td class="SubHead" valign="top" width="150"><dnn:Label ID="plPermanentRedirect" runat="server" ControlName="chkPermanentRedirect"/></td>
                                    <td><asp:CheckBox ID="chkPermanentRedirect" runat="server" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<p>
    <dnn:CommandButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:CommandButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="CommandButton" ImageUrl="~/images/delete.gif" CausesValidation="False" />&nbsp;
    <dnn:CommandButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" CausesValidation="False" />
</p>
<dnn:audit id="ctlAudit" runat="server" />
