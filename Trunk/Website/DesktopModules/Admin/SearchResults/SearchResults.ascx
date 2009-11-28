<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.SearchResults.SearchResults" CodeFile="SearchResults.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:label ID="lblMessage" runat="server" cssClass="Normal"/>
<asp:Datagrid id="dgResults" runat="server" AutoGenerateColumns="False" AllowPaging="true" BorderStyle="None" ShowHeader="False" CellPadding="4" GridLines="None" PagerStyle-Visible="false">
	<Columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:Label id="lblNo" runat="server" Text='<%# DataBinder.Eval(Container, "ItemIndex") + 1 %>' CssClass="SubHead" />
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:HyperLink id="lnkTitle" runat="server" CssClass="SubHead" NavigateUrl='<%# FormatURL(DataBinder.Eval(Container.DataItem,"TabId"),DataBinder.Eval(Container.DataItem,"Guid")) %>' Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' />&nbsp;-
				<asp:Label id="lblRelevance" runat="server" CssClass="Normal" Text='<%# FormatRelevance(DataBinder.Eval(Container.DataItem, "Relevance")) %>' />
				<br/>
				<asp:Label id="lblSummary" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "Description") + "<br>" %>' Visible="<%# ShowDescription() %>" />
				<asp:HyperLink id="lnkLink" runat="server" CssClass="CommandButton" NavigateUrl='<%# FormatURL(DataBinder.Eval(Container.DataItem,"TabId"),DataBinder.Eval(Container.DataItem,"Guid")) %>' Text='<%# FormatURL(DataBinder.Eval(Container.DataItem,"TabId"),DataBinder.Eval(Container.DataItem,"Guid")) %>'/>&nbsp;-
				<asp:Label id="lblPubDate" runat="server" CssClass="Normal" Text='<%# FormatDate(DataBinder.Eval(Container.DataItem, "PubDate")) %>'/>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:Datagrid>
<br/><br/>
<dnn:pagingcontrol id=ctlPagingControl runat="server" Mode="PostBack"/>
