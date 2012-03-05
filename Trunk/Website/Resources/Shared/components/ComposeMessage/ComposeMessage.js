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
                messageId = -1;

            if (self.data('bound')) return self;
            self.data('bound', true);

            //construct the form
            html = "<div class='dnnFormMessage dnnFormWarning'>ONLY FOR TESTING PURPOSES: Enter an string of integers (user identifiers) separated by commas in the To input box. For example: 1,2. This will send a message to Host and Admin users. Please do not add whitespaces or letters.</div>";
            html += "<div class='MessageSent dnnFormMessage dnnFormSuccess'>" + opts.messageSentText + "</div>";
            html += "<div class='ThrottlingWarning dnnFormMessage dnnFormWarning'>" + opts.throttlingText + "</div>";
            html += "<fieldset>";
            html += "<div class='dnnFormItem'><label for='to'>" + opts.toText + "</label><input type='text' id='to' name='to'/></div>";
            html += "<div class='dnnFormItem'><label for='subject'>" + opts.subjectText + "</label><input type='text' id='subject' name='subject' maxlength='400'/></div>";
            html += "<div class='dnnFormItem'><label for='bodytext'>" + opts.messageText + "</label><textarea rows='2' cols='20' id='bodytext' name='bodytext'/></div>";

            if (opts.showAttachments) {
                html += "<div class='dnnFormItem'><label>Attachments</label><a href='#' id='fileFromSite'>Browse from site</a></div>";
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
                        alert(file.id);
                    };
                    composeMessageDialog.find('#userFileManager').userFileManager(opts.userFileManagerOptions);
                }

                composeMessageDialog.find('input[type="text"]').keyup(function () {
                    var sendButton = $('.ui-dialog-buttonpane button:first');
                    if (composeMessageDialog.find('#to').val().trim().length > 0 && composeMessageDialog.find('#subject').val().trim().length > 0 && canSend) {
                        sendButton.removeAttr('disabled').removeClass('disabled');
                    } else {
                        sendButton.attr('disabled', 'disabled').addClass('disabled');
                    }
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
                                var params = {};
                                params.subject = encodeURIComponent(composeMessageDialog.find('#subject').val());
                                params.body = encodeURIComponent(composeMessageDialog.find('#bodytext').val());
                                params.roleIds = {}; //TODO hardcoded for now
                                params.userIds = "[" + composeMessageDialog.find('#to').val() + "]";
                                params.fileIds = {}; //TODO hardcoded for now 
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

})(jQuery);