<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Dashboard.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Dashboard.Dashboard" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<%-- Uses the jquery template engine in tabs.js --%>
<div id="dashboardTabs">
    <asp:Repeater ID="rptTabs" runat="server">
        <HeaderTemplate>
            <ul id="tablist">
        </HeaderTemplate>
        <ItemTemplate>
            <li><a href='#<%# Eval("DashboardControlKey") %>-tab' class="dashboardTab"><%# Eval("LocalizedTitle")%></a></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>        
    </asp:Repeater>
    <asp:Repeater ID="rptControls" runat="server">
        <ItemTemplate>
            <div id='<%# Eval("DashboardControlKey") %>-tab' class="dashboardPanel">
                <asp:PlaceHolder ID="phControl" runat="server" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
