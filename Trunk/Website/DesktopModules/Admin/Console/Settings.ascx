<%@ Control Language="C#" AutoEventWireup="false" Inherits="DesktopModules.Admin.Console.Settings" CodeFile="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnConsole dnnClear">
	<div class="dnnFormItem">
		<dnn:label id="Label1" runat="server" ControlName="ParentTab" ResourceKey="Mode" Suffix=":" />
		<asp:DropDownList ID="modeList" runat="server">
		    <asp:ListItem Value="Normal" ResourceKey="Normal" />
		    <asp:ListItem Value="Profile" ResourceKey="Profile" />
		    <asp:ListItem Value="Group" ResourceKey="Group" />
		</asp:DropDownList>
	</div>
	<div id="parentTabRow" class="dnnFormItem">
		<dnn:label id="lblParentTab" runat="server" ControlName="ParentTab" ResourceKey="ParentTab" Suffix=":" />
		<asp:DropDownList ID="ParentTab" runat="server" />
	</div>
	<div id="includeParentRow" class="dnnFormItem">
		<dnn:label id="lblIncludeParent" runat="server" ControlName="IncludeParent" ResourceKey="IncludeParent" Suffix=":" />
		 <asp:Checkbox ID="IncludeParent" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblDefaultSize" runat="server" ControlName="DefaultSize" ResourceKey="DefaultSize" Suffix=":" />
		<asp:DropDownList ID="DefaultSize" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowResize" runat="server" ControlName="AllowResize" ResourceKey="AllowResize" Suffix=":" />
		 <asp:Checkbox ID="AllowResize" runat="server" Checked="true" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblDefaultView" runat="server" ControlName="DefaultView" ResourceKey="DefaultView" Suffix=":" />
		<asp:DropDownList ID="DefaultView" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowViewChange" runat="server" ControlName="AllowViewChange" ResourceKey="AllowViewChange" Suffix=":" />
		<asp:Checkbox ID="AllowViewChange" runat="server" Checked="true" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblShowTooltip" runat="server" ControlName="ShowTooltip" ResourceKey="ShowTooltip" Suffix=":" />
		<asp:Checkbox ID="ShowTooltip" runat="server" Checked="true" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblConsoleWidth" runat="server" ControlName="ConsoleWidth" ResourceKey="ConsoleWidth" Suffix=":" />
		<asp:TextBox ID="ConsoleWidth" runat="server" Text="" />
	</div>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function toggleMode(animation) {
            var mode = $('#<%= modeList.ClientID %>').val(); /*0,1*/
            if (mode !== "Profile") {
                animation ? $('#parentTabRow').slideDown() : $('#parentTabRow').show();
                animation ? $('#includeParentRow').slideDown() : $('#includeParentRow').show();
            }
            else {
                animation ? $('#parentTabRow').slideUp('fast') : $('#parentTabRow').hide();
                animation ? $('#includeParentRow').slideUp('fast') : $('#includeParentRow').hide();
            }
        }

        function setupSettings() {
            toggleMode(false);
            $('#<%= modeList.ClientID %>').change(function () {
                toggleMode(true);
            });
        }

        $(document).ready(function () {
            setupSettings();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupSettings();
            });
        });

    } (jQuery, window.Sys));
</script>   