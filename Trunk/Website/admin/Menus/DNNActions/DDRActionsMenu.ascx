<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="dnn" TagName="ACTIONS" src="~/DesktopModules/DDRMenu/Actions.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<dnn:ACTIONS ID="actionMenu" MenuStyle="admin/Menus/DNNActions" runat="server" />
<dnn:DnnScriptBlock ID="confirmScript" runat="server">
	<script type="text/javascript">
	(function($) {
		$(document).ready(function() {
			var confirmString = "confirm('<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("DeleteModule.Confirm"))%>')";
			$("a[href*=\"" + confirmString +"\"]").attr("href", function () {
				return $(this).attr("href").replace( /if\(.+?\)\{(.+?)\}/ , "$1");//remove its original confirm function
			}).dnnConfirm({
				text: '<%= Localization.GetString("DeleteModule.Confirm"), Localization.SharedResourceFile %>',
				yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
				noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
				title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
			});
		});
	}(jQuery));
	</script>
</dnn:DnnScriptBlock>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if(!Context.Items.Contains("ActionsMenuLoaded"))
		{
			Context.Items.Add("ActionsMenuLoaded", true);
		}
		else
		{
			confirmScript.Visible = false;
		}
	}
</script>