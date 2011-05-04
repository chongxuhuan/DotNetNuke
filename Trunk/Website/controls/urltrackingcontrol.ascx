<%@ Control Language="C#" AutoEventWireup="false" Explicit="true" Inherits="DotNetNuke.UI.UserControls.URLTrackingControl" %>
<table width="750" cellspacing="0" cellpadding="2" summary="URL tracking Design table" border="0">
	<tr>
		<td class="SubHead" valign="middle" width="150"><asp:label id="Label1" resourcekey="Url" runat="server" enableviewstate="False">URL</asp:label>:</td>
		<td nowrap="nowrap"><asp:label id="lblURL" CssClass="Normal" Runat="server" Width="300"></asp:label>
			<asp:label id="lblLogURL" Runat="server" CssClass="Normal" Visible="False"></asp:label></td>
	</tr>
	<tr>
		<td class="SubHead" valign="middle" width="150"><asp:label id="Label3" resourcekey="Created" runat="server" enableviewstate="False">Created</asp:label>:</td>
		<td><asp:label id="lblCreatedDate" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
	</tr>
</table>
<asp:Panel id="pnlTrack" runat="server" visible="False">
	<br/>
	<table width="750" cellspacing="0" cellpadding="2" summary="URL tracking Design table" border="0">
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:label id="Label2" resourcekey="trackingUrl" runat="server" enableviewstate="False">tracking URL</asp:label>:</td>
			<td nowrap="nowrap"><asp:label id="lblTrackingURL" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:label id="Label4" resourcekey="Clicks" runat="server" enableviewstate="False">Clicks</asp:label>:</td>
			<td><asp:label id="lblClicks" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
		<tr>
			<td class="SubHead" valign="middle" width="150"><asp:label id="Label5" resourcekey="LastClick" runat="server" enableviewstate="False">Last Click</asp:label>:</td>
			<td><asp:label id="lblLastClick" CssClass="Normal" Runat="server" Width="300"></asp:label></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel id="pnlLog" runat="server" visible="False">
	<br/>
	<table cellspacing="0" cellpadding="2" width="750" summary="URL Log Criteria Design table"
		border="0">
		<tr>
			<td class="SubHead" valign="middle" width="150">
			<label for="<%= txtStartDate.ClientID%>">
				<asp:label id="Label6" runat="server" resourcekey="Startdate" enableviewstate="False">Start Date</asp:label>:
			</label>
			</td>
			<td>
				<asp:TextBox id="txtStartDate" runat="server" CssClass="NormalTextBox" width="120" Columns="20"></asp:TextBox>&nbsp;
				<asp:HyperLink id="cmdStartCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton" enableviewstate="False">Calendar</asp:HyperLink>
			</td>
		</tr>
		<tr>
			<td class="SubHead" valign="middle" width="150">
			<label for="<%=txtEndDate.ClientID%>">
				<asp:label id="Label7" runat="server" resourcekey="EndDate" enableviewstate="False">End Date</asp:label>:
			</label>
			</td>
			<td>
				<asp:TextBox id="txtEndDate" runat="server" CssClass="NormalTextBox" width="120" Columns="20"></asp:TextBox>&nbsp;
				<asp:HyperLink id="cmdEndCalendar" resourcekey="Calendar" Runat="server" CssClass="CommandButton" enableviewstate="False">Calendar</asp:HyperLink>
			</td>
		</tr>
	</table>
	<p>
    	<asp:LinkButton id="cmdDisplay" runat="server" resourcekey="cmdDisplay" cssclass="CommandButton" Text="Display" enableviewstate="False"></asp:LinkButton>
	</p>
	<asp:datagrid id="grdLog" runat="server" CellPadding="4" Summary="URL Log Design table" EnableViewState="false" AutoGenerateColumns="false" CellSpacing="3" Border="0">
		<Columns>
			<asp:BoundColumn HeaderText="Date" DataField="ClickDate" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
			<asp:BoundColumn HeaderText="User" DataField="FullName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		</Columns>
	</asp:datagrid>
</asp:Panel>
