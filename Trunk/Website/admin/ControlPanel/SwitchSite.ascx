<%@ Control language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanel.SwitchSite" CodeFile="SwitchSite.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnFormItem">
    <dnn:DnnFieldLabel id="SitesLbl" runat="server" Text="Sites" AssociatedControlID="SitesLst" />
    <asp:DropDownList ID="SitesLst" runat="server" MaxHeight="300px" />
</div>
<dnn:DnnButton ID="cmdSwitch" runat="server" Text="SwitchButton" CausesValidation="false" CssClass="dnnLeft dnnPrimaryAction" />