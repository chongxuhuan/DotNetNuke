<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Portals.ascx.vb" Inherits="DotNetNuke.Modules.Dashboard.Controls.Portals" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label id="plPortals" runat="Server" CssClass="Head" ControlName="grdPortals" />
<asp:DataGrid ID="grdPortals" runat="server" GridLines="None"
    AutoGenerateColumns="false" EnableViewState="False">
    <Columns>
        <asp:BoundColumn DataField="PortalName" HeaderText="PortalName" />
        <asp:BoundColumn DataField="GUID" HeaderText="GUID" />
        <asp:BoundColumn DataField="Pages" HeaderText="Pages" />
        <asp:BoundColumn DataField="Roles" HeaderText="Roles" />
        <asp:BoundColumn DataField="Users" HeaderText="Users" />
    </Columns>
</asp:DataGrid>
