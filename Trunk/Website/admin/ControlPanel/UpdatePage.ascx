<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.UpdatePage" CodeFile="UpdatePage.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnFormItem">
    <asp:Label runat="server" ResourceKey="Name" AssociatedControlID="Name" />
    <asp:TextBox ID="Name" runat="server" />
</div>
<div class="dnnFormItem">
    <asp:Label runat="server" Location="Location" AssociatedControlID="LocationLst" />
    <asp:DropDownList ID="LocationLst" runat="server" />
    <asp:DropDownList ID="PageLst" runat="server" MaxHeight="300px" />
</div>
<div class="dnnFormItem">
    <asp:Label id="SkinLbl" runat="server" ResourceKey="Skin" AssociatedControlID="SkinLst" />
    <dnn:DnnComboBox ID="SkinLst" runat="server" Width="290px" MaxHeight="300px" />
</div>
<div class="cppeNavOptions">	
    <div class="dnnFormItem dnnLeft">
		<asp:CheckBox ID="IncludeInMenu" runat="server" Checked="true" />
        <asp:Label id="IncludeInMenuLbl" runat="server" ResourceKey="IncludeInMenu" AssociatedControlID="IncludeInMenu" />
    </div>
    <div class="dnnFormItem dnnLeft penoDisabled">
        <asp:CheckBox ID="IsDisabled" runat="server" Checked="false" />
        <asp:Label id="DisabledLbl" runat="server" ResourceKey="Disabled" AssociatedControlID="IsDisabled" />
    </div>
</div>
<asp:Panel ID="IsSecurePanel" runat="server" CssClass="dnnFormItem">
    <dnn:DnnFieldLabel id="IsSecureLbl" runat="server" Text="Secured" AssociatedControlID="IsSecure" />
    <asp:CheckBox ID="IsSecure" runat="server" Checked="false" />
</asp:Panel>
<asp:Button ID="cmdUpdate" runat="server" ResourceKey="UpdateButton" CssClass="dnnPrimaryAction" />