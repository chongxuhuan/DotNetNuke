/*globals jQuery, knockout */
(function ($, ko) {
    "use strict";
    $.fn.userFileManager = function (options) {
        var opts = $.extend({}, $.fn.userFileManager.defaultOptions, options),
            $wrap = $(this),
            templateUrl = opts.templatePath + opts.templateName + opts.templateExtension;

        function fileManagerModel(items) {
            var self = this,
                data = items;

            self.items = ko.observableArray(data);
            self.chosenFolderId = ko.observable();
            self.chosenFolderData = ko.observable();
            self.chosenFileData = ko.observable();
            self.nameHeaderText = ko.observable();
            self.typeHeaderText = ko.observable();
            self.lastModifiedHeaderText = ko.observable();
            self.fileSizeText = ko.observable();

            // generic method for navigating the folder hierarchy
            self.find = function (array, id) {
                if (array) {
                    for (var i = 0; i < array.length; i++) {
                        if (array[i].id === id) {
                            return array[i];
                        }
                        var a = self.find(array[i].children, id);
                        if (a !== null) {
                            return a;
                        }
                    }
                }
                return null;
            };

            // show folder event handler
            self.goToFolder = function (folder) {
                self.chosenFileData(null); // internal usage
                $wrap.data('chosenFileData', null); // external usage
                self.chosenFolderData(self.find(data, folder.id));
                self.chosenFolderId(folder.id);
            };

            // refresh the data
            self.updateItems = function (newItems) {
                data = newItems;
                self.items = ko.observable(newItems);
                self.goToFolder(newItems[0]);
            };

            // show file preview event handler
            self.showFilePreview = function (file) {
                self.chosenFileData(file);
                $wrap.data('chosenFileData', file);
            };

            // an alternative to using the "attach" button
            self.attachFile = function (file) {
                opts.attachCallback(file);
                $wrap.dialog('close');
            };

            // custom binding handler used to run some arbitrary JS when the model changes
            ko.bindingHandlers.updateRowNumbers = {
                update: function () {
                    $('#' + $wrap.attr("id") + ' tbody tr').each(function (i) {
                        $(this).find('td.fm-number').text(i + 1);
                    });
                }
            };

            // computed property which creates the breadcrumb data
            self.currentBreadcrumbs = ko.computed(function () {
                var result = [], current = self.chosenFolderData();
                while (current) {
                    result.unshift(current);
                    current = current.parentId ? self.find(data, current.parentId) : null;
                }
                return result;
            }, self);

            self.nameHeaderText = opts.nameHeaderText;
            self.typeHeaderText = opts.typeHeaderText;
            self.lastModifiedHeaderText = opts.lastModifiedHeaderText;
            self.fileSizeText = opts.fileSizeText;

            // go to the root folder by default
            self.goToFolder(data[0]);
        }

        $(opts.openTriggerSelector).click(function (e) {
            e.preventDefault();

            if (!$wrap.data('bound')) {
                // fetch template, populate placeholder (i.e. $wrap)
                $.get(templateUrl, function (data) {
                    $wrap.html(data);
                });

                // initial load and binding of the data
                $.get(opts.getItemsServiceUrl, function (result) {
                    // apply bindings, scope to this instance of the plugin
                    ko.applyBindings(new fileManagerModel(result), document.getElementById($wrap.attr('id')));
                    $wrap.data('bound', true);
                });
            }
            else {
                // refresh the data
                $.get(opts.getItemsServiceUrl, function (result) {
                    var boundDomElement = $wrap.get(0); // expose the underlying DOM element.
                    var context = ko.contextFor(boundDomElement); // get our KO context
                    context.$root.updateItems(result); // call the updateItems method
                });
            }

            $wrap.dialog({
                dialogClass: opts.dialogClass,
                width: opts.width,
                minHeight: opts.minHeight,
                title: opts.title,
                modal: true,
                resizable: true,
                open: function (event, ui) {

                    // wrapper div
                    var $div = $('<div />', { 'class': 'fm-actions' });

                    // list to hold our actions
                    var $actionList = $('<ul />', { 'class': 'dnnActions' });

                    // add a cancel link
                    $('<a />', { 'class': 'dnnSecondaryAction', text: opts.cancelText, href: '#' })
                    .appendTo($actionList)
                    .click(function (ev) {
                        ev.preventDefault();
                        $(event.target).dialog('close');
                    });

                    // add the attach link
                    $('<a />', { 'class': 'dnnPrimaryAction', text: opts.attachText, href: '#' })
                    .appendTo($actionList)
                    .click(function (ev) {
                        ev.preventDefault();
                        var file = $wrap.data('chosenFileData');
                        if (file) {
                            opts.attachCallback(file);
                            $(event.target).dialog("close");
                        }
                    });
                    $actionList.appendTo($div);
                    $div.appendTo('.ui-dialog-buttonpane');
                    $('.ui-dialog-buttonset').hide();

                },
                buttons: [{}],
                close: function () {
                }
            });
        });
    };

    $.fn.userFileManager.defaultOptions = {
        openTriggerSelector: '#photoFromSite', // defined in template, opens dialog
        dialogClass: 'dnnFormPopup fileManagerPopup',
        width: '700px', // dialog width
        minHeight: '400px', // dialog height
        getItemsServiceUrl: '/DesktopModules/Journal/API/UserFile.ashx/GetItems',
        attachCallback: function (file) {
            // available properties .id, .modified, .name, .parentId, .size, .thumb_url, .type
            alert(file.name + ' attached.');
        },
        templatePath: '/Resources/Shared/Components/UserFileManager/Templates/',
        templateName: 'Default',
        templateExtension: '.html',
        /* localized text values: */
        title: 'My Files', // dialog title
        cancelText: 'Cancel', // dialog cancel button
        attachText: 'Attach', // dialog attach button
        nameHeaderText: 'Name',
        typeHeaderText: 'Type',
        lastModifiedHeaderText: 'Last Modified',
        fileSizeText: 'File size: '
    };
} (jQuery, ko));