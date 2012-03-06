<%@ Control Language="C#" Inherits="FiftyOne.Modules.Administration, FiftyOne.DotNetNuke" AutoEventWireup="true" CodeBehind="Administration.ascx.cs" %>
<%@ Register Assembly="FiftyOne.Foundation" Namespace="FiftyOne.Foundation.UI.Web" TagPrefix="fiftyOne" %>
<%@ Register Src="DeviceExplorer.ascx" TagPrefix="fiftyOneDnn" TagName="DeviceExplorer" %>
<%@ Register Src="PropertyDictionary.ascx" TagPrefix="fiftyOneDnn" TagName="PropertyDictionary" %>
<%@ Register Src="UserAgentTester.ascx" TagPrefix="fiftyOneDnn" TagName="UserAgentTester" %>

<div class="dnnForm" id="51D">
    <ul class="dnnAdminTabNav">
        <li><a href="#Dictionary">Property Dictionary</a></li>
        <li><a href="#Explorer">Device Explorer</a></li>
        <li><a href="#UserAgentTester">UserAgent Tester</a></li>
        <% if (IsPremium == false)
           { %>
        <li><a href="#Activate">Activate Premium</a></li>
        <% } %>
    </ul>
    <div id="Dictionary" class="dnnClear">
        <fiftyOneDnn:PropertyDictionary runat="server" ID="Dictionary51" CssClass="propertyDictionary" />
    </div>
    <div id="Explorer" class="dnnClear">
        <fiftyOneDnn:DeviceExplorer runat="server" ID="Explorer51" Navigation="true" CssClass="explorer" />
    </div>
    <div id="UserAgentTester" class="dnnClear">
        <fiftyOneDnn:UserAgentTester runat="server" ID="UserAgentTester51" />
    </div>
    <% if (IsPremium == false) { %>
    <div id="Activate" class="dnnClear">
        <fiftyOne:Activate runat="server" ID="Activate51" LogoEnabled="false" CssClass="activate" ErrorCssClass="dnnClear dnnFormMessage dnnFormValidationSummary" SuccessCssClass="dnnClear dnnFormMessage dnnFormSuccess" />
    </div>
    <% } %>
</div>

<script type="text/javascript">
    jQuery(function ($) {
        $('#51D').dnnTabs();
    });
</script>
