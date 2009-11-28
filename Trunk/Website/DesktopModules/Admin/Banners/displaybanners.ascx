<%@ Control language="vb" Inherits="DotNetNuke.Modules.Admin.Vendors.DisplayBanners" CodeFile="DisplayBanners.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<asp:DataList id=lstBanners runat="server" summary="Banner Design Table" >
	<ItemTemplate><asp:Label ID="lblItem" Runat="server" Text='<%# FormatItem(DataBinder.Eval(Container.DataItem,"VendorId"),DataBinder.Eval(Container.DataItem,"BannerId"),DataBinder.Eval(Container.DataItem,"BannerTypeId"),DataBinder.Eval(Container.DataItem,"BannerName"),DataBinder.Eval(Container.DataItem,"ImageFile"),DataBinder.Eval(Container.DataItem,"Description"),DataBinder.Eval(Container.DataItem,"Url"),DataBinder.Eval(Container.DataItem,"Width"),DataBinder.Eval(Container.DataItem,"Height")) %>'></asp:Label></ItemTemplate>
</asp:DataList>
