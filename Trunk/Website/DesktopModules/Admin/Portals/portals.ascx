<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Portals.Portals" CodeFile="Portals.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<asp:panel id=plLetterSearch Runat="server" HorizontalAlign="Center">
    <asp:Repeater id=rptLetterSearch Runat="server">
		<itemtemplate>
			<asp:HyperLink ID="HyperLink1" runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL(Container.DataItem,"1") %>' Text='<%# Container.DataItem %>'>
			</asp:HyperLink>&nbsp;&nbsp;
		</ItemTemplate>
	</asp:Repeater>
</asp:panel>
<br />
<asp:DataGrid ID="grdPortals" runat="server" Width="100%" 
    AutoGenerateColumns="false" CellPadding="2" GridLines="None" cssclass="DataGrid_Container">
	<headerstyle cssclass="DataGrid_Header" verticalalign="Top" horizontalalign="Center"/>
	<itemstyle CssClass="DataGrid_Item" horizontalalign="Center" />
	<alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
    <Columns>
		<dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="PortalID" />
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" keyfield="PortalID" />
        <asp:TemplateColumn HeaderText="PortalId">
            <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
            <HeaderStyle HorizontalAlign="Left" />
            <ItemTemplate>
                <asp:Label ID="lblPortalId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalId") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Title">
            <HeaderStyle HorizontalAlign="Left" />
            <ItemStyle HorizontalAlign="Left" VerticalAlign="Top"/>
            <ItemTemplate>
                <asp:Label ID="lblPortal" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PortalName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Portal Aliases">
            <HeaderStyle HorizontalAlign="Left" />
            <ItemStyle HorizontalAlign="Left" VerticalAlign="Top"/>
            <ItemTemplate>
                <asp:Label ID="lblPortalAliases" runat="server" Text='<%# FormatPortalAliases(Convert.toInt32(DataBinder.Eval(Container.DataItem, "PortalID"))) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <dnn:textcolumn DataField="Users" HeaderText="Users" ItemStyle-VerticalAlign="Top" />
        <dnn:textcolumn DataField="Pages" HeaderText="Pages" ItemStyle-VerticalAlign="Top"/>
        <dnn:textcolumn DataField="HostSpace" HeaderText="DiskSpace" ItemStyle-VerticalAlign="Top"/>
        <asp:BoundColumn DataField="HostFee" HeaderText="HostingFee" DataFormatString="{0:0.00}" ItemStyle-VerticalAlign="Top"/>
        <asp:TemplateColumn HeaderText="Expires" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label runat="server" Text='<%#FormatExpiryDate(DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>'
                    CssClass="Normal" ID="Label1" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<br><br>
<dnn:pagingcontrol id=ctlPagingControl runat="server"></dnn:pagingcontrol>

