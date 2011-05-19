<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Dashboard.Controls.Portals" CodeFile="Portals.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label id="plPortals" runat="Server" CssClass="Head" ControlName="grdPortals" />
<asp:DataGrid ID="grdPortals" runat="server" GridLines="None"
    AutoGenerateColumns="false" EnableViewState="False">
    <Columns>
        <asp:BoundColumn DataField="PortalName" HeaderText="PortalName" ItemStyle-Width="200px"/>
        <asp:BoundColumn DataField="GUID" HeaderText="GUID" ItemStyle-Width="300px"/>
        <asp:BoundColumn DataField="Pages" HeaderText="Pages" ItemStyle-Width="100px"/>
        <asp:BoundColumn DataField="Roles" HeaderText="Roles" ItemStyle-Width="100px"/>
        <asp:BoundColumn DataField="Users" HeaderText="Users" ItemStyle-Width="100px" />
    </Columns>
</asp:DataGrid>
