<%@ Control Language="vb" AutoEventWireup="false" CodeFile="ListEntries.ascx.vb" Inherits="DotNetNuke.Common.Lists.ListEntries" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<table id="tblDetails" cellSpacing="0" cellPadding="0" width="100%" border="0" runat="server">
	<tr id="rowListdetails" runat="server">
		<td class="SubHead" width="100%">
			<table id="tblEntryInfo" cellSpacing="0" cellPadding="3" width="100%" border="0" runat="server">
				<tr id="rowListParent" runat="server">
					<td class="SubHead" width="120"><dnn:label id="plListParent" text="Parent:" runat="server" controlname="lblListParent"></dnn:label></td>
					<td><asp:label id="lblListParent" cssclass="Normal" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td class="SubHead" width="120"><dnn:label id="plListName" text="List Name:" runat="server" controlname="lblListName"></dnn:label></td>
					<td><asp:label id="lblListName" cssclass="Normal" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td class="SubHead" width="120"><dnn:label id="plEntryCount" text="Total:" runat="server" controlname="lblEntryCount"></dnn:label></td>
					<td><asp:label id="lblEntryCount" cssclass="Normal" runat="server"></asp:label></td>
				</tr>
				<tr id="rowListCommand" runat="server">
					<td class="SubHead" colSpan="2">
						<dnn:commandbutton id="cmdAddEntry" runat="server" resourcekey="cmdAddEntry" CssClass="CommandButton"
							 CausesValidation="False" imageurl="~/images/add.gif" />&nbsp;
						<dnn:commandbutton id="cmdDeleteList" runat="server" resourcekey="cmdDeleteList" CssClass="CommandButton"
							CausesValidation="False" imageurl="~/images/delete.gif" />
					</td>
				</tr>
			</table>
			<hr noshade="noshade" SIZE="1">
		</td>
	</tr>
	<tr id="rowEntryGrid" runat="server">
		<td class="SubHead" width="100%"><asp:datagrid id="grdEntries" runat="server" DataKeyField="EntryID" AutoGenerateColumns="false"
				width="100%" CellPadding="2" Border="0" BorderWidth="0px" GridLines="None">
				<Columns>
					<dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="Command" KeyField="EntryID" />
					<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" EditMode="Command" keyfield="EntryID" />
					<asp:BoundColumn DataField="Text" HeaderText="Text">
						<HeaderStyle CssClass="NormalBold"></HeaderStyle>
						<ItemStyle CssClass="Normal"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="Value" HeaderText="Value">
						<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="18px" CssClass="NormalBold"></HeaderStyle>
						<ItemStyle CssClass="Normal"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton ID="btnUp" Visible="<%# EnableSortOrder() %>" ImageUrl="~/Images/up.gif" Runat="server" CssClass="CommandButton" AlternateText="Move entry up" resourcekey="btnUp.AlternateText" CommandName="up">
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="18px" CssClass="NormalBold"></HeaderStyle>
						<ItemStyle CssClass="Normal"></ItemStyle>
						<ItemTemplate>
							<asp:ImageButton ID="btnDown" Visible="<%# EnableSortOrder() %>" ImageUrl="~/Images/dn.gif" Runat="server" CssClass="CommandButton" AlternateText="Move entry down" resourcekey="btnDown.AlternateText" CommandName="down">
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
	<tr id="rowEntryEdit" runat="server">
		<td class="SubHead">
			<table id="tblEntryEdit" cellSpacing="0" cellPadding="3" width="100%" border="0" runat="server">
				<tr id="rowParentKey" runat="server">
					<td class="SubHead" width="160"><dnn:label id="plParentKey" text="Parent:" runat="server" controlname="txtParentKey"></dnn:label></td>
					<td><asp:textbox id="txtParentKey" cssclass="NormalTextBox" runat="server" width="240" maxlength="100"
							ReadOnly="true"></asp:textbox></td>
				</tr>
				<tr id="rowListName" runat="server">
					<td class="SubHead" width="160"><dnn:label id="plEntryName" text="Entry Name:" runat="server" controlname="txtEntryName"></dnn:label></td>
					<td>
						<asp:textbox id="txtEntryName" cssclass="NormalTextBox" runat="server" width="240" maxlength="100"></asp:textbox>
						<asp:textbox id="txtEntryID" cssclass="NormalTextBox" runat="server" visible="false"></asp:textbox>
					</td>
				</tr>
				<tr id="rowSelectList" runat="server">
					<td class="SubHead" width="160"><dnn:label id="plSelectList" text="Parent List:" runat="server" controlname="ddlSelectList"></dnn:label></td>
					<td><asp:dropdownlist id="ddlSelectList" cssclass="NormalTextBox" AutoPostBack="true" Width="240" Runat="server"
							Enabled="False"></asp:dropdownlist></td>
				</tr>
				<tr id="rowSelectParent" runat="server">
					<td class="SubHead" width="160"><dnn:label id="plSelectParent" text="Parent Entry:" runat="server" controlname="ddlSelectParent"></dnn:label></td>
					<td><asp:dropdownlist id="ddlSelectParent" cssclass="NormalTextBox" Width="240" Runat="server" Enabled="False"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td class="SubHead" width="160"><dnn:label id="plEntryText" text="Entry Text:" runat="server" controlname="txtEntryText"></dnn:label></td>
					<td>
					    <asp:textbox id="txtEntryText" cssclass="NormalTextBox" runat="server" width="240" maxlength="100"></asp:textbox>
						<asp:requiredfieldvalidator id="valEntryText" cssclass="NormalRed" runat="server" resourcekey="valEntryText.ErrorMessage"
								display="Dynamic" errormessage="<br>Text Is Required" controltovalidate="txtEntryText"></asp:requiredfieldvalidator>
				    </td>
				</tr>
				<tr>
					<td class="SubHead" width="160"><dnn:label id="plEntryValue" text="Entry Value:" runat="server" controlname="txtEntryValue"></dnn:label></td>
					<td><asp:textbox id="txtEntryValue" cssclass="NormalTextBox" runat="server" width="240" maxlength="100"></asp:textbox>
						<asp:requiredfieldvalidator id="valEntryValue" cssclass="NormalRed" runat="server" resourcekey="valEntryValue.ErrorMessage"
								display="Dynamic" errormessage="<br>Value Is Required" controltovalidate="txtEntryValue"></asp:requiredfieldvalidator>
				    </td>
				</tr>
				<tr id="rowEnableSortOrder" runat="server">
					<td class="SubHead" width="160"><dnn:label id="plEnableSortOrder" text="Enable Sort Order:" runat="server" controlname="chkEnableSortOrder"></dnn:label></td>
					<td><asp:checkbox id="chkEnableSortOrder" Runat="server"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" colSpan="2">
						<dnn:commandbutton id="cmdSaveEntry" runat="server" resourcekey="cmdSave" CssClass="CommandButton"
							imageurl="~/images/save.gif" CausesValidation="True" />&nbsp;
						<dnn:commandbutton id="cmdDelete" runat="server" resourcekey="cmdDeleteEntry" CssClass="CommandButton"
							imageurl="~/images/delete.gif" causesvalidation="False" />&nbsp;
						<dnn:commandbutton id="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton" imageurl="~/images/lt.gif"
							causesvalidation="False" />&nbsp;
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
