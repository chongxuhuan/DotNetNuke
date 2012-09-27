<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ModuleActions.ascx.cs" Inherits="DotNetNuke.Admin.Containers.ModuleActions" %>
<asp:LinkButton runat="server" ID="actionButton" />

<script language="javascript" type="text/javascript">
    /*globals jQuery, window */
    (function ($) {
        function setUpActions() {
            var moduleId = <% = ModuleContext.Configuration.ModuleID %>;
            var tabId = <% = ModuleContext.Configuration.TabID %>;
            
            //Initialise the actions menu plugin
            $('#<%= actionButton.ClientID %>').dnnModuleActions({
                actionButton: "<% =actionButton.UniqueID %>",
                moduleId: moduleId,
                tabId: tabId,
                customActions: <% = CustomActionsJSON %>,
                adminActions: <% = AdminActionsJSON %>,
                panes: <% = Panes %>,
                customText: "<% = CustomText %>",
                adminText: "<% = AdminText %>",
                moveText: "<% = MoveText %>",
                topText: '<% = LocalizeString("MoveTop.Action")%>',
                upText: '<% = LocalizeString("MoveUp.Action")%>',
                downText: '<% = LocalizeString("MoveDown.Action")%>',
                bottomText: '<% = LocalizeString("MoveBottom.Action")%>',
                movePaneText: '<% = LocalizeString("MoveToPane.Action")%>',
                supportsMove: <% = SupportsMove.ToString().ToLower() %>
            });
        }
        
        $(document).ready(function () {
            setUpActions();
        });
        
    } (jQuery));
</script>
