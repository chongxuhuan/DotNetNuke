<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.UpdatePage" CodeFile="UpdatePage.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnFormItem">
    <dnn:DnnFieldLabel id="NameLbl" runat="server" Text="Name" AssociatedControlID="Name" />
    <asp:TextBox ID="Name" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:DnnFieldLabel id="LocationLbl" runat="server" Text="Location" AssociatedControlID="LocationLst" />
    <asp:DropDownList ID="LocationLst" runat="server" />
    <asp:DropDownList ID="PageLst" runat="server" MaxHeight="300px" />
</div>
<div class="dnnFormItem">
    <dnn:DnnFieldLabel id="SkinLbl" runat="server" Text="Skin" AssociatedControlID="SkinLst" />
    <dnn:DnnComboBox ID="SkinLst" runat="server" MaxHeight="300px" />
</div>
<div class="dnnFormItem cppeNavOptions">	
    <div class="dnnFormItem dnnLeft">
        <dnn:DnnFieldLabel id="IncludeInMenuLbl" runat="server" Text="IncludeInMenu" AssociatedControlID="IncludeInMenu" />
        <asp:CheckBox ID="IncludeInMenu" runat="server" Checked="true" />
    </div>
    <div class="dnnFormItem dnnLeft penoDisabled">
        <dnn:DnnFieldLabel id="DisabledLbl" runat="server" Text="Disabled" AssociatedControlID="IsDisabled" />
        <asp:CheckBox ID="IsDisabled" runat="server" Checked="false" />
    </div>
</div>
<asp:Panel ID="IsSecurePanel" runat="server" CssClass="dnnFormItem">
    <dnn:DnnFieldLabel id="IsSecureLbl" runat="server" Text="Secured" AssociatedControlID="IsSecure" />
    <asp:CheckBox ID="IsSecure" runat="server" Checked="false" />
</asp:Panel>
<dnn:DnnButton ID="cmdUpdate" runat="server" Text="UpdateButton" CssClass="dnnPrimaryAction" />