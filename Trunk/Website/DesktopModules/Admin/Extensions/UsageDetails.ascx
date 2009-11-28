<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UsageDetails.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Extensions.UsageDetails" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>                

<h3><asp:Literal ID="lblTitle" runat="server" /></h3>

<asp:UpdatePanel ID="PnlUsageDetails" runat="server" ChildrenAsTriggers="true">
<ContentTemplate>
<table id="tblFilterUsage" runat="server" cellspacing="0" cellpadding="4" border="0">
    <tr>
		<td width="150px" nowrap="nowrap"><dnn:Label ID="lblFilterUsageList" runat="server" CssClass="SubHead" ControlName="FilterUsageList" /></td>
		<td width="99%" align="left"><asp:DropDownList ID="FilterUsageList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterUsageList_SelectedIndexChanged" /></td>
	</tr>
</table>
<br />
<asp:Label ID="UsageListMsg" runat="server" />
<br /><br />
<asp:GridView ID="UsageList" runat="server" AutoGenerateColumns="false" PageSize="100" AllowPaging="true" 
	GridLines="None" CellPadding="4" style="border:solid 0px #ececec; margin-bottom:10px;" EnableViewState="False">
	<HeaderStyle Wrap="False" CssClass="NormalBold" BackColor="#f1f6f9" />
	<PagerSettings Mode="NextPreviousFirstLast" />
	<Columns>
		<asp:TemplateField HeaderText="Page">
			<ItemTemplate>
				<%#GetFormattedLink(Container.DataItem)%>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>
</ContentTemplate>
</asp:UpdatePanel>
