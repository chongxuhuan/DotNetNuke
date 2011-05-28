<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.AddModule"
    CodeFile="AddModule.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<asp:UpdatePanel ID="UpdateAddModule" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="dnnCPAddModule dnnClear">
            <h4>
                <asp:Label runat="server" ResourceKey="AddModule" />
            </h4>
            <asp:RadioButton ID="AddNewModule" runat="server" ResourceKey="AddNew" GroupName="AddModule"
                Checked="True" AutoPostBack="true" CssClass="cpamAddNew" />
            <asp:RadioButton ID="AddExistingModule" runat="server" ResourceKey="AddExisting" GroupName="AddModule"
                AutoPostBack="true" CssClass="cpamAddExisting" />
        </div>
        <div class="cpAddModuleContent">
            <div class="dnnFormItem cpamSelectOption dnnClear">
                <asp:Panel ID="CategoryListPanel" runat="server" Visible="false" CssClass="amCatList dnnLeft">
                    <asp:Label ID="CategoryListLbl" runat="server" ResourceKey="Category" AssociatedControlID="CategoryList" />
                    <asp:DropDownList ID="CategoryList" runat="server" MaxHeight="300px" AutoPostBack="true"
                        DataTextField="Name" DataValueField="Name" />
                </asp:Panel>
                <asp:Panel ID="PageListPanel" runat="server" Visible="false" class="amPageList dnnLeft">
                    <asp:Label ID="PageListLbl" runat="server" ResourceKey="Page" AssociatedControlID="PageLst" />
                    <asp:DropDownList ID="PageLst" runat="server" MaxHeight="300px" AutoPostBack="true" />
                </asp:Panel>
                <div class="amModuleList dnnLeft">
                    <asp:Label ID="ModuleLstLbl" runat="server" ResourceKey="Module" AssociatedControlID="ModuleLst" />
                    <dnn:DnnModuleComboBox ID="ModuleLst" runat="server" />
                </div>
            </div>
            <div class="dnnFormItem amTitleVisibility dnnClear">
                <asp:Panel ID="TitlePanel" runat="server" Visible="false" CssClass="cpamModuleTitle dnnLeft">
                    <asp:Label ID="TitleLbl" runat="server" ResourceKey="Title" AssociatedControlID="Title" />
                    <asp:TextBox ID="Title" runat="server" />
                </asp:Panel>
                <div class="cpamModuleVisibility dnnLeft">
                    <asp:Label ID="VisibilityLstLbl" runat="server" ResourceKey="Visibility" AssociatedControlID="VisibilityLst" />
                    <asp:DropDownList ID="VisibilityLst" runat="server" />
                </div>
            </div>
            <div class="dnnFormItem cpamModulePlacement dnnClear">
                <div class="mpPaneList dnnLeft">
                    <asp:Label ID="PaneLstLbl" runat="server" ResourceKey="Pane" AssociatedControlID="PaneLst" />
                    <asp:DropDownList ID="PaneLst" runat="server" AutoPostBack="true" />
                </div>
                <div class="mpPlacementPosition dnnLeft">
                    <asp:Label ID="PositionLstLbl" runat="server" ResourceKey="Insert" AssociatedControlID="PositionLst" />
                    <asp:DropDownList ID="PositionLst" runat="server" AutoPostBack="true" />
                </div>
                <div class="mpModuleList dnnLeft">
                    <asp:Label ID="PaneModulesLstLbl" runat="server" ResourceKey="Module" AssociatedControlID="PaneModulesLst" />
                    <asp:DropDownList ID="PaneModulesLst" runat="server" />
                </div>
            </div>
            <div>
                <asp:CheckBox ID="chkCopyModule" runat="server" CssClass="cpamCopyModule" /></div>
            <asp:button ID="cmdAddModule" runat="server" ResourceKey="AddModule" CssClass="dnnPrimaryAction" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
