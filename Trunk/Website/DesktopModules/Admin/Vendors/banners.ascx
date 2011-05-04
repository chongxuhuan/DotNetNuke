<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Vendors.Banners" CodeFile="Banners.ascx.cs" %>

<asp:datagrid id="grdBanners" runat="server" Width="100%" Border="0" CellPadding="4" AutoGenerateColumns="false" 
            EnableViewState="true" BorderStyle="None" BorderWidth="0px" GridLines="None" CssClass="dnnBannersGrid">
    <headerstyle CssClass="dnnBannersGridHeader" />
    <itemstyle CssClass="dnnBannersGridItem" />
    <alternatingitemstyle CssClass="dnnBannersGridAltItem" />  
    <Columns>
        <asp:TemplateColumn>
            <ItemStyle Width="20px"/>
            <ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# FormatURL("BannerId",DataBinder.Eval(Container.DataItem,"BannerId").ToString()) %>' runat="server" ID="Hyperlink1">
					<asp:image imageurl="~/images/edit.gif" resourcekey="Edit" alternatetext="Edit" runat="server" id="Hyperlink1Image"/>
				</asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="BannerName" HeaderText="Banner"/>
        <asp:TemplateColumn HeaderText="Type">
            <ItemTemplate>
			    <asp:Label ID="lblType" Runat="server" Text='<%# DisplayType((int)DataBinder.Eval(Container.DataItem, "BannerTypeId")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="GroupName" HeaderText="Group"/>
        <asp:BoundColumn DataField="Impressions" HeaderText="Impressions"/>
        <asp:BoundColumn DataField="CPM" HeaderText="CPM" DataFormatString="{0:#,##0.00}"/>
        <asp:BoundColumn DataField="Views" HeaderText="Views"/>
        <asp:BoundColumn DataField="ClickThroughs" HeaderText="Clicks"/>
        <asp:TemplateColumn HeaderText="Start">
            <ItemTemplate>
		        <asp:Label ID="lblStartDate" Runat="server" Text='<%# DisplayDate((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="End">
            <ItemTemplate>
			    <asp:Label ID="lblEndDate" Runat="server" Text='<%# DisplayDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:datagrid>
<ul class="dnnActions dnnClear">
    <li><asp:hyperlink CssClass="dnnPrimaryAction" id="cmdAdd" resourcekey="cmdAdd" runat="server"/></li>
</ul>

