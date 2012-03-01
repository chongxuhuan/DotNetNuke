(function ($) {
    $.fn.dnnComposeMessage = function (options) {
        var opts = $.extend({}, $.fn.dnnComposeMessage.defaultOptions, options),
            $wrap = this;

        $wrap.each(function () {
            var self = $(this),
                composeMessageDialog,
                html,
                canSend = false,
                autoclose,
                messageId = -1;

            self.addClass('ui-dnncomposemessage');

            //construct the form
            html = "<div class='MessageSent dnnFormMessage dnnFormSuccess'>" + opts.messageSentText + "</div>";
            html += "<div class='ThrottlingWarning dnnFormMessage dnnFormWarning'>" + opts.throttlingText + "</div>";
            html += "<fieldset>";
            html += "<div class='dnnFormItem'><label for='to'>" + opts.toText + "</label><input type='text' id='to' name='to'/></div>";
            html += "<div class='dnnFormItem'><label for='subject'>" + opts.subjectText + "</label><input type='text' id='subject' name='subject' maxlength='400'/></div>";
            html += "<div class='dnnFormItem'><label for='bodytext'>" + opts.messageText + "</label><textarea rows='2' cols='20' id='bodytext' name='bodytext'/></div>";

            //TODO What to do with attachment?
            if (opts.showAttachements) {
                html += '<img src="attachment.gif"/>';
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
                        $('.ui-dialog-buttonpane :button').removeClass().addClass('dnnTertiaryAction');
                        composeMessageDialog.find('.MessageSent').hide();
                        composeMessageDialog.find('.ThrottlingWarning').hide();
                        messageId = -1;

                        canSend = false;
                        var sendButton = $('.ui-dialog-buttonpane button:first');
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
                                data = {};
                                data.subject = composeMessageDialog.find('#subject').val();
                                data.body = composeMessageDialog.find('#bodytext').val();
                                data.roleIds = {}; //TODO hardcoded for now
                                data.userIds = 1; //TODO hardcoded for now 
                                data.fileIds = {}; //TODO hardcoded for now 
                                $.ajax({
                                    type: "POST",
                                    url: opts.serviceurlbase + "Create",
                                    data: data
                                }).done(function (data) {
                                    composeMessageDialog.find('.MessageSent').show();
                                    $('.ui-dialog-buttonpane button:first').attr('disabled', 'disabled').addClass('disabled');
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
        onMessageSent: null, // callback function that accepts a meesage id
        serviceurlbase: '/desktopmodules/CoreServices/API/MessagingService/',
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
        showAttachements: false,
        msgSentAutoCloseTimeout: 3000
    };

})(jQuery);