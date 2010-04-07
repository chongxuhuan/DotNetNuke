<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanels.RibbonBar" CodeFile="RibbonBar.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="AddModule" Src="~/admin/ControlPanel/AddModule.ascx" %>
<%@ Register TagPrefix="dnn" TagName="AddPage" Src="~/admin/ControlPanel/AddPage.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UpdatePage" Src="~/admin/ControlPanel/UpdatePage.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SwitchSite" Src="~/admin/ControlPanel/SwitchSite.ascx" %>

<div id="RB" runat="server" class="ControlPanel">
<div id="RB_Header" runat="server" class="dnnRibbonBarHeader">
	<table cellspacing="1" cellpadding="1" border="0">
	<tr>
		<td class="dnnRibbonBarCol1">
			<dnn:DnnLabel id="lblMode" runat="server" Text="Mode" CssClass="SubHead" />&nbsp;
			<asp:radiobuttonlist id="optMode" cssclass="SubHead" runat="server" repeatdirection="Horizontal" repeatlayout="Flow" autopostback="True">
				<asp:listitem value="VIEW" resourcekey="ModeView" />
				<asp:listitem value="EDIT" resourcekey="ModeEdit" />
				<asp:listitem value="LAYOUT" resourcekey="ModeLayout" />
			</asp:radiobuttonlist>
		</td>
		<td class="dnnRibbonBarCol2"><asp:HyperLink ID="hypMessage" runat="server" Target="_new" /></td>
		<td class="dnnRibbonBarCol3">
			<dnn:DnnRibbonBarTool ID="cmdAdmin" runat="server" ImageUrl="~/admin/ControlPanel/ribbonimages/Console16px.gif" ToolName="Console" />
			&nbsp;&nbsp;
			<dnn:DnnRibbonBarTool ID="cmdHost" runat="server" ImageUrl="~/admin/ControlPanel/ribbonimages/HostConsole16px.gif" ToolName="HostConsole" />
			&nbsp;&nbsp;
			<asp:LinkButton ID="cmdVisibility" Runat="server" CausesValidation="False"><asp:Image ID="imgVisibility" Runat="server" /></asp:LinkButton>&nbsp;
		</td>
	</tr>
	</table>
</div>
<div id="RB_RibbonBar" runat="server" class="dnnRibbonBar">
<dnn:DnnTabStrip ID="RibbonBarTabs" runat="server" MultiPageID="Pages"
	SelectedIndex="0" Align="Justify" ReorderTabsOnSelect="true" CausesValidation="false">
	<Tabs>
		<telerik:RadTab Text="Common Tasks" PageViewID="PageHome" />
		<telerik:RadTab Text="Current Page" PageViewID="PageCurrent" />
		<telerik:RadTab Text="Site" PageViewID="PageSite" />
		<telerik:RadTab Text="Host" PageViewID="PageHostSystem" />
	</Tabs>
</dnn:DnnTabStrip>
<dnn:DnnMultiPage ID="Pages" runat="server" SelectedIndex="0">
	<telerik:RadPageView ID="PageHome" runat="server">
		<dnn:DnnRibbonBar ID="RB1" runat="server">
			<Groups>
				<dnn:dnnRibbonBarGroup id="G1" runat="server" CssClass="dnnRibbonGroup Tab1_Actions">
					<Content>
						<dnn:DnnRibbonBarTool id="PageSettings" runat="server" ToolName="PageSettings" ToolCssClass="IconTop" />
						<dnn:DnnRibbonBarTool id="NewPage" runat="server" ToolName="NewPage" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CommonTabActions" runat="server" Text="CommonTabActions" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="CommonTabAddPageGroup" runat="server" CssClass="dnnRibbonGroup Tab1_AddPage">
					<Content>
						<dnn:AddPage id="AddPage" runat="server" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CommonTabAddPage" runat="server" Text="CommonTabAddPage" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="CommonTabAddModuleGroup" runat="server" CssClass="dnnRibbonGroup Tab1_AddMod">
					<Content>
						<dnn:AddModule id="AddMod" runat="server" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CommonTabInsertModule" runat="server" Text="CommonTabInsertModule" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
			</Groups>
		</dnn:DnnRibbonBar>
	</telerik:RadPageView>
	<telerik:RadPageView ID="PageCurrent" runat="server">
		<dnn:DnnRibbonBar ID="RB2" runat="server">
			<Groups>
				<dnn:dnnRibbonBarGroup id="G4" runat="server" CssClass="dnnRibbonGroup Tab2_Settings">
					<Content>
						<dnn:DnnRibbonBarTool id="EditCurrentSettings" runat="server" ToolName="PageSettings" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CurrentTabSettings" runat="server" Text="CurrentTabSettings" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G5" runat="server" CssClass="dnnRibbonGroup Tab2_Actions">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="CopyPage" runat="server" ToolName="CopyPage" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="DeletePage" runat="server" ToolName="DeletePage" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="ImportPage" runat="server" ToolName="ImportPage" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="ExportPage" runat="server" ToolName="ExportPage" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CurrentTabActions" runat="server" Text="CurrentTabActions" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G6" runat="server" CssClass="dnnRibbonGroup Tab2_EditPage">
					<Content>
						<dnn:UpdatePage id="EditPage" runat="server" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CurrentTabEditPage" runat="server" Text="CurrentTabEditPage" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G7" runat="server" CssClass="dnnRibbonGroup Tab2_Copy">
					<Content>
						<dnn:DnnRibbonBarTool id="CopyPermissionsToChildren" runat="server" ToolName="CopyPermissionsToChildren" ToolCssClass="IconLeft" />
						<dnn:DnnRibbonBarTool id="CopyDesignToChildren" runat="server" ToolName="CopyDesignToChildren" ToolCssClass="IconLeft" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CurrentTabCopyToChildren" runat="server" Text="CurrentTabCopyToChildren" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G8" runat="server" CssClass="dnnRibbonGroup Tab2_Help">
					<Content>
						<dnn:DnnRibbonBarTool id="Help" runat="server" ToolName="Help" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="CurrentTabHelp" runat="server" Text="CurrentTabHelp" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
			</Groups>
		</dnn:DnnRibbonBar>
	</telerik:RadPageView>
	<telerik:RadPageView ID="PageSite" runat="server">
		<dnn:DnnRibbonBar ID="RB3" runat="server">
			<Groups>
				<dnn:dnnRibbonBarGroup id="G9" runat="server" CssClass="dnnRibbonGroup Tab3_Settings">
					<Content>
						<dnn:DnnRibbonBarTool id="SiteSettings" runat="server" ToolName="SiteSettings" ToolCssClass="IconTop" />
						<dnn:DnnRibbonBarTool id="SiteConsole" runat="server" ToolName="Console" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabSettings" runat="server" Text="SiteTabSettings" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G10" runat="server" CssClass="dnnRibbonGroup Tab3_Manage">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="Users" runat="server" ToolName="Users" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="NewUser" runat="server" ToolName="NewUser" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="UserRoles" runat="server" ToolName="UserRoles" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="NewRole" runat="server" ToolName="NewRole" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabUsers" runat="server" Text="SiteTabUsers" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G101" runat="server" CssClass="dnnRibbonGroup Tab3_Manage">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="ManagePages" runat="server" ToolName="ManagePages" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="SiteNewPage" runat="server" ToolName="NewPage" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="ManageFiles" runat="server" ToolName="FileManager" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="UploadFile" runat="server" ToolName="UploadFile" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabManage" runat="server" Text="SiteTabManage" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G121" runat="server" CssClass="dnnRibbonGroup Tab3_Features">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="NewsLetters" runat="server" ToolName="Newsletters" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="Extensions" runat="server" ToolName="Extensions" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="Skins" runat="server" ToolName="Skins" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="Languages" runat="server" ToolName="Languages" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabFeatures" runat="server" Text="SiteTabFeatures" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G12" runat="server" CssClass="dnnRibbonGroup Tab3_Features">
					<Content>
						<dnn:DnnRibbonBarTool id="EventLog" runat="server" ToolName="EventLog" ToolCssClass="IconTop" />
						<dnn:DnnRibbonBarTool id="SiteLog" runat="server" ToolName="SiteLog" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabLogging" runat="server" Text="SiteTabLogging" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G13" runat="server" CssClass="dnnRibbonGroup Tab3_Restore">
					<Content>
						<dnn:DnnRibbonBarTool id="RecycleBin" runat="server" ToolName="RecycleBin" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SiteTabRestore" runat="server" Text="SiteTabRestore" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
			</Groups>
		</dnn:DnnRibbonBar>
	</telerik:RadPageView>
	<telerik:RadPageView ID="PageHostSystem" runat="server">
		<dnn:DnnRibbonBar ID="RB4" runat="server">
			<Groups>
				<dnn:dnnRibbonBarGroup id="G14" runat="server" CssClass="dnnRibbonGroup Tab4_Settings">
					<Content>
						<dnn:DnnRibbonBarTool id="HostSettings" runat="server" ToolName="HostSettings" ToolCssClass="IconTop" />
						<dnn:DnnRibbonBarTool id="HostConsole" runat="server" ToolName="HostConsole" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabSettings" runat="server" Text="SystemTabSettings" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G15" runat="server" CssClass="dnnRibbonGroup Tab4_Manage">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="SuperUsers" runat="server" ToolName="SuperUsers" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="HostFileManager" runat="server" ToolName="HostFileManager" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="HostExtensions" runat="server" ToolName="HostExtensions" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="Sites" runat="server" ToolName="Sites" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabManage" runat="server" Text="SystemTabManage" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G16" runat="server" CssClass="dnnRibbonGroup Tab4_Info">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="Dashboard" runat="server" ToolName="Dashboard" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="WhatsNew" runat="server" ToolName="WhatsNew" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="WebServerManager" runat="server" ToolInfo-ToolName="WebServerManager" ToolInfo-IsHostTool="True" ToolInfo-ModuleFriendlyName="WebServerManager" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="SupportTickets" runat="server" ToolInfo-ToolName="SupportTickets" ToolInfo-IsHostTool="True" ToolInfo-LinkWindowTarget="_Blank" NavigateUrl="http://customers.dotnetnuke.com/Main/frmTickets.aspx" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabInformation" runat="server" Text="SystemTabInformation" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G17" runat="server" CssClass="dnnRibbonGroup Tab4_Tools">
					<Content>
						<table border="0" cellpadding="0" cellspacing="0">
						<tr><td class="dnnRibbonActionsStart">
							<dnn:DnnRibbonBarTool id="Scheduler" runat="server" ToolName="Scheduler" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="SQL" runat="server" ToolName="SQL" ToolCssClass="IconTop" />
						</td><td class="dnnRibbonActions">
							<dnn:DnnRibbonBarTool id="ImpersonateUser" runat="server" ToolInfo-ToolName="ImpersonateUser" ToolInfo-IsHostTool="True" ToolInfo-ModuleFriendlyName="User Switcher" ToolCssClass="IconTop" />
							<dnn:DnnRibbonBarTool id="IntegrityChecker" runat="server" ToolInfo-ToolName="IntegrityChecker" ToolInfo-IsHostTool="True" ToolInfo-ModuleFriendlyName="IntegrityChecker" ToolCssClass="IconTop" />
						</td></tr>
						</table>
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabTools" runat="server" Text="SystemTabTools" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G18" runat="server" CssClass="dnnRibbonGroup Tab4_SwitchSite">
					<Content>
						<dnn:SwitchSite id="SwitchSite" runat="server" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabSwitchSite" runat="server" Text="SystemTabSwitchSite" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
				<dnn:dnnRibbonBarGroup id="G19" runat="server" CssClass="dnnRibbonGroup Tab4_Marketplace">
					<Content>
						<dnn:DnnRibbonBarTool id="Marketplace" runat="server" ToolName="Marketplace" ToolCssClass="IconTop" />
					</Content>
					<Footer>
						<dnn:DnnLiteral id="SystemTabMarketplace" runat="server" Text="SystemTabMarketplace" />
					</Footer>
				</dnn:dnnRibbonBarGroup>
			</Groups>
		</dnn:DnnRibbonBar>
	</telerik:RadPageView>
</dnn:DnnMultiPage>
</div>
</div>

