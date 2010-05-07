<%@ Control Inherits="DotNetNuke.Modules.Admin.Tabs.Tabs" language="vb" AutoEventWireup="false" Explicit="True" CodeFile="Tabs.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Tabs Design Table" border="0">
	<tr>
		<td width="560">
		    <asp:Panel ID="pnlHost" runat="server">
		        <asp:CheckBox ID="chkDisplayHost" runat="server" CssClass="SubHead" resourcekey="HostTabs" AutoPostBack="true" TextAlign="Left" /><br />
		        <br />
		    </asp:Panel>
			<asp:panel id="pnlTabs" runat="server" cssclass="WorkPanel" visible="True">
				<table cellspacing="0" cellpadding="0" border="0" summary="Tabs Design Table">
					<tr valign="top">
						<td width="400">
							<label style="DISPLAY:none" for="<%=lstTabs.ClientID%>">First Tabs</label>
							<asp:listbox id="lstTabs" runat="server" rows="40" datatextfield="IndentedTabName" datavaluefield="TabId" cssclass="NormalTextBox" width="450px"></asp:listbox>
						</td>
						<td>&nbsp;</td>
						<td>
							<table summary="Tabs Design Table">
								<tr>
									<td colspan="2" valign="top" class="SubHead">
										<asp:label id="lblMovePage" runat="server" resourcekey="MovePage">Move Page</asp:label>
										<hr noshade size="1">
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdTop" resourcekey="cmdTop.Help" runat="server" alternatetext="Move Tab To Top Of Current Level" commandname="top" imageurl="~/images/action_top.gif" />
									</td>
									<td valign="top" width="90%">
										<dnn:HelpButton id="hbtnTopHelp" resourcekey="cmdTop" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdUp" resourcekey="cmdUp.Help" runat="server" alternatetext="Move Tab Up In Current Level" commandname="up" imageurl="~/images/up.gif" />
									</td>
									<td valign="top" width="90%">
										<dnn:HelpButton id="hbtnUpHelp" resourcekey="cmdUp" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdDown" resourcekey="cmdDown.Help" runat="server" alternatetext="Move Tab Down In Current Level" commandname="down" imageurl="~/images/dn.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnDownHelp" resourcekey="cmdDown" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdBottom" resourcekey="cmdBottom.Help" runat="server" alternatetext="Move Tab To Bottom Of Current Level" commandname="bottom" imageurl="~/images/action_bottom.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnBottomHelp" resourcekey="cmdBottom" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdLeft" resourcekey="cmdLeft.Help" runat="server" alternatetext="Move Tab Up One Hierarchical Level" commandname="left" imageurl="~/images/lt.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnLeftHelp" resourcekey="cmdLeft" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdRight" resourcekey="cmdRight.Help" runat="server" alternatetext="Move Tab Down One Hierarchical Level" commandname="right" imageurl="~/images/rt.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnRightHelp" resourcekey="cmdRight" runat="server" />
									</td>
								</tr>
								<tr>
									<td colspan="2" height="25">&nbsp;</td>
								</tr>
								<tr>
									<td colspan="2" valign="top" class="SubHead">
										<asp:label id="lblActions" runat="server" resourcekey="Actions">Actions</asp:label>
										<hr noshade size="1">
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdAdd" resourcekey="cmdAdd.Help" runat="server" alternatetext="Add Tab" imageurl="~/images/add.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnAddHelp" resourcekey="cmdAdd" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdEdit" resourcekey="cmdEdit.Help" runat="server" alternatetext="Edit Tab" imageurl="~/images/edit.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnEditHelp" resourcekey="cmdEdit" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdView" resourcekey="cmdView.Help" runat="server" alternatetext="View Tab" imageurl="~/images/view.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnViewHelp" resourcekey="cmdView" runat="server" />
									</td>
								</tr>
								<tr>
									<td valign="top" width="10%">
										<asp:imagebutton id="cmdDelete" resourcekey="cmdDelete.Help" runat="server" alternatetext="Delete Tab" imageurl="~/images/delete.gif"></asp:imagebutton>
									</td>
									<td valign="top" width="90%">
										<dnn:helpbutton id="hbtnDeleteHelp" resourcekey="cmdDelete" runat="server" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:panel>
		<td width="10">&nbsp;</td>
	</tr>
</table>
