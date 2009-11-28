<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.EditVendors" CodeFile="EditVendors.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Address" Src="~/controls/Address.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Reference Control="~/DesktopModules/Admin/Vendors/Affiliates.ascx" %>
<%@ Reference Control="~/DesktopModules/Admin/Vendors/Banners.ascx" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Edit Vendors Design Table">
  <tr>
    <td valign="top">
      <table cellspacing="0" cellpadding="0" border="0" summary="Edit Vendors Design Table">
        <tr>
          <td valign="top" width="560">
            <dnn:sectionhead id="dshSettings" runat="server" 
              cssclass="Head" 
              includerule="True" 
              resourcekey="Settings" 
              section="tblSettings" 
              text="Vendor Details" />
            <table id="tblSettings" runat="server" cellspacing="2" cellpadding="2" border="0" summary="Edit Vendors Design Table">
              <tr valign="top">
                <td class="SubHead" width="120"><dnn:label id="plVendorName" runat="server" controlname="txtVendorName" suffix=":"></dnn:label></td>
                <td align="left" class="NormalBold" nowrap>
                  <asp:textbox id="txtVendorName" runat="server" cssclass="NormalTextBox" width="200px" maxlength="50" tabindex="1"></asp:textbox>&nbsp;*
                  <asp:requiredfieldvalidator id="valVendorName" runat="server" cssclass="NormalRed" display="Dynamic" errormessage="<br>Company Name Is Required" controltovalidate="txtVendorName"></asp:requiredfieldvalidator>
                </td>
              </tr>
              <tr valign="top">
                <td class="SubHead" width="120"><dnn:label id="plFirstName" runat="server" controlname="txtFirstName" suffix=":"></dnn:label></td>
                <td class="NormalBold" nowrap>
                  <asp:textbox id="txtFirstName" runat="server" cssclass="NormalTextBox" width="200px" maxlength="50" tabindex="2"></asp:textbox>&nbsp;*
                  <asp:requiredfieldvalidator id="valFirstName" runat="server" cssclass="NormalRed" display="Dynamic" errormessage="<br>First Name Is Required" controltovalidate="txtFirstName"></asp:requiredfieldvalidator>
                </td>
              </tr>
              <tr valign="top">
                <td class="SubHead" width="120"><dnn:label id="plLastName" runat="server" controlname="txtLastName" suffix=":"></dnn:label></td>
                <td class="NormalBold" nowrap>
                  <asp:textbox id="txtLastName" runat="server" cssclass="NormalTextBox" width="200px" maxlength="50" tabindex="3"></asp:textbox>&nbsp;*
                  <asp:requiredfieldvalidator id="valLastName" runat="server" cssclass="NormalRed" display="Dynamic" errormessage="<br>Last Name Is Required" controltovalidate="txtLastName"></asp:requiredfieldvalidator>
                </td>
              </tr>
              <tr valign="top">
                <td class="SubHead" width="120"><dnn:label id="plEmail" runat="server" controlname="txtEmail" suffix=":"></dnn:label></td>
                <td class="NormalBold" nowrap>
                  <asp:textbox id="txtEmail" runat="server" cssclass="NormalTextBox" width="200px" maxlength="50" tabindex="4"></asp:textbox>&nbsp;*
                  <asp:requiredfieldvalidator id="valEmail" runat="server" cssclass="NormalRed" display="Dynamic" errormessage="<br>Email Is Required" controltovalidate="txtEmail"></asp:requiredfieldvalidator>
                  <asp:RegularExpressionValidator id="valEmail2" runat="server" ErrorMessage="<br>Email Must be Valid." ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ControlToValidate="txtEmail" Display="Dynamic"></asp:RegularExpressionValidator>
                </td>
              </tr>
            </table>
            <br>
            <dnn:sectionhead id="dshAddress" runat="server" 
              cssclass="Head" 
              includerule="True" 
              resourcekey="Address" 
              section="tblAddress" 
              text="Address Details" />
            <table id="tblAddress" runat="server" cellspacing="2" cellpadding="2" border="0" summary="Edit Vendors Design Table">
              <tr>
                <td><dnn:address runat="server" id="addresssVendor" /></td>
              </tr>
            </table>
            <br>
            <dnn:sectionhead id="dshOther" runat="server" 
              cssclass="Head" 
              includerule="True" 
              resourcekey="Other" 
              section="tblOther" 
              text="Other Details" />
            <table id="tblOther" runat="server" cellspacing="2" cellpadding="2" border="0" summary="Edit Vendors Design Table">
              <tr valign="top">
                <td class="SubHead" width="120"><dnn:label id="plWebsite" runat="server" controlname="txtWebsite" suffix=":"></dnn:label></td>
                <td><asp:textbox id="txtWebsite" runat="server" cssclass="NormalTextBox" width="200px" maxlength="100" tabindex="20"></asp:textbox></td>
              </tr>
              <tr id="rowVendor1" runat="server" valign="top">
                <td class="SubHead" width="120" valign="middle">
					<dnn:label id="plLogo" runat="server" controlname="ctlLogo" suffix=""></dnn:label></td>
 				<td width="325">
					<portal:url id="ctlLogo" runat="server" width="200" showurls="False" showtabs="False" showlog="False" showtrack="False" required="False" /></td>
              </tr>
              <tr id="rowVendor2" runat="server" valign="top">
                <td class="SubHead" width="120"><dnn:label id="plAuthorized" runat="server" controlname="chkAuthorized" suffix=":"></dnn:label></td>
                <td><asp:checkbox id="chkAuthorized" runat="server" tabindex="22"></asp:checkbox></td>
              </tr>
            </table>
            <br>
            <asp:panel id="pnlVendor" runat="server">
              <dnn:sectionhead id="dshClassification" runat="server" 
                cssclass="Head" 
                includerule="True"
                isExpanded ="False" 
                resourcekey="VendorClassification" 
                section="tblClassification" 
                text="Vendor Classification" />
              <table id="tblClassification" runat="server" cellspacing="2" cellpadding="2" border="0" summary="Edit Vendors Design Table">
                <tr valign="top">
                  <td class="SubHead" align="center" width="200"><dnn:label id="plClassifications" runat="server" controlname="lstClassifications" suffix=":"></dnn:label></td>
                  <td width="60"></td>
                  <td class="SubHead" align="center" width="200"><dnn:label id="plKeyWords" runat="server" controlname="txtKeyWords" suffix=":"></dnn:label></td>
                </tr>
                <tr valign="top">
                  <td align="center" width="200"><asp:listbox id="lstClassifications" width="200px" cssclass="NormalTextBox" runat="server" rows="10" selectionmode="Multiple" tabindex="16"></asp:listbox></td>
                  <td width="60"></td>
                  <td align="center" width="200"><asp:textbox id="txtKeyWords" width="200px" cssclass="NormalTextBox" runat="server" rows="10" textmode="MultiLine" tabindex="17"></asp:textbox></td>
                </tr>
              </table>
            </asp:panel>
            <br>
            <asp:placeholder id="pnlBanners" runat="server">
              <dnn:sectionhead id="dshBanners" runat="server" 
                cssclass="Head" 
                includerule="True"
                isexpanded ="False" 
                resourcekey="BannerAdvertising" 
                section="divBanners" 
                text="Banner Advertising" />
              <div id="divBanners" runat="server" />
            </asp:placeholder>
            <br>
            <asp:placeholder id="pnlAffiliates" runat="server">
              <dnn:sectionhead id="dshAffiliates" runat="server" 
                cssclass="Head" 
                includerule="True"
                isexpanded ="False" 
                resourcekey="AffiliateReferrals" 
                section="divAffiliates" 
                text="Affiliate Referrals" />
              <div id="divAffiliates" runat="server" />
            </asp:placeholder>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>

<p>
  <asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update" borderstyle="none"></asp:linkbutton>&nbsp;
  <asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel" borderstyle="none" causesvalidation="False"></asp:linkbutton>&nbsp;
  <asp:linkbutton cssclass="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" text="Delete" borderstyle="none" causesvalidation="False"></asp:linkbutton>
</p>

<table cellspacing="0" cellpadding="4" border="0" summary="Edit Vendors Design Table">
  <tr>
    <td>
      <asp:panel id="pnlAudit" runat="server">
        <dnn:audit id="ctlAudit" runat="server" />
      </asp:panel>
    </td>
  </tr>
</table>