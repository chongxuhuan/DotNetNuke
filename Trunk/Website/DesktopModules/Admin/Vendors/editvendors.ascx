<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Vendors.EditVendors" CodeFile="EditVendors.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Address" Src="~/controls/Address.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Reference Control="~/DesktopModules/Admin/Vendors/Affiliates.ascx" %>
<%@ Reference Control="~/DesktopModules/Admin/Vendors/Banners.ascx" %>

<div class="dnnForm dnnEditVendor dnnClear" id="dnnEditVendor">
 	<div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
   <h2 id="Panel-Settings" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("Settings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plVendorName" runat="server" controlname="txtVendorName" />
            <asp:textbox id="txtVendorName" runat="server" CssClass="dnnFormRequired" maxlength="50" />
            <asp:requiredfieldvalidator id="valVendorName" runat="server" CssClass="dnnFormMessage dnnFormError"  display="Dynamic" 
                    resourceKey="companyRequired" controltovalidate="txtVendorName"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plFirstName" runat="server" controlname="txtFirstName"/>
            <asp:textbox id="txtFirstName" runat="server" cssclass="dnnFormRequired" maxlength="50" />
            <asp:requiredfieldvalidator id="valFirstName" runat="server" CssClass="dnnFormMessage dnnFormError"  display="Dynamic" 
                        resourceKey="firstNameRequired" controltovalidate="txtFirstName"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plLastName" runat="server" controlname="txtLastName" />
            <asp:textbox id="txtLastName" runat="server" cssclass="dnnFormRequired" maxlength="50" />
            <asp:requiredfieldvalidator id="valLastName" runat="server" cssclass="dnnFormMessage dnnFormError" display="Dynamic" 
                    resourceKey="lastNameRequired" controltovalidate="txtLastName"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plEmail" runat="server" controlname="txtEmail"/>
            <asp:textbox id="txtEmail" runat="server" cssclass="dnnFormRequired" maxlength="50" />
            <asp:requiredfieldvalidator id="valEmail" runat="server" cssclass="dnnFormMessage dnnFormError" display="Dynamic" 
                    resourceKey="eMailRequired" controltovalidate="txtEmail"></asp:requiredfieldvalidator>
            <asp:RegularExpressionValidator id="valEmail2" runat="server" cssclass="dnnFormMessage dnnFormError" 
                    resourceKey="eMailInvalid" 
                    ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ControlToValidate="txtEmail" Display="Dynamic"></asp:RegularExpressionValidator>
        </div>
    </fieldset>
    <h2 id="Panel-Address" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("Address")%></a></h2>
    <fieldset>
        <dnn:address runat="server" id="addresssVendor" />
    </fieldset>
    <h2 id="Panel-Other" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("Other")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plWebsite" runat="server" controlname="txtWebsite"/>
            <asp:textbox id="txtWebsite" runat="server" maxlength="100" />
        </div>
        <div id="rowVendor1" runat="server" class="dnnFormItem">
            <dnn:label id="plLogo" runat="server" controlname="ctlLogo"/>
            <portal:url id="ctlLogo" runat="server" width="200" showurls="False" showtabs="False" showlog="False" showtrack="False" required="False" />
        </div>
        <div id="rowVendor2" runat="server" class="dnnFormItem">
            <dnn:label id="plAuthorized" runat="server" controlname="chkAuthorized"/>
            <asp:checkbox id="chkAuthorized" runat="server" />
        </div>
    </fieldset>
    <asp:panel id="pnlVendor" runat="server">
        <h2 id="Panel-VendorClassification" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("VendorClassification")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:label id="plClassifications" runat="server" controlname="lstClassifications"/>
                <asp:listbox id="lstClassifications" width="200px" runat="server" rows="10" selectionmode="Multiple" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plKeyWords" runat="server" controlname="txtKeyWords"/>
                <asp:textbox id="txtKeyWords" width="200px" cssclass="NormalTextBox" runat="server" rows="10" textmode="MultiLine" />
            </div>
        </fieldset>
    </asp:panel>
    <asp:placeholder id="pnlBanners" runat="server">
        <h2 id="Panel-BannerAdvertising" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("BannerAdvertising")%></a></h2>
        <fieldset>
            <div id="divBanners" runat="server" />
        </fieldset>
    </asp:placeholder>
     <asp:placeholder id="pnlAffiliates" runat="server">
        <h2 id="Panel-AffiliateReferrals" class="dnnFormSectionHead"><a href="" class="dnnLabelExpanded"><%=LocalizeString("AffiliateReferrals")%></a></h2>
        <fieldset>
            <div id="divAffiliates" runat="server" />
        </fieldset>
    </asp:placeholder>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" /></li>
        <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDelete" Causesvalidation="False" /></li>
        <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" Causesvalidation="False" /></li>
    </ul>
</div>

<dnn:audit id="ctlAudit" runat="server" />

<script language="javascript" type="text/javascript">
    function setUpDnnEditVendors() {
        $('#dnnEditVendor').dnnPanels();
    }
    $(document).ready(function () {
        setUpDnnEditVendors();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpDnnEditVendors();
        });
    });
</script>