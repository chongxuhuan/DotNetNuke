<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.AddModule"
    CodeFile="AddModule.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<asp:UpdatePanel ID="UpdateAddModule" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <h4>
            <asp:Label runat="server" ResourceKey="AddModule" />
        </h4>
        <div class="cpAddModuleContent  dnnCPContainer">
            <div class="dnnCPAddModule">
                <asp:RadioButton ID="AddNewModule" runat="server" ResourceKey="AddNew" GroupName="AddModule"
                    Checked="true" AutoPostBack="true" CssClass="cpamAddNew" />
                <asp:RadioButton ID="AddExistingModule" runat="server" ResourceKey="AddExisting"
                    GroupName="AddModule" AutoPostBack="true" CssClass="cpamAddExisting" />
            </div>
            <h5>
                <asp:Label ID="Label1" runat="server" ResourceKey="SelectModule" />
            </h5>
            <div class="dnnCPModuleSelection dnnCPContainer dnnFormItem">
                <asp:Panel ID="CategoryListPanel" runat="server" Visible="false" CssClass="cpamLeft amCatList">
                    <asp:Label ID="CategoryListLbl" runat="server" ResourceKey="Category" AssociatedControlID="CategoryList" />
                    <asp:DropDownList ID="CategoryList" runat="server" MaxHeight="300px" AutoPostBack="true"
                        DataTextField="Name" DataValueField="Name" />
                </asp:Panel>
                <asp:Panel ID="PageListPanel" runat="server" Visible="false" class="cpamLeft">
                    <asp:Label ID="PageListLbl" runat="server" ResourceKey="Page" AssociatedControlID="PageLst" />
                    <asp:DropDownList ID="PageLst" runat="server" MaxHeight="300px" AutoPostBack="true" />
                </asp:Panel>
                <div class="cpamRight">
                    <asp:Label ID="ModuleLstLbl" runat="server" ResourceKey="Module" AssociatedControlID="ModuleLst" />
                    <dnn:DnnModuleComboBox ID="ModuleLst" runat="server" />
                </div>
                <asp:Panel ID="TitlePanel" runat="server" Visible="true" CssClass="cpamLeft dnnClear">
                    <asp:Label ID="TitleLbl" runat="server" ResourceKey="Title" AssociatedControlID="Title" />
                    <asp:TextBox ID="Title" runat="server" />
                </asp:Panel>
                <div class="cpamRight">
                    <asp:Label ID="VisibilityLstLbl" runat="server" ResourceKey="Visibility" AssociatedControlID="VisibilityLst" />
                    <asp:DropDownList ID="VisibilityLst" runat="server" />
                </div>
            </div>
            <h5>
                <asp:Label ID="Label2" runat="server" ResourceKey="LocateModule" />
            </h5>
            <div class="dnnCPModuleLocation  dnnCPContainer dnnFormItem">
                <div class="cpamLeft">
                    <asp:Label ID="PaneLstLbl" runat="server" ResourceKey="Pane" AssociatedControlID="PaneLst" />
                    <asp:DropDownList ID="PaneLst" runat="server" AutoPostBack="true" />
                </div>
                <div class="cpamLeft">
                    <asp:Label ID="PositionLstLbl" runat="server" ResourceKey="Insert" AssociatedControlID="PositionLst" />
                    <asp:DropDownList ID="PositionLst" runat="server" AutoPostBack="true" />
                </div>
                <div class="cpamLeft">
                    <asp:Label ID="PaneModulesLstLbl" runat="server" ResourceKey="Module" AssociatedControlID="PaneModulesLst" />
                    <asp:DropDownList ID="PaneModulesLst" runat="server" />
                </div>
                <div class="cpamLeft">
                    <asp:CheckBox ID="chkCopyModule" runat="server" CssClass="cpamCopyModule" />
                </div>
            </div>
            <asp:LinkButton ID="cmdAddModule" runat="server"  ResourceKey="AddModule" CssClass="dnnPrimaryAction" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
