(function ($) {
    $.fn.dnnComposeMessage = function (options) {
        var opts = $.extend({}, $.fn.dnnComposeMessage.defaultOptions, options),
            $wrap = this;

        opts.serviceurlbase = $wrap.dnnSF().getServiceRoot('CoreServices') + 'MessagingService.ashx/';

        $wrap.each(function () {
            var self = $(this),
                composeMessageDialog,
                html,
                canSend = false,
                autoclose,
                messageId = -1,
                users = [],
                roles = [],
                attachments = [];

            if (self.data('bound')) return self;
            self.data('bound', true);

            //construct the form
            html = "<div class='MessageSent dnnFormMessage dnnFormSuccess'>" + opts.messageSentText + "</div>";
            html += "<div class='ThrottlingWarning dnnFormMessage dnnFormWarning'>" + opts.throttlingText + "</div>";
            html += "<fieldset>";
            html += "<div class='dnnFormItem'><label for='to'>" + opts.toText + "</label><input type='text' id='to' name='to'/></div>";
            html += "<div class='dnnFormItem'><label for='subject'>" + opts.subjectText + "</label><input type='text' id='subject' name='subject' maxlength='400'/></div>";
            html += "<div class='dnnFormItem'><label for='bodytext'>" + opts.messageText + "</label><textarea rows='2' cols='20' id='bodytext' name='bodytext'/></div>";

            if (opts.showAttachments) {
                html += "<div class='dnnFormItem'><label>" + opts.attachmentsText + "</label><div class='dnnLeft'><button type='button' id='fileFromSite' class='dnnTertiaryAction'><span>" + opts.browseText + "</span></button></div></div>";
                html += "<div id='userFileManager'></div>";
            }

            html += "</fieldset>";

            self.getWaitTimeForNextMessage = function () {
                var returnValue = 0;
                $.ajax({
                    url: opts.serviceurlbase + "WaitTimeForNextMessage",
                    async: false
                }).done(function (data) { returnValue = data; });
                return returnValue;
            };

            self.click(function (e) {
                e.preventDefault();

                composeMessageDialog = $("<div class='composeMessageDialog dnnForm dnnClear'></div>").html(html).dialog(opts);

                if (opts.showAttachments) {
                    opts.userFileManagerOptions.openTriggerSelector = '#fileFromSite';
                    opts.userFileManagerOptions.attachCallback = function (file) {
                        if ($.inArray(file.id, attachments) === -1) {
                            attachments.push(file.id);
                            composeMessageDialog.find('.dnnLeft').append('<div class="dnnFormItem">' + file.name + ' <a href="#"><img src="images/delete.gif" alt="' + opts.removeText + '" title="' + opts.removeText + '" /></a></div>');
                            composeMessageDialog.find('.dnnLeft div:last-child a').click(function () {
                                var index = $.inArray(file.id, attachments);
                                if (index !== -1) {
                                    attachments.splice(index, 1);
                                    $(this).parent().remove();
                                }
                                return false;
                            });
                        }
                    };
                    composeMessageDialog.find('#userFileManager').userFileManager(opts.userFileManagerOptions);
                }

                function updateSendButtonStatus() {
                    var sendButton = $('.ui-dialog-buttonpane button:first');
                    if ((users.length > 0 || roles.length > 0) && composeMessageDialog.find('#subject').val().trim().length > 0 && canSend) {
                        sendButton.removeAttr('disabled').removeClass('disabled');
                    } else {
                        sendButton.attr('disabled', 'disabled').addClass('disabled');
                    }
                }

                composeMessageDialog.find('#to').tokenInput([
                    { id: "user-1", name: "SuperUser Account" },
                    { id: "user-2", name: "Administrator Account" },
                    { id: "role-0", name: "Administrators" },
                    { id: "role-1", name: "Registered Users" },
                    { id: "role-2", name: "Subscribers" },
                    { id: "role-3", name: "Translator (en-US)" },
                    { id: "role-4", name: "Unverified Users" }
                ], {
                    theme: "facebook",
                    preventDuplicates: true,
                    onAdd: function (item) {
                        if (item.id.startsWith("user-")) { users.push(item.id.substring(5)); }
                        else if (item.id.startsWith("role-")) { roles.push(item.id.substring(5)); }
                        updateSendButtonStatus();
                    },
                    onDelete: function (item) {
                        var array = item.id.startsWith("user-") ? users : roles,
                            id = item.id.substring(5),
                            index = $.inArray(id, array);

                        if (index !== -1) { array.splice(index, 1); }
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
                        composeMessageDialog.find('.MessageSent').hide();
                        composeMessageDialog.find('.ThrottlingWarning').hide();
                        messageId = -1;

                        canSend = false;
                        var sendButton = composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane button:first');
                        var timeForNextMessage = self.getWaitTimeForNextMessage();
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
                                    composeMessageDialog.find('.MessageSent').show();
                                    composeMessageDialog.dialog("widget").find('.ui-dialog-buttonpane button:first').attr('disabled', 'disabled').addClass('disabled');
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
                        composeMessageDialog.find('input[type="text"],textarea').val('');
                        composeMessageDialog.dialog("destroy");
                    }
                });
                composeMessageDialog.dialog('open');
            });

            return $wrap;
        });
    };

    $.fn.dnnComposeMessage.defaultOptions = {
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
        messageSentText: 'Your message has been sent successfully.',
        throttlingText: 'Please wait before sending a new message.',
        dialogClass: 'dnnFormPopup dnnClear',
        autoOpen: false,
        showAttachments: false,
        msgSentAutoCloseTimeout: 3000,
        userFileManagerOptions: {}
    };

} (jQuery));