<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.Affiliates" CodeFile="Affiliates.ascx.vb" %>
<asp:datagrid id="grdAffiliates" runat="server" Width="100%" Border="0" CellSpacing="3" AutoGenerateColumns="false" EnableViewState="true" BorderStyle="None" BorderWidth="0px" GridLines="None">
<Columns>
<asp:TemplateColumn>
<ItemStyle Width="20px">
</ItemStyle>

<ItemTemplate>
				<asp:hyperlink navigateurl='<%# FormatURL("AffilId",DataBinder.Eval(Container.DataItem,"AffiliateId")) %>' runat="server" id="Hyperlink1">
					<asp:image imageurl="~/images/edit.gif" resourcekey="Edit" alternatetext="Edit" runat="server" id="Hyperlink1Image" />
				</asp:hyperlink>
			
</ItemTemplate>
</asp:TemplateColumn>
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
<asp:BoundColumn DataField="CPC" HeaderText="CPC" DataFormatString="{0:#,##0.0####}">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Clicks" HeaderText="Clicks">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="CPCTotal" HeaderText="Total" DataFormatString="{0:#,##0.0####}">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="CPA" HeaderText="CPA" DataFormatString="{0:#,##0.0####}">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Acquisitions" HeaderText="Acquisitions">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="CPATotal" HeaderText="Total" DataFormatString="{0:#,##0.0####}">
<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" CssClass="Normal">
</ItemStyle>
</asp:BoundColumn>
</Columns>
</asp:datagrid>
<br>
<asp:HyperLink CssClass="CommandButton" ID="cmdAdd" resourcekey="cmdAdd" Runat="server" BorderStyle="None">Create New Affiliate</asp:HyperLink>
