<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.ControlPanels.ControlBar"
    CodeFile="ControlBar.ascx.cs" %>
<%@ Import Namespace="DotNetNuke.Security.Permissions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<asp:Panel ID="ControlPanel" runat="server">
    <div id="ControlBar">
        <div class="ControlContainer">
            <div class="ServiceIcon professional">
                <asp:Image ID="conrolbar_logo" runat="server" ImageUrl="~/admin/controlpanel/controlbarimages/dnnLogo.png"
                    AlternateText="DNNlogo" />
                <a href="#" id="ServiceImg">
                    <asp:Image ID="controlbar_serviceUpdate" runat="server" ImageUrl="~/admin/controlpanel/controlbarimages/service-update.png"
                        AlternateText="update" />
                </a>
            </div>
            <!-- close ServiceIcon -->
            <ul id="ControlNav">
                <% if (UserController.GetCurrentUserInfo().IsSuperUser)
                   {%>
                <li><a href="<%= GetTabURL("Host", true) %>">
                    <%= GetString("Tool.Host.Text") %></a>
                    <div class="subNav advanced">
                        <ul class="subNavToggle">
                            <li class="active BasicToggle"><a href="#controlbar_host_basic" title="Basic Controls">
                                <span></span></a></li>
                            <li class="AdvancedToggle"><a href="#controlbar_host_advanced" title="Advanced Controls">
                                <span></span></a></li>
                            <li class="BookmarkToggle"><a href="#controlbar_host_bookmarked" title="Bookmarked Controls">
                                <span></span></a></li>
                        </ul>
                        <dl id="controlbar_host_basic" class="active">
                            <dd>
                                <ul>
                                    <%= GetHostBaseMenu() %>
                                </ul>
                            </dd>
                        </dl>
                        <dl id="controlbar_host_advanced">
                            <dd>
                                <ul>
                                    <%= GetHostAdvancedMenu() %>
                                </ul>
                            </dd>
                        </dl>
                        <dl id="controlbar_host_bookmarked">
                            <dd>
                                <ul>
                                    <%= GetBookmarkItems("host") %>
                                </ul>
                            </dd>
                        </dl>
                    </div>
                    <!--close subNav-->
                </li>
                <% } %>
                <% if (UserController.GetCurrentUserInfo().IsInRole("Administrators"))
                   {%>
                <li><a href="<%= GetTabURL("Admin", false) %>">
                    <%= GetString("Tool.Admin.Text") %></a>
                    <div class="subNav advanced">
                        <ul class="subNavToggle">
                            <li class="active BasicToggle"><a href="#controlbar_admin_basic" title="Basic Controls">
                                <span></span></a></li>
                            <li class="AdvancedToggle"><a href="#controlbar_admin_advanced" title="Advanced Controls">
                                <span></span></a></li>
                            <li class="BookmarkToggle"><a href="#controlbar_admin_bookmarked" title="Bookmarked Controls">
                                <span></span></a></li>
                        </ul>
                        <dl id="controlbar_admin_basic" class="active">
                            <dd>
                                <ul>
                                    <%= GetAdminBaseMenu() %>
                                </ul>
                            </dd>
                        </dl>
                        <dl id="controlbar_admin_advanced">
                            <dd>
                                <ul>
                                    <%= GetAdminAdvancedMenu() %>
                                </ul>
                            </dd>
                        </dl>
                        <dl id="controlbar_admin_bookmarked">
                            <dd>
                                <ul>
                                    <%= GetBookmarkItems("admin") %>
                                </ul>
                            </dd>
                        </dl>
                    </div>
                    <!--close subNav-->
                </li>
                <li><a href="javascript:void(0)">
                    <%= GetString("Tool.Tools.Text") %></a>
                    <div class="subNav">
                        <dl>
                            <dd>
                                <ul>
                                    <li><a href='<%= BuildToolUrl("UploadFile", false, "File Manager", "Edit", "", true) %>'
                                        class="ControlBar_PopupLink_EditMode">
                                        <%= GetString("Tool.UploadFile.Text") %></a></li>
                                   <% if (UserController.GetCurrentUserInfo().IsSuperUser)
                                      {%>
                                    <li><a href='javascript:void(0)' id="controlBar_ClearCache">
                                        <%= GetString("Tool.ClearCache.Text") %></a></li>
                                    <li><a href='javascript:void(0)' id="controlBar_RecycleAppPool">
                                        <%= GetString("Tool.RecycleApp.Text") %></a></li>
                                    <li>
                                        <div id="SiteSelector">
                                            <p>
                                                <%= GetString("Tool.SwitchSites.Text") %></p>
                                            <select id="controlBar_SwitchSite">
                                                <% var switchSites = LoadPortalsList();%>
                                                <% foreach (var site in switchSites)
                                                   { %>
                                                <option value='<%= site[1] %>'>
                                                    <%= site[0] %></option>
                                                <% } %>
                                            </select>
                                            <input type="submit" value="<%= GetString("Tool.SwitchSites.Button") %>" id="ControlBar_SwitchSiteButton" />
                                        </div>
                                    </li>
                                    <% } %>
                                </ul>
                            </dd>
                        </dl>
                    </div>
                    <!--close subNav-->
                </li>
                <li><a href="javascript:void(0)">
                    <%= GetString("Tool.Help.Text") %></a>
                    <div class="subNav">
                        <dl>
                            <dd>
                                <ul>
                                    <li><a href="http://help.dotnetnuke.com">
                                        <%= GetString("Tool.Help.ToolTip") %></a></li>
                                    <li><a href="<%= GetTabURL("Getting Started", false) %>">
                                        <%= GetString("Tool.InstallWizard.Text") %></a></li>
                                </ul>
                            </dd>
                        </dl>
                    </div>
                    <!--close subNav-->
                </li>
                <% } %>
            </ul>
            <!--close ControlNav-->
          
            <ul id="ControlActionMenu">
               <% if (UserController.GetCurrentUserInfo().IsInRole("Administrators"))
                  {%>
                <li><a href="javascript:void(0)" id="ControlBar_Action_Menu">
                    <%= GetString("Tool.Modules.Text") %></a>
                    <ul>
                        <li><a href="javascript:void(0)" id="controlBar_AddNewModule">
                            <%= GetString("Tool.AddNewModule.Text") %></a> </li>
                        <li><a href="javascript:void(0)" id="controlBar_AddExistingModule">
                            <%= GetString("Tool.AddExistingModule.Text") %></a> </li>
                        <li><a href='<%= GetTabURL("Extensions", true) %>?editmode=true'>
                            <%= GetString("Tool.FindModules.Text") %></a> </li>
                    </ul>
                </li>
               <% } %>
               
                <% if (TabPermissionController.CanAddPage() || TabPermissionController.CanCopyPage() || TabPermissionController.CanImportPage())
                   {%>
                <li><a href="#">
                    <%= GetString("Tool.Pages.Text") %></a>
                    <ul>
                        <% if (TabPermissionController.CanAddPage())
                           {%>
                        <li><a href="<%= BuildToolUrl("NewPage", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.AddNewPage.Text") %></a></li>
                        <% } %>
                        
                        <% if (TabPermissionController.CanCopyPage())
                           {%>
                        <li><a href="<%= BuildToolUrl("CopyPage", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.CopyPage.Text") %></a></li>
                        <% } %>
                        
                        <% if (TabPermissionController.CanImportPage())
                           {%>
                        <li><a href="<%= BuildToolUrl("ImportPage", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.ImportPage.Text") %></a></li>
                        <% } %>
                    </ul>
                </li>
                <% } %>
                
                 <% if (UserController.GetCurrentUserInfo().IsInRole("Administrators"))
                    {%>
                <li><a href="javascript:void(0)">
                    <%= GetString("Tool.Users.Text") %></a>
                    <ul>
                        <li><a href='<%= BuildToolUrl("NewUser", false, "User Accounts", "Edit", "", true) %>'
                            class="ControlBar_PopupLink_EditMode">
                            <%= GetString("Tool.NewUser.Text") %></a></li>
                        <li><a href='<%= GetTabURL("User Accounts", false) %>?editmode=true'>
                            <%= GetString("Tool.ManageUsers.Text") %></a></li>
                        <li><a href='<%= GetTabURL("Security Roles", false) %>?editmode=true'>
                            <%= GetString("Tool.ManageRoles.Text") %></a></li>
                    </ul>
                </li>
                <% } %>
            </ul>
           <% if (TabPermissionController.CanAddContentToPage() || TabPermissionController.CanManagePage() || TabPermissionController.CanAdminPage() ||
                  TabPermissionController.CanExportPage() || TabPermissionController.CanDeletePage())
           { %>
            <ul id="ControlEditPageMenu">
                <li><a href="javascript:void(0)" class="<%= SpecialClassWhenNotInViewMode() %>"><span
                    class="controlBar_editPageIcon"></span><span class="controlBar_editPageTxt">
                        <%= GetString("Tool.EditPage.Text") %></span></a>
                    <ul>
                        <% if (TabPermissionController.CanAddContentToPage())
                           {%>
                        <li class="controlBar_BlueEditPageBtn"><a href="javascript:void(0)" id="ControlBar_EditPage">
                            <%= GetEditButtonLabel() %></a></li>
                        <li class="controlBar_EditPageSection">
                            <input type="checkbox" id="ControlBar_ViewInLayout" <%= CheckedWhenInLayoutMode() %> /><label
                                for="ControlBar_ViewInLayout"><%= GetString("Tool.LayoutMode.Text") %></label></li>
                      
                        <li><a href="javascript:void(0)" id="ControlBar_ViewInPreview">
                            <%= GetString("Tool.MobilePreview.Text") %></a></li>
                        <% } %>
                        <% if (TabPermissionController.CanManagePage())
                           {%>
                        <li><a href="<%= BuildToolUrl("PageSettings", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.PageSettings.Text") %></a></li>
                        
                        <li><a href="<%= BuildToolUrl("PageTemplate", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.ManageTemplate.Text") %></a></li>
                        <% } %>
                        <% if (TabPermissionController.CanAdminPage())
                           {%>
                        <li><a href="<%= BuildToolUrl("PagePermission", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.PagePermissions.Text") %></a></li>
                        <% } %>
                        <% if (TabPermissionController.CanExportPage())
                           {%>
                        <li><a href="<%= BuildToolUrl("ExportPage", false, "", "", "", true) %>" class="ControlBar_PopupLink">
                            <%= GetString("Tool.ExportPage.Text") %></a></li>
                        <% } %>
                        <% if (TabPermissionController.CanDeletePage())
                           {%>
                        <li><a href="<%= BuildToolUrl("DeletePage", false, "", "", "", true) %>" id="ControlBar_DeletePage">
                            <%= GetString("Tool.DeletePage.Text") %></a></li>
                        <% } %>
                       
                    </ul>
                    <div class="dnnClear">
                    </div>
                </li>
            </ul>
             <%}%>
        </div>
        
         <% if (UserController.GetCurrentUserInfo().IsInRole("Administrators"))
            {%>
        <div id="ControlBar_Module_AddNewModule" class="ControlModulePanel">
            <div class="ControlModuleContainer">
                <dnn:DnnComboBox ID="CategoryList" runat="server" DataTextField="Name" DataValueField="Name"
                    OnClientSelectedIndexChanged="dnn.controlBar.ControlBar_Module_CategoryList_Changed" />
            </div>
            <div id="ControlBar_ModuleListHolder_NewModule" class="ControlBar_ModuleListHolder">
                <ul class="ControlBar_ModuleList">
                </ul>
            </div>
        </div>
        <div id="ControlBar_Module_AddExistingModule" class="ControlModulePanel">
            <div class="ControlModuleContainer">
                <dnn:DnnComboBox ID="SiteList" runat="server" CssClass="dnnLeftComboBox" OnClientSelectedIndexChanged="dnn.controlBar.ControlBar_Module_SiteList_Changed" />
                <dnn:DnnComboBox ID="PageList" runat="server" CssClass="dnnLeftComboBox" OnClientSelectedIndexChanged="dnn.controlBar.ControlBar_Module_PageList_Changed" />
                <dnn:DnnComboBox ID="VisibilityLst" runat="server" CssClass="dnnLeftComboBox" Enabled="false" />
                <div class="ControlBar_chckCopyModule">
                    <input type="checkbox" id="ControlBar_Module_chkCopyModule" /><label for="ControlBar_Module_chkCopyModule"><%= GetString("Tool.MakeCopy.Text") %></label></div>
                <div class="dnnClear">
                </div>
            </div>
            <div id="ControlBar_ModuleListHolder_ExistingModule" class="ControlBar_ModuleListHolder">
                <ul class="ControlBar_ModuleList">
                </ul>
            </div>
        </div>
        <ul id="ControlBar_Module_ModulePosition">
            <% var panes = LoadPaneList();%>
            <% foreach (var p in panes)
               { %>
            <li data-pane='<%= p[1] %>' data-position='<%= p[2] %>'>
                <%= p[0] %></li>
            <% } %>
            <div class="dnnClear">
            </div>
        </ul>
        <% } %>
    </div>
    <!--close ControlBar	-->
    <div id="shareableWarning">
        <h3>
            <%= GetString("ShareableWarningHeader") %></h3>
        <br />
        <p>
            <%= GetString("ShareableWarningContent") %>
        </p>
        <br />
        <p>
            <%= GetString("ShareableWarningAlternate") %>
        </p>
        <br />
        <div align="center">
            <a href="javascript:void(0)" id="shareableWarning_cmdConfirm" class="dnnPrimaryAction">
                <%= GetString("cmdConfirmAdd") %>
            </a><a href="javascript:void(0)" id="sharableWarning_cmdCancel" class="dnnSecondaryAction">
                <%= GetString("cmdCancelAdd") %>
            </a>
        </div>
    </div>
</asp:Panel>
<script type="text/javascript">

    $(document).ready(function () {
        //attach mouse move to detect mouse button
        $(document).mousedown(function() { 
            dnn.controlBar.isMouseDown = true;
        });
        $(document).mouseup(function() {
            dnn.controlBar.isMouseDown = false;
        });

        var currentUserMode = '<%= GetModeForAttribute() %>';
        $('div.subNav').hide();
        $("#ControlNav li").hoverIntent(
            function() {
                $('.onActionMenu').removeClass('onActionMenu');
                toggleModulePane($('.ControlModulePanel'), false);
                $(this).find('div.subNav').slideDown(300);
            },
            function() {
                $(this).find('div.subNav').slideUp(200);
            }
        );

        $('#ControlActionMenu > li').hoverIntent(
            function() {
                $('.onActionMenu').removeClass('onActionMenu');
                toggleModulePane($('.ControlModulePanel'), false);
                $(this).find('ul').slideDown(200);
            },
            function() {
                $(this).find('ul').slideUp(150);
            }
        );

        $('ul#ControlEditPageMenu > li').hoverIntent(function() {
            $('.onActionMenu').removeClass('onActionMenu');
            toggleModulePane($('.ControlModulePanel'), false);
            $(this).find('ul').slideDown(400);
        }, function (){ $(this).find('ul').slideUp(300);});

        $('#ControlBar_Module_ModulePosition > li').hover(function() {
            $(this).parent().data('display', 'show').show();
        }, function() {
            var p = $(this).parent();
            p.data('display', 'none');
            setTimeout(function() {
                if (p.data('display') === 'none') {
                    p.hide();
                }
            }, 300);
        });

        //Handling the Advanced Subnav Toggling
        $(".subNavToggle li a").click(function(event) {
            var ul = $(this).closest('ul');
            var divSubNav = ul.parent();
            if (!($(this).parent('li').hasClass('active'))) {

                // Handling the toggle states
                $('li', ul).removeClass('active');
                $(this).parent('li').addClass('active');

                // Handling the respective subnavs
                var anchorTarget = $(this).attr('href');
                $('dl', divSubNav).hide();
                $(anchorTarget).show();
            }
            return false;
        });

        $('#controlBar_AddNewModule').click(function() {
            if (currentUserMode !== 'EDIT') {
                var service = dnn.controlBar.getService();
                var serviceUrl = dnn.controlBar.getServiceUrl(service);
                $.ajax({
                    url: serviceUrl + 'ToggleUserMode',
                    type: 'POST',
                    data: { UserMode: 'EDIT' },
                    beforeSend: service.setModuleHeaders,
                    success: function() {
                        dnn.dom.setCookie('ControlBarInit', 'AddNewModule');
                        window.location.href = window.location.href.split('#')[0];
                    },
                    error: function() {
                    }
                });

                return false;   
            }

            var category = $find('<%= CategoryList.ClientID %>').get_value();
            dnn.controlBar.getDesktopModulesForNewModule(category);
            dnn.controlBar.addNewModule = true;
            toggleModulePane($('#ControlBar_Module_AddNewModule'), true);
            $('#ControlBar_Action_Menu').addClass('onActionMenu');
            return false;
        });

        $('#controlBar_AddExistingModule').click(function() {
            if (currentUserMode !== 'EDIT') {
                var service = dnn.controlBar.getService();
                var serviceUrl = dnn.controlBar.getServiceUrl(service);
                $.ajax({
                    url: serviceUrl + 'ToggleUserMode',
                    type: 'POST',
                    data: { UserMode: 'EDIT' },
                    beforeSend: service.setModuleHeaders,
                    success: function() {
                        dnn.dom.setCookie('ControlBarInit', 'AddExistingModule');
                        window.location.href = window.location.href.split('#')[0];
                    },
                    error: function() {
                    }
                });

                return false;
            }
            var portal = $find('<%= SiteList.ClientID %>').get_value();
            dnn.controlBar.getPageList(portal);
            dnn.controlBar.addNewModule = false;
            toggleModulePane($('#ControlBar_Module_AddExistingModule'), true);
            $('#ControlBar_Action_Menu').addClass('onActionMenu');
            return false;
        });

        $('#ControlBar_Module_ModulePosition > li').click(function() {
            if (dnn.controlBar.addingModule) return false;
            dnn.controlBar.addingModule = true;

            var module = dnn.controlBar.selectedModule;
            var page = $find('<%= PageList.ClientID %>').get_value();
            var pane = $(this).data('pane');
            var position = $(this).data('position');
            var visibility = $find('<%= VisibilityLst.ClientID %>').get_value();
            var copyModule = $('#ControlBar_Module_chkCopyModule').get(0).checked;
            var addExistingModule = !dnn.controlBar.addNewModule;
            dnn.controlBar.addModule(module.data('module') + '', page, pane, position, '-1', visibility, addExistingModule + '', copyModule + '');
            return false;
        });

        $('#controlBar_ClearCache').click(function() {
            dnn.controlBar.clearHostCache();
        });

        $('#controlBar_RecycleAppPool').click(function() {
            dnn.controlBar.recycleAppPool();
        });

        $('#ControlBar_SwitchSiteButton').click(function() {
            var site = $('#controlBar_SwitchSite').val();
            dnn.controlBar.switchSite(site);
            return false;
        });

        var toggleModulePane = function(pane, show) {
            if (show) {
                pane.animate({ height: 'show' }, 300, function() {
                    $('#ControlBar_ControlPanel').css('height', 303);
                    $(window).resize();
                });
                
            } else {
                pane.animate({ height: 'hide'}, 200, function () {
                    $('#ControlBar_ControlPanel').css('height', 53);
                    $(window).resize();
                });
            }
        };

        // generate url and popup
        $('a.ControlBar_PopupLink').click(function() {
            var href = $(this).attr('href');
            if (href) {
                dnnModal.show(href + '?popUp=true', true, 550, 950, true, '');
            }
            return false;
        });

        $('a.ControlBar_PopupLink_EditMode').click(function() {
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            var that = this;

            if (currentUserMode !== 'EDIT') {
                var mode = 'EDIT';
                $.ajax({
                    url: serviceUrl + 'ToggleUserMode',
                    type: 'POST',
                    data: { UserMode: mode },
                    beforeSend: service.setModuleHeaders,
                    success: function() {
                        // then popup
                        var href = $(that).attr('href');
                        if (href) {
                            dnnModal.show(href + '?popUp=true', true, 550, 950, true, '');
                        }
                    },
                    error: function() {
                    }
                });
            } else {
                var href = $(that).attr('href');
                if (href) {
                    dnnModal.show(href + '?popUp=true', true, 550, 950, true, '');
                }
            }
            return false;
        });

        var yesText = '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Yes.Text", Localization.SharedResourceFile)) %>';
        var noText = '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("No.Text", Localization.SharedResourceFile)) %>';
        var titleText = '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Confirm.Text", Localization.SharedResourceFile)) %>';

        $('a#ControlBar_DeletePage').dnnConfirm({
            text: '<%= GetButtonConfirmMessage("DeletePage") %>',
            yesText: yesText,
            noText: noText,
            title: titleText
        });

        $('a#shareableWarning_cmdConfirm').click(function() {
            dnn.controlBar.hideShareableWarning();
            if (dnn.controlBar.addModuleDataVar) {
                dnn.controlBar.doAddModule(dnn.controlBar.addModuleDataVar);
            }
            dnn.controlBar.addModuleDataVar = null;
            dnn.controlBar.addingModule = false;
        });

        $('a#shareableWarning_cmdCancel').click(function() {
            dnn.controlBar.hideShareableWarning();
            dnn.controlBar.addModuleDataVar = null;
            dnn.controlBar.addingModule = false;
        });

        $('a#ControlBar_EditPage').click(function() {
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            var mode = currentUserMode === 'EDIT' ? 'VIEW' : 'EDIT';
            $.ajax({
                url: serviceUrl + 'ToggleUserMode',
                type: 'POST',
                data: { UserMode: mode },
                beforeSend: service.setModuleHeaders,
                success: function() {
                    window.location.href = window.location.href.split('#')[0];
                },
                error: function() {
                }
            });
        });

        $('#ControlBar_ViewInLayout').change(function() {
            var mode = this.checked ? "LAYOUT" : "VIEW";
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'ToggleUserMode',
                type: 'POST',
                data: { UserMode: mode },
                beforeSend: service.setModuleHeaders,
                success: function() {
                    window.location.href = window.location.href;
                },
                error: function() {
                }
            });
        });

        $('a#ControlBar_ViewInPreview').click(function() {
            <%=PreviewPopup() %>;
        });

        $('a.bookmark').live('click', function() {
            var $this = $(this);
            var wrapper = $this.closest('dl');
            var title = wrapper.attr('id').indexOf('host') > 0 ? 'host' : 'admin';
            var outerWrapper = wrapper.parent();
            var wrapperId = wrapper.attr('id');
            var bookmarkWrapper = $('dl', outerWrapper).last();
            var ul = $('ul', bookmarkWrapper);
            var bookmarkList = $('ul > li > a', bookmarkWrapper).not('.bookmark');
            var removeBookmarksTip = '<%= GetString("Tool.RemoveFromBookmarks.ToolTip") %>';
            if (wrapperId.indexOf('bookmarked') > 0) {
                // already bookmarked, remove it
                $this.parent().remove();
                
                // save bookmark to server
                dnn.controlBar.saveBookmark(title, ul);
                
            } else {
                // add to bookmark
                var bookmarkUrl = $this.prev();
                var bookmarkTabname = $this.parent().data('tabname');
                var bookmarkHtml = bookmarkUrl.html();
                // check conflict or not
                var isConflict = false;
                bookmarkList.each(function(n, v) {
                    var html = $(v).html();
                    if (bookmarkHtml === html) {
                        isConflict = true;
                        return false;
                    }
                    return true;
                });

                if (!isConflict) {
                    // add url to bookmark
                    var li = $('<li></li>');
                    li.data('tabname', bookmarkTabname);
                    li.append(bookmarkUrl.clone());
                    li.append($("<a href='javascript:void(0)' class='bookmark' title='" + removeBookmarksTip + "'><span></span></a>"));
                    ul.append(li);

                    // save bookmark to server
                    dnn.controlBar.saveBookmark(title, ul);
                    // focus on bookmark tab
                    $('li.BookmarkToggle > a', outerWrapper).click();
                }
            }
        });

        // initialize
        var initAction = dnn.dom.getCookie('ControlBarInit');
        if (initAction){
            dnn.dom.setCookie('ControlBarInit', '', -1);
            switch(initAction)
            {
                case 'AddNewModule':
                    setTimeout(function() {
                        $('#controlBar_AddNewModule').click();
                    }, 500);
                    
                    break;
                case 'AddExistingModule':                    
                     setTimeout(function() {
                        $('#controlBar_AddExistingModule').click();
                    }, 500);
                    break;
            }
       }

    });



    dnn = dnn || {};
    dnn.controlBar = dnn.controlBar || {};
    dnn.controlBar.selectedModule = null;
    dnn.controlBar.addNewModule = true;
    dnn.controlBar.addingModule = false;
    dnn.controlBar.addModuleDataVar = null;
    dnn.controlBar.isMouseDown = false;
    dnn.controlBar.getService = function () {
        return $.dnnSF();
    };
    dnn.controlBar.getServiceUrl = function (service) {
        service = service || dnn.controlBar.getService();
        return service.getServiceRoot('internalservices') + 'controlbar/';
    };
    dnn.controlBar.getBookmarkItems = function(ul) {
        var items = [];
        $('li', ul).each(function() {
            var tabname = $(this).data('tabname');
            if(tabname)
                items.push(tabname);
        });
        return items.join(',');
    };
    dnn.controlBar.saveBookmark = function(title, ul) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        var bookmark = dnn.controlBar.getBookmarkItems(ul);
        $.ajax({
            url: serviceUrl + 'SaveBookmark',
            type: 'POST',
            data: { Title: title, Bookmark: bookmark },
            beforeSend: service.setModuleHeaders,
            success: function() {
            },
            error: function() {
            }
        });
    };
    dnn.controlBar.getDesktopModulesForNewModule = function (category) {
        var serviceUrl = dnn.controlBar.getServiceUrl();
        $.ajax({
            url: serviceUrl + 'GetPortalDesktopModules',
            type: 'GET',
            data: 'category=' + category,
            success: function (d) {
                var containerId = '#ControlBar_ModuleListHolder_NewModule';
                dnn.controlBar.renderModuleList(containerId, d);
            },
            error: function () {
            }
        });
    };
    dnn.controlBar.getTabModules = function (tab) {
        var serviceUrl = dnn.controlBar.getServiceUrl();
        $.ajax({
            url: serviceUrl + 'GetTabModules',
            type: 'GET',
            data: 'tab=' + tab,
            success: function (d) {
                var containerId = '#ControlBar_ModuleListHolder_ExistingModule';
                dnn.controlBar.renderModuleList(containerId, d);
            },
            error: function () {
            }
        });
    };
    dnn.controlBar.getPageList = function (portal) {
        var serviceUrl = dnn.controlBar.getServiceUrl();
        $.ajax({
            url: serviceUrl + 'GetPageList',
            type: 'GET',
            data: 'portal=' + portal,
            success: function (d) {
                var combo = $find('<%= PageList.ClientID %>');
                combo.clearItems();
                for (var i = 0; i < d.length; i++) {
                    var txt = d[i].IndentedTabName;
                    var val = d[i].TabID;

                    var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(txt);
                    comboItem.set_value(val);
                    combo.get_items().add(comboItem);
                }
            },
            error: function () {
            }
        });
    };

    dnn.controlBar.addModule = function (module, page, pane, position, sort, visibility, addExistingModule, copyModule) {
        var dataVar = { Module: module, Page: page, Pane: pane,
            Position: position, Sort: sort,
            Visibility: visibility,
            AddExistingModule: addExistingModule,
            CopyModule: copyModule
        };
        var sharing = (dnn.getVar('moduleSharing') || 'false') == 'true';

        if (sharing) {
            var selectedPortalId = $find('<%= SiteList.ClientID %>').get_value();
            var selectedTabId = page;
            var selectedModuleId = module;

            var parameters = {
                ModuleId: selectedModuleId,
                TabId: selectedTabId,
                PortalId: selectedPortalId
            };

            $.ajax({
                url: '<%= ResolveClientUrl("~/DesktopModules/InternalServices/API/ModuleService/GetModuleShareable") %>',
                type: 'POST',
                async: false,
                data: parameters,
                success: function (m) {
                    if (!m) {
                        dnn.controlBar.addingModule = false;
                        return;
                    }

                    if (m.RequiresWarning) {
                        dnn.controlBar.popupShareableWarning();
                        dnn.controlBar.addModuleDataVar = dataVar;
                    }
                },
                error: function () {
                    // Ugh, just let the request continue...
                    dnn.controlBar.addingModule = false;
                }
            });
        }
        else
            dnn.controlBar.doAddModule(dataVar);
    };

    dnn.controlBar.doAddModule = function(dataVar) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'AddModule',
            type: 'POST',
            data: dataVar,
            beforeSend: service.setModuleHeaders,
            success: function() {
                dnn.controlBar.addingModule = false;
                window.location.href = window.location.href.split('#')[0];
            },
            error: function() {
                dnn.controlBar.addingModule = false;
            }
        });
    };

    dnn.controlBar.clearHostCache = function () {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'ClearHostCache',
            type: 'POST',
            beforeSend: service.setModuleHeaders,
            success: function () {
                window.location.href = window.location.href.split('#')[0];
            },
            error: function () {
            }
        });
    };

    dnn.controlBar.recycleAppPool = function () {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'RecycleApplicationPool',
            type: 'POST',
            beforeSend: service.setModuleHeaders,
            success: function () {
                window.location.href = window.location.href.split('#')[0];
            },
            error: function () {
            }
        });
    };

    dnn.controlBar.switchSite = function(site) {
        if (site) {
            var dataVar = { Site: site };
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'SwitchSite',
                type: 'POST',
                data: dataVar,
                beforeSend: service.setModuleHeaders,
                success: function(d) {
                    if (d && d.RedirectURL)
                        window.location.href = d.RedirectURL;
                },
                error: function() {
                }
            });
        }
    };

    dnn.controlBar.popupShareableWarning = function() {
        $('#shareableWarning').dialog({
            autoOpen: true,
            resizable: false,
            modal: true,
            width: '500px',
            zIndex: 1000,
            stack: false,
            title: '<%= GetString("ShareableWarningTitle") %>',
            dialogClass: 'dnnFormPopup dnnClear',
            open: function() {
            },
            close: function() {
            }
        });
    };

    dnn.controlBar.hideShareableWarning = function() {
        $('#shareableWarning').dialog('close');
    };
    
    dnn.controlBar.renderModuleList = function (containerId, moduleList) {
        var container = $(containerId);
        var api = container.data('jsp');
        if (api) {
            api.scrollToX(0, null);
            api.destroy();
        }
        
        container = $(containerId).css('overflow', 'hidden');
        var ul = $('ul.ControlBar_ModuleList', container);
        ul.empty().css('left', 1000);

        $('#ControlBar_Module_ModulePosition').hide();
        for (var i = 0; i < moduleList.length; i++) {
            ul.append('<li><div class="ControlBar_ModuleDiv" data-module=' + moduleList[i].ModuleID + '><div class="ModuleLocator_Menu"></div><img src="' + moduleList[i].ModuleImage + '" alt="" /><span>' + moduleList[i].ModuleName + '</span></div></li>');
        }
        var ulWidth = moduleList.length * 160;
        ul.css('width', ulWidth + 'px');
        ul.animate({ left: 0 }, 300, function () {
            container.css('overflow', 'auto').jScrollPane();
            $('div.ControlBar_ModuleDiv', ul).each(function() {
                if(!this.id)
                    this.id = 'ControlBar_ModuleDiv_' + $(this).data('module');
            }).hover(function() {
                
                if(!dnn.controlBar.isMouseDown) {
                    var $this = $(this);
                    var dataModuleId = $this.data('module');
                    if (dnn.controlBar.selectedModule && dnn.controlBar.selectedModule.data('module') !== dataModuleId) {
                        dnn.controlBar.selectedModule.removeClass('ControlBar_Module_Selected');
                    }
                    $this.addClass('ControlBar_Module_Selected');
                    dnn.controlBar.selectedModule = $this;

                    var holderId = this.id;
                    $this.dnnHelperTip({
                        helpContent: "Drag this Module to new Location",
                        holderId: holderId
                    });
                }

            }, function() {
                $(this).dnnHelperTipDestroy();
            })
            .draggable({
                helper: function(event, ui) {
                    var dragTip = $('<div class="dnnDragdropTip"></div>');
                    var title = $('span', this).html();
                    dragTip.html(title);
                    $('body').append(dragTip);

                    // destroy tooltip
                    $(this).dnnHelperTipDestroy();
                    //set data
                    dnn.controlBar.dragdropModule = $(this).data('module');
                    dnn.controlBar.dragdropPage = $find('<%= PageList.ClientID %>').get_value();
                    dnn.controlBar.dragdropVisibility = $find('<%= VisibilityLst.ClientID %>').get_value();
                    dnn.controlBar.dragdropCopyModule = $('#ControlBar_Module_chkCopyModule').get(0).checked;
                    dnn.controlBar.dragdropAddExistingModule = !dnn.controlBar.addNewModule;
                    return dragTip;
                },
                cursorAt: { left: 10, top: 30 },
                connectToSortable: '.dnnSortable'
            });
                

            $('div.ModuleLocator_Menu', ul).hoverIntent(function () {
                var $this = $(this);
                var left = $this.offset().left;
                $('#ControlBar_Module_ModulePosition').data('display', 'show').css({ left: left - 180, top: 133 }).show();
            }, function () {
                var p = $('#ControlBar_Module_ModulePosition');
                p.data('display', 'none');
                setTimeout(function() {
                    if(p.data('display') === 'none') {
                        p.hide();
                    }
                }, 300);
            });
        });
    };

    dnn.controlBar.ControlBar_Module_CategoryList_Changed = function(sender, e) {
        var item = e.get_item();
        if (item) {
            dnn.controlBar.getDesktopModulesForNewModule(item.get_value());
        }
    };

    dnn.controlBar.ControlBar_Module_SiteList_Changed = function(sender, e) {
        var item = e.get_item();
        if (item) {
            dnn.controlBar.getPageList(item.get_value());
        }
    };

    dnn.controlBar.ControlBar_Module_PageList_Changed = function(sender, e) {
        var item = e.get_item();
        var visibilityCombo = $find('<%= VisibilityLst.ClientID %>');
        if (item) {
            var val = item.get_value();
            if (val) {
                dnn.controlBar.getTabModules(item.get_value());
                visibilityCombo.enable();
            } else {
                dnn.controlBar.renderModuleList($('#ControlBar_ModuleListHolder_ExistingModule'), []);
                visibilityCombo.disable();
            }
        }
    };
</script>
