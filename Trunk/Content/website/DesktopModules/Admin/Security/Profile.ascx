<%@ Control Language="C#" AutoEventWireup="false" Inherits="DesktopModules.Admin.Security.DNNProfile" CodeFile="Profile.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<script language="javascript" type="text/javascript">
/*globals jQuery, window, Sys */
(function ($, Sys) {
    function setUpProfile() {
        $('.dnnButtonDropdown').dnnSettingDropdown();
    }

    $(document).ready(function () {
        setUpProfile();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpProfile();
        });
    });
} (jQuery, window.Sys));
</script>
<div class="dnnForm dnnProfile dnnClear">
	<h2 id="divTitle" runat="server" class="dnnFormSectionHead"><asp:label id="lblTitle" runat="server" /></h2>
	<fieldset>
		<div class="propertyList">
			<dnn:ProfileEditorControl id="ProfileProperties" runat="Server" enableClientValidation="true" />
            <div class="dnnClear"></div>
		</div>
		<ul id="actionsRow" runat="server" class="dnnActions dnnClear">
			<li><asp:LinkButton class="dnnPrimaryAction" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" /></li>
		</ul>
	</fieldset>
</div>