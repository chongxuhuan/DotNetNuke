<%@ Control language="vb" Inherits="DotNetNuke.Modules.Html.MyWork" CodeBehind="MyWork.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<p>
    <asp:Label ID="lblMessage" runat="server" CssClass="Normal" resourcekey="lblMessage" />
</p>
<p>
<asp:DataGrid ID="grdTabs" runat="server" AutoGenerateColumns="False" BorderStyle="None" BorderWidth="0" CellPadding="4" AllowPaging="false" EnableViewState="true" ShowHeader="False">
	<HeaderStyle CssClass="NormalBold" />
	<Columns>
	<asp:TemplateColumn HeaderText="Page">
		<ItemTemplate><%#Me.FormatURL(Container.DataItem)%></ItemTemplate>
	</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
</p>
<asp:linkbutton id="cmdCancel" runat="server" class="CommandButton" resourcekey="cmdCancel" borderstyle="none" text="Cancel" causesvalidation="False"></asp:linkbutton>
