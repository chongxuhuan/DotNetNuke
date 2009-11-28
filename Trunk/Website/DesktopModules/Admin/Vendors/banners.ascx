<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.Banners" CodeFile="Banners.ascx.vb" %>
<asp:datagrid id="grdBanners" runat="server" Width="100%" Border="0" CellSpacing="3" AutoGenerateColumns="false" EnableViewState="true" summary="Edit Vendors Design Table" BorderStyle="None" BorderWidth="0px" GridLines="None">
<Columns>
<asp:TemplateColumn>
<ItemStyle Width="20px">
</ItemStyle>

<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# FormatURL("BannerId",DataBinder.Eval(Container.DataItem,"BannerId")) %>' runat="server" ID="Hyperlink1">
					<asp:image imageurl="~/images/edit.gif" resourcekey="Edit" alternatetext="Edit" runat="server" id="Hyperlink1Image"/>
				</asp:HyperLink>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="BannerName" HeaderText="Banner">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Type">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
				<asp:Label ID="lblType" Runat="server" Text='<%# DisplayType(DataBinder.Eval(Container.DataItem, "BannerTypeId")) %>'></asp:Label>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="GroupName" HeaderText="Group">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Impressions" HeaderText="Impressions">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="CPM" HeaderText="CPM" DataFormatString="{0:#,##0.00}">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Views" HeaderText="Views">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="ClickThroughs" HeaderText="Clicks">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Start">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
				<asp:Label ID="lblStartDate" Runat="server" Text='<%# DisplayDate(DataBinder.Eval(Container.DataItem, "StartDate")) %>'></asp:Label>
			
</ItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn HeaderText="End">
<HeaderStyle CssClass="NormalBold">
</HeaderStyle>

<ItemStyle CssClass="Normal">
</ItemStyle>

<ItemTemplate>
				<asp:Label ID="lblEndDate" Runat="server" Text='<%# DisplayDate(DataBinder.Eval(Container.DataItem, "EndDate")) %>'></asp:Label>
			
</ItemTemplate>
</asp:TemplateColumn>
</Columns>
</asp:datagrid>
<br>
<asp:hyperlink CssClass="CommandButton" id="cmdAdd" resourcekey="cmdAdd" runat="server" borderstyle="none">Create New Banner</asp:hyperlink>
