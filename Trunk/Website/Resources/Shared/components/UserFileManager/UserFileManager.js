/*globals jQuery, window */
(function ($, ko) {
    "use strict";
    $.fn.userFileManager = function (options) {
        var opts = $.extend({}, $.fn.userFileManager.defaultOptions, options),
            $wrap = $(this),
            templateUrl = '/Resources/Shared/Components/UserFileManager/Templates/' + opts.templateName + '.html';

        function fileManagerModel(items) {
            var self = this;

            self.items = ko.observableArray(items);
            self.chosenFolderId = ko.observable();
            self.chosenFolderData = ko.observable();
            self.chosenFileData = ko.observable();
            self.nameHeaderText = ko.observable();
            self.typeHeaderText = ko.observable();
            self.lastModifiedHeaderText = ko.observable();

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
                self.chosenFolderData(self.find(items, folder.id));
                self.chosenFolderId(folder.id);
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
                    current = current.parentId ? self.find(items, current.parentId) : null;
                }
                return result;
            }, self);

            self.nameHeaderText = opts.nameHeaderText;
            self.typeHeaderText = opts.typeHeaderText;
            self.lastModifiedHeaderText = opts.lastModifiedHeaderText;

            // go to the root folder by default
            self.goToFolder(items[0]);
        }

        $(opts.openTriggerSelector).click(function (e) {
            e.preventDefault();

            if (!$wrap.data('bound')) {
                $.get(templateUrl, function (data) {
                    $wrap.html(data);
                });

                // initial load and binding of the data
                $.get(opts.getItemsServiceUrl, function (result) {
                    ko.applyBindings(new fileManagerModel(result), document.getElementById($wrap.attr('id')));
                    $wrap.data('bound', true);
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
        dialogClass: 'dnnFormPopup fileManagerPopup',
        getItemsServiceUrl: '/DesktopModules/Journal/API/UserFile.ashx/GetItems',
        openTriggerSelector: '#photoFromSite',
        title: 'My Files',
        cancelText: 'Cancel',
        attachText: 'Attach',
        width: '700px',
        minHeight: '400px',
        attachCallback: function (file) { alert(file.name + ' attached.'); },
        templateName: 'Default',
        nameHeaderText: 'Name',
        typeHeaderText: 'Type',
        lastModifiedHeaderText: 'Last Modified'
    };
} (jQuery, ko));