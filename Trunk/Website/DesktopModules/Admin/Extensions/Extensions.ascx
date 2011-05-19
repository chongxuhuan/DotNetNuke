<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.Extensions" CodeFile="Extensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>                
<%@ Register TagPrefix="dnn" TagName="InstalledExtensions" Src="~/DesktopModules/Admin/Extensions/InstalledExtensions.ascx" %>                
<%@ Register TagPrefix="dnn" TagName="AvailableExtensions" Src="~/DesktopModules/Admin/Extensions/AvailableExtensions.ascx" %>                
<%@ Register TagPrefix="dnn" TagName="MoreExtensions" Src="~/DesktopModules/Admin/Extensions/MoreExtensions.ascx" %>    


<div class="dnnForm dnnExtensions dnnClear" id="dnnExtensions">
	<ul class="dnnAdminTabNav dnnClear">
        <li id="installedExtensionsTab" runat="server" visible="false"><a href="#installedExtensions"><%=LocalizeString("InstalledExtensions")%></a></li>
        <li id="availableExtensionsTab" runat="server" visible="false"><a href="#availableExtensions"><%=LocalizeString("AvailableExtensions")%></a></li>
        <li id="moreExtensionsTab" runat="server" visible="false"><a href="#moreExtensions"><%=LocalizeString("MoreExtensions")%></a></li>
    </ul>
    <div id="installedExtensions" class="exInstalledExtensions dnnClear">
        <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
        <dnn:InstalledExtensions id="installedExtensionsControl" runat="Server"/>
    </div>
    <div id="availableExtensions" class="exAvailableExtensions dnnClear">
        <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
        <div class="exaeContent dnnClear">
            <dnn:AvailableExtensions id="availableExtensionsControl" runat="Server" Visible="false"/>
        </div>
    </div>
    <div id="moreExtensions" class="exMoreExtensions dnnClear">
        <div class="exmeContent dnnClear">
        <dnn:MoreExtensions id="moreExtensionsControl" runat="Server" Visible="false"/>
        </div>
    </div>
</div>

<script language="javascript" type="text/javascript">
    function setUpDnnExtensions() {
        $('#dnnExtensions').dnnTabs().dnnPanels();
        $('#availableExtensions .dnnFormExpandContent a').dnnExpandAll({
            expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
            collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
            targetArea: '#availableExtensions'
        });
        $('#installedExtensions .dnnFormExpandContent a').dnnExpandAll({
            expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
            collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
            targetArea: '#installedExtensions'
        });
    }
    $(document).ready(function () {
        setUpDnnExtensions();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpDnnExtensions();
        });
    });
</script>