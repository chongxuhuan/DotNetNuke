(function ($) {
    $.fn.dnnComposeMessage = function (options) {
        var opts = $.extend({}, $.fn.dnnComposeMessage.defaultOptions, options),
            $wrap = $(opts.openTriggerScope),
            html,
            composeMessageDialog,
            canSend = false,
            users = [],
            roles = [],
            attachments = [];

        opts.serviceurlbase = $wrap.dnnSF().getServiceRoot('CoreServices') + 'MessagingService.ashx/';

        //construct the form
        html = "<div class='ThrottlingWarning dnnFormMessage dnnFormWarning'>" + opts.throttlingText + "</div>";
        html += "<fieldset>";
        html += "<div class='dnnFormItem'><label for='to'>" + opts.toText + "</label><input type='text' id='to' name='to'/></div>";
        html += "<div class='dnnFormItem'><label for='subject'>" + opts.subjectText + "</label><input type='text' id='subject' name='subject' maxlength='400'/></div>";
        html += "<div class='dnnFormItem'><label for='bodytext'>" + opts.messageText + "</label><textarea rows='2' cols='20' id='bodytext' name='bodytext'/></div>";

        if (opts.showAttachments) {
            html += "<div class='dnnFormItem'><label>" + opts.attachmentsText + "</label><div class='dnnLeft'><button type='button' id='fileFromSite' class='dnnTertiaryAction'><span>" + opts.browseText + "</span></button><div class='messageAttachments'><ul></ul></div></div></div>";
            html += "<div id='userFileManager'></div>";

            opts.userFileManagerOptions.openTriggerSelector = '#fileFromSite';
            opts.userFileManagerOptions.attachCallback = function (file) {
                if ($.inArray(file.id, attachments) === -1) {
                    attachments.push(file.id);
                    composeMessageDialog.find('.messageAttachments ul').append('<li><a href="#" title="' + file.name + '">' + file.name + '</a><a href="#" class="removeAttachment" title="' + opts.removeText + '"></a></li>');
                    composeMessageDialog.find('.messageAttachments li:last-child .removeAttachment').click(function () {
                        var index = $.inArray(file.id, attachments);
                        if (index !== -1) {
                            attachments.splice(index, 1);
                            $(this).parent().remove();
                        }
                        return false;
                    });
                }
            };
        }

        html += "</fieldset>";

        function getWaitTimeForNextMessage() {
            var returnValue = 0;
            $.ajax({
                url: opts.serviceurlbase + "WaitTimeForNextMessage",
                async: false
            }).done(function (data) { returnValue = data; });
            return returnValue;
        };

        function updateSendButtonStatus() {
            var sendButton = composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane button:first');
            if ((users.length > 0 || roles.length > 0) && composeMessageDialog.find('#subject').val().trim().length > 0 && canSend) {
                sendButton.removeAttr('disabled').removeClass('disabled');
            } else {
                sendButton.attr('disabled', 'disabled').addClass('disabled');
            }
        }

        $wrap.delegate(opts.openTriggerSelector, 'click', function (e) {
            e.preventDefault();

            var autoclose,
                messageId = -1;

            // Reset variable values
            canSend = false;
            users = [];
            roles = [];
            attachments = [];

            composeMessageDialog = $("<div class='composeMessageDialog dnnForm dnnClear'/>").html(html).dialog(opts);

            if (!$wrap.data('fileManagerInitialized')) {
                // we only need to initialize this plugin once, doing so more than once will lead to multiple dialogs.
                // this is because the #userFileManager element is never destroyed when the compose message dialog is closed.
                composeMessageDialog.find('#userFileManager').userFileManager(opts.userFileManagerOptions);
                $wrap.data('fileManagerInitialized', true);
            }

            composeMessageDialog.find('#to').tokenInput(opts.serviceurlbase + "Search", {
                // We can set the tokenLimit here
                theme: "facebook",
                resultsFormatter: function (item) {
                    if (item.id.startsWith("user-")) {
                        return "<li class='user'><img src='profilepic.ashx?UserId=" + item.id.substring(5) + "&w=32&h=32' title='" + item.name + "' height='25px' width='25px' /><span>" + item.name + "</span></li>";
                    } else if (item.id.startsWith("role-")) {
                        return "<li class='role'><img src='" + item.iconfile + "' title='" + item.name + "' height='25px' width='25px' /><span>" + item.name + "</span></li>";
                    }
                    return "<li>" + item[this.propertyToSearch] + "</li>"; // Default formatter
                },
                minChars: 2,
                preventDuplicates: true,
                hintText: '',
                noResultsText: opts.noResultsText,
                searchingText: opts.searchingText,
                onAdd: function (item) {
                    if (item.id.startsWith("user-")) {
                        users.push(item.id.substring(5));
                    } else if (item.id.startsWith("role-")) {
                        roles.push(item.id.substring(5));
                    }
                    updateSendButtonStatus();
                },
                onDelete: function (item) {
                    var array = item.id.startsWith("user-") ? users : roles,
                    id = item.id.substring(5),
                    index = $.inArray(id, array);

                    if (index !== -1) {
                        array.splice(index, 1);
                    }
                    updateSendButtonStatus();
                }
            });

            composeMessageDialog.find('#subject').keyup(function () {
                updateSendButtonStatus();
            });

            composeMessageDialog.dialog({
                minWidth: 650,
                modal: true,
                resizable: false,
                open: function () {
                    composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane :button').removeClass().addClass('dnnTertiaryAction');
                    composeMessageDialog.find('.ThrottlingWarning').hide();
                    messageId = -1;

                    canSend = false;
                    var sendButton = composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane button:first');
                    var timeForNextMessage = getWaitTimeForNextMessage();
                    if (timeForNextMessage > 0) {
                        composeMessageDialog.find('.ThrottlingWarning').show().text(opts.throttlingText + ' ' + timeForNextMessage + ' sec');
                        sendButton.attr('disabled', 'disabled').addClass('disabled');
                        var countdown = setInterval(function () {
                            timeForNextMessage--;
                            if (timeForNextMessage == 0) {
                                canSend = true;
                                composeMessageDialog.find('.ThrottlingWarning').hide();
                                if (composeMessageDialog.find('#to').val().trim().length > 0 && composeMessageDialog.find('#subject').val().trim().length > 0) {
                                    sendButton.removeAttr('disabled').removeClass('disabled');
                                }
                                clearInterval(countdown);
                            } else {
                                composeMessageDialog.find('.ThrottlingWarning').text(opts.throttlingText + ' ' + timeForNextMessage + ' sec');
                            }
                        }, 1000);
                    } else {
                        canSend = true;
                        sendButton.attr('disabled', 'disabled').addClass('disabled');
                    }
                },
                buttons: [
                    {
                        text: opts.sendText,
                        click: function () {
                            var params = {
                                subject: encodeURIComponent(composeMessageDialog.find('#subject').val()),
                                body: encodeURIComponent(composeMessageDialog.find('#bodytext').val()),
                                roleIds: (roles.length > 0 ? JSON.stringify(roles) : {}),
                                userIds: (users.length > 0 ? JSON.stringify(users) : {}),
                                fileIds: (attachments.length > 0 ? JSON.stringify(attachments) : {})
                            };
                            $.post(opts.serviceurlbase + "Create", params, function (data) {
                                composeMessageDialog.dialog("option", "title", opts.messageSentTitle);
                                var dismissThis = $('<a href="#"/>')
                                    .text(' ' + opts.dismissThisText)
                                    .click(function () {
                                        composeMessageDialog.dialog("close");
                                    });
                                var messageSent = $('<div/>')
                                    .addClass('MessageSent dnnFormMessage dnnFormSuccess')
                                    .text(opts.messageSentText)
                                    .append(dismissThis);
                                composeMessageDialog.html(messageSent);
                                composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane button').remove();

                                messageId = data;
                                autoclose = setInterval(function () {
                                    composeMessageDialog.dialog("close");
                                }, opts.msgSentAutoCloseTimeout);
                            }).fail(function (xhr, status, error) {
                                alert(error);
                            });
                        }
                    },
                    {
                        text: opts.cancelText,
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ],
                close: function () {
                    if (autoclose != null) {
                        clearInterval(autoclose);
                    }
                    if (messageId != -1 && opts.onMessageSent != null) {
                        opts.onMessageSent(messageId);
                    }
                }
            });

            composeMessageDialog.dialog('open');
        });
    };

    $.fn.dnnComposeMessage.defaultOptions = {
        openTriggerScope: 'body', // defines parent scope for openTriggerSelector, allows for event delegation
        openTriggerSelector: '.ComposeMessage', // opens dialog
        onMessageSent: function (messageId) {
            // messageId is the identifier of the newly created message
        },
        title: 'Compose Message',
        toText: 'Send to',
        subjectText: 'Subject',
        messageText: 'Your Message',
        attachmentsText: 'Attachment(s)',
        browseText: 'Browse',
        removeText: 'Remove attachment',
        sendText: 'Send',
        cancelText: 'Cancel',
        messageSentTitle: 'Message Sent',
        messageSentText: 'Your message has been sent successfully.',
        dismissThisText: 'Dismiss this',
        throttlingText: 'Please wait before sending a new message.',
        noResultsText: 'No results',
        searchingText: 'Searching...',
        dialogClass: 'dnnFormPopup dnnClear',
        autoOpen: false,
        showAttachments: false,
        msgSentAutoCloseTimeout: 3000,
        userFileManagerOptions: {}
    };

} (jQuery));