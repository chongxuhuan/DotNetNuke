<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanels.IconBar" CodeFile="IconBar.ascx.cs" %>
<table id="tblControlPanel" runat="server" class="ControlPanel" cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td>
			<table cellspacing="1" cellpadding="1" style="width:100%;">
				<tr>
					<td style="text-align:left; vertical-align:middle; width:33%; white-space: nowrap;">
					    &nbsp;<asp:Label ID="lblMode" Runat="server" CssClass="SubHead" enableviewstate="False" />
						<asp:radiobuttonlist id="optMode" cssclass="SubHead" runat="server" repeatdirection="Horizontal" repeatlayout="Flow" autopostback="True">
							<asp:listitem value="VIEW" resourcekey="ModeView" />
							<asp:listitem value="EDIT" resourcekey="ModeEdit" />
							<asp:listitem value="LAYOUT" resourcekey="ModeLayout" />
						</asp:radiobuttonlist>
					</td>
					<td style="text-align:center; vertical-align:middle; width:33%;"><asp:HyperLink ID="hypMessage" runat="server" Target="_new" /></td>
					<td style="text-align:right; vertical-align:middle; white-space: nowrap; width:33%;">
                        <asp:ImageButton ID="imgAdmin" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_console.gif" CausesValidation="False" Visible="false" />
                        <asp:linkbutton id="cmdAdmin" runat="server" cssclass="CommandButton" CausesValidation="False" />&nbsp;&nbsp;&nbsp;
						<asp:ImageButton ID="imgHost" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_hostconsole.gif" CausesValidation="False"  Visible="false" />
                        <asp:linkbutton id="cmdHost" runat="server" cssclass="CommandButton" CausesValidation="False" />&nbsp;
                        <asp:LinkButton ID="cmdVisibility" Runat="server" CausesValidation="False"><asp:Image ID="imgVisibility" Runat="server" /></asp:LinkButton>&nbsp;
					</td>
				</tr>
			</table>
			<table cellspacing="1" cellpadding="1" style="width:100%;">
				<tr id="rowControlPanel" runat="server">
					<td style="border-top:1px #CCCCCC dotted; text-align:center; vertical-align:middle; width:25%;">
                        <asp:Label ID="lblPageFunctions" Runat="server" CssClass="SubHead" enableviewstate="False" />
						 <table  border="0" cellpadding="2" cellspacing="0" style="margin: 0 auto;">
							<tr style="height:24px; vertical-align:bottom">
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdAddTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgAddTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_addtab.gif" />
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdEditTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgEditTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_edittab.gif" />
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdDeleteTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgDeleteTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_deletetab.gif" />
									</asp:LinkButton>
								</td>
							</tr>
							<tr style="vertical-align:bottom">
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdAddTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdEditTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdDeleteTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
							</tr>
							<tr style="height:24px; vertical-align:bottom">
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdCopyTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgCopyTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_copytab.gif" />
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdExportTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgExportTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_exporttab.gif" />
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdImportTabIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgImportTabIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_importtab.gif" />
									</asp:LinkButton>
								</td>
							</tr>
							<tr style="vertical-align:bottom">
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdCopyTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdExportTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
								<td style="width:60px; text-align:center;" class="Normal"><asp:LinkButton ID="cmdImportTab" Runat="server" CssClass="CommandButton" CausesValidation="False" /></td>
							</tr>
						</table>
					</td>
					<td style="border-left:1px #CCCCCC dotted; border-right:1px #CCCCCC dotted; border-top:1px #CCCCCC dotted; text-align:center; vertical-align:top; width:50%;">
					    <asp:Panel ID="pnlModules" runat="server">
						    <asp:radiobuttonlist id="optModuleType" cssclass="SubHead" runat="server" repeatdirection="Horizontal" repeatlayout="Flow" autopostback="True">
							    <asp:listitem value="0" resourcekey="optModuleTypeNew" />
							    <asp:listitem value="1" resourcekey="optModuleTypeExisting" />
						    </asp:radiobuttonlist>
						    <table cellspacing="1" cellpadding="2" border="0">
							    <tr>
								    <td align="center">
									    <table cellspacing="1" cellpadding="0" border="0">
							                <tr style="vertical-align:bottom">
											    <td class="SubHead" style="text-align:right; white-space: nowrap;"><asp:Label ID="lblModule" Runat="server" CssClass="SubHead" enableviewstate="False" />&nbsp;</td>
											    <td style="white-space: nowrap;">
											        <asp:dropdownlist id="cboTabs" runat="server" cssclass="NormalTextBox" Width="200" datavaluefield="TabID" datatextfield="IndentedTabName" visible="False" autopostback="True" />
										                <asp:dropdownlist id="cboDesktopModules" runat="server" cssclass="NormalTextBox" Width="200" datavaluefield="DesktopModuleID" datatextfield="FriendlyName"/>&nbsp;&nbsp;
											    </td>
											    <td class="SubHead" style="text-align:right; white-space: nowrap;"><asp:Label ID="lblPane" Runat="server" CssClass="SubHead" enableviewstate="False" />&nbsp;</td>
											    <td style="white-space: nowrap;"><asp:dropdownlist id="cboPanes" runat="server" cssclass="NormalTextBox" Width="120" autopostback="True"/>&nbsp;&nbsp;</td>
										    </tr>
							                <tr style="vertical-align:bottom">
											    <td class="SubHead"  style="text-align:right; white-space: nowrap;"><asp:Label ID="lblTitle" Runat="server" CssClass="SubHead" enableviewstate="False"/>&nbsp;</td>
											    <td style="white-space: nowrap;">
											        <asp:dropdownlist id="cboModules" runat="server" cssclass="NormalTextBox" Width="200" datavaluefield="ModuleID" datatextfield="ModuleTitle" visible="False" />
											        <asp:TextBox ID="txtTitle" Runat="server" CssClass="NormalTextBox" Width="195" />&nbsp;&nbsp;
											    </td>
											    <td class="SubHead" style="text-align:right; white-space: nowrap;"><asp:Label ID="lblPosition" Runat="server" CssClass="SubHead" resourcekey="Position" enableviewstate="False" />&nbsp;</td>
											    <td style="white-space: nowrap;">
												    <asp:dropdownlist id="cboPosition" runat="server" CssClass="NormalTextBox" Width="120" AutoPostBack="true"/>&nbsp;&nbsp;
											    </td>
										    </tr>
							                <tr style="vertical-align:bottom">
											    <td class="SubHead" style="text-align:right; white-space: nowrap;"><asp:Label ID="lblPermission" Runat="server" CssClass="SubHead" resourcekey="Permission" enableviewstate="False" />&nbsp;</td>
											    <td style="white-space: nowrap;">
												    <asp:dropdownlist id="cboPermission" runat="server" CssClass="NormalTextBox" Width="200">
													    <asp:ListItem Value="0" resourcekey="PermissionView" />
													    <asp:ListItem Value="1" resourcekey="PermissionEdit"/>
												    </asp:dropdownlist>&nbsp;&nbsp;
											    </td>
											    <td class="SubHead" style="text-align:right; white-space: nowrap;"><asp:Label ID="lblInstance" Runat="server" CssClass="SubHead" enableviewstate="False" resourcekey="Instance" /></td>
											    <td style="white-space: nowrap;">
												    <asp:dropdownlist id="cboInstances" runat="server" CssClass="NormalTextBox" Width="120"></asp:dropdownlist>&nbsp;&nbsp;
											    </td>
										    </tr>
									    </table>
								    </td>
							    </tr>
                                <tr>
                                        <td>
										    <asp:ImageButton ID="imgAddModule" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_addmodule.gif" CausesValidation="False" />
                                            <asp:LinkButton id="cmdAddModule" runat="server" cssclass="CommandButton" CausesValidation="False" />
                                        </td>
                                </tr>
                        </table>
					    </asp:Panel>
					</td>
					<td style="border-top:1px #CCCCCC dotted; text-align:center; vertical-align:middle; width:25%;">
                        <asp:Label ID="lblCommonTasks" Runat="server" CssClass="SubHead" enableviewstate="False"/>
						 <table  border="0" cellpadding="2" cellspacing="0" style="margin: 0 auto;">
							<tr style="height:24px; vertical-align:bottom">
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdSiteIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgSiteIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_site.gif" />
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdUsersIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgUsersIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_users.gif"></asp:Image>
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdRolesIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgRolesIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_roles.gif"></asp:Image>
									</asp:LinkButton>
								</td>
							</tr>
							<tr valign="bottom">
								<td style="width:60px;" align="center" class="Normal"><asp:LinkButton ID="cmdSite" Runat="server" CssClass="CommandButton" CausesValidation="False"/></td>
								<td style="width:60px;" align="center" class="Normal"><asp:LinkButton ID="cmdUsers" Runat="server" CssClass="CommandButton" CausesValidation="False"/></td>
								<td style="width:60px;" align="center" class="Normal"><asp:LinkButton ID="cmdRoles" Runat="server" CssClass="CommandButton" CausesValidation="False"/></td>
							</tr>
							<tr style="height:24px; vertical-align:bottom">
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdFilesIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgFilesIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_files.gif"></asp:Image>
									</asp:LinkButton>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:Hyperlink ID="cmdHelpIcon" Runat="server" CssClass="CommandButton" CausesValidation="False" Target="_new">
										<asp:Image ID="imgHelpIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_help.gif"></asp:Image>
									</asp:Hyperlink>
								</td>
								<td style="width:60px; text-align:center;">
								    <asp:LinkButton ID="cmdExtensionsIcon" Runat="server" CssClass="CommandButton" CausesValidation="False">
										<asp:Image ID="imgExtensionsIcon" Runat="server" ImageUrl="~/admin/ControlPanel/images/iconbar_extensions.gif"></asp:Image>
									</asp:LinkButton>
								</td>
							</tr>
							<tr valign="bottom">
								<td style="width:60px;" align="center" class="Normal"><asp:LinkButton ID="cmdFiles" Runat="server" CssClass="CommandButton" CausesValidation="False"/></td>
								<td style="width:60px;" align="center" class="Normal"><asp:Hyperlink ID="cmdHelp" Runat="server" CssClass="CommandButton" CausesValidation="False" Target="_new"/></td>
								<td style="width:60px;" align="center" class="Normal"><asp:LinkButton ID="cmdExtensions" Runat="server" CssClass="CommandButton" CausesValidation="False"/></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
