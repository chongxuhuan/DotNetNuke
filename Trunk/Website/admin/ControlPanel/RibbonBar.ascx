<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanels.RibbonBar" CodeFile="RibbonBar.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="AddModule" Src="~/admin/ControlPanel/AddModule.ascx" %>
<%@ Register TagPrefix="dnn" TagName="AddPage" Src="~/admin/ControlPanel/AddPage.ascx" %>
<%@ Register TagPrefix="dnn" TagName="UpdatePage" Src="~/admin/ControlPanel/UpdatePage.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SwitchSite" Src="~/admin/ControlPanel/SwitchSite.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.DDRMenu.TemplateEngine" Assembly="DotNetNuke.Web.DDRMenu" %>
<%@ Register TagPrefix="dnn" TagName="MENU" src="~/DesktopModules/DDRMenu/Menu.ascx" %>
<asp:Panel id="ControlPanel" runat="server" CssClass="dnnForm dnnControlPanel dnnClear">
    <div class="dnnCPHeader dnnClear">
        <div class="dnnCPHMode dnnLeft">
            <dnn:MENU ID="adminMenus" MenuStyle="admin/Menus/DNNAdmin" IncludeHidden="True" runat="server" OnInit="DetermineNodesToInclude" />
            <asp:HyperLink ID="hypMessage" runat="server" Target="_new" CssClass="dnnCPHMessage dnnLeft" />
        </div>
        <div class="dnnCPHNav dnnRight">
            <asp:Label id="lblMode" runat="server" ResourceKey="Mode" />
            <asp:DropDownList ID="ddlMode" runat="server" AutoPostBack="true">
                <asp:listitem value="VIEW" ResourceKey="ModeView" />
                <asp:listitem value="EDIT" ResourceKey="ModeEdit" />
                <asp:listitem value="LAYOUT" ResourceKey="ModeLayout" />
            </asp:DropDownList>
            <asp:LinkButton ID="cmdVisibility" Runat="server" CausesValidation="False"><asp:Image ID="imgVisibility" Runat="server" /></asp:LinkButton>
        </div>
    </div>
    <asp:Panel ID="BodyPanel" runat="server" CssClass="dnnCPContent">
        <asp:Panel ID="CommonTasksPanel" runat="server" CssClass="cpcbCommonTasks dnnClear">

            <div class="cbctAddModule dnnLeft"><dnn:AddModule id="AddMod" runat="server" /></div>
        </asp:Panel>
        <asp:Panel ID="CurrentPagePanel" runat="server" CssClass="cpcbCurrentPage dnnClear">
        	<div class="cbcpFiCol dnnLeft">
                <div class="cbcpPageSettings dnnClear">
                    <h4><dnn:DnnLiteral id="CurrentTabSettings" runat="server" Text="CurrentTabSettings" /></h4>
                    <dnn:DnnRibbonBarTool id="EditCurrentSettings" runat="server" ToolName="PageSettings" ToolCssClass="cpEditCurrentPage" />
                </div>
                <div class="cbcpPageActions dnnClear">
                    <h4><dnn:DnnLiteral id="CurrentTabActions" runat="server" Text="CurrentTabActions" /></h4>
                    <dnn:DnnRibbonBarTool id="NewPage" runat="server" ToolName="NewPage" ToolCssClass="cpAddNewPage" />
                    <dnn:DnnRibbonBarTool id="CopyPage" runat="server" ToolName="CopyPage" ToolCssClass="cpCopyPage" />
                    <dnn:DnnRibbonBarTool id="DeletePage" runat="server" ToolName="DeletePage" ToolCssClass="cpDeletePage" />
                    <dnn:DnnRibbonBarTool id="ImportPage" runat="server" ToolName="ImportPage" ToolCssClass="cpImportPage" />
                    <dnn:DnnRibbonBarTool id="ExportPage" runat="server" ToolName="ExportPage" ToolCssClass="cpExportPage" />
                </div>
                <div class="cbcpPageCopy dnnClear">
                    <h4><dnn:DnnLiteral id="CurrentTabCopyToChildren" runat="server" Text="CurrentTabCopyToChildren" /></h4>
                    <dnn:DnnRibbonBarTool id="CopyPermissionsToChildren" runat="server" ToolName="CopyPermissionsToChildren" ToolCssClass="cpCopyPermissions"/>
                    <dnn:DnnRibbonBarTool id="CopyDesignToChildren" runat="server" ToolName="CopyDesignToChildren" ToolCssClass="cpCopyDesign" />
                </div>
                <div class="cbcpPageHelp dnnClear">
                    <h4><dnn:DnnLiteral id="CurrentTabHelp" runat="server" Text="CurrentTabHelp" /></h4>
                    <dnn:DnnRibbonBarTool id="Help" runat="server" ToolName="Help" ToolCssClass="cpPageHelp" />
                </div>
            </div>
            <div class="cbctAddPage dnnLeft">
                <h4><dnn:DnnLiteral id="CurrentTabAddPage" runat="server" Text="CurrentTabAddPage" /></h4>
                <dnn:AddPage id="AddPage" runat="server" />
            </div>
            <div class="cbcpPageEdit dnnLeft">
                <h4><dnn:DnnLiteral id="CurrentTabEditPage" runat="server" Text="CurrentTabEditPage" /></h4>
                <dnn:UpdatePage id="EditPage" runat="server" />
            </div>
        </asp:Panel>
        <asp:Panel ID="AdminPanel" runat="server" CssClass="cpcbAdmin">
            <div class="cbaManage dnnClear">
				<h4><dnn:DnnLiteral id="SiteTabManage" runat="server" Text="SiteTabManage" /></h4>
                <dnn:DnnRibbonBarTool id="NewUser" runat="server" ToolName="NewUser" ToolCssClass="cpNewUser" />
                <dnn:DnnRibbonBarTool id="NewRole" runat="server" ToolName="NewRole" ToolCssClass="cpNewRole" />
                <dnn:DnnRibbonBarTool id="UploadFile" runat="server" ToolName="UploadFile" ToolCssClass="cpUploadFile" />
            </div>
            <asp:Panel runat="server" ID="AdvancedToolsPanel" CssClass="cbhTools dnnClear">
                <h4><dnn:DnnLiteral id="SystemTabTools" runat="server" Text="SystemTabTools" /></h4>
                <dnn:DnnRibbonBarTool id="WebServerManager" runat="server" ToolInfo-ToolName="WebServerManager" ToolInfo-IsHostTool="True" ToolInfo-ModuleFriendlyName="WebServerManager" ToolCssClass="cpWebServerManager" />
                <dnn:DnnRibbonBarTool id="SupportTickets" runat="server" ToolInfo-ToolName="SupportTickets" ToolInfo-IsHostTool="True" ToolInfo-LinkWindowTarget="_Blank" NavigateUrl="http://customers.dotnetnuke.com/Main/frmTickets.aspx" ToolCssClass="cpSupportTickets" />
                <dnn:DnnRibbonBarTool id="ImpersonateUser" runat="server" ToolInfo-ToolName="ImpersonateUser" ToolInfo-IsHostTool="False" ToolInfo-ModuleFriendlyName="UserSwitcher" ToolCssClass="cpImpersonateUser" />
                <dnn:DnnRibbonBarTool id="IntegrityChecker" runat="server" ToolInfo-ToolName="IntegrityChecker" ToolInfo-IsHostTool="True" ToolInfo-ModuleFriendlyName="IntegrityChecker" ToolCssClass="cpIntegrityChecker" />
            </asp:Panel>
            <div class="cbhSwitchSite dnnClear">
                <h4><dnn:DnnLiteral id="SystemTabSwitchSite" runat="server" Text="SystemTabSwitchSite" /></h4>
                <dnn:SwitchSite id="SwitchSite" runat="server" />
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(function () {
//        $('#<%= BodyPanel.ClientID %>').dnnTabs();
        var yesText = '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>';
        var noText = '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>';
        var titleText = '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>';

        // Client IDs for the following three have _CPCommandBtn appended as a rule
        $('#<%= DeletePage.ClientID %>_CPCommandBtn').dnnConfirm({
            text: '<%= GetButtonConfirmMessage("DeletePage") %>',
            yesText: yesText,
            noText: noText,
            title: titleText
        });
        $('#<%= CopyPermissionsToChildren.ClientID %>_CPCommandBtn').dnnConfirm({
            text: '<%= GetButtonConfirmMessage("CopyPermissionsToChildren") %>',
            yesText: yesText,
            noText: noText,
            title: titleText
        });
        $('#<%= CopyDesignToChildren.ClientID %>_CPCommandBtn').dnnConfirm({
            text: '<%= GetButtonConfirmMessage("CopyDesignToChildren") %>',
            yesText: yesText,
            noText: noText,
            title: titleText
        });
	});
</script>