<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Modules.ModuleSettingsPage" CodeFile="ModuleSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" style="width:760px" summary="Module Settings Design Table" border="0">
	<tr>
		<td style="width:760px" valign="top">
			<dnn:sectionhead id="dshModule" cssclass="Head" runat="server" text="Module Settings" section="tblModule" resourcekey="ModuleSettings" includerule="True" />
			<table id="tblModule" cellspacing="2" cellpadding="2" summary="Module Details Design Table" border="0" runat="server">
				<tr>
					<td colspan="2"><asp:label id="lblModuleSettingsHelp" cssclass="Normal" runat="server" resourcekey="ModuleSettingsHelp" enableviewstate="False" /></td>
				</tr>
				<tr>
					<td style="width:25px"></td>
					<td valign="top" style="width:675px">
						<dnn:sectionhead id="dshDetails" cssclass="Head" runat="server" text="Basic Settings" section="tblDetails" resourcekey="GeneralDetails" />
						<table id="tblDetails" cellspacing="2" cellpadding="2" summary="Appearance Design Table" border="0" runat="server">
							<tr>
								<td class="SubHead" style="width:150px"><dnn:label id="plFriendlyName" text="Module:" runat="server" controlname="lblFriendlyName"></dnn:label></td>
								<td><asp:TextBox ID="txtFriendlyName" runat="server" CssClass="NormalTextBox" Width="525" Enabled="False"></asp:TextBox></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:150px"><dnn:label id="plTitle" text="Title:" runat="server" controlname="txtTitle"></dnn:label></td>
								<td><asp:textbox id="txtTitle" runat="server" cssclass="NormalTextBox" style="width:525px"></asp:textbox></td>
							</tr>
                            <tr>
                                <td><dnn:Label ID="plTags" runat="server" cssClass="SubHead" ControlName="termsSelector"/></td>
                                <td><dnn:TermsSelector ID="termsSelector" runat="server" Height="250px" Width="525px"/></td>
							</tr>
							<tr id="rowPerm" runat="server">
								<td colspan="2" align="left">
									<div style="text-align:left" class="SubHead" ><dnn:label id="plPermissions" runat="server" controlname="ctlPermissions" text="Permissions:"/></div>
									<table border="0" cellpadding="0" cellspacing="0" style="text-align:center;">
							            <tr>
											<td><dnn:modulepermissionsgrid id="dgPermissions" runat="server"/></td>
										</tr>
										<tr><td>&nbsp;</td></tr>
										<tr>
											<td><asp:checkbox id="chkInheritPermissions" cssclass="Normal" autopostback="true" runat="server" resourcekey="InheritPermissions" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<br/>
						<dnn:sectionhead id="dshSecurity" cssclass="Head" runat="server" text="Security Settings" section="tblSecurity" resourcekey="Security" isexpanded="False" />
						<table id="tblSecurity" cellspacing="2" cellpadding="2" summary="Security Details Design Table" border="0" runat="server">
							<tr id="trAllTabs" runat="server">
								<td class="SubHead" style="width:225px"><dnn:label id="plAllTabs" runat="server" controlname="chkAllTabs"/></td>
								<td><asp:checkbox id="chkAllTabs" runat="server" AutoPostback="true"/></td>
							</tr>
							<tr id="trnewPages" runat="server" visible="false">
								<td class="SubHead" style="width:225px"><dnn:label id="plNewTabs" runat="server" controlname="chkNewTabs"/></td>
								<td><asp:checkbox id="chkNewTabs" runat="server"/></td>
							</tr>
							<tr id="tradminBorder" runat="server" visible="true">
								<td class="SubHead" style="width:225px"><dnn:label id="plAdminBorder" runat="server" controlname="chkAdminBorder"/></td>
								<td><asp:checkbox id="chkAdminBorder" runat="server"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:225px"><dnn:label id="plHeader" text="Header:" runat="server" controlname="txtHeader"/></td>
								<td valign="top"><asp:textbox id="txtHeader" style="width:450px" cssclass="NormalTextBox" runat="server" textmode="MultiLine" rows="6"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:225px"><dnn:label id="plFooter" text="Footer:" runat="server" controlname="txtFooter"/></td>
								<td valign="top"><asp:textbox id="txtFooter" style="width:450px" cssclass="NormalTextBox" runat="server" textmode="MultiLine" rows="6"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:150px" valign="top"><dnn:label id="plStartDate" runat="server" controlname="txtStartDate" /></td>
								<td>
									<asp:textbox id="txtStartDate" runat="server" cssclass="NormalTextBox" style="width:120px" columns="30" maxlength="11"/>&nbsp;
									<asp:hyperlink id="cmdStartCalendar" cssclass="CommandButton" runat="server" resourcekey="Calendar"/>
									<asp:CompareValidator ID="valtxtStartDate" ControlToValidate="txtStartDate" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valStartDate.ErrorMessage" />
								</td>
							</tr>
							<tr>
								<td class="SubHead" style="width:150px" valign="top"><dnn:label id="plEndDate" runat="server" controlname="txtEndDate" /></td>
								<td>
									<asp:textbox id="txtEndDate" runat="server" cssclass="NormalTextBox" style="width:120px" columns="30" maxlength="11"/>&nbsp;
									<asp:hyperlink id="cmdEndCalendar" cssclass="CommandButton" runat="server" resourcekey="Calendar"/>
									<asp:CompareValidator ID="valtxtEndDate" ControlToValidate="txtEndDate" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valEndDate.ErrorMessage" />
									<asp:CompareValidator ID="val2txtEndDate" ControlToValidate="txtEndDate" ControlToCompare="txtStartdate" Operator="GreaterThanEqual" Type="Date" Runat="server" Display="Dynamic" resourcekey="valEndDate2.ErrorMessage" />
								</td>
							</tr>
						</table>
						<br />
						<dnn:SectionHead ID="dshModuleInstalledOn" CssClass="Head" runat="server" Text="Module Instance Installed on Tabs" Section="tblInstalledOn" ResourceKey="ModuleInstalledOn" IsExpanded="False" />
						<table id="tblInstalledOn" cellspacing="2" cellpadding="2" summary="Module Instance Installed on Tabs" border="0" runat="server">
							<tr><td>
								<dnn:Label ID="lblInstalledOn" Text="This module instance has been added to these pages" runat="server" />
							</td></tr>
							<tr><td style="padding-left:10px">
								<asp:DataGrid ID="lstInstalledOnTabs" runat="server" AutoGenerateColumns="False" BorderStyle="None" BorderWidth="0" CellPadding="4" AllowPaging="true" PageSize="20" EnableViewState="true" ShowHeader="False">
									<HeaderStyle CssClass="NormalBold" />
									<Columns>
									<asp:TemplateColumn HeaderText="Page">
										<ItemTemplate><%#GetInstalledOnLink(Container.DataItem)%></ItemTemplate>
									</asp:TemplateColumn>
									</Columns>
								</asp:DataGrid>
							</td></tr>
						</table>
					</td>
				</tr>
			</table>
			<br/>
			<dnn:sectionhead id="dshPage" cssclass="Head" runat="server" text="Page Settings" section="tblPage" resourcekey="PageSettings" isexpanded="False" includerule="True" />
			<table id="tblPage" cellspacing="0" cellpadding="2" style="width:725px" summary="Advanced Settings Design Table" border="0" runat="server">
				<tr>
					<td colspan="2"><asp:label id="lblPageSettingsHelp" cssclass="Normal" runat="server" resourcekey="PageSettingsHelp" enableviewstate="False"/></td>
				</tr>
				<tr>
					<td style="width:25px"></td>
					<td valign="top" style="width:675px">
						<dnn:sectionhead id="dshAppearance" cssclass="Head" runat="server" text="Basic Settings" section="tblAppearance" resourcekey="Appearance" />
						<table id="tblAppearance" cellspacing="2" cellpadding="2" summary="Appearance Design Table" border="0" runat="server">
							<tr>
								<td class="SubHead" style="width:200px" valign="top"><dnn:label id="plIcon" text="Icon:" runat="server" controlname="ctlIcon"/></td>
								<td style="width:475px"><dnn:url id="ctlIcon" runat="server" style="width:475px" ShowImages="true" showurls="False" showtabs="False" showlog="False" showtrack="False" required="False" ShowNone="true" /></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plAlign" text="Alignment:" runat="server" controlname="cboAlign"/></td>
								<td valign="top">
									<asp:radiobuttonlist id="cboAlign" cssclass="Normal" runat="server" repeatdirection="Horizontal">
										<asp:listitem resourcekey="Left" value="left" />
										<asp:listitem resourcekey="Center" value="center" />
										<asp:listitem resourcekey="Right" value="right" />
										<asp:listitem resourcekey="Not_Specified" value="" />
									</asp:radiobuttonlist>
								</td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plColor" text="Color:" runat="server" controlname="txtColor"/></td>
								<td valign="top"><asp:textbox id="txtColor" style="width:450px" cssclass="NormalTextBox" runat="server" columns="7"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plBorder" text="Border:" runat="server" controlname="txtBorder"/></td>
								<td valign="top">
									<asp:textbox id="txtBorder" style="width:450px" cssclass="NormalTextBox" runat="server" MaxLength="1"/>
									<asp:CompareValidator ID="valBorder" ControlToValidate="txtBorder" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valBorder.ErrorMessage" />
								</td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plVisibility" text="Visibility:" runat="server" controlname="cboVisibility"/></td>
								<td>
									<asp:radiobuttonlist id="cboVisibility" cssclass="Normal" runat="server" repeatdirection="Horizontal">
										<asp:listitem resourcekey="Maximized" value="0" />
										<asp:listitem resourcekey="Minimized" value="1" />
										<asp:listitem resourcekey="None" value="2" />
									</asp:radiobuttonlist>
								</td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plDisplayTitle" runat="server" controlname="chkDisplayTitle"/></td>
								<td valign="top"><asp:CheckBox ID="chkDisplayTitle" Runat="server" CssClass="NormalTextBox"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plDisplayPrint" runat="server" controlname="chkDisplayPrint"/></td>
								<td valign="top"><asp:CheckBox ID="chkDisplayPrint" Runat="server" CssClass="NormalTextBox"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plDisplaySyndicate" runat="server" controlname="chkDisplaySyndicate"/></td>
								<td valign="top"><asp:CheckBox ID="chkDisplaySyndicate" Runat="server" CssClass="NormalTextBox"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plWebSlice" runat="server" controlname="chkWebSlice"/></td>
								<td valign="top"><asp:CheckBox ID="chkWebSlice" Runat="server" CssClass="NormalTextBox" AutoPostBack="true"/></td>
							</tr>
							<tr>
							    <td colspan="2">
							        <table id="tblWebSlice" runat="server">
							            <tr>
							                <td style="width:20px"></td>
								            <td class="SubHead" style="width:180px"><dnn:label id="plWebSliceTitle" runat="server" controlname="txtWebSliceTitle"/></td>
								            <td valign="top"><asp:TextBox ID="txtWebSliceTitle" runat="server" CssClass="NormalTextBox" style="width:200px" /></td>
							            </tr>
							            <tr>
							                <td style="width:20px"></td>
								            <td class="SubHead" style="width:180px" valign="top"><dnn:label id="plWebSliceExpiry" runat="server" controlname="txtWebSliceExpiry" /></td>
								            <td>
									            <asp:textbox id="txtWebSliceExpiry" runat="server" cssclass="NormalTextBox" style="width:120px" columns="30" maxlength="11"/>&nbsp;
									            <asp:hyperlink id="cmdWebSliceExpiry" cssclass="CommandButton" runat="server" resourcekey="Calendar"/>
									            <asp:CompareValidator ID="valWebSliceExpiry" ControlToValidate="txtWebSliceExpiry" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valWebSliceExpiry.ErrorMessage" />
								            </td>
							            </tr>
							            <tr>
							                <td style="width:20px"></td>
								            <td class="SubHead" style="width:180px"><dnn:label id="plWebSliceTTL" runat="server" controlname="txtWebSliceTTL"/></td>
								            <td valign="top">
								                <asp:TextBox ID="txtWebSliceTTL" runat="server" CssClass="NormalTextBox" style="width:100px" />
									            <asp:CompareValidator ID="valWebSliceTTL" ControlToValidate="txtWebSliceTTL" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valWebSliceTTL.ErrorMessage" />
								            </td>
							            </tr>
							        </table>
							    </td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plModuleContainer" runat="server" controlname="ctlModuleContainer"/></td>
								<td valign="top"><dnn:skin id="ctlModuleContainer" runat="server" DefaultKey="Page" /></td>
							</tr>
						</table>
						<br/>
						<dnn:SectionHead ID="dshCaching" CssClass="Head" runat="server" Text="Cache Settings"
                                Section="tblCaching" ResourceKey="CacheSettings"></dnn:SectionHead>
                            <table id="tblCaching" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
                            border="0" runat="server">
                                <tr>
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheProvider" runat="server" Text="Cache Provider" ControlName="cboCacheProvider" ResourceKey="CacheProvider" HelpKey="CacheProvider.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cboCacheProvider" runat="server" AutoPostBack="true" DataValueField="Key" DataTextField="filteredkey" />&nbsp;&nbsp;
                                        <asp:Label ID="lblCacheInherited" runat="server" resourceKey="CacheInherited" CssClass="NormalRed">Inherited</asp:Label>
                                    </td>
                                </tr>
                                <tr id="trCacheDuration" runat="server" visible="false">
                                    <td class="SubHead" valign="top" width="200">
                                        <dnn:Label ID="lblCacheDuration" runat="server" Text="Cache Duration (seconds)" ControlName="txtCacheDuration" ResourceKey="CacheDuration" HelpKey="CacheDuration.Help"></dnn:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCacheDuration" runat="server" />
                                        <asp:CompareValidator ID="valCacheTime" ControlToValidate="txtCacheDuration" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valCacheTime.ErrorMessage" />
                                        <br /><asp:Label ID="lblCacheDurationWarning" runat="server" ResourceKey="CacheDurationWarning" CssClass="NormalRed"/>
                                    </td>
                                </tr>
                            </table>
                            <br />
						<dnn:sectionhead id="dshOther" cssclass="Head" runat="server" text="Advanced Settings" section="tblOther" resourcekey="OtherSettings" isexpanded="False" />
						<table id="tblOther" cellspacing="2" cellpadding="2" summary="Security Details Design Table" border="0" runat="server">
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plDefault" text="Set As Default Settings?" runat="server" controlname="chkDefault"/></td>
								<td valign="top"><asp:CheckBox ID="chkDefault" Runat="server" CssClass="NormalTextBox"/></td>
							</tr>
							<tr>
								<td class="SubHead" style="width:200px"><dnn:label id="plAllModules" text="Apply To All Modules?" runat="server" controlname="chkAllModules"/></td>
								<td valign="top"><asp:CheckBox ID="chkAllModules" Runat="server" CssClass="NormalTextBox"></asp:CheckBox></td>
							</tr>
							<tr id="rowTab" runat="server">
								<td class="SubHead" style="width:200px"><dnn:label id="plTab" text="Move To Tab:" runat="server" controlname="cboTab"/></td>
								<td>
								    <asp:dropdownlist id="cboTab" style="width:450px" datatextfield="IndentedTabName" datavaluefield="TabId" cssclass="NormalTextBox" runat="server"/>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<br/>
			<dnn:sectionhead id="dshSpecific" cssclass="Head" runat="server" text="Module Specific Settings" section="tblSpecific" isexpanded="True" includerule="True" />
			<table id="tblSpecific" cellspacing="0" cellpadding="2" style="width:525px" summary="Specific Settings Design Table" border="0" runat="server">
				<tr id="rowspecifichelp" runat="server">
					<td colSpan="2" class="NormalBold" align="left">
						<asp:Image id="imgSpecificHelp" runat="server" ImageUrl="~/images/help.gif"/>
						<asp:HyperLink id="lnkSpecificHelp" runat="server" />&nbsp;:&nbsp;
						<asp:label id="lblSpecificSettingsHelp" cssclass="Normal" runat="server" resourcekey="SpecificSettingsHelp" enableviewstate="False" />
					</td>
				</tr>
				<tr>
					<td style="width:25px"></td>
					<td valign="top" style="width:475px"><asp:panel id="pnlSpecific" runat="server"/></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<p>
    <dnn:CommandButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" />&nbsp;
    <dnn:CommandButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="CommandButton" ImageUrl="~/images/delete.gif" CausesValidation="False" />&nbsp;
    <dnn:CommandButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" CausesValidation="False" />
</p>
        <dnn:audit id="ctlAudit" runat="server" />
