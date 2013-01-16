if (typeof dnn === 'undefined') dnn = {};
dnn.controlBar = dnn.controlBar || {};
dnn.controlBar.init = function (settings) {
    dnn.controlBar.selectedModule = null;
    dnn.controlBar.addNewModule = true;
    dnn.controlBar.addingModule = false;
    dnn.controlBar.addModuleDataVar = null;
    dnn.controlBar.isMouseDown = false;
    dnn.controlBar.hideModuleLocationMenu = true;
    dnn.controlBar.showSelectedModule = false;
    dnn.controlBar.status = null;
    dnn.controlBar.getService = function () {
        return $.dnnSF();
    };
    dnn.controlBar.getServiceUrl = function (service) {
        service = service || dnn.controlBar.getService();
        return service.getServiceRoot('internalservices') + 'controlbar/';
    };
    dnn.controlBar.saveStatus = function () {
        var categoryComboVal = $find(settings.categoryComboId).get_value();
        var siteComboVal = $find(settings.portalComboId).get_value();
        var pageComboVal = $find(settings.pageComboId).get_value();
        var visibilityComboVal = $find(settings.visibilityComboId).get_value();
        var persistValue = [dnn.controlBar.addNewModule, categoryComboVal, siteComboVal, pageComboVal, visibilityComboVal].join('|');
        dnn.dom.setCookie('ControlBarInitStatus', persistValue);
        dnn.controlBar.status = null;
    };
    dnn.controlBar.loadStatus = function () {
        var persistValue = dnn.dom.getCookie('ControlBarInitStatus');
        if (persistValue) {
            var persits = persistValue.split('|');
            dnn.controlBar.status = {
                addNewModule: persits[0] == 'true',
                category: persits[1],
                site: persits[2],
                page: persits[3],
                visibility: persits[4]
            };
        }
        else {
            dnn.controlBar.status = null;
        }
        dnn.dom.setCookie('ControlBarInitStatus', '', -1);
    };
    dnn.controlBar.removeStatus = function () {
        dnn.controlBar.status = null;
        dnn.dom.setCookie('ControlBarInitStatus', '', -1);
    };
    dnn.controlBar.responseError = function (xhr) {
        if (xhr) {
            if (xhr.status == '401') {
                dnnModal.show(settings.loginUrl + '?popUp=true', true, 300, 650, true, '');
            }
        }
    };
    dnn.controlBar.getBookmarkItems = function (ul) {
        var items = [];
        $('li', ul).each(function () {
            var tabname = $(this).data('tabname');
            if (tabname)
                items.push(tabname);
        });
        return items.join(',');
    };
    dnn.controlBar.saveBookmark = function (title, ul) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        var bookmark = dnn.controlBar.getBookmarkItems(ul);
        $.ajax({
            url: serviceUrl + 'SaveBookmark',
            type: 'POST',
            data: { Title: title, Bookmark: bookmark },
            beforeSend: service.setModuleHeaders,
            success: function () {
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    };
    dnn.controlBar.setModuleListLoading = function (containerId, hideLoading, showNoResultMessage) {
        var loadingContainer = $(containerId);
        var messageContainer = loadingContainer.prev();
        var listContainer = loadingContainer.next();
        var scrollContainer = listContainer.next();
        if (hideLoading) {
            if (showNoResultMessage) {
                listContainer.hide();
                scrollContainer.hide();
                loadingContainer.hide();
                messageContainer.show();

                $('p.ControlBar_ModuleListMessage_InitialMessage', messageContainer).hide();
                $('p.ControlBar_ModuleListMessage_NoResultMessage', messageContainer).show();

            }
            else {
                listContainer.show();
                scrollContainer.show();
                loadingContainer.hide();
                messageContainer.hide();
            }
        } else {
            listContainer.hide();
            scrollContainer.hide();
            loadingContainer.show();
            messageContainer.hide();
        }
    };
    dnn.controlBar.getDesktopModulesForNewModule = function (category) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_NewModule');
        $.ajax({
            url: serviceUrl + 'GetPortalDesktopModules',
            type: 'GET',
            data: 'category=' + category,
            beforeSend: service.setModuleHeaders,
            success: function (d) {
                if (d && d.length) {
                    dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_NewModule', true);
                    var containerId = '#ControlBar_ModuleListHolder_NewModule';
                    dnn.controlBar.renderModuleList(containerId, d);
                }
                else {
                    dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_NewModule', true, true);
                }
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    };
    dnn.controlBar.getTabModules = function (tab) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_ExistingModule');
        $.ajax({
            url: serviceUrl + 'GetTabModules',
            type: 'GET',
            data: 'tab=' + tab,
            beforeSend: service.setModuleHeaders,
            success: function (d) {
                if (d && d.length) {
                    dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_ExistingModule', true);
                    var containerId = '#ControlBar_ModuleListHolder_ExistingModule';
                    dnn.controlBar.renderModuleList(containerId, d);
                }
                else {
                    dnn.controlBar.setModuleListLoading('#ControlBar_ModuleListWaiter_ExistingModule', true, true);
                }
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    };
    dnn.controlBar.getPageList = function (portal) {

        var messageContainer = $('#ControlBar_ModuleListMessage_ExistingModule');
        var loadingContainer = messageContainer.next();
        var listContainer = loadingContainer.next();
        var scrollContainer = listContainer.next();

        messageContainer.show();
        loadingContainer.hide();
        listContainer.hide();
        scrollContainer.hide();

        $('p.ControlBar_ModuleListMessage_InitialMessage', messageContainer).show();
        $('p.ControlBar_ModuleListMessage_NoResultMessage', messageContainer).hide();

        if (!portal) {
            var combo = $find(settings.pageComboId);
            combo.clearItems();
            var comboItem = new Telerik.Web.UI.RadComboBoxItem();
            comboItem.set_text(settings.selectPageText);
            comboItem.set_value("");
            combo.get_items().add(comboItem);
            comboItem.select();
            return;
        }

        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'GetPageList',
            type: 'GET',
            data: 'portal=' + portal,
            beforeSend: service.setModuleHeaders,
            success: function (d) {
                var combo = $find(settings.pageComboId);
                combo.clearItems();
                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                comboItem.set_text(settings.selectPageText);
                comboItem.set_value("");
                combo.get_items().add(comboItem);
                comboItem.select();
                for (var i = 0; i < d.length; i++) {
                    var txt = d[i].IndentedTabName;
                    var val = d[i].TabID;

                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(txt);
                    comboItem.set_value(val);
                    combo.get_items().add(comboItem);
                }

                if (dnn.controlBar.status && !dnn.controlBar.status.addNewModule && dnn.controlBar.status.page) {
                    combo.findItemByValue(dnn.controlBar.status.page).select();
                    dnn.controlBar.getTabModules(dnn.controlBar.status.page);

                }

            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
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

        if (sharing && !dnn.controlBar.addNewModule) {
            var selectedPortalId = $find(settings.portalComboId).get_value();
            var selectedTabId = page;
            var selectedModuleId = module;

            var parameters = {
                ModuleId: selectedModuleId,
                TabId: selectedTabId,
                PortalId: selectedPortalId
            };

            var moduleShareableUrl = $.dnnSF().getServiceRoot('internalservices') + 'ModuleService/GetModuleShareable';

            $.ajax({
                url: moduleShareableUrl,
                type: 'GET',
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
                    } else {
                        dnn.controlBar.doAddModule(dataVar);
                    }
                },
                error: function (xhr) {
                    dnn.controlBar.addingModule = false;
                    dnn.controlBar.responseError(xhr);
                }
            });
        }
        else
            dnn.controlBar.doAddModule(dataVar);
    };

    dnn.controlBar.doAddModule = function (dataVar) {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'AddModule',
            type: 'POST',
            data: dataVar,
            beforeSend: service.setModuleHeaders,
            success: function (d) {
                dnn.controlBar.addingModule = false;
                dnn.dom.setCookie('FadeModuleID', d.TabModuleID);
                // save status to restore add module panel when page refresh
                dnn.controlBar.saveStatus();
                window.location.href = window.location.href.split('#')[0];
            },
            error: function (xhr) {
                dnn.controlBar.addingModule = false;
                dnn.controlBar.responseError(xhr);
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
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    };

    dnn.controlBar.copyPermissionsToChildren = function () {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'CopyPermissionsToChildren',
            type: 'POST',
            beforeSend: service.setModuleHeaders,
            success: function () {
                window.location.href = window.location.href.split('#')[0];
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
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
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    };

    dnn.controlBar.switchSite = function (site) {
        if (site) {
            var dataVar = { Site: site };
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'SwitchSite',
                type: 'POST',
                data: dataVar,
                beforeSend: service.setModuleHeaders,
                success: function (d) {
                    if (d && d.RedirectURL)
                        window.location.href = d.RedirectURL;
                },
                error: function (xhr) {
                    dnn.controlBar.responseError(xhr);
                }
            });
        }
    };

    dnn.controlBar.switchLanguage = function (language) {
        if (language) {
            var dataVar = { Language: language };
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'SwitchLanguage',
                type: 'POST',
                data: dataVar,
                beforeSend: service.setModuleHeaders,
                success: function () {
                    window.location.href = window.location.href.split('#')[0];
                },
                error: function (xhr) {
                    dnn.controlBar.responseError(xhr);
                }
            });
        }
    };

    dnn.controlBar.popupShareableWarning = function () {
        $('#shareableWarning').dialog({
            autoOpen: true,
            resizable: false,
            modal: true,
            width: '500px',
            zIndex: 1000,
            stack: false,
            title: settings.moduleShareableTitle,
            dialogClass: 'dnnFormPopup dnnClear',
            open: function () {
            },
            close: function () {
            }
        });
    };

    dnn.controlBar.hideShareableWarning = function () {
        $('#shareableWarning').dialog('close');
    };

    dnn.controlBar.renderModuleList = function (containerId, moduleList) {

        var container = $(containerId);
        var scrollContainer = container.next();
        var api = scrollContainer.data('jsp');
        if (api) {
            api.scrollToX(0, null);
            api.destroy();
        }
        scrollContainer = container.next(); // reinit because api destroy...

        $(containerId).css('overflow', 'hidden');
        var ul = $('ul.ControlBar_ModuleList', container);
        ul.empty().css('left', 1000);
        var windowWidth = $(window).width();
        var margin = Math.round((windowWidth - 980) / 2);

        $('#ControlBar_Module_ModulePosition').hide();
        for (var i = 0; i < moduleList.length; i++) {
            ul.append('<li><div class="ControlBar_ModuleDiv" data-module=' + moduleList[i].ModuleID + '><div class="ModuleLocator_Menu"></div><img src="' + moduleList[i].ModuleImage + '" alt="" /><span>' + moduleList[i].ModuleName + '</span></div></li>');
        }
        var ulWidth = moduleList.length * 160;
        ul.css('width', ulWidth + 'px');
        // some math here
        var dummyScrollWidth = Math.round((980 * (ulWidth + margin)) / windowWidth);
        var ulLeft = margin;
        var oldX = 0;
        var modulesInitFunc = function () {
            $('div.controlBar_ModuleListScrollDummy_Content', scrollContainer).css('width', dummyScrollWidth);
            scrollContainer.jScrollPane();
            scrollContainer.bind('jsp-scroll-x', function (e, x, isAtleft, isAtRight) {
                var xOffset, leftOffset;
                if (isAtleft) {
                    oldX = 0;
                    ulLeft = margin;
                } else if (isAtRight) {
                    oldX = Math.round((980 * (ulWidth + margin)) / windowWidth) - 980;
                    ulLeft = -(ulWidth - windowWidth);
                } else {
                    if (x > oldX) {
                        // scroll to right
                        xOffset = x - oldX;
                        leftOffset = (ulWidth / ((980 * (ulWidth + margin)) / windowWidth)) * xOffset;
                        ulLeft -= Math.abs(leftOffset);
                    } else {
                        // scroll to left
                        xOffset = oldX - x;
                        leftOffset = (ulWidth / ((980 * (ulWidth + margin)) / windowWidth)) * xOffset;
                        ulLeft += Math.abs(leftOffset);
                    }
                    oldX = x;
                }
                ul.css('left', ulLeft);
            });

            $('div.ControlBar_ModuleDiv', ul).each(function () {
                if (!this.id)
                    this.id = 'ControlBar_ModuleDiv_' + $(this).data('module');
            }).hover(function () {

                if (!dnn.controlBar.isMouseDown) {
                    var $this = $(this);
                    var dataModuleId = $this.data('module');
                    if (dnn.controlBar.selectedModule && dnn.controlBar.selectedModule.data('module') !== dataModuleId) {
                        dnn.controlBar.selectedModule.removeClass('ControlBar_Module_Selected');
                    }
                    $this.addClass('ControlBar_Module_Selected');
                    $this.find('div.ModuleLocator_Menu').removeClass('ModuleLocator_Hover');
                    dnn.controlBar.selectedModule = $this;
                    dnn.controlBar.showSelectedModule = true;

                    var holderId = this.id;
                    var $self = $(this);
                    $self.dnnHelperTip({
                        helpContent: settings.dragModuleToolTip,
                        holderId: holderId,
                        show: true
                    });

                }

            }, function () {
                if (!dnn.controlBar.isMouseDown) {
                    dnn.controlBar.showSelectedModule = false;

                    setTimeout(function () {
                        if (!dnn.controlBar.showSelectedModule && dnn.controlBar.selectedModule) {
                            dnn.controlBar.selectedModule.removeClass('ControlBar_Module_Selected');
                        }
                    }, 600);
                }

                $(this).dnnHelperTipDestroy();
            })
            .mousedown(function () {
                $('.DNNEmptyPane').each(function () {
                    $(this).removeClass('DNNEmptyPane').addClass('dnnDropEmptyPanes');
                });
                $('.contentPane').each(function () {
                    // this special code is for you -- IE8
                    this.className = this.className;
                });
            })
            .mouseup(function () {
                $('.dnnDropEmptyPanes').each(function () {
                    $(this).removeClass('dnnDropEmptyPanes').addClass('DNNEmptyPane');
                });
                $('.contentPane').each(function () {
                    // this special code is for you -- IE8
                    this.className = this.className;
                });
            })
            .draggable({
                dropOnEmpty: true,
                cursor: 'move',
                placeholder: "dnnDropTarget",
                helper: function (event, ui) {
                    var dragTip = $('<div class="dnnDragdropTip"></div>');
                    var title = $('span', this).html();
                    dragTip.html(title);
                    $('body').append(dragTip);

                    // destroy tooltip
                    $(this).dnnHelperTipDestroy();

                    //set data
                    dnn.controlBar.dragdropModule = $(this).data('module');
                    dnn.controlBar.dragdropPage = $find(settings.pageComboId).get_value();
                    dnn.controlBar.dragdropVisibility = $find(settings.visibilityComboId).get_value();
                    dnn.controlBar.dragdropCopyModule = $('#ControlBar_Module_chkCopyModule').get(0).checked;
                    dnn.controlBar.dragdropAddExistingModule = !dnn.controlBar.addNewModule;
                    return dragTip;
                },
                cursorAt: { left: 10, top: 30 },
                connectToSortable: '.dnnSortable',
                stop: function (event, ui) {

                    $('.dnnDropEmptyPanes').each(function () {
                        $(this).removeClass('dnnDropEmptyPanes').addClass('DNNEmptyPane');
                    });
                    $('.contentPane').each(function () {
                        // this special code is for you -- IE8
                        this.className = this.className;
                    });

                    $('div.actionMenu').show();
                },
                start: function (event, ui) {
                    $('div.actionMenu').hide();
                }
            });

            $('div.ModuleLocator_Menu', ul).hover(function () {
                var $this = $(this);
                $this.addClass('ModuleLocator_Hover');
                var left = $this.offset().left;
                dnn.controlBar.hideModuleLocationMenu = false;
                dnn.controlBar.showSelectedModule = true;
                $('#ControlBar_Module_ModulePosition')
                    .css({ left: left - 177, top: 135 })
                    .slideDown('fast', function () {
                        $(this).jScrollPane();
                    })
                    .hover(function () {
                        dnn.controlBar.hideModuleLocationMenu = false;
                        dnn.controlBar.showSelectedModule = true;

                    }, function () {
                        dnn.controlBar.hideModuleLocationMenu = true;
                        dnn.controlBar.showSelectedModule = false;
                        setTimeout(function () {
                            if (dnn.controlBar.hideModuleLocationMenu) {
                                $('#ControlBar_Module_ModulePosition').slideUp('fast');
                            }

                            if (!dnn.controlBar.showSelectedModule && dnn.controlBar.selectedModule) {
                                dnn.controlBar.selectedModule.removeClass('ControlBar_Module_Selected');
                            }
                        }, 600);
                    });

                // hide tooltip
                $this.parent().dnnHelperTipDestroy();

            }, function () {
                var $this = $(this);
                dnn.controlBar.showSelectedModule = false;
                dnn.controlBar.hideModuleLocationMenu = true;
                setTimeout(function () {
                    if (dnn.controlBar.hideModuleLocationMenu) {
                        $this.removeClass('ModuleLocator_Hover');
                        $('#ControlBar_Module_ModulePosition').slideUp('fast');
                    }

                    if (!dnn.controlBar.showSelectedModule && dnn.controlBar.selectedModule) {
                        dnn.controlBar.selectedModule.removeClass('ControlBar_Module_Selected');
                    }
                }, 600);
            });
        };
        setTimeout(modulesInitFunc, 0);
        ul.animate({ left: margin }, 300);
    };

    dnn.controlBar.ControlBar_Module_CategoryList_Changed = function (sender, e) {
        var item = e.get_item();
        if (item) {
            dnn.controlBar.getDesktopModulesForNewModule(item.get_value());
        }
    };

    dnn.controlBar.ControlBar_Module_SiteList_Changed = function (sender, e) {
        var item = e.get_item();
        if (item) {
            dnn.controlBar.getPageList(item.get_value());
        }
    };

    dnn.controlBar.ControlBar_Module_PageList_Changed = function (sender, e) {
        var item = e.get_item();
        var visibilityCombo = $find(settings.visibilityComboId);
        if (item) {
            var val = item.get_value();
            if (val) {
                dnn.controlBar.getTabModules(item.get_value());
                visibilityCombo.enable();

                if (dnn.controlBar.status && !dnn.controlBar.status.addNewModule) {
                    visibilityCombo.findItemByValue(dnn.controlBar.status.visibility).select();
                }

            } else {
                visibilityCombo.disable();
            }
        }
    };

    //attach mouse move to detect mouse button
    $(document).mousedown(function () {
        dnn.controlBar.isMouseDown = true;
    });
    $(document).mouseup(function () {
        dnn.controlBar.isMouseDown = false;
    });

    var currentUserMode = settings.currentUserMode;

    $('div.subNav').hide();
    $("#ControlNav li").hoverIntent({
        over: function () {
            $('.onActionMenu').removeClass('onActionMenu');
            toggleModulePane($('.ControlModulePanel'), false);
	        $(this).addClass("hover");
            $(this).find('div.subNav').slideDown(300);
        },
        out: function () {
        	if (!dnn.controlBar.focused) {
        		$(this).removeClass("hover");
		        $(this).find('div.subNav').slideUp(200);
	        }
        },
        timeout: 300,
        interval: 150
    });

    $('#ControlActionMenu > li').hoverIntent({
        over: function () {
            $('.onActionMenu').removeClass('onActionMenu');
            toggleModulePane($('.ControlModulePanel'), false);
            $(this).find('ul').slideDown(200);
        },
        out: function () {
        	$(this).find('ul').slideUp(150);
        },
        timeout: 300,
        interval: 150
    });

    $('ul#ControlEditPageMenu > li').hoverIntent({
        over: function () {
            $('.onActionMenu').removeClass('onActionMenu');
            toggleModulePane($('.ControlModulePanel'), false);
            $(this).find('ul').slideDown(400);
        },
        out: function () { $(this).find('ul').slideUp(300); },
        timeout: 300,
        interval: 150
    });

    //Handling the Advanced Subnav Toggling
    $(".subNavToggle li a").click(function (event) {
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

    $('#controlBar_AddNewModule').click(function () {
        if (currentUserMode !== 'EDIT') {
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'ToggleUserMode',
                type: 'POST',
                data: { UserMode: 'EDIT' },
                beforeSend: service.setModuleHeaders,
                success: function () {
                    dnn.dom.setCookie('ControlBarInit', 'AddNewModule');
                    window.location.href = window.location.href.split('#')[0];
                },
                error: function (xhr) {
                    dnn.controlBar.responseError(xhr);
                }
            });

            return false;
        }

        var category = null;
        if (dnn.controlBar.status && dnn.controlBar.status.addNewModule) {
            var selectedCategory = dnn.controlBar.status.category;
            if (selectedCategory) {
                $find(settings.categoryComboId).findItemByValue(selectedCategory).select();
                category = selectedCategory;
                dnn.controlBar.removeStatus();
            }
        } else {
            category = $find(settings.categoryComboId).get_value();
        }

        dnn.controlBar.getDesktopModulesForNewModule(category);
        dnn.controlBar.addNewModule = true;
        toggleModulePane($('#ControlBar_Module_AddNewModule'), true);
        $('#ControlBar_Action_Menu').addClass('onActionMenu');
        $('#ControlBar_ModuleListMessage_NewModule').hide();
        return false;
    });

    $('#controlBar_AddExistingModule').click(function () {
        if (currentUserMode !== 'EDIT') {
            var service = dnn.controlBar.getService();
            var serviceUrl = dnn.controlBar.getServiceUrl(service);
            $.ajax({
                url: serviceUrl + 'ToggleUserMode',
                type: 'POST',
                data: { UserMode: 'EDIT' },
                beforeSend: service.setModuleHeaders,
                success: function () {
                    dnn.dom.setCookie('ControlBarInit', 'AddExistingModule');
                    window.location.href = window.location.href.split('#')[0];
                },
                error: function (xhr) {
                    dnn.controlBar.responseError(xhr);
                }
            });

            return false;
        }

        var portal = null;
        if (dnn.controlBar.status && !dnn.controlBar.status.addNewModule) {
            var selectedPortal = dnn.controlBar.status.site;
            if (selectedPortal) {
                $find(settings.portalComboId).findItemByValue(selectedPortal).select();
                portal = selectedPortal;
            }
        }
        else {
            portal = $find(settings.portalComboId).get_value();
        }

        // check portal combobox has only one item or not
        var portals = $find(settings.portalComboId).get_items();
        var portalsCount = portals.get_count();
        var hidePortalCombo = portalsCount < 3;
        if (hidePortalCombo) {
            // only one portal available
            $('#ControlBar_SiteList > table').hide();
            if (!portal) {
                // portal not selected, select default one
                portal = portals.getItem(1).get_value();
            }
        }

        dnn.controlBar.getPageList(portal);
        dnn.controlBar.addNewModule = false;
        toggleModulePane($('#ControlBar_Module_AddExistingModule'), true);
        $('#ControlBar_Action_Menu').addClass('onActionMenu');

        var messageContainer = $('#ControlBar_ModuleListMessage_ExistingModule');
        var loadingContainer = messageContainer.next();
        var listContainer = loadingContainer.next();
        var scrollContainer = listContainer.next();

        messageContainer.show();
        loadingContainer.hide();
        listContainer.hide();
        scrollContainer.hide();

        $('p.ControlBar_ModuleListMessage_InitialMessage', messageContainer).show();
        $('p.ControlBar_ModuleListMessage_NoResultMessage', messageContainer).hide();

        return false;
    });

    $('#ControlBar_Module_ModulePosition > li').click(function () {
        if (dnn.controlBar.addingModule) return false;
        dnn.controlBar.addingModule = true;

        var module = dnn.controlBar.selectedModule;
        var page = $find(settings.pageComboId).get_value();
        var pane = $(this).data('pane');
        var position = $(this).data('position');
        var visibility = $find(settings.visibilityComboId).get_value();
        var copyModule = $('#ControlBar_Module_chkCopyModule').get(0).checked;
        var addExistingModule = !dnn.controlBar.addNewModule;
        dnn.controlBar.addModule(module.data('module') + '', page, pane, position, '-1', visibility, addExistingModule + '', copyModule + '');
        return false;
    });

    $('#controlBar_ClearCache').click(function () {
        dnn.controlBar.clearHostCache();
        return false;
    });

    $('#controlBar_RecycleAppPool').click(function () {
        dnn.controlBar.recycleAppPool();
        return false;
    });

    $('a#controlBar_CopyPermissionsToChildren').dnnConfirm({
        text: settings.copyPermissionsToChildrenText,
        yesText: settings.yesText,
        noText: settings.noText,
        title: settings.titleText,
        callbackTrue: function () {
            dnn.controlBar.copyPermissionsToChildren();
        }
    });

    $('#controlBar_SwitchSite, #controlBar_SwitchLanguage').focus(function (e) {
	    dnn.controlBar.focused = true;
    }).change(function (e) {
    	dnn.controlBar.focused = false;
    }).blur(function(e) {
    	dnn.controlBar.focused = false;
    }).click(function (e) {
    	dnn.controlBar.focused = true;
    });

    $('#controlBar_SwitchSiteButton').click(function () {
        var site = $('#controlBar_SwitchSite').val();
        dnn.controlBar.switchSite(site);
        return false;
    });

    $('#controlBar_SwitchLanguageButton').click(function () {
        var language = $('#controlBar_SwitchLanguage').val();
        dnn.controlBar.switchLanguage(language);
        return false;
    });

    var toggleModulePane = function (pane, show) {
        if (show) {
            pane.animate({ height: 'show' }, 100, function () {
                $('#Form').addClass("showModulePane");
                $(window).resize();
            });

        } else {
            pane.animate({ height: 'hide' }, 100, function () {
	            $('#Form').removeClass("showModulePane");
                $(window).resize();
            });
        }
    };

    // generate url and popup
    $('a.ControlBar_PopupLink').click(function () {
        var href = $(this).attr('href');
        if (href) {
            dnnModal.show(href + '?popUp=true', true, 550, 950, true, '');
        }
        return false;
    });

    $('a.ControlBar_PopupLink_EditMode').click(function () {
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
                success: function () {
                    // then popup
                    var href = $(that).attr('href');
                    if (href) {
                        dnnModal.show(href + '?popUp=true', true, 550, 950, true, '');
                    }
                },
                error: function (xhr) {
                    dnn.controlBar.responseError(xhr);
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

    $('a#ControlBar_DeletePage').dnnConfirm({
        text: settings.deleteText,
        yesText: settings.yesText,
        noText: settings.noText,
        title: settings.titleText
    });

    $('a#shareableWarning_cmdConfirm').click(function () {
        dnn.controlBar.hideShareableWarning();
        if (dnn.controlBar.addModuleDataVar) {
            dnn.controlBar.doAddModule(dnn.controlBar.addModuleDataVar);
        }
        dnn.controlBar.addModuleDataVar = null;
        dnn.controlBar.addingModule = false;
        return false;
    });

    $('a#shareableWarning_cmdCancel').click(function () {
        dnn.controlBar.hideShareableWarning();
        dnn.controlBar.addModuleDataVar = null;
        dnn.controlBar.addingModule = false;
        return false;
    });

    $('a#ControlBar_EditPage').click(function () {
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        var mode = currentUserMode === 'EDIT' ? 'VIEW' : 'EDIT';
        $.ajax({
            url: serviceUrl + 'ToggleUserMode',
            type: 'POST',
            data: { UserMode: mode },
            beforeSend: service.setModuleHeaders,
            success: function () {
                dnn.dom.setCookie('StayInEditMode', 'NO');
                window.location.href = window.location.href.split('#')[0];
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
        return false;
    });

    $('#ControlBar_StayInEditMode').change(function () {
        var mode = this.checked ? "YES" : "NO";
        if (this.checked) {
            // disable view in layout mode
            $('#ControlBar_ViewInLayout').attr('disabled', 'disabled');
        } else {
            $('#ControlBar_ViewInLayout').removeAttr('disabled');
        }
        dnn.dom.setCookie('StayInEditMode', mode);
    }).change();

    $('#ControlBar_ViewInLayout').change(function () {
        if (this.disabled) return;
        var mode = this.checked ? "LAYOUT" : "VIEW";
        var service = dnn.controlBar.getService();
        var serviceUrl = dnn.controlBar.getServiceUrl(service);
        $.ajax({
            url: serviceUrl + 'ToggleUserMode',
            type: 'POST',
            data: { UserMode: mode },
            beforeSend: service.setModuleHeaders,
            success: function () {
                window.location.href = window.location.href;
            },
            error: function (xhr) {
                dnn.controlBar.responseError(xhr);
            }
        });
    });

    $('a.bookmark').live('click', function () {
        var $this = $(this);
        if ($this.hasClass('hideBookmark')) return false;

        var wrapper = $this.closest('dl');
        var title = wrapper.attr('id').indexOf('host') > 0 ? 'host' : 'admin';
        var outerWrapper = wrapper.parent();
        var bookmarkWrapper = $('dl', outerWrapper).last();
        var ul = $('ul', bookmarkWrapper);
        var bookmarkList = $('ul > li > a', bookmarkWrapper).not('.bookmark');

        // add to bookmark
        var bookmarkUrl = $this.prev();
        var bookmarkTabname = $this.parent().data('tabname');
        var bookmarkHtml = bookmarkUrl.html();
        // check conflict or not
        var isConflict = false;
        bookmarkList.each(function (n, v) {
            var html = $(v).html();
            if (bookmarkHtml === html) {
                isConflict = true;
                return false;
            }
            return true;
        });

        if (!isConflict) {
            // add url to bookmark
            var li = $('<li data-tabname="' + bookmarkTabname + '"></li>');
            li.append(bookmarkUrl.clone());
            li.append($("<a href='javascript:void(0)' class='removeBookmark' title='" + settings.removeBookmarksTip + "'><span></span></a>"));
            ul.append(li);
            // hide this bookmark
            var bookmarkTitle = $this.attr('title');
            $this.addClass('hideBookmark').removeAttr('title').attr('data-title', bookmarkTitle);
            // save bookmark to server
            dnn.controlBar.saveBookmark(title, ul);
            // focus on bookmark tab
            $('li.BookmarkToggle > a', outerWrapper).click();
        }
    });

    $('a.removeBookmark').live('click', function () {
        var $this = $(this);
        var li = $this.parent();
        var tabname = li.attr('data-tabname');
        console.log(tabname);
        var wrapper = $this.closest('dl');
        var title = wrapper.attr('id').indexOf('host') > 0 ? 'host' : 'admin';
        var outerWrapper = wrapper.parent();
        var bookmarkWrapper = $('dl', outerWrapper).last();
        var ul = $('ul', bookmarkWrapper);
        // toggle class for original menu item
        if (title === 'admin') {
            $('#controlbar_admin_basic li, #controlbar_admin_advanced li').each(function () {
                if ($(this).data('tabname') === tabname) {
                    var addbookmarkLink = $(this).find('a.bookmark');
                    var addbookmarktitle = addbookmarkLink.data('title');
                    addbookmarkLink.removeClass('hideBookmark').removeAttr('data-title').attr('title', addbookmarktitle);
                    return false;
                }
                return true;
            });
        }
        else {
            $('#controlbar_host_basic li, #controlbar_host_advanced li').each(function () {
                if ($(this).data('tabname') === tabname) {
                    var addbookmarkLink = $(this).find('a.bookmark');
                    var addbookmarktitle = addbookmarkLink.data('title');
                    addbookmarkLink.removeClass('hideBookmark').removeAttr('data-title').attr('title', addbookmarktitle);
                    return false;
                }
                return true;
            });
        }
        // already bookmarked, remove it
        li.remove();
        // save bookmark to server
        dnn.controlBar.saveBookmark(title, ul);
    });

    $('a.controlBar_CloseAddModules').click(function () {
        var modulePane = dnn.controlBar.addNewModule ? $('#ControlBar_Module_AddNewModule') : $('#ControlBar_Module_AddExistingModule');
        toggleModulePane(modulePane, false);
    });

    // push page down
	$('#Form').addClass("showControlBar");

    // initialize -- this action is between page mode toggle
    var initAction = dnn.dom.getCookie('ControlBarInit');
    if (initAction) {
        dnn.dom.setCookie('ControlBarInit', '', -1);
        switch (initAction) {
            case 'AddNewModule':
                setTimeout(function () {
                    $('#controlBar_AddNewModule').click();
                }, 500);

                break;
            case 'AddExistingModule':
                setTimeout(function () {
                    $('#controlBar_AddExistingModule').click();
                }, 500);
                break;
        }

        return;
    }

    // fade module if needed
    var fadeModule = dnn.dom.getCookie('FadeModuleID');
    if (fadeModule) {
        dnn.dom.setCookie('FadeModuleID', '', -1);
        var anchorLink = $('a[name="' + fadeModule + '"]');
        var module = anchorLink.parent();
        module.css('background-color', '#fffacd');
        setTimeout(function () {
            module.css('background', '#fffff0');
            setTimeout(function () {
                module.css('background', 'transparent');
            }, 300);
        }, 2500);

        // scroll to new added module
        var moduleTop = (module.offset().top - 50);
        if (moduleTop > 0) {
            $('body').scrollTop(moduleTop);
        }

        // load status
        dnn.controlBar.loadStatus();
        if (dnn.controlBar.status) {
            if (dnn.controlBar.status.addNewModule) {
                setTimeout(function () {
                    $('#controlBar_AddNewModule').click();
                }, 500);
            }
            else {
                setTimeout(function () {
                    $('#controlBar_AddExistingModule').click();
                }, 500);
            }
        }
    }
};

$(function () {
    if (dnn.controlBarSettings)
    	dnn.controlBar.init(dnn.controlBarSettings);
	
	//extend dnnControlPanel to show or hide control panel
    if (typeof $.fn.dnnControlPanel == "undefined") {
    	$.fn.dnnControlPanel = {};
	    $.fn.dnnControlPanel.show = function() {
		    $("#ControlBar").slideDown();
	    };
	    $.fn.dnnControlPanel.hide = function () {
	    	$("#ControlBar").slideUp();
	    };
    }
});