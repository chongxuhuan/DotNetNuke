(function ($) {
    $.fn.dnnComposeMessage = function (options) {
        var opts = $.extend({}, $.fn.dnnComposeMessage.defaultOptions, options),
            $wrap = this;

        $wrap.each(function () {
            var $this = $(this),
                $composeMessageDialog,
                html,
                canSend = false,
                autoclose,
                messageId = -1;

            //construct the form
            html = "<div class='MessageSent dnnFormMessage dnnFormSuccess'>" + opts.messageSentText + "</div>";
            html += "<fieldset>";
            html += "<div class='dnnFormItem'><label for='to'>" + opts.toText + "</label><input type='text' id='to' name='to'/></div>";
            html += "<div class='dnnFormItem'><label for='subject'>" + opts.subjectText + "</label><input type='text' id='subject' name='subject'/></div>";
            html += "<div class='dnnFormItem'><label for='bodytext'>" + opts.messageText + "</label><textarea rows='2' cols='20' id='bodytext' name='bodytext'/></div>";

            //TODO What to do with attachment?
            if (opts.showAttachements) {
                html += '<img src="attachment.gif"/>';
            }

            html += "</fieldset>";

            $composeMessageDialog = $("<div class='composeMessageDialog dnnForm dnnClear'></div>").html(html).dialog(opts);

            function getWaitTimeForNextMessage() {
                var returnValue = 0;
                $.ajax({
                    type: "GET",
                    url: opts.serviceurlbase + "WaitTimeForNextMessage",
                    data: {},
                    async: false,
                    success: function (data) {
                        returnValue = data;
                    }
                });
                return returnValue;
            };

            $this.click(function (e) {
                if ($composeMessageDialog.is(':visible')) {
                    $composeMessageDialog.dialog("close");
                    return true;
                }

                e.preventDefault();

                $composeMessageDialog.find('input[type="text"]').keyup(function () {
                    var sendButton = $('.ui-dialog-buttonpane button:first');
                    if ($composeMessageDialog.find('#to').val().trim().length > 0 && $composeMessageDialog.find('#subject').val().trim().length > 0 && canSend) {
                        sendButton.removeAttr('disabled').removeClass('disabled');
                    } else {
                        sendButton.attr('disabled', 'disabled').addClass('disabled');
                    }
                });

                $composeMessageDialog.dialog({
                    minWidth: 650,
                    modal: true,
                    resizable: false,
                    open: function () {
                        $('.ui-dialog-buttonpane :button').removeClass().addClass('dnnTertiaryAction');
                        $composeMessageDialog.find('.MessageSent').hide();
                        messageId = -1;

                        canSend = false;
                        var sendButton = $('.ui-dialog-buttonpane button:first');
                        var timeForNextMessage = getWaitTimeForNextMessage();
                        if (timeForNextMessage > 0) {
                            sendButton.text(timeForNextMessage + ' sec').attr('disabled', 'disabled').addClass('disabled');
                            sendButton.before('<span>' + opts.throttlingText + '</span>');
                            var countdown = setInterval(function () {
                                timeForNextMessage--;
                                if (timeForNextMessage == 0) {
                                    canSend = true;
                                    sendButton.text(opts.sendText);
                                    if ($composeMessageDialog.find('#to').val().trim().length > 0 && $composeMessageDialog.find('#subject').val().trim().length > 0) {
                                        sendButton.removeAttr('disabled').removeClass('disabled');
                                    }
                                    clearInterval(countdown);
                                    sendButton.prev().remove();
                                } else {
                                    sendButton.text(timeForNextMessage + ' sec');
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
                                data.subject = $composeMessageDialog.find('#subject').val();
                                data.body = $composeMessageDialog.find('#bodytext').val();
                                data.roleIds = {}; //TODO hardcoded for now
                                data.userIds = 1; //TODO hardcoded for now 
                                data.fileIds = {}; //TODO hardcoded for now 
                                $.ajax({
                                    type: "POST",
                                    url: opts.serviceurlbase + "Create",
                                    data: data,
                                    success: function (data) {
                                        $composeMessageDialog.find('.MessageSent').show();
                                        $('.ui-dialog-buttonpane button:first').attr('disabled', 'disabled').addClass('disabled');
                                        messageId = data;
                                        autoclose = setInterval(function () {
                                            $composeMessageDialog.dialog("close");
                                        }, opts.msgSentAutoCloseTimeout);
                                    },
                                    error: function (xhr, status, error) {
                                        alert(error);
                                    }
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
                        $composeMessageDialog.find('input[type="text"],textarea').val('');
                    }
                });
                $composeMessageDialog.dialog('open');
            });

            return $wrap;
        });
    };

    $.fn.dnnComposeMessage.defaultOptions = {
        onMessageSent: null, // callback function that accepts a meesage id
        serviceurlbase: '/desktopmodules/SocialMessaging/API/MessagingService/',
        title: 'Compose Message',
        toText: 'Send to',
        subjectText: 'Subject',
        messageText: 'Your Message',
        sendText: 'Send',
        cancelText: 'Cancel',
        messageSentText: 'Your message has been sent successfully.',
        throttlingText: 'Interval to wait before sending a new message:',
        dialogClass: 'dnnFormPopup dnnClear',
        autoOpen: false,
        showAttachements: false,
        msgSentAutoCloseTimeout: 3000
    };

})(jQuery);