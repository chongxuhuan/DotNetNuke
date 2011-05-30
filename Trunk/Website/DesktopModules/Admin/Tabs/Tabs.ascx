﻿<%@ Control Language="C#" AutoEventWireup="false" CodeFile="Tabs.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Pages.View" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<script language="javascript" type="text/javascript">
/*globals jQuery, window, Sys */
(function ($, Sys) {
	function setUpTabsModule() {
		$('#dnnTabsModule').dnnPanels()
			.find('.dnnFormExpandContent a').dnnExpandAll({
				expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
				collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
				targetArea: '#dnnTabsModule' 
			});
	}

	$(document).ready(function () {
		setUpTabsModule();
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
			setUpTabsModule();
		});
	});

	function toggleSection(id, isToggled) {
		$("div[id$='"+ id + "']").toggle(isToggled);
	}
}(jQuery, window.Sys));
</script>
<dnnweb:DnnScriptBlock ID="RadScriptBlock1" runat="server">
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
</dnnweb:DnnScriptBlock>
<div class="dnnForm dnnTabsModule dnnClear" id="dnnTabsModule">
	<div class="dnnTreeArea">
		<asp:Panel ID="pnlHost" runat="server">
			<div class="dnnFormItem">
				<asp:Label ID="lblHostOnly" runat="server" resourcekey="lblHostOnly" AssociatedControlID="rblMode" />
				<asp:RadioButtonList ID="rblMode" runat="server" AutoPostBack="true" CssClass="dnnHSRadioButtons" RepeatLayout="Flow" RepeatDirection="Vertical">
					<asp:ListItem Value="P" Selected="True" />
					<asp:ListItem Value="H" />
				</asp:RadioButtonList>
			</div>		
		</asp:Panel> 
		<div class="dnnTreeExpand">
			<asp:LinkButton ID="cmdExpandTree" runat="server" CommandName="Expand" />
		</div>
		<dnnweb:DnnTreeView ID="ctlPages" cssclass="dnnTreePages" runat="server" AllowNodeEditing="true" EnableDragAndDrop="true" OnClientContextMenuShowing="onContextShowing" OnClientContextMenuItemClicking="onContextClicking" EnableDragAndDropBetweenNodes="true">
			<ContextMenus>                
				<dnnweb:DnnTreeViewContextMenu ID="ctlContext" runat="server">
					<Items>                                            
						<dnnweb:DnnMenuItem Text="View" Value="view" />
						<dnnweb:DnnMenuItem Text="Edit" Value="edit" />
						<dnnweb:DnnMenuItem Text="Delete" Value="delete" />
						<dnnweb:DnnMenuItem Text="MoveUp" Value="moveup" />
						<dnnweb:DnnMenuItem Text="MoveDown" Value="movedown" />
						<dnnweb:DnnMenuItem Text="Add" Value="add" />
						<dnnweb:DnnMenuItem Text="Hide Page in Menu" Value="hide" />
						<dnnweb:DnnMenuItem Text="Show Page in Menu" Value="show" />
						<dnnweb:DnnMenuItem Text="Enable Page" Value="enable" />
						<dnnweb:DnnMenuItem Text="Disable Page" Value="disable" />
						<dnnweb:DnnMenuItem Text="Make Homepage" Value="makehome" />
					</Items>
				</dnnweb:DnnTreeViewContextMenu>               
			</ContextMenus>
		</dnnweb:DnnTreeView>
		<div class="dnnTreeLegend">
			<h3><asp:Label ID="lblLegend" runat="server" resourcekey="lblLegend" /></h3>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_Home.png" alt="" />
				<asp:Literal ID="lblHome" runat="server" />
			</div>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_Everyone.png" alt="" />
				<asp:Literal ID="lblEveryone" runat="server" />
			</div>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_User.png" alt="" />
				<asp:Literal ID="lblRegistered" runat="server" />
			</div>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_UserSecure.png" alt="" />
				<asp:Literal ID="lblSecure" runat="server" />
			</div>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_UserAdmin.png" alt="" />
				<asp:Literal ID="lblAdminOnly" runat="server" />
			</div>
			<div class="dtlItem">
				<img runat="server" src="images/Icon_Hidden.png" alt="" />
				<asp:Literal ID="lblHidden" runat="server" />
			</div>                
			<div class="dtlItem">
				<img runat="server" src="images/Icon_Disabled.png" alt="" />
				<asp:Literal ID="lblDisabled" runat="server" />
			</div>
		</div>
	</div>        
	<div class="tmTabContainer" runat="server" visible="false" id="pnlDetails">
		<div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-Common" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Common.Tabname")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label ID="lblName" runat="server" Suffix=":" />
					<asp:TextBox ID="txtName" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblTitle" runat="server" suffix=":" />
					<asp:TextBox ID="txtTitle" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblVisible" runat="server" suffix="?" />
					<asp:CheckBox ID="chkVisible" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblDisabledPage" runat="server" suffix="?" />
					<asp:CheckBox ID="chkDisabled" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblPageSSL" runat="server" suffix="?" />
					<asp:CheckBox ID="chkSecure" runat="server" />
				</div>   
			</fieldset>
		</div>    														 
		<div id="PermissionsSection" class="ssasContent dnnClear" runat="server">
			<h2 id="Panel-Permissions" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Permissions.Tabname")%></a></h2>
			<fieldset>
				<dnn:TabPermissionsGrid ID="dgPermissions" runat="server" />
			</fieldset>
		</div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-Modules" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Modules.Tabname")%></a></h2>
			<fieldset>
				<dnnweb:DnnGrid ID="grdModules" runat="server" AllowPaging="false" AllowSorting="false">
					<MasterTableView AutoGenerateColumns="false">
						<Columns>
							<dnnweb:DnnGridTemplateColumn HeaderText="ModuleTitle">
								<ItemTemplate>
									<%#DataBinder.Eval(Container.DataItem, "ModuleTitle")%>
								</ItemTemplate>
							</dnnweb:DnnGridTemplateColumn>
							<dnnweb:DnnGridTemplateColumn HeaderText="Module">
								<ItemTemplate>
									<%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
								</ItemTemplate>
							</dnnweb:DnnGridTemplateColumn>
							<dnnweb:DnnGridTemplateColumn HeaderText="Options">
								<ItemTemplate>
									<asp:ImageButton ID="cmdDeleteModule" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ModuleId")%>' OnClick="CmdDeleteModuleClick" ImageUrl="images/Icon_Delete.png" resourcekey="cmdDelete" />
									<a href="<%#ModuleEditUrl((int)DataBinder.Eval(Container.DataItem, "ModuleId"))%>">
										<asp:Image ID="imgEdit" runat="server" ImageUrl="images/Icon_Edit.png" resourcekey="Edit" />
									</a>
								</ItemTemplate>
							</dnnweb:DnnGridTemplateColumn>
						</Columns>
						<NoRecordsTemplate>
							<div class="dnnFormMessage dnnFormWarning">
								<asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords" />
							</div>
						</NoRecordsTemplate>
					</MasterTableView>
				</dnnweb:DnnGrid>
			</fieldset>
		</div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-SEO" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("SEO.Tabname")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label ID="lblSitemapPriority" runat="server" suffix=":" />
					<asp:TextBox ID="txtSitemapPriority" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblDescription" runat="server" suffix=":" />
					<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="40px" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblKeywords" runat="server" suffix=":" />
					<asp:TextBox ID="txtKeywords" runat="server" TextMode="MultiLine" Height="40px" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblTags" runat="server" suffix=":" />
					<dnnweb:TermsSelector ID="termsSelector" runat="server" Height="250" Width="354" />
				</div>
			</fieldset>
		</div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-Meta" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Metatags.Tabname")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label ID="lblMetaRefresh" runat="server" suffix=":" />
					<asp:TextBox ID="txtRefresh" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblMetaHead" runat="server" suffix=":" />
					<asp:TextBox ID="txtMeta" runat="server" TextMode="MultiLine" Height="40px" />
				</div>
			</fieldset>
		</div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-Appearance" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Appearance.Tabname")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label ID="lblSkin" runat="server" suffix=":" />
					<asp:DropDownList ID="drpSkin" runat="server" DataTextField="Key" DataValueField ="Value" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblContainer" runat="server" suffix=":" />
					<asp:DropDownList ID="drpContainer" runat="server" DataTextField="Key" DataValueField ="Value"  />
					<asp:LinkButton ID="cmdCopySkin" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCopySkin" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblIconLarge" runat="server" suffix=":" />
					<dnn:URL ID="ctlIconLarge" runat="server" ShowLog="False" ShowTrack="false" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblIconSmall" runat="server" suffix=":" />
					<dnn:URL ID="ctlIcon" runat="server" ShowLog="False" />
				</div>
			</fieldset>
		</div>
		<div class="ssasContent dnnClear">
			<h2 id="Panel-Link" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Link.Tabname")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:Label ID="lblUrl" runat="server" suffix=":" />
					<dnn:URL ID="ctlURL" runat="server" ShowLog="False" ShowNone="True" ShowTrack="False" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblPermanentRedirect" runat="server" suffix=":" />
					<asp:CheckBox ID="chkPermanentRedirect" runat="server" />
				</div>
			</fieldset>
		</div>
	    <ul class="dnnActions dnnClear">
		    <li><asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction" /></li>
		    <li><asp:HyperLink ID="cmdMore" runat="server" resourcekey="cmdMore" CssClass="dnnSecondaryAction" /></li>
	    </ul>     
	</div>
	<div runat="server" visible="false" id="pnlBulk">
		<p><asp:Literal ID="lblBulkIntro" runat="server" /></p>
		<asp:TextBox ID="txtBulk" runat="server" Height="200" Width="400" TextMode="MultiLine" />
		<asp:LinkButton ID="btnBulkCreate" runat="server" resourcekey="btnBulkCreate" Text="Create Pages" CssClass="dnnSecondaryAction" />            
	</div>
</div>