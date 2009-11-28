<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.Vendors" CodeFile="Vendors.ascx.vb" %>
<%@ Register TagPrefix="dnnsc" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table width="100%" border="0">
	<tr>
		<td align="left" class="Normal">
			<asp:Label ID="Label1" Runat="server" resourcekey="Search" CssClass="SubHead">Search:</asp:Label><br>
			<asp:TextBox id="txtSearch" Runat="server" />
			<asp:DropDownList id="ddlSearchType" Runat="server">
				<asp:ListItem Value="name" resourcekey="Name.Header">Name</asp:ListItem>
				<asp:ListItem Value="email" resourcekey="Email.Header">Email</asp:ListItem>
			</asp:DropDownList>
			<asp:ImageButton ID="btnSearch" Runat="server" ImageUrl="~/images/icon_search_16px.gif" />
		</td>
		<td>
		</td>
		<td align="right" class="Normal">
			<asp:Label ID="Label5" Runat="server" resourcekey="RecordsPage" CssClass="SubHead">Records Per Page:</asp:Label><br>
			<asp:DropDownList id="ddlRecordsPerPage" Runat="server" AutoPostBack="True">
				<asp:ListItem Value="10">10</asp:ListItem>
				<asp:ListItem Value="25">25</asp:ListItem>
				<asp:ListItem Value="50">50</asp:ListItem>
				<asp:ListItem Value="100">100</asp:ListItem>
				<asp:ListItem Value="250">250</asp:ListItem>
			</asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td colspan="3" height="15"></td>
	</tr>
</table>
<asp:Panel ID="plLetterSearch" Runat="server" HorizontalAlign="Center">
	<asp:Repeater id="rptLetterSearch" Runat="server">
		<ItemTemplate>
			<asp:HyperLink runat="server" CssClass="CommandButton" NavigateUrl='<%# FilterURL(Container.DataItem,"1") %>' Text='<%# Container.DataItem %>' ID="Hyperlink2" NAME="Hyperlink2">
			</asp:HyperLink>&nbsp;&nbsp;
		</ItemTemplate>
	</asp:Repeater>
</asp:Panel>
<asp:datagrid id="grdVendors" Border="0" CellPadding="4" width="100%" AutoGenerateColumns="False"
	EnableViewState="False" runat="server" summary="Vendors Design Table" BorderStyle="None" BorderWidth="0px"
	GridLines="None" cssclass="DataGrid_Container">
	<Columns>
		<asp:TemplateColumn>
			<ItemStyle Width="20px"></ItemStyle>
			<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# FormatURL("VendorID",DataBinder.Eval(Container.DataItem,"VendorID")) %>' Visible="<%# IsEditable %>" runat="server" ID="Hyperlink1">
					<asp:Image ImageUrl="~/images/edit.gif" AlternateText="Edit" Visible="<%# IsEditable %>" runat="server" ID="Hyperlink1Image" resourcekey="Edit"/>
				</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="VendorName" HeaderText="Name">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Address">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
			<ItemTemplate>
				<asp:Label ID="lblAddress" Runat="server" Text='<%# DisplayAddress(DataBinder.Eval(Container.DataItem, "Unit"),DataBinder.Eval(Container.DataItem, "Street"), DataBinder.Eval(Container.DataItem, "City"), DataBinder.Eval(Container.DataItem, "Region"), DataBinder.Eval(Container.DataItem, "Country"), DataBinder.Eval(Container.DataItem, "PostalCode")) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Telephone" HeaderText="Telephone">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
		<asp:BoundColumn DataField="Fax" HeaderText="Fax">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Email">
			<HeaderStyle CssClass="NormalBold"></HeaderStyle>
			<ItemStyle CssClass="Normal"></ItemStyle>
			<ItemTemplate>
				<asp:Label ID="lblEmail" Runat="server" Text='<%# DisplayEmail(DataBinder.Eval(Container.DataItem, "Email")) %>'>
				</asp:Label>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Authorized">
			<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
			<ItemTemplate>
				<asp:Image Runat="server" ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"Authorized")="true" %>'/>
				<asp:Image Runat="server" ID="Image2" ImageUrl="~/images/unchecked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"Authorized")="false" %>'/>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="Banners" HeaderText="Banners">
			<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
			<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
		</asp:BoundColumn>
	</Columns>
</asp:datagrid>
<br>
<br>
<dnnsc:PagingControl id="ctlPagingControl" runat="server"></dnnsc:PagingControl>
<p align="center">
	<asp:LinkButton id="cmdDelete" resourcekey="cmdDelete" Runat="server" CssClass="CommandButton">Delete Unauthorized Vendors</asp:LinkButton>
</p>
