
(function ($) {
    $.fn.dnnModuleActions = function (options) {
        var opts = $.extend({}, $.fn.dnnModuleActions.defaultOptions, options);
        var $self = this;
        var actionButton = opts.actionButton;
        var moduleId = opts.moduleId;
        var tabId = opts.tabId;
        var adminActions = opts.adminActions;
        var adminCount = adminActions.length;
        var customActions = opts.customActions;
        var customCount = customActions.length;
        var panes = opts.panes;
        var supportsMove = opts.supportsMove;
        var count = adminCount + customCount;

        $(window).resize(function () {
            resetMenu(moduleId);
        });

        if (count > 0 || supportsMove) {
            var $form = $("body form");
            if ($form.find("div#moduleActions-" + moduleId).length === 0) {
                $form.append("<div id=\"moduleActions-" + moduleId + "\" class=\"actionMenu\"><ul class=\"dnn_mact\"></ul></div>");
                var menu = $form.find("div:last");
                var menuRoot = menu.find("ul");
                if (customCount > 0) {
                    buildMenu(menuRoot, "Edit", "actionMenuEdit", customActions, customCount);
                }
                if (adminCount > 0) {
                    buildMenu(menuRoot, "Admin", "actionMenuAdmin", adminActions, adminCount);
                }
                if (supportsMove) {
                    buildMoveMenu(menuRoot, "Move", "actionMenuMove");
                }

                position(moduleId);
            }
        }

        function buildMoveMenu(root, rootText, rootClass) {
            var parent = buildMenuRoot(root, rootText, rootClass);
            var modulePane = $(".DnnModule-" + moduleId).parent();
            var paneName = modulePane.attr("id").replace("dnn_", "");

            var htmlString;
            var moduleIndex = -1;
            var id = paneName + moduleId;
            var modules = modulePane.children();
            var moduleCount = modules.length;
            var i;

            for (i = 0; i < moduleCount; i++) {
                var module = modules[i];
                var mid = getModuleId(module);

                if (moduleId == parseInt(mid)) {
                    moduleIndex = i;
                    break;
                }
            }

            //Add Top/Up actions
            if (moduleIndex > 0) {
                //htmlString = "<li id=\"" + id + "-top\"><a href=\"#\"><img src=\"" + rootFolder + "images/action_top.gif\"><span>" + opts.topText + "</span></a>";
                htmlString = "<li id=\"" + id + "-top\">" + opts.topText;
                parent.append(htmlString);

                //Add click event handler to just added element
                parent.find("li#" + id + "-top").click(function () {
                    moveTop(paneName);
                });

                //htmlString = "<li id=\"" + id + "-up\"><a href=\"#\"><img src=\"" + rootFolder + "images/action_up.gif\"><span>" + opts.upText + "</span></a>";
                htmlString = "<li id=\"" + id + "-up\">" + opts.upText;
                parent.append(htmlString);

                //Add click event handler to just added element
                parent.find("li#" + id + "-up").click(function () {
                    moveUp(paneName, moduleIndex);
                });
            }

            //Add Bottom/Down actions
            if (moduleIndex < moduleCount - 1) {
                //htmlString = "<li id=\"" + id + "-down\"><a href=\"#\"><img src=\"" + rootFolder + "images/action_down.gif\"><span>" + opts.downText + "</span></a>";
                htmlString = "<li id=\"" + id + "-down\">" + opts.downText;
                parent.append(htmlString);

                //Add click event handler to just added element
                parent.find("li#" + id + "-down").click(function () {
                    moveDown(paneName, moduleIndex);
                });

                //htmlString = "<li id=\"" + id + "-bottom\"><a href=\"#\"><img src=\"" + rootFolder + "images/action_bottom.gif\"><span>" + opts.bottomText + "</span></a>";
                htmlString = "<li id=\"" + id + "-bottom\">" + opts.bottomText;
                parent.append(htmlString);

                //Add click event handler to just added element
                parent.find("li#" + id + "-bottom").click(function () {
                    moveBottom(paneName);
                });
            }

            //Add move to pane entries
            for (i = 0; i < panes.length; i++) {
                var pane = panes[i];
                if (paneName !== pane) {
                    id = pane + moduleId;
                    //htmlString = "<li id=\"" + id + "\"><a href=\"#\"><img src=\"" + rootFolder + "images/action_move.gif\"><span>" + opts.movePaneText.replace("{0}", pane) + "</span></a>";
                    htmlString = "<li id=\"" + id + "\">" + opts.movePaneText.replace("{0}", pane);
                    parent.append(htmlString);

                    //Add click event handler to just added element
                    parent.find("li#" + id).click(function () {
                        moveToPane($(this).attr("id").replace(moduleId, ""));
                    });
                }
            }
        }

        function buildMenu(root, rootText, rootClass, actions, actionCount) {
            var $parent = buildMenuRoot(root, rootText, rootClass);

            for (var i = 0; i < actionCount; i++) {
                var action = actions[i];

                if (action.Title !== "~") {
                    if (!action.Url) {
                        action.Url = "javascript: __doPostBack('" + actionButton + "', '" + action.ID + "')";
                    }

                    var htmlString = "<li>";
                    if (action.CommandName === "DeleteModule.Action") {
                        htmlString = "<li id=\"moduleActions-" + moduleId + "-Delete\">";
                    }
                    if (isEnabled(action)) {
                        htmlString += "<a href=\"" + action.Url + "\"><img src=\"" + action.Icon + "\"><span>" + action.Title + "</span></a>";
                    } else {
                        htmlString += "<img src=\"" + action.Icon + "\"><span>" + action.Title + "</span>";
                    }

                    $parent.find("#moduleActions-" + moduleId + "-Delete a").dnnConfirm({
                        text: opts.deleteText,
                        yesText: opts.yesText,
                        noText: opts.noText,
                        title: opts.confirmTitle
                    });

                    $parent.append(htmlString);
                }
            }
        }

        function buildMenuRoot(root, rootText, rootClass) {
            root.append("<li class=\"" + rootClass + "\"><a href='javascript:void(0)' /><ul></ul>");
            var parent = root.find("li." + rootClass + " > ul");

            return parent;
        }

        function getModuleId(module) {
            var $anchor = $(module).children("a");
            if ($anchor.length === 0) {
                $anchor = $(module).children("div.dnnDraggableContent").children("a");
            }
            return $anchor.attr("name");
        }

        function isEnabled(action) {
            return action.ClientScript || action.Url || action.CommandArgument;
        }

        function moveBottom(targetPane) {
            moveToPane(targetPane);
        }

        function moveDown(targetPane, moduleIndex) {
            var container = $(".DnnModule-" + moduleId);

            //move module to target pane
            container.fadeOut("slow", function () {
                $(this).detach()
                    .insertAfter($("#dnn_" + targetPane).children()[moduleIndex])
                    .fadeIn("slow", function () {

                        //update server
                        completeMove(targetPane, ((moduleIndex * 2) + 4));
                    });
            });
        }

        function moveTop(targetPane) {
            var container = $(".DnnModule-" + moduleId);

            //move module to target pane
            container.fadeOut("slow", function () {
                $(this).detach()
                    .prependTo($("#dnn_" + targetPane))
                    .fadeIn("slow", function () {

                        //update server
                        completeMove(targetPane, 0);
                    });
            });
        }

        function moveToPane(targetPane) {
            var container = $(".DnnModule-" + moduleId);

            //move module to target pane
            container.fadeOut("slow", function () {
                $(this).detach()
                    .appendTo("#dnn_" + targetPane)
                    .fadeIn("slow", function () {

                        //update server
                        completeMove(targetPane, -1);
                    });
            });
        }

        function moveUp(targetPane, moduleIndex) {
            var container = $(".DnnModule-" + moduleId);

            //move module to target pane
            container.fadeOut("slow", function () {
                $(this).detach()
                    .insertBefore($("#dnn_" + targetPane).children()[moduleIndex - 1])
                    .fadeIn("slow", function () {
                        //update server
                        completeMove(targetPane, (moduleIndex * 2) - 2);
                    });
            });
        }

        function position(mId) {
            var container = $(".DnnModule-" + mId);
            var root = $("#moduleActions-" + mId + " > ul");
            var containerPosition = container.offset();
            var containerWidth = container.width();

            root.css({
                position: "absolute",
                marginLeft: 0,
                marginTop: 0,
                top: containerPosition.top,
                left: containerPosition.left + containerWidth - 65
            });
        };

        function resetMenu(mId) {
            var root = $("#moduleActions-" + mId + " > ul");
            root.find("li.actionMenuMove").remove();
            if (supportsMove) {
                buildMoveMenu(root, opts.moveText, "actionMenuMove");
            }

            position(mId);

            // rebind hoverIntent 
            $('#moduleActions-' + mId + ' li.actionMenuMove > ul').jScrollPane();
            $('#moduleActions-' + mId + ' li.actionMenuMove').hoverIntent({
                over: function () {
                    // detect position
                    var windowHeight = $(window).height();
                    var windowScroll = $(window).scrollTop();
                    var thisTop = $(this).offset().top;
                    var atViewPortTop = (thisTop - windowScroll) < windowHeight / 2;
                    var ul = $(this).find('ul');
                    var ulHeight = ul.height();
                    if (!atViewPortTop) {
                        ul.css('position', 'absolute').css({ top: -ulHeight, right: 0 }).show('slide', { direction: 'down' }, 80, function () {
                            $(this).jScrollPane();
                        });
                    }
                    else {
                        ul.css('position', 'absolute').css({ top: 20, right: 0 }).show('slide', { direction: 'up' }, 80, function () {
                            $(this).jScrollPane();
                        });
                    }
                },
                out: function () {
                    var ul = $(this).find('ul');

                    if (ul && ul.position()) {
                        if (ul.position().top > 0) {
                            ul.hide('slide', { direction: 'up' }, 80);
                        } else {
                            ul.hide('slide', { direction: 'down' }, 80);
                        }
                    }
                },
                timeout: 400,
                interval: 200
            });
        }

        function completeMove(targetPane, moduleOrder) {
            //remove empty pane class
            $("#dnn_" + targetPane).removeClass("DNNEmptyPane");

            var dataVar = {
                TabId: tabId,
                ModuleId: moduleId,
                Pane: targetPane,
                ModuleOrder: moduleOrder
            };

            var service = $.dnnSF();
            var serviceUrl = $.dnnSF().getServiceRoot("InternalServices") + "ModuleService/";
            $.ajax({
                url: serviceUrl + 'MoveModule',
                type: 'POST',
                data: dataVar,
                beforeSend: service.setModuleHeaders,
                success: function () {
                },
                error: function () {
                }
            });

            //fire window resize to reposition action menus
            $(window).resize();
        }

        return $self;
    };

    $.fn.dnnModuleActions.defaultOptions = {
        customText: "CustomText",
        adminText: "AdminText",
        moveText: "MoveText",
        topText: "Top",
        upText: "Up",
        downText: "Down",
        bottomText: "Bottom",
        movePaneText: "To {0}"
    };

})(jQuery);