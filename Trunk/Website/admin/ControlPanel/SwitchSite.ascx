<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.ControlPanel.SwitchSite" CodeFile="SwitchSite.ascx.vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="SwitchSiteFields">
<div class="row"><dnn:DnnFieldLabel id="SitesLbl" runat="server" Text="Sites" AssociatedControlID="SitesLst" /></div>
<div class="row"><dnn:DnnComboBox ID="SitesLst" runat="server" Width="245px" MaxHeight="300px" /></div>
<div class="row"><dnn:DnnButton ID="cmdSwitch" runat="server" Text="SwitchButton" /></div>
</div>
