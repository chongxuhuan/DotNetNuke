<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.AddModule" CodeFile="AddModule.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<asp:UpdatePanel ID="UpdateAddModule" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="dnnFormItem dnnCPAddModule dnnClear">
            <dnn:DnnFieldLabel id="AddModuleLabel" runat="server" Text="AddModule" />
            <dnn:DnnRadioButton ID="AddNewModule" runat="server" Text="AddNew" GroupName="AddModule" Checked="True" AutoPostBack="true" CssClass="cpamAddNew" />
            <dnn:DnnRadioButton ID="AddExistingModule" runat="server" Text="AddExisting" GroupName="AddModule" AutoPostBack="true" CssClass="cpamAddExisting" />
        </div>
		<div class="dnnFormItem cpamSelectOption dnnClear">
            <asp:Panel id="CategoryListPanel" runat="server" visible="false" CssClass="amCatList">
                <dnn:DnnFieldLabel id="CategoryListLbl" runat="server" Text="Category" AssociatedControlID="CategoryList" />
                <asp:DropDownList ID="CategoryList" runat="server" MaxHeight="300px" AutoPostBack="true"  DataTextField="Name" DataValueField="Name" />
            </asp:Panel>
            <asp:Panel ID="PageListPanel" runat="server" Visible="false" class="amPageList">
                <dnn:DnnFieldLabel id="PageListLbl" runat="server" Text="Page" AssociatedControlID="PageLst" />
                <asp:DropDownList ID="PageLst" runat="server" MaxHeight="300px" AutoPostBack="true" />
            </asp:Panel>
            <div class="amModuleList">
                <dnn:DnnFieldLabel id="ModuleLstLbl" runat="server" Text="Module" AssociatedControlID="ModuleLst" />
                <dnn:DnnModuleComboBox ID="ModuleLst" runat="server" />
            </div>
        </div>
		<div class="dnnFormItem amTitleVisibility dnnClear">
            <asp:Panel ID="TitlePanel" runat="server" Visible="false" CssClass="cpamModuleTitle">
                <dnn:DnnFieldLabel id="TitleLbl" runat="server" Text="Title" AssociatedControlID="Title" />
                <asp:TextBox ID="Title" runat="server" Height="14px" />
            </asp:Panel>
            <div class="cpamModuleVisibility dnnClear">
                <dnn:DnnFieldLabel id="VisibilityLstLbl" runat="server" Text="Visibility" AssociatedControlID="VisibilityLst" />
                <asp:DropDownList ID="VisibilityLst" runat="server" />
            </div>
		</div>
		<div class="dnnFormItem cpamModulePlacement dnnClear">
            <div class="dnnFormItem dnnLeft mpPaneList">
                <dnn:DnnFieldLabel id="PaneLstLbl" runat="server" Text="Pane" AssociatedControlID="PaneLst" />
                <asp:DropDownList ID="PaneLst" runat="server" AutoPostBack="true" />
            </div>
            <div class="mpPlacementPosition">
                <dnn:DnnFieldLabel id="PositionLstLbl" runat="server" Text="Insert" AssociatedControlID="PositionLst" />
                <asp:DropDownList ID="PositionLst" runat="server" AutoPostBack="true" />
            </div>
            <div class="mpModuleList">
                <dnn:DnnFieldLabel id="PaneModulesLstLbl" runat="server" Text="Module" AssociatedControlID="PaneModulesLst" />
                <asp:DropDownList ID="PaneModulesLst" runat="server" />
            </div>
    	</div>
		<div class="cpamCopyModule dnnClear"><asp:CheckBox ID="chkCopyModule" runat="server" /></div>                        
        <dnn:DnnButton ID="cmdAddModule" runat="server" Text="AddModule" CssClass="dnnPrimaryAction" />
    </ContentTemplate>
</asp:UpdatePanel>