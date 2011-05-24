<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Modules.ModuleSettingsPage" CodeFile="ModuleSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ModuleLocalization" Src="~/Admin/Modules/ModuleLocalization.ascx" %>
<div class="dnnForm dnnModuleSettings dnnClear" id="dnnModuleSettings">
    <ul class="dnnAdminTabNav dnnClear">
		<li><a href="#msModuleSettings"><%=LocalizeString("ModuleSettings")%></a></li>
        <li><a href="#msPermissions"><%=LocalizeString("Permissions")%></a></li>
		<li><a href="#msPageSettings"><%=LocalizeString("PageSettings")%></a></li>
		<li id="specificSettingsTab" runat="server"><asp:HyperLink href="#msSpecificSettings" id="hlSpecificSettings" runat="server" /></li>
	</ul>
    <div class="msModuleSettings dnnClear" id="msModuleSettings">
        <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
        <div class="msmsContent dnnClear">
            <h2 id="dnnPanel-ModuleGeneralDetails" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("GeneralDetails")%></a></h2>
            <fieldset>
                <div class="dnnFormItem" id="cultureRow" runat="server">
                    <dnn:Label ID="cultureLabel" runat="server" ControlName="cultureLanguageLabel" />
                    <dnn:DnnLanguageLabel ID="cultureLanguageLabel" runat="server"  />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plFriendlyName" runat="server" controlname="txtFriendlyName" />
                    <asp:TextBox ID="txtFriendlyName" runat="server" Enabled="False" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plTitle" runat="server" controlname="txtTitle" />
                    <asp:textbox id="txtTitle" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plTags" runat="server" ControlName="termsSelector" />
                    <dnn:TermsSelector ID="termsSelector" runat="server" Height="250px" Width="525px" />
                </div>
            </fieldset>
            <h2 id="dnnPanel-ModuleSecuritySettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Security")%></a></h2>
            <fieldset>
                <div class="dnnFormItem" id="rowAllTabs" runat="server">
                    <dnn:Label id="plAllTabs" runat="server" controlname="chkAllTabs" />
                    <asp:checkbox id="chkAllTabs" runat="server" AutoPostback="true" />
                </div>
                <div class="dnnFormItem" id="trnewPages" runat="server">
                    <dnn:Label id="plNewTabs" runat="server" controlname="chkNewTabs" />
                    <asp:checkbox id="chkNewTabs" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plAdminBorder" runat="server" controlname="chkAdminBorder" />
                    <asp:checkbox id="chkAdminBorder" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plHeader" runat="server" controlname="txtHeader" />
                    <asp:textbox id="txtHeader" runat="server" textmode="MultiLine" rows="6" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plFooter" runat="server" controlname="txtFooter" />
                    <asp:textbox id="txtFooter" runat="server" textmode="MultiLine" rows="6" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plStartDate" runat="server" controlname="txtStartDate" />
                    <asp:textbox id="txtStartDate" runat="server" maxlength="11" />
					<asp:hyperlink id="cmdStartCalendar" cssclass="dnnSecondaryAction" runat="server" resourcekey="Calendar" />
					<asp:CompareValidator ID="valtxtStartDate" ControlToValidate="txtStartDate" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valStartDate.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plEndDate" runat="server" controlname="txtEndDate" />
                    <asp:textbox id="txtEndDate" runat="server" maxlength="11" />
					<asp:hyperlink id="cmdEndCalendar" cssclass="dnnSecondaryAction" runat="server" resourcekey="Calendar" />
					<asp:CompareValidator ID="valtxtEndDate" ControlToValidate="txtEndDate" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valEndDate.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
					<asp:CompareValidator ID="val2txtEndDate" ControlToValidate="txtEndDate" ControlToCompare="txtStartdate" Operator="GreaterThanEqual" Type="Date" Runat="server" Display="Dynamic" resourcekey="valEndDate2.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                </div>
            </fieldset>
            <h2 id="dnnPanel-ModuleAdditionalPages" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("ModuleInstalledOn")%></a></h2>
            <fieldset>
                <div>
                    <dnn:Label ID="lblInstalledOn" runat="server" ResourceKey="InstalledOn" />
                    <asp:DataGrid ID="lstInstalledOnTabs" runat="server" AutoGenerateColumns="False" BorderStyle="None" AllowPaging="true" PageSize="20" EnableViewState="true" ShowHeader="False">
						<HeaderStyle CssClass="NormalBold" />
						<Columns>
						    <asp:TemplateColumn HeaderText="Page">
							    <ItemTemplate><%#GetInstalledOnLink(Container.DataItem)%></ItemTemplate>
						    </asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
                </div>
            </fieldset>
        </div>
    </div>
    <div class="msPermissions dnnClear" id="msPermissions">
        <div class="mspContent dnnClear">
            <fieldset>
                <div id="permissionsRow" runat="server">
                    <dnn:modulepermissionsgrid id="dgPermissions" runat="server" />
                    <asp:checkbox id="chkInheritPermissions" autopostback="true" runat="server" resourcekey="InheritPermissions" />
                </div>
            </fieldset>
        </div>
    </div>
    <div class="msPageSettings dnnClear" id="msPageSettings">
        <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
        <div class="mspsContent dnnClear">
            <h2 id="dnnPanel-ModuleAppearance" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Appearance")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label id="plIcon" runat="server" controlname="ctlIcon" />
                    <div class="dnnLeft"><dnn:url id="ctlIcon" runat="server" ShowImages="true" showurls="False" showtabs="False" showlog="False" showtrack="False" required="False" ShowNone="true" /></div>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plAlign" runat="server" controlname="cboAlign" />
                    <div class="dnnLeft">
                        <asp:radiobuttonlist id="cboAlign" cssclass="dnnFormRadioButtons" runat="server" RepeatLayout="Flow">
                            <asp:listitem resourcekey="Left" value="left" />
                            <asp:listitem resourcekey="Center" value="center" />
                            <asp:listitem resourcekey="Right" value="right" />
                            <asp:listitem resourcekey="Not_Specified" value="" />
                        </asp:radiobuttonlist>
					</div>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plColor" runat="server" controlname="txtColor" />
                    <asp:textbox id="txtColor" runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plBorder" runat="server" controlname="txtBorder" />
                    <asp:textbox id="txtBorder" runat="server" MaxLength="1" />
					<asp:CompareValidator ID="valBorder" ControlToValidate="txtBorder" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valBorder.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plVisibility" runat="server" controlname="cboVisibility" />
                    <asp:radiobuttonlist id="cboVisibility" cssclass="dnnFormRadioButtons" runat="server" RepeatLayout="Flow">
						<asp:listitem resourcekey="Maximized" value="0" />
						<asp:listitem resourcekey="Minimized" value="1" />
						<asp:listitem resourcekey="None" value="2" />
					</asp:radiobuttonlist>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plDisplayTitle" runat="server" controlname="chkDisplayTitle" />
                    <asp:CheckBox ID="chkDisplayTitle" Runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plDisplayPrint" runat="server" controlname="chkDisplayPrint" />
                    <asp:CheckBox ID="chkDisplayPrint" Runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plDisplaySyndicate" runat="server" controlname="chkDisplaySyndicate" />
                    <asp:CheckBox ID="chkDisplaySyndicate" Runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plWebSlice" runat="server" controlname="chkWebSlice" />
                    <asp:CheckBox ID="chkWebSlice" Runat="server" AutoPostBack="true" />
                </div>
                <div class="dnnFormItem" id="webSliceTitle" runat="server">
                    <dnn:Label id="plWebSliceTitle" runat="server" controlname="txtWebSliceTitle" />
                    <asp:TextBox ID="txtWebSliceTitle" runat="server" />
                </div>
                <div class="dnnFormItem" id="webSliceExpiry" runat="server">
                    <dnn:Label id="plWebSliceExpiry" runat="server" controlname="txtWebSliceExpiry" />
                    <asp:textbox id="txtWebSliceExpiry" runat="server" maxlength="11" />
					<asp:hyperlink id="cmdWebSliceExpiry" cssclass="dnnSecondaryAction" runat="server" resourcekey="Calendar" />
					<asp:CompareValidator ID="valWebSliceExpiry" ControlToValidate="txtWebSliceExpiry" Operator="DataTypeCheck" Type="Date" Runat="server" Display="Dynamic" resourcekey="valWebSliceExpiry.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                </div>
                <div class="dnnFormItem" id="webSliceTTL" runat="server">
                    <dnn:Label id="plWebSliceTTL" runat="server" controlname="txtWebSliceTTL" />
                    <asp:TextBox ID="txtWebSliceTTL" runat="server" />
                    <asp:CompareValidator ID="valWebSliceTTL" ControlToValidate="txtWebSliceTTL" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valWebSliceTTL.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                </div>
                <div class="dnnFormItem dnnContainerPreview">
                    <dnn:Label id="plModuleContainer" runat="server" controlname="ctlModuleContainer" />
                    <asp:DropDownList ID="moduleContainerCombo" runat="server" DataTextField="Key" DataValueField ="Value" />
                    <a href="#" class="dnnSecondaryAction"><%=LocalizeString("ContainerPreview")%></a>
                </div>
            </fieldset>
            <h2 id="dnnPanel-ModuleCacheSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("CacheSettings")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblCacheProvider" runat="server" ControlName="cboCacheProvider" ResourceKey="CacheProvider"  />
                    <asp:DropDownList ID="cboCacheProvider" runat="server" AutoPostBack="true" DataValueField="Key" DataTextField="filteredkey" />
                    <asp:Label ID="lblCacheInherited" runat="server" resourceKey="CacheInherited" CssClass="dnnFormError" />
                </div>
                <div class="dnnFormItem" id="divCacheDuration" runat="server" visible="false">
                     <dnn:Label ID="lblCacheDuration" runat="server" ControlName="txtCacheDuration" ResourceKey="CacheDuration" />
                     <asp:TextBox ID="txtCacheDuration" runat="server" />
                    <asp:CompareValidator ID="valCacheTime" ControlToValidate="txtCacheDuration" Operator="DataTypeCheck" Type="Integer" Runat="server" Display="Dynamic" resourcekey="valCacheTime.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
                    <asp:Label ID="lblCacheDurationWarning" runat="server" ResourceKey="CacheDurationWarning" CssClass="dnnFormError" />
                </div>
            </fieldset>
            <h2 id="dnnPanel-ModuleOtherSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("OtherSettings")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label id="plDefault" runat="server" controlname="chkDefault"  />
                    <asp:CheckBox ID="chkDefault" Runat="server" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plAllModules" runat="server" controlname="chkAllModules" />
                    <asp:CheckBox ID="chkAllModules" Runat="server" />
                </div>
                <div class="dnnFormItem" id="rowTab" runat="server">
                    <dnn:Label id="plTab" runat="server" controlname="cboTab" />
                    <asp:dropdownlist id="cboTab" datatextfield="IndentedTabName" datavaluefield="TabId" runat="server" />
                </div>
            </fieldset>
        </div>
     </div>
    <div class="msSpecificSettings dnnClear" id="msSpecificSettings">
        <div class="mspsContent dnnClear"><fieldset id="fsSpecific" runat="server"><asp:panel id="pnlSpecific" runat="server" /></fieldset></div>
    </div>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" /></li>
        <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDelete" Causesvalidation="False" /></li>
        <li><asp:Hyperlink id="cancelHyperLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
    </ul>
    <div class="dnnmsStat dnnClear"><dnn:audit id="ctlAudit" runat="server" /></div>
</div>
<script language="javascript" type="text/javascript">
    function setUpDnnModuleSettings() {
        $('#dnnModuleSettings').dnnTabs().dnnPanels();
        $('#msModuleSettings .dnnFormExpandContent a').dnnExpandAll({
            expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
            collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
            targetArea: '#msModuleSettings'
        });
        $('#msPageSettings .dnnFormExpandContent a').dnnExpandAll({
            expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
            collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
            targetArea: '#msPageSettings'
        });
        $('#<%= cmdDelete.ClientID %>').dnnConfirm({
            text: '<%= Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile) %>',
            yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
            noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
            title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
        });
        $('.dnnContainerPreview').dnnPreview({
            containerSelector: 'select',
            baseUrl: '<%= DotNetNuke.Common.Globals.NavigateURL(this.TabId) %>',
            noSelectionMessage: '<%= LocalizeString("PreviewNoSelectionMessage.Text") %>',
            alertCloseText: '<%= Localization.GetString("Close.Text", Localization.SharedResourceFile)%>',
            alertOkText: '<%= Localization.GetString("Ok.Text", Localization.SharedResourceFile)%>'
        });
    }

    $(document).ready(function () {
        setUpDnnModuleSettings();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpDnnModuleSettings();
        });
    });
</script>