<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpleSettingsView.ascx.cs"
	Inherits="DotNetNuke.Modules.MobileManagement.SimpleSettingsView" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnForm  dnnClear" id="dnnMobileManagement">
	<h2 id="dnnSimpleSettings" class="dnnFormSectionHead">
		<%=LocalizeString("SimpleSettings")%></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:label id="lblRedirectName" runat="server" controlname="txtRedirectName" resourcekey="lblRedirectName"
				suffix=":"  CssClass="dnnFormRequired" />
			<asp:TextBox ID="txtRedirectName" runat="server" CssClass="dnnFixedSizeComboBox" />
			<asp:RequiredFieldValidator ID="valTrackingId" runat="server" CssClass="dnnFormMessage dnnFormError"
				ControlToValidate="txtRedirectName" Display="Dynamic" resourcekey="valRedirectName"
				ValidationGroup="Form" />
		</div>
		<div class="dnnFormItem">
			  <dnn:label id="lblRedirect" runat="server" resourcekey="lblRedirect" controlname="cboSourcePage" />
			  <asp:Label ID="lblHomePage" runat="server" CssClass="dnnFixedSizeComboBox" />
			  <dnn:DnnComboBox ID="cboSourcePage" runat="server" CssClass="dnnFixedSizeComboBox"  OnClientSelectedIndexChanged="cboSourcePageChanged" />
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblRedirectTarget" runat="server" controlname="optRedirectTarget"
				resourcekey="lblRedirectTarget" />
			<asp:RadioButtonList ID="optRedirectTarget" CssClass="dnnFormRadioButtons" runat="server"
				RepeatDirection="Horizontal">
				<asp:ListItem Value="Portal" resourcekey="optPortal" name="optTarget" Selected="true" />
				<asp:ListItem Value="Tab" resourcekey="optPage" name="optTarget" />
				<asp:ListItem Value="Url" resourcekey="optUrl" name="optTarget" />
			</asp:RadioButtonList>
			<div id="dvTargetPortal" runat="server" class="dnnFormItem">
				<div class="dnnLabel"></div>
				<%--<asp:DropDownList ID="cboPortal" runat="server" CssClass="dnnFixedSizeComboBox" />--%>
                <dnn:DnnComboBox ID="cboPortal" runat="server" CssClass="dnnFixedSizeComboBox" />
			</div>
			<div id="dvTargetPage" runat="server" class="dnnFormItem">
				<div class="dnnLabel"></div>
				<%--<asp:DropDownList ID="cboTargetPage" runat="server" CssClass="dnnFixedSizeComboBox" />--%>
                <dnn:DnnComboBox ID="cboTargetPage" runat="server" CssClass="dnnFixedSizeComboBox" />
			</div>
			<div id="dvTargetUrl" runat="server" class="dnnFormItem">				
				<dnn:label id="lblTargetUrl" runat="server" controlname="txtTargetUrl" resourcekey="lblTargetUrl" />
				<asp:TextBox ID="txtTargetUrl" runat="server" class="dnnFormInput" Text="http://" CssClass="dnnFixedSizeComboBox" />
			</div>
		</div>
	</fieldset>
	<ul class="dnnActions dnnClear">
		<li>
			<asp:LinkButton ID="lnkSave" runat="server" resourcekey="cmdSave" CssClass="dnnPrimaryAction"
				ValidationGroup="Form" /></li>
		<li>
			<asp:LinkButton ID="lnkCancel" runat="server" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
	</ul>
</div>
<script language="javascript" type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    function EndRequestHandler(sender, args) {
        if (args.get_error() == undefined) {
            DisaplyUI();
        }
    }
    DisaplyUI();
    function DisaplyUI() {
        (function ($) {
            /* Hide optional upon inital page load sections */
            $(document).ready(
                function () {
                    var target = $('#<%= optRedirectTarget.ClientID %> input:checked').val();
                    if (target === "Portal") {
                        $('#<%= dvTargetPortal.ClientID %>').show();
                        $('#<%= dvTargetPage.ClientID %>').hide();
                        $('#<%= dvTargetUrl.ClientID %>').hide();
                    }
                    else if (target === "Tab") {
                        $('#<%= dvTargetPortal.ClientID %>').hide();
                        $('#<%= dvTargetPage.ClientID %>').show();
                        $('#<%= dvTargetUrl.ClientID %>').hide();
                    }
                    else if (target === "Url") {
                        $('#<%= dvTargetPortal.ClientID %>').hide();
                        $('#<%= dvTargetPage.ClientID %>').hide();
                        $('#<%= dvTargetUrl.ClientID %>').show();
                    }

                    /* Toggle source page drop down whenever radio buttons are checked */
                    $("[name=optTarget]").change(function () {
                        var value = $('#<%= optRedirectTarget.ClientID %> input:checked').val();
                        if (value === "Portal") {
                            $('#<%= dvTargetPortal.ClientID %>').show();
                            $('#<%= dvTargetPage.ClientID %>').hide();
                            $('#<%= dvTargetUrl.ClientID %>').hide();
                        }
                        else if (value === "Tab") {
                            $('#<%= dvTargetPortal.ClientID %>').hide();
                            $('#<%= dvTargetPage.ClientID %>').show();
                            $('#<%= dvTargetUrl.ClientID %>').hide();
                        }
                        else if (value === "Url") {
                            $('#<%= dvTargetPortal.ClientID %>').hide();
                            $('#<%= dvTargetPage.ClientID %>').hide();
                            $('#<%= dvTargetUrl.ClientID %>').show();
                        }
                    });

                    $("#<%=cboSourcePage.ClientID %>").change(function (e) {
                        filterTargetList();
                    });

                    setTimeout(filterTargetList, 100);
                });

        } (jQuery));
    }
    function cboSourcePageChanged(sender, e) {
        filterTargetList();
    }

    /*filter target page list to hide the select value of source page list*/
    function filterTargetList() {
        var sourceList = $find("<%=cboSourcePage.ClientID %>");

        if (sourceList.get_items().length == 0) {
            return;
        }

        var sourcePage = sourceList.get_value();
        var targetList = $find("<%=cboTargetPage.ClientID %>");
        var options = targetList.get_items();
        var initialSel = -1;
        for (var i = 0; i < options.length; i++) {
            if (options[i].value == sourcePage)
                options[i].disable();
            else if (initialSel == -1)
                initialSel = i;
        }
        options[initialSel].select();
    };
</script>
