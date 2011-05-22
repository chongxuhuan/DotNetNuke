<%@ Control Language="C#" AutoEventWireup="false" CodeFile="Tabs.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Pages.View" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
	    function onContextClicking(sender, eventArgs) {
		    var id = '<%=ctlContext.ClientID%>';
		    var item = eventArgs.get_menuItem();
		    var cmd = item.get_value();
		    if (cmd == 'delete') {
			    if (!confirm('<%=GetConfirmString()%>')) {
				    item.get_menu().hide();
				    eventArgs.set_cancel(true);
			    }
		    }
	    }
	    function onContextShowing(sender, eventArgs) {
		    var node = eventArgs.get_node();
		    var menu = eventArgs.get_menu();
		    if (node) {
			    var a = node.get_attributes();

			    menu.findItemByValue('view').set_visible(a.getAttribute("CanView") == 'True');
			    menu.findItemByValue('edit').set_visible(a.getAttribute("CanEdit") == 'True');
			    menu.findItemByValue('add').set_visible(a.getAttribute("CanAdd") == 'True');
			    menu.findItemByValue('hide').set_visible(a.getAttribute("CanHide") == 'True');
			    menu.findItemByValue('show').set_visible(a.getAttribute("CanMakeVisible") == 'True');
			    menu.findItemByValue('disable').set_visible(a.getAttribute("CanDisable") == 'True');
			    menu.findItemByValue('enable').set_visible(a.getAttribute("CanEnable") == 'True');
			    menu.findItemByValue('delete').set_visible(a.getAttribute("CanDelete") == 'True');
			    menu.findItemByValue('moveup').set_visible(a.getAttribute("CanMoveUp") == 'True');
			    menu.findItemByValue('movedown').set_visible(a.getAttribute("CanMoveDown") == 'True');
			    menu.findItemByValue('makehome').set_visible(a.getAttribute("CanMakeHome") == 'True');
		    }
	    }

    </script>
</telerik:RadScriptBlock>
<div class="dotnetnuke_pagesmain">
	<div class="dnnFormMessage dnnFormInfo">
		<asp:Label ID="lblHead" runat="server"/>
	</div>
	<telerik:RadAjaxPanel ID="ctlAjax" runat="server" LoadingPanelID="ctlLoading" EnablePageHeadUpdate="true" RestoreOriginalRenderDelegate="false" EnableViewState="true">

		<div class="dotnetnuke_pageselect">

            <div class="btnLeft">
                <asp:ImageButton ID="btnTreeCommand" runat="server" CommandName="Expand" ImageUrl="images/Icon_Expand.png" />
			</div>

            <asp:Panel ID="pnlHost" runat="server" class="pnlHost">
                <div class="pnlHostLabel">
					<asp:Label ID="lblHostOnly" runat="server" resourcekey="lblHostOnly"></asp:Label>
				</div>
                <div class="pnlHostRbl">
					<asp:RadioButtonList ID="rblMode" runat="server" RepeatColumns="2" AutoPostBack="true">
						<asp:ListItem Value="P" Selected="True"></asp:ListItem>
						<asp:ListItem Value="H"></asp:ListItem>
					</asp:RadioButtonList>
				</div>
			</asp:Panel> 
			<div class="clearfix"></div>

		</div> 

		<div class="dotnetnuke_tree">
			<telerik:RadTreeView ID="ctlPages" runat="server" AllowNodeEditing="true" EnableDragAndDrop="true" OnClientContextMenuShowing="onContextShowing" OnClientContextMenuItemClicking="onContextClicking" EnableDragAndDropBetweenNodes="true">
			<ContextMenus>                
				<telerik:RadTreeViewContextMenu ID="ctlContext" runat="server">
					<Items>                                            
						<telerik:RadMenuItem Text="View" Value="view"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Edit" Value="edit"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Delete" Value="delete"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="MoveUp" Value="moveup"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="MoveDown" Value="movedown"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Add" Value="add"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Hide Page in Menu" Value="hide"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Show Page in Menu" Value="show"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Enable Page" Value="enable"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Disable Page" Value="disable"></telerik:RadMenuItem>
						<telerik:RadMenuItem Text="Make Homepage" Value="makehome"></telerik:RadMenuItem>
					</Items>
				</telerik:RadTreeViewContextMenu>               
			</ContextMenus>
		</telerik:RadTreeView>
		</div>        

		<div class="dotnetnuke_details" runat="server" visible="false" id="pnlDetails">

			<telerik:RadTabStrip ID="ctlTabstrip" runat="server" MultiPageID="ctlTabPages" SelectedIndex="0" Skin="Sunset" Align="Justify">
				<Tabs>
					<telerik:RadTab Text="Common" PageViewID="ctlPageDetails"></telerik:RadTab>
					<telerik:RadTab Text="Permissions" PageViewID="ctlPagePermissions"></telerik:RadTab>
					<telerik:RadTab Text="Modules" PageViewID="ctlPageModules"></telerik:RadTab>
					<telerik:RadTab Text="SEO" PageViewID="ctlPageCEO"></telerik:RadTab>
					<telerik:RadTab Text="Metatags" PageViewID="ctlPageMeta"></telerik:RadTab>
					<telerik:RadTab Text="Appearance" PageViewID="ctlPageAppearance"></telerik:RadTab>
					<telerik:RadTab Text="Link" PageViewID="ctlPageLink"></telerik:RadTab>                    
				</Tabs>
			</telerik:RadTabStrip>

			<telerik:RadMultiPage ID="ctlTabPages" runat="server" SelectedIndex="0">

				<telerik:RadPageView ID="ctlPageDetails" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblHelp_Common" runat="server" resourcekey="lblHelp_Common"></asp:Label></p>

					<table>
						<tr>
							<td><asp:Label ID="lblName" runat="server" Text="Page Name:" resourcekey="lblName"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtName" runat="server" Width="200px"></telerik:RadTextBox></td>
						</tr>                     
						<tr>
							<td><asp:Label ID="lblTitle" runat="server" Text="Page Title:" resourcekey="lblTitle"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtTitle" runat="server" Width="200px"></telerik:RadTextBox></td>
						</tr>
						<tr><td colspan="2">&nbsp;</td></tr>
						<tr>
							<td><asp:Label ID="lblVisible" runat="server" Text="Visible in Navigation:" resourcekey="lblVisible"></asp:Label></td>
							<td><asp:CheckBox ID="chkVisible" runat="server" /></td>
						</tr> 
						<tr>
							<td><asp:Label ID="lblPageDisabled" runat="server" Text="Disabled Link in Navigation:" resourcekey="lblDisabledPage"></asp:Label></td>
							<td><asp:CheckBox ID="chkDisabled" runat="server" /></td>
						</tr>
						<tr>
							<td><asp:Label ID="lblPageSSL" runat="server" Text="SSL enabled:" resourcekey="lblPageSSL"></asp:Label></td>
							<td><asp:CheckBox ID="chkSecure" runat="server" /></td>
						</tr>                                                                                                                                                                
					</table>                                                       

				</telerik:RadPageView>

				<telerik:RadPageView ID="ctlPagePermissions" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblhelp_Permissions" runat="server" resourcekey="lblHelp_Permissions"></asp:Label></p>

					<dnn:TabPermissionsGrid ID="dgPermissions" runat="server" />

				</telerik:RadPageView>

				<telerik:RadPageView ID="ctlPageModules" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblhelp_Modules" runat="server" resourcekey="lblHelp_Modules"></asp:Label></p>

					<telerik:RadGrid ID="grdModules" runat="server">
						<MasterTableView AutoGenerateColumns="false" NoMasterRecordsText="<p style='padding:20px;'>No modules on that page...</p>">
							<Columns>
								<telerik:GridTemplateColumn HeaderText="ModuleTitle">
									<ItemTemplate>

										<%#DataBinder.Eval(Container.DataItem, "ModuleTitle")%>

									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn HeaderText="Module">
									<ItemTemplate>

										<%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>

									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn HeaderText="Options">
									<ItemTemplate>

										<asp:ImageButton ID="cmdDeleteModule" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ModuleId")%>' OnClick="CmdDeleteModuleClick" ImageUrl="images/Icon_Delete.png" />
										&nbsp;
										<a href='<%#ModuleEditUrl((int)DataBinder.Eval(Container.DataItem, "ModuleId"))%>' target="_blank">
											<asp:Image ID="imgEdit" runat="server" ImageUrl="images/Icon_Edit.png" />
										</a>

									</ItemTemplate>
								</telerik:GridTemplateColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>

				</telerik:RadPageView>                              

				<telerik:RadPageView ID="ctlPageCEO" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblHelp_SEO" runat="server" resourcekey="lblHelp_SEO"></asp:Label></p>

					<table>
						<tr>
							<td><asp:Label ID="lblSitemapPriority" runat="server" Text="Sitemap Priority:" resourcekey="lblSitemapPriority"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtSitemapPriority" runat="server" Width="250px"></telerik:RadTextBox></td>
						</tr>
						<tr>
							<td><asp:Label ID="lblDescription" runat="server" Text="Page Description:" resourcekey="lblDescription"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtDescription" runat="server" Width="250px" TextMode="MultiLine" Height="40px"></telerik:RadTextBox></td>
						</tr>  
						<tr>
							<td><asp:Label ID="lblKeywords" runat="server" Text="Page Keywords:" resourcekey="lblKeywords"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtKeywords" runat="server" Width="250px" TextMode="MultiLine" Height="40px"></telerik:RadTextBox></td>
						</tr> 
						<tr>
							<td><asp:Label ID="lblTags" runat="server" Text="Page Keywords:" resourcekey="lblTags"></asp:Label></td>
							<td><dnn:TermsSelector ID="termsSelector" runat="server" Height="250" Width="254"/></td>
						</tr>                                                                                                               
					</table> 

				</telerik:RadPageView>

				<telerik:RadPageView ID="ctlPageMeta" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblHelp_Meta" runat="server" resourcekey="lblHelp_Meta"></asp:Label></p>

					<table>
						<tr>
							<td><asp:Label ID="lblMetaRefresh" runat="server" Text="Refresh Interval (seconds):" resourcekey="lblMetaRefresh"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtRefresh" runat="server" Width="200px"></telerik:RadTextBox></td>
						</tr>
						<tr>
							<td><asp:Label ID="lblMetaHead" runat="server" Text="Page Header Tags:" resourcekey="lblMetaHead"></asp:Label></td>
							<td><telerik:RadTextBox ID="txtMeta" runat="server" Width="200px" TextMode="MultiLine" Height="40px"></telerik:RadTextBox></td>
						</tr>                                                                                         
					</table> 

				</telerik:RadPageView> 

				<telerik:RadPageView ID="ctlPageAppearance" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblHelp_Appearance" runat="server" resourcekey="lblHelp_Appearance"></asp:Label></p>

					<table>
						<tr>
                            <td class="tdLabel"><asp:Label ID="lblSkin" runat="server" Text="Skin:" resourcekey="lblSkin"></asp:Label></td>
                            <td class="tdControl">
								<telerik:RadComboBox ID="drpSkin" runat="server" Width="250px"></telerik:RadComboBox>
							</td>
                            <td class="tdControl">
								<asp:RadioButtonList ID="rblSkinMode" runat="server" RepeatDirection="Horizontal">
									<asp:ListItem resourcekey="Host" Value="H" Selected="True"></asp:ListItem>
									<asp:ListItem resourcekey="Site" Value="S"></asp:ListItem>
								</asp:RadioButtonList>
							</td>                            
						</tr>
						<tr>
                            <td class="tdLabel"><asp:Label ID="lblContainer" runat="server" Text="Container:" resourcekey="lblContainer"></asp:Label></td>
                            <td class="tdControl">
								<telerik:RadComboBox ID="drpContainer" runat="server" Width="250px"></telerik:RadComboBox>
							</td>
                            <td class="tdControl">
								<asp:RadioButtonList ID="rblContainerMode" runat="server" RepeatDirection="Horizontal">
									<asp:ListItem resourcekey="Host" Value="H" Selected="True"></asp:ListItem>
									<asp:ListItem resourcekey="Site" Value="S"></asp:ListItem>
								</asp:RadioButtonList>
							</td>
						</tr>  
						<tr>
							<td colspan="3">&nbsp;</td>
						</tr>
						<tr>
							<td colspan="3"><asp:LinkButton ID="cmdCopySkin" runat="server" CssClass="cmd Copy" resourcekey="cmdCopySkin"></asp:LinkButton></td>
						</tr>
						<tr>
							<td colspan="3">&nbsp;</td>
						</tr>                                                
						<tr>
                            <td class="tdIconLabel"><asp:Label ID="lblIconLarge" runat="server" Text="Large Icon:" resourcekey="lblIconLarge"></asp:Label></td>
                            <td class="tdControl" colspan="2">
								<dnn:URL ID="ctlIconLarge" runat="server" Width="300" ShowLog="False" ShowTrack="false"></dnn:URL>
							</td>
						</tr>
						<tr>
                            <td class="tdIconLabel"><asp:Label ID="lblIconSmall" runat="server" Text="Small Icon:" resourcekey="lblIconSmall"></asp:Label></td>
                            <td class="tdControl" colspan="2">
								<dnn:URL ID="ctlIcon" runat="server" Width="300" ShowLog="False"></dnn:URL>
							</td>
						</tr>                                                                                                                                         
					</table> 

				</telerik:RadPageView>                                                

				<telerik:RadPageView ID="ctlPageLink" runat="server" CssClass="pageview">

					<p class="tabhelp"><asp:Label ID="lblHelp_Link" runat="server" resourcekey="lblHelp_Link"></asp:Label></p>

					<table>
						<tr>
                            <td class="tdIconLabel"><asp:Label ID="lblUrl" runat="server" Text="Link:" resourcekey="lblUrl"></asp:Label></td>
                            <td class="tdControl"><dnn:URL ID="ctlURL" runat="server" Width="300" ShowLog="False" ShowNone="True" ShowTrack="False"/></td>
						</tr>
						<tr>
							<td><asp:Label ID="lblPermanentRedirect" runat="server" Text="Permanent Redirect:" resourcekey="lblPermanentRedirect"></asp:Label></td>
							<td><asp:CheckBox ID="chkPermanentRedirect" runat="server" /></td>
						</tr>                                                               
					</table> 

				</telerik:RadPageView>                                                                

			</telerik:RadMultiPage>        

			<div class="dtBtn">
				<div class="btnLeft"><asp:HyperLink ID="cmdMore" runat="server" Text="More" resourcekey="cmdMore" CssClass="cmd More"></asp:HyperLink></div>
				<div class="btnRight"><asp:LinkButton ID="cmdUpdate" runat="server" Text="Update" resourcekey="cmdUpdate" CssClass="cmd Update"></asp:LinkButton></div>
				<div class="clearfix"></div>
			</div>

		</div>

		<div class="dotnetnuke_details" runat="server" visible="false" id="pnlBulk">

			<p>
				<asp:Literal ID="lblBulkIntro" runat="server"></asp:Literal>
			</p>

			<telerik:RadTextBox ID="txtBulk" runat="server" Height="200" Width="400" TextMode="MultiLine"></telerik:RadTextBox>

			<asp:Button ID="btnBulkCreate" runat="server" resourcekey="btnBulkCreate" Text="Create Pages" />            

		</div>

		<div class="dotnetnuke_legend">
			<div class="legenditem"><img id="Img1" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_Home.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblHome" runat="server"></asp:Literal></div>
			<div class="legenditem"><img id="Img2" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_Everyone.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblEveryone" runat="server"></asp:Literal></div>
			<div class="legenditem"><img id="Img3" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_User.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblRegistered" runat="server"></asp:Literal></div>
			<div class="legenditem"><img id="Img4" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_UserSecure.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblSecure" runat="server"></asp:Literal></div>
			<div class="legenditem"><img id="Img5" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_UserAdmin.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblAdminOnly" runat="server"></asp:Literal></div>
			<div class="legenditem"><img id="Img6" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_Hidden.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblHidden" runat="server"></asp:Literal></div>                
			<div class="legenditem"><img id="Img7" runat="server" src="~/Desktopmodules/Admin/Tabs/images/Icon_Disabled.png" alt="Icon Visible" />&nbsp;<asp:Literal ID="lblDisabled" runat="server"></asp:Literal></div>
			<div class="clearfix"></div>
		</div>

		<div class="clearfix"></div>

	</telerik:RadAjaxPanel>

	<telerik:RadAjaxLoadingPanel ID="ctlLoading" runat="server" Skin="Default">
	</telerik:RadAjaxLoadingPanel>

</div>