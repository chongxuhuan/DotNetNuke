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
        <li id="avalableExtensionsTab" runat="server" visible="false"><a href="#avalableExtensions"><%=LocalizeString("AvailableExtensions")%></a></li>
        <li id="moreExtensionsTab" runat="server" visible="false"><a href="#moreExtensions"><%=LocalizeString("MoreExtensions")%></a></li>
        <li class="dnnFormExpandContent"><a href=""><%=LocalizeString("ExpandAll")%></a></li>
    </ul>
    <div id="installedExtensions" class="exInstalledExtensions dnnClear">
        <dnn:InstalledExtensions id="installedExtensionsControl" runat="Server"/>
    </div>
    <div id="avalableExtensions" class="exAvailableExtensions dnnClear">
        <div class="exaeContent dnnClear">
            <dnn:AvailableExtensions id="avalableExtensionsControl" runat="Server" Visible="false"/>
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
    }
    $(document).ready(function () {
        setUpDnnExtensions();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpDnnExtensions();
        });
    });
</script>