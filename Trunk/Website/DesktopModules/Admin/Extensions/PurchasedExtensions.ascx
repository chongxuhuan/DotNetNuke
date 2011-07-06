<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.PurchasedExtensions" CodeFile="PurchasedExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"%>

<dnn:DnnAjaxLoadingPanel ID="loadingPanel" runat="server" Skin="Default">

</dnn:DnnAjaxLoadingPanel>
    <dnn:DnnAjaxPanel ID="ajaxPanel" runat="server" LoadingPanelID="loadingPanel" RestoreOriginalRenderDelegate="false">
        <div class="dnnForm dnnPurchasedExtensions dnnClear" id="dnnPurchasedExtensions">
            <h2 class="dnnFormSectionHead"><asp:Label ID="lblTitle" runat="server"><% =GetLocalizedString("PurchasedTitle")%></asp:Label></h2>
            <div class="dnnFormMessage dnnFormInfo"><asp:Label ID="lblHelp" runat="server"><% =GetLocalizedString("PurchasedHelp")%></asp:Label></div>
            <div id="loginWarning" runat="server" class="dnnFormMessage dnnFormWarning" visible="false"><asp:Label ID="Label1" runat="server"><% =GetLocalizedString("SnowcoveredLogin")%></asp:Label></div>
            <div id="error" runat="server" class="dnnFormMessage dnnFormWarning" visible="false"><asp:Label ID="Label2" runat="server"><% =GetLocalizedString("WebserviceFailure")%></asp:Label></div>
   
            <asp:GridView ID="grdSnow" runat="server"></asp:GridView>
    
            <ul class="dnnActions dnnClear">
    	        <li><asp:HyperLink id="snowcoveredLogin" runat="server" CssClass="dnnPrimaryAction" resourcekey="snowcoveredLogin" /></li>
                <li><asp:HyperLink id="deleteCredentials" runat="server" CssClass="dnnPrimaryAction" resourcekey="deleteCredentials" /></li>
                <li><asp:LinkButton id="fetchExtensions" runat="server" CssClass="dnnPrimaryAction" resourcekey="fetchExtensions" Visible="false" /></li>
                
            </ul>
        </div>          
    </dnn:DnnAjaxPanel>