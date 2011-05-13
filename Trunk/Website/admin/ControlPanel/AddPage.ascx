<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.AddPage" CodeFile="AddPage.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="cbcpPageSettingsContent">
    <div class="dnnFormItem">
        <dnn:DnnFieldLabel id="NameLbl" runat="server" Text="Name" AssociatedControlID="Name" />
        <asp:TextBox ID="Name" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:DnnFieldLabel id="TemplateLbl" runat="server" Text="Template" AssociatedControlID="TemplateLst" />
        <asp:DropDownList ID="TemplateLst" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:DnnFieldLabel id="LocationLbl" runat="server" Text="Location" AssociatedControlID="LocationLst" />
        <asp:DropDownList ID="LocationLst" runat="server" />
        <asp:DropDownList ID="PageLst" runat="server" Width="180px" MaxHeight="300px" />
    </div>
    <div class="cpIncludeInMenu">
        <asp:CheckBox ID="IncludeInMenu" runat="server" Checked="true" />
		<dnn:DnnFieldLabel id="IncludeInMenuLbl" runat="server" Text="IncludeInMenu" AssociatedControlID="IncludeInMenu" />
        <dnn:DnnButton ID="cmdAddPage" runat="server" Text="AddButton" CssClass="dnnPrimaryAction" />
    </div>
</div>