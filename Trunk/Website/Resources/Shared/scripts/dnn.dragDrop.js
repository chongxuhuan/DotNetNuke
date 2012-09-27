(function ($) {
    $.fn.dnnModuleDragDrop = function (options) {
        //Default settings
        var settings = {
            actionMenu: "div.actionMenu",
            cursor: "move",
            draggingHintText: "DraggingHintText",
            dragHintText: "DragHintText",
            dropOnEmpty: true,
            dropHintText: "DropHintText",
            dropTargetText: "DropModuleText"
        };

        settings = $.extend(settings, options || {});
        var $self = this;
        var paneModuleIndex;
        var modulePaneName;
        var tabId = settings.tabId;
        var mid;

        var $modules = $('.DnnModule');

        var $module;

        for (var moduleNo = 0; moduleNo < $modules.length; moduleNo++) {
            $module = $($modules[moduleNo]);
            mid = getModuleId($module);

            //Add Draggable style to modules
            $module.addClass("dnnDraggable");

            //Add a drag handle
            if ($module.find(".dnnDragHint").length === 0) {
                $module.children().wrapAll("<div class = \"dnnDraggableContent\" />");
                $module.prepend("<div class=\"dnnDragHint\">&nbsp;</div>");
            }
            
            //Add a drag hint
            $module.find(".dnnDragHint").dnnHelperTip({
                helpContent: settings.dragHintText,
                holderId: "ModuleDragToolTip-" + mid
            });
        }

        //call jQuery UI Sortable plugin
        $self.sortable({
            connectWith: ".dnnSortable",
            dropOnEmpty: settings.dropOnEmpty,
            cursor: settings.cursor,
            cursorAt: { left: 10, top: 30 },
            handle: "div.dnnDragHint",
            placeholder: "dnnDropTarget",
            tolerance: "pointer",
            helper: function (event, ui) {
                
                var dragTip = $('<div class="dnnDragdropTip ControlBar_DragdropModule"></div>');
                var title = $('span.Head', ui).html();
                if (!title)
                    title = "The Dragging Module";

                dragTip.html(title);
                $('body').append(dragTip);
                return dragTip;
            },

            start: function (event, ui) {
                var $pane = ui.item.parent();
                modulePaneName = $pane.attr("id").substring(4);
                mid = getModuleId(ui.item);
                paneModuleIndex = getModuleIndex(mid, $pane);

                //remove EmptyPane class - and replace by a holding class so it can be reapplied
                $(".DNNEmptyPane").removeClass("DNNEmptyPane").addClass("dnnDropEmptyPanes");
                $(settings.actionMenu).hide();

                //Add drop target text
                var $dropTarget = $(".dnnDropTarget");
                $dropTarget.append("<span>" + settings.dropTargetText + "</span>");
                ui.item.addClass("dnnDragging");

                $("div[data-tipholder=\"" + "ModuleDragToolTip-" + mid + "\"] .dnnHelpText").text(settings.draggingHintText);
            },

            over: function (event, ui) {
                mid = getModuleId(ui.item);
                $("div[data-tipholder=\"" + "ModuleDragToolTip-" + mid + "\"] .dnnHelpText").text(settings.dropHintText);
            },

            out: function (event, ui) {
                mid = getModuleId(ui.item);
                $("div[data-tipholder=\"" + "ModuleDragToolTip-" + mid + "\"] .dnnHelpText").text(settings.draggingHintText);
            },

            stop: function (event, ui) {
                var dropItem = ui.item;

                if (dnn.controlBar && dropItem.hasClass('ControlBar_ModuleDiv')) {
                    // add module

                    var pane = ui.item.parent();
                    var order = -1;
                    var paneName = pane.attr("id").substring(4);
                    var modules = $('div.DnnModule, div.ControlBar_ModuleDiv', pane);
                    for (var i = 0; i < modules.length; i++) {
                        var module = modules.get(i);
                        if ($(module).hasClass('ControlBar_ModuleDiv')) {
                            order = i;
                        }
                    }
                    dropItem.remove();
                    dnn.controlBar.addModule(dnn.controlBar.dragdropModule + '',
                        dnn.controlBar.dragdropPage,
                        paneName,
                        '-1',
                        order + '',
                        dnn.controlBar.dragdropVisibility + '',
                        dnn.controlBar.dragdropAddExistingModule + '',
                        dnn.controlBar.dragdropCopyModule + '');
                } else {
                    // move module
                    mid = getModuleId(ui.item);
                    updateServer(mid, ui.item.parent());
                    $(settings.actionMenu).show();
                    ui.item.removeClass("dnnDragging");

                    //remove the empty pane holder class for current pane
                    ui.item.parent().removeClass("dnnDropEmptyPanes");
                    $(".dnnDropEmptyPanes").addClass("DNNEmptyPane").removeClass("dnnDropEmptyPanes");

                    $("div[data-tipholder=\"" + "ModuleDragToolTip-" + mid + "\"] .dnnHelpText").text(settings.dragHintText);

                    //fire window resize to reposition action menus
                    $(window).resize();
                }
            }
        }); //.disableSelection();

        function getModuleId($mod) {
            return $mod.find("a").first().attr("name");
        }

        function getModuleIndex(moduleId, $pane) {
            var index = -1;
            var modules = $pane.children(".DnnModule");
            for (var i = 0; i < modules.length; i++) {
                var module = modules[i];
                mid = getModuleId($(module));

                if (moduleId == parseInt(mid)) {
                    index = i;
                    break;
                }
            }
            return index;
        }

        function updateServer(moduleId, $pane) {
            var order;
            var paneName = $pane.attr("id").substring(4);
            var index = getModuleIndex(moduleId, $pane);

            if (paneName !== modulePaneName) {
                //Moved to new Pane
                order = index * 2;
            } else {
                //Module moved within Pane
                if (index > paneModuleIndex) {
                    //Module moved down
                    order = (index + 1) * 2;
                } else {
                    //Module moved up
                    order = index * 2;
                }
            }

            var dataVar = {
                TabId: tabId,
                ModuleId: moduleId,
                Pane: paneName,
                ModuleOrder: order
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
        }

        return $self;
    };
})(jQuery);