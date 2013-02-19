(function ($) {
    $.fn.dnnTabs = function (options) {
        var opts = $.extend({}, $.fn.dnnTabs.defaultOptions, options),
        $wrap = this;

        // patch for period in selector - http://jsfiddle.net/9Mst9/2/
        $.ui.tabs.prototype._sanitizeSelector = function (hash) {
            return hash.replace(/:/g, "\\:").replace(/\./g, "\\\.");
        };

        $wrap.each(function () {
            var showEvent = null,
                cookieId;

            if (this.id) {
                cookieId = 'dnnTabs-' + this.id;
                if (opts.selected === -1) {
                    var cookieValue = dnn.dom.getCookie(cookieId);
                    if (cookieValue) {
                        opts.selected = cookieValue;
                    }
                    if (opts.selected === -1) {
                        opts.selected = 0;
                    }
                }
            }

            showEvent = (function (cookieId) {
                return function (event, ui) {
                    dnn.dom.setCookie(cookieId, ui.index, opts.cookieDays, '/', '', false, opts.cookieMilleseconds);
                };
            })(cookieId);

            $wrap.tabs({
                show: showEvent,
                selected: opts.selected,
                disabled: opts.disabled,
                fx: {
                    opacity: opts.opacity,
                    duration: opts.duration
                }
            });

            // page validation integration - select tab that contain tripped validators
            if (typeof Page_ClientValidate != "undefined" && $.isFunction(Page_ClientValidate)) {
                $wrap.find(opts.validationTriggerSelector).click(function (e) {
                    if (!Page_ClientValidate(opts.validationGroup)) {
                        var invalidControl = $wrap.find(opts.invalidItemSelector).eq(0);

                        var $parent = invalidControl.closest(".ui-tabs-panel");
                        if ($parent.length > 0) {
                            var tabId = $parent.attr("id");
                            $parent.parent().find("a[href='#" + tabId + "']").click();
                        }
                    }
                });
            };
        });

        return $wrap;
    };

    $.fn.dnnTabs.defaultOptions = {
        opacity: 'toggle',
        duration: 'fast',
        validationTriggerSelector: '.dnnPrimaryAction',
        validationGroup: '',
        invalidItemSelector: '.dnnFormError[style*="inline"]',
        regionToToggleSelector: 'fieldset',
        selected: -1,
        cookieDays: 0,
        cookieMilleseconds: 120000 // two minutes
    };

})(jQuery);

(function ($) {
    $.fn.dnnConfirm = function (options) {
        var opts = $.extend({}, $.fn.dnnConfirm.defaultOptions, options),
        $wrap = this;

        $wrap.each(function () {
            var $this = $(this),
                defaultAction = $this.attr('href'),
                $dnnDialog;

            if (defaultAction || opts.isButton) {
                $dnnDialog = $("<div class='dnnDialog'></div>").html(opts.text).dialog(opts);
                $this.click(function (e, isTrigger) {
                    if (isTrigger) {
                        return true;
                    }

                    if ($dnnDialog.is(':visible')) {
                        $dnnDialog.dialog("close");
                        return true;
                    }

                    e.preventDefault();

                    $dnnDialog.dialog({
                        open: function (e) {
                            $('.ui-dialog-buttonpane').find('button:contains("' + opts.noText + '")').addClass('dnnConfirmCancel');
                        },
                        position: 'center',
                        buttons: [
                        {
                            text: opts.yesText,
                            click: function () {
                                $dnnDialog.dialog("close");
                                if ($.isFunction(opts.callbackTrue)) {
                                    opts.callbackTrue.call(this);
                                }
                                else {
                                    if (opts.isButton) {
                                        $this.trigger("click", [true]);
                                    }
                                    else {
                                        window.location.href = defaultAction;
                                    }
                                }
                            }
                        },
                        {
                            text: opts.noText,
                            click: function () {
                                $(this).dialog("close");
                                if ($.isFunction(opts.callbackFalse)) {
                                    opts.callbackFalse.call(this);
                                };
                            }
                        }
                        ]
                    });
                    $dnnDialog.dialog('open');
                });
            }
        });

        return $wrap;
    };

    $.fn.dnnConfirm.defaultOptions = {
        text: 'Are you sure?',
        yesText: 'Yes',
        noText: 'No',
        actionUrl: window.location.href,
        autoOpen: false,
        resizable: false,
        modal: true,
        title: 'Confirm',
        dialogClass: 'dnnFormPopup dnnClear',
        isButton: false
    };

})(jQuery);

(function ($) {
    $.dnnAlert = function (options) {
        var opts = $.extend({}, $.dnnAlert.defaultOptions, options),
        $dnnDialog = $("<div class='dnnDialog'></div>").html(opts.text).dialog(opts);

        $dnnDialog.dialog({
            buttons: [
                {
                    text: opts.okText,
                    click: function () {
                        $(this).dialog("close");
                        if ($.isFunction(opts.callback)) {
                            opts.callback.call(this);
                        };
                        return false;
                    }
                }
            ]
        });
        $dnnDialog.dialog('open');
    };

    $.dnnAlert.defaultOptions = {
        okText: 'Ok',
        autoOpen: false,
        resizable: false,
        modal: true,
        dialogClass: 'dnnFormPopup dnnClear'
    };

})(jQuery);

(function ($) {
    $.fn.dnnPanels = function (options) {
        var opts = $.extend({}, $.fn.dnnPanels.defaultOptions, options),
        $wrap = this;

        $wrap.each(function () {
            var $this = $(this);

            // wire up click event to perform slide toggle
            $this.find(opts.clickToToggleSelector).click(function (e) {
                e.preventDefault();

                var toggle = $(this).toggleClass(opts.toggleClass).parent().next(opts.regionToToggleSelector).slideToggle(function () {
                    var id = $(toggle.context.parentNode).attr("id");
                    var cookieId = id ? id.replace(/[^a-zA-Z0-9\-]+/g, "") : '';
                    if (cookieId)
                        dnn.dom.setCookie(cookieId, $(this).is(':visible'), opts.cookieDays, '/', '', false, opts.cookieMilleseconds);
                });

                //stop event from called again
                e.stopImmediatePropagation();
            });

            function collapsePanel($clicker, $region) {
                $clicker.removeClass(opts.toggleClass);
                $region.hide();
            }

            function expandPanel($clicker, $region) {
                $clicker.addClass(opts.toggleClass);
                $region.show();
            }

            // walk over each selector and expand or collapse as necessary
            $this.find(opts.sectionHeadSelector).each(function (indexInArray, valueOfElement) {
                var $this = $(valueOfElement),
                    elementId = $this.attr("id"),
            	    cookieId = elementId ? elementId.replace(/[^a-zA-Z0-9\-]+/g, "") : '',
                    cookieValue = cookieId ? dnn.dom.getCookie(cookieId) : '',
                    $clicker = $this.find(opts.clickToToggleIsolatedSelector),
                    $region = $this.next(opts.regionToToggleSelector),
                    $parentSeparator = $this.parents(opts.panelSeparatorSelector),
                    groupPanelIndex = $parentSeparator.find(opts.sectionHeadSelector).index($this)

                if (cookieValue == "false") { // cookie explicitly set to false
                    collapsePanel($clicker, $region);
                }
                else if (cookieValue == "true" || indexInArray === 0) { // cookie set to true OR first panel
                    expandPanel($clicker, $region);
                }
                else if ($parentSeparator.size() > 0 && groupPanelIndex === 0) { // grouping is used & its the first panel in its group
                    expandPanel($clicker, $region);
                }
                else { // nada...
                    collapsePanel($clicker, $region);
                }
            });

            // page validation integration - expand collapsed panels that contain tripped validators
            $this.find(opts.validationTriggerSelector).click(function (e) {
                if (typeof Page_ClientValidate != "undefined" && $.isFunction(Page_ClientValidate)) {
                    if (!Page_ClientValidate(opts.validationGroup)) {
                        $this.find(opts.invalidItemSelector).each(function () {
                            var $parent = $(this).closest(opts.regionToToggleSelector);
                            if ($parent.is(':hidden')) {
                                $parent.prev(opts.sectionHeadSelector).find(opts.clickToToggleIsolatedSelector).click();
                            }
                        });
                    }
                }
            });

        });

        return $wrap;
    };

    $.fn.dnnPanels.defaultOptions = {
        clickToToggleSelector: 'h2.dnnFormSectionHead a',
        sectionHeadSelector: '.dnnFormSectionHead',
        regionToToggleSelector: 'fieldset',
        toggleClass: 'dnnSectionExpanded',
        clickToToggleIsolatedSelector: 'a',
        validationTriggerSelector: '.dnnPrimaryAction',
        invalidItemSelector: '.dnnFormError[style*="inline"]',
        validationGroup: '',
        panelSeparatorSelector: '.ui-tabs-panel',
        cookieDays: 0,
        cookieMilleseconds: 120000 // two minutes
    };

})(jQuery);

(function ($) {
    $.fn.dnnPreview = function (options) {
        var opts = $.extend({}, $.fn.dnnPreview.defaultOptions, options),
        $wrap = this;

        $wrap.each(function () {
            var $this = $(this);
            $this.find(opts.linkSelector).click(function (e) {
                e.preventDefault();
                var params = "?";
                var skin, container;

                if (opts.useComboBox) {
                    var skinComboBox = $find(opts.skinSelector);
                    var containerComboBox = $find(opts.containerSelector);

                    skin = skinComboBox ? skinComboBox.get_value() : '';
                    container = containerComboBox ? containerComboBox.get_value() : '';
                }
                else {
                    skin = $this.find(opts.skinSelector).val();
                    container = $this.find(opts.containerSelector).val();
                }
                if (skin) {
                    params += "SkinSrc=" + skin;
                }
                if (container) {
                    if (skin) {
                        params += "&";
                    }
                    params += "ContainerSrc=" + container;
                }
                if (params != "?") {
                    window.open(encodeURI(opts.baseUrl + params.replace(/.ascx/gi, '')), "skinpreview");
                }
                else {
                    $.dnnAlert({ text: opts.noSelectionMessage, okText: opts.alertOkText, closeText: opts.alertCloseText });
                }
            });
        });

        return $wrap;
    };

    $.fn.dnnPreview.defaultOptions = {
        baseUrl: window.location.protocol + "//" + window.location.host + window.location.pathname,
        linkSelector: 'a.dnnSecondaryAction',
        skinSelector: '',
        containerSelector: '',
        noSelectionMessage: 'Please select a preview option.',
        alertOkText: 'Ok',
        alertCloseText: 'close',
        useComboBox: false
    };

})(jQuery);

(function ($) {
    $.fn.dnnExpandAll = function (options) {
        var opts = $.extend({}, $.fn.dnnExpandAll.defaultOptions, options),
        $elem = this;

        if (($(opts.targetArea).find(opts.targetSelector + ':visible').length ===
            $(opts.targetArea).find(opts.targetSelector + opts.targetExpandedSelector + ':visible').length)
            && !$(this).hasClass('expanded')) {
            $(this).addClass('expanded').text(opts.collapseText);
        }

        $elem.click(function (e) {
            e.preventDefault();
            var $this = $(this);
            if ($this.hasClass('expanded')) {
                $this.removeClass('expanded').text(opts.expandText);
                $(opts.targetArea).find(opts.targetSelector + opts.targetExpandedSelector + ':visible').click();
            }
            else {
                $this.addClass('expanded').text(opts.collapseText);
                $(opts.targetArea).find(opts.targetSelector + ':visible').not(opts.targetExpandedSelector).click();
            }

            //stop event from called again
            e.stopImmediatePropagation();
        });

        return $elem;
    };
    $.fn.dnnExpandAll.defaultOptions = {
        expandText: 'Expand All',
        collapseText: 'Collapse All',
        targetArea: '#dnnHostSettings',
        targetSelector: 'h2.dnnFormSectionHead a',
        targetExpandedSelector: '.dnnSectionExpanded'
    };
})(jQuery);

(function ($) {
    $.fn.dnnTooltip = function (options) {
        var opts = $.extend({}, $.fn.dnnTooltip.defaultOptions, options),
        $wrap = this;

        $wrap.each(function () {
            var $this = $(this);
            var dnnFormHelp = $this.prev();
            dnnFormHelp.click(function (e) {
                e.preventDefault();
            });
            var helpSelector = $this.find(opts.helpSelector);
            $this.parent().css({ position: 'relative' });
            var tooltipHeight = helpSelector.height();
            var top = -(tooltipHeight + 30);
            $this.css({ position: 'absolute', right: '-29%', top: top + 'px' });
            var hoverOnToolTip = false, hoverOnPd = false;
            dnnFormHelp.hoverIntent({
                over: function () {
                    hoverOnPd = true;
                    helpSelector.show();
                },
                out: function () {
                    hoverOnPd = false;
                    if (!$this.hasClass(opts.pinnedClass) && !hoverOnToolTip) {
                        helpSelector.hide();
                    }
                },
                timeout: 200,
                interval: 200
            });

            helpSelector.hover(function () { hoverOnToolTip = true; }, function () {
                hoverOnToolTip = false;
                if (!$this.hasClass(opts.pinnedClass) && !hoverOnPd) {
                    helpSelector.hide();
                }
            });

            var pinHelper = helpSelector.find(opts.pinSelector);
            pinHelper.click(function (e) {
                e.preventDefault();
                if ($this.hasClass(opts.pinnedClass)) {
                    helpSelector.css({ "left": '0px', "top": '0px' }).hide().draggable('destroy');
                    $this.removeClass(opts.pinnedClass);
                }
                else {
                    $this.addClass(opts.pinnedClass);
                    if ($.isFunction($().draggable)) {
                        helpSelector.draggable();
                    }
                }

            });
        });

        return $wrap;
    };

    $.fn.dnnTooltip.defaultOptions = {
        pinSelector: 'a.pinHelp',
        helpSelector: '.dnnFormHelpContent',
        pinnedClass: 'dnnTooltipPinned'
    };

})(jQuery);

(function ($) {
    var CB = function (e) {
        if (!e) var e = window.event;
        e.cancelBubble = true;
        if (e.stopPropagation) e.stopPropagation();
    };

    /* DNN customized checkbox/radiobox */
    $.fn.dnnCheckbox = function (options) {

        /* Default settings */
        var settings = {
            cls: 'dnnCheckbox'  /* checkbox  */
        };

        /* Processing settings */
        settings = $.extend(settings, options || {});

        /* Adds check/uncheck & disable/enable events */
        var addEvents = function (object) {
            var checked = object.checked;
            var disabled = object.disabled;
            var $object = $(object);

            if (object.stateInterval)
                clearInterval(object.stateInterval);

            object.stateInterval = setInterval(
                function () {
                    if (object.disabled != disabled)
                        $object.trigger((disabled = !!object.disabled) ? 'disable' : 'enable');
                    if (object.checked != checked)
                        $object.trigger((checked = !!object.checked) ? 'check' : 'uncheck');
                },
                10 /* in miliseconds. Low numbers this can decrease performance on slow computers, high will increase responce time */
            );
            return $object;
        };

        return this.each(function () {

            var ch = this; /* Reference to DOM Element*/
            /* Check we needs to add customization or not*/
            if ($(this).hasClass('normalCheckBox') || $(this).hasClass('normalRadioButton')) return;
            var parentCheckBoxHolder = $(this).closest('.normalCheckBox');
            var parentRadioButtonHolder = $(this).closest('.normalRadioButton');
            if (parentCheckBoxHolder.length || parentRadioButtonHolder.length) return;

            var $ch = addEvents(ch); /* Adds custom events and returns, jQuery enclosed object */



            /* Removing wrapper if already applied  */
            if (ch.wrapper) ch.wrapper.remove();

            /* Creating wrapper for checkbox and assigning "hover" event */
            ch.wrapper = $('<span class="' + settings.cls + '"><span class="mark"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAMAAAAoyzS7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAAZQTFRFAAAAAAAApWe5zwAAAAF0Uk5TAEDm2GYAAAAMSURBVHjaYmAACDAAAAIAAU9tWeEAAAAASUVORK5CYII=" /></span></span>');
            ch.wrapperInner = ch.wrapper.children('span:eq(0)');
            ch.wrapper.hover(
                function (e) { ch.wrapperInner.addClass(settings.cls + '-hover'); CB(e); },
                function (e) { ch.wrapperInner.removeClass(settings.cls + '-hover'); CB(e); }
            );

            /* Wrapping checkbox */
            $ch.css({ position: 'absolute', zIndex: -1, visibility: 'hidden' }).after(ch.wrapper);

            /* Trying to find "our" label */
            var label = false, parentLabel = false;
            label = $ch.closest('label');
            if (!label.length) label = false; else parentLabel = true;
            if (!label && $ch.attr('id')) {
                label = $('label[for="' + $ch.attr('id') + '"]');
                if (!label.length) label = false;
            }

            /* Labe found, applying event hanlers */
            if (label) {
                label.css('display', 'inline-block');

                if (!parentLabel) {
                    label.click(function (e) {
                        $ch.triggerHandler('focus');
                        $ch.trigger('click', [e]);
                        $ch.trigger('change', [e]);
                        CB(e);
                        return false;
                    });
                }
                else {
                    label.children().each(function () {
                        var $this = $(this);
                        if ($this.is('input')) return;

                        $this.click(function (e) {
                            $ch.triggerHandler('focus');
                            $ch.trigger('click', [e]);
                            $ch.trigger('change', [e]);
                            CB(e);
                            return false;
                        });
                    });
                }
            }

            if (!parentLabel) {
                ch.wrapper.click(function (e) {
                    $ch.triggerHandler('focus');
                    $ch.trigger('click', [e]);
                    $ch.trigger('change', [e]);
                    CB(e);
                    return false;
                });
            }
            if ($ch.hasClass("rtChk")) { // stop event propagation for checkbox in rad control.
		        $ch.click(function(e) { CB(e); });
	        }
	        $ch.bind('disable', function () { ch.wrapperInner.addClass(settings.cls + '-disabled'); }).bind('enable', function () { ch.wrapperInner.removeClass(settings.cls + '-disabled'); });
            $ch.bind('check', function () { ch.wrapper.addClass(settings.cls + '-checked'); }).bind('uncheck', function () { ch.wrapper.removeClass(settings.cls + '-checked'); });

            /* Applying checkbox state */
            if (ch.checked)
                ch.wrapper.addClass(settings.cls + '-checked');
            if (ch.disabled)
                ch.wrapperInner.addClass(settings.cls + '-disabled');

        });
    };

    $.fn.dnnHelperTipDestroy = function () {
        return this.each(function () {
            var pd = this;
            if (pd.id) {
                $('div[data-tipholder="' + pd.id + '"]').remove();
            }
        });
    };

    $.fn.dnnHelperTip = function (options) {

        /* Default settings */
        var settings = {
            cls: 'dnnHelperTip',
            helpContent: "This is hover helper tooltip",
            holderId: '',
            show: false // immediately show tooltip after call
        };

        /* Processing settings */
        settings = $.extend(settings, options || {});
        return this.each(function () {
            var pd = this;
            var $pd = $(this);

            /* Removing tooltip if already applied  */
            if (pd.tooltipWrapper) pd.tooltipWrapper.remove();

            /* Creating tooltip wrapper for selected dom and assigning "hover" event */
            pd.tooltipWrapper = $('<div class="' + settings.cls + '" data-tipholder="' + settings.holderId + '"> <div class="dnnFormHelpContent dnnClear"><span class="dnnHelpText">' + settings.helpContent + '</span></div></div>');
            $('body').append(pd.tooltipWrapper);
            pd.tooltipWrapper.css({ position: 'absolute' });
            pd.tooltipWrapperInner = $('.dnnFormHelpContent', pd.tooltipWrapper);

            var tooltipHeight = pd.tooltipWrapperInner.height();
            pd.tooltipWrapperInner.css({ left: '-10px', top: -(tooltipHeight + 30) + 'px' })

            var hoverOnPd = false;
            $pd.hover(
                function () {
                    hoverOnPd = true;
                    setTimeout(function () {
                        if (hoverOnPd)
                            pd.tooltipWrapperInner.show();
                    }, 400);
                },
                function () {
                    hoverOnPd = false;
                    setTimeout(function () {
                        if (!hoverOnPd)
                            pd.tooltipWrapperInner.hide();
                    }, 400);

                });

            if (settings.show) {
                hoverOnPd = true;
                setTimeout(function () {
                    pd.tooltipWrapperInner.show();
                }, 400);
            }

            $pd.bind('mousemove', function (e) {
                var x = e.pageX; var y = e.pageY;
                var pos = $('body').css('position');
                if (pos == 'relative') y -= 38;
                pd.tooltipWrapper.css({ left: x + 'px', top: y + 'px', 'z-index': '99999' });
            });
        });
    };

    $.fn.dnnProgressbar = function () {
        var $pd = $(this);
        var pd = this;

        if (pd.tooltipWrapper) pd.tooltipWrapper.remove();
        pd.tooltipWrapper = $('<div class="dnnTooltip"> <div class="dnnFormHelpContent dnnClear"><span class="dnnHelpText"></span></div></div>').insertAfter($pd);
        pd.tooltipWrapperInner = $('.dnnFormHelpContent', pd.tooltipWrapper);
        pd.tooltipWrapperInner.css({ width: '30px', padding: '8px' });
        $pd.parent().css({ position: 'relative' });

        var hoverOnToolTip = false, hoverOnPd = false;
        $pd.hoverIntent({
            over: function () {
                hoverOnPd = true;
                var val = $(this).children(':first').progressbar('value');
                pd.tooltipWrapperInner.find('span').html(val + ' %');
                var pdTop = $pd.position().top,
                    tooltipHeight = pd.tooltipWrapperInner.height();

                pdTop -= (tooltipHeight + 10);
                var pdLeft = val > 50 ? (val - 4) + '%' : val > 0 ? (val - 2) + '%' : '10px';
                pd.tooltipWrapper.css({ position: 'absolute', left: pdLeft, top: pdTop + 'px' });
                pd.tooltipWrapperInner.show();

            },
            out: function () {
                hoverOnPd = false;
                if (!hoverOnToolTip) {
                    pd.tooltipWrapperInner.hide();
                }
            },
            timeout: 200,
            interval: 200
        });

        pd.tooltipWrapperInner.hover(function () { hoverOnToolTip = true; }, function () {
            hoverOnToolTip = false;
            if (!hoverOnPd) {
                pd.tooltipWrapperInner.hide();
            }
        });

        return this;
    };

    $.fn.dnnSpinner = function (options) {

        // set default options
        var opt = $.extend({
            type: 'range',
            typedata: '',
            width: '150px',
            looping: false
        }, options);

        var otypedata;

        if (options != null && options.typedata != null) {
            otypedata = $.extend({
                min: 1,
                max: 10,
                interval: 1,
                decimalplaces: 0
            }, options.typedata);
        }
        else {
            otypedata = $.extend({
                min: 1,
                max: 10,
                interval: 1,
                decimalplaces: 0
            });
        }
        opt.typedata = otypedata;

        var inputControl = this;

        // validate if the object is a input of text type.
        if (!inputControl.is(':text'))
            return inputControl;

        if (inputControl.hasClass('dnnSpinnerInput')) {
            return inputControl;
        }
        else {
            inputControl.addClass('dnnSpinnerInput');
        }

        // create the Spinner Control body.
        var strContainerDiv = '';
        strContainerDiv += '<div class="dnnSpinner">';
        strContainerDiv += '<div class="dnnSpinnerDisplay"></div>';
        strContainerDiv += '<div class="dnnSpinnerCtrl">';
        strContainerDiv += '<a class="dnnSpinnerTopButton"></a>';
        strContainerDiv += '<a class="dnnSpinnerBotButton"></a>';
        strContainerDiv += '</div></div>';

        // add the above created control to page
        var objContainerDiv = $(strContainerDiv).insertAfter(inputControl);

        // hide the input control and place within the Spinner Control body
        inputControl.insertAfter($("div.dnnSpinnerDisplay", objContainerDiv));
        $("div.dnnSpinnerDisplay", objContainerDiv).click(function () {
            inputControl.triggerHandler('focus');
        });
        inputControl.css('display', 'none');

        switch (opt.type) {
            case 'range':
                // set default value;
                if (opt.defaultVal < opt.typedata.min || opt.defaultVal > opt.typedata.max) {
                    opt.defaultVal = opt.typedata.min;
                }
                if (opt.defaultVal % opt.typedata.interval > 0) {
                    opt.defaultVal = parseInt((opt.defaultVal / opt.typedata.interval).toFixed(0)) * opt.typedata.interval;
                }
                inputControl.val(opt.defaultVal.toFixed(opt.typedata.decimalplaces));
                ($("div.dnnSpinnerDisplay", objContainerDiv)).html(opt.defaultVal.toFixed(opt.typedata.decimalplaces));
                var selectedValue = opt.defaultVal;

                if ((opt.typedata.max - opt.typedata.min) > opt.typedata.interval) {
                    // attach events;
                    $("a.dnnSpinnerTopButton", objContainerDiv).click(function () {
                        if ((selectedValue + opt.typedata.interval) <= opt.typedata.max || opt.looping) {
                            if ((selectedValue + opt.typedata.interval) > opt.typedata.max) {
                                selectedValue = opt.typedata.min - opt.typedata.interval;
                            }
                            var valueData = (selectedValue + opt.typedata.interval).toFixed(opt.typedata.decimalplaces);
                            selectedValue += opt.typedata.interval;
                            ($("div.dnnSpinnerDisplay", objContainerDiv)).html(valueData);
                            inputControl.val(valueData);
                        }
                        inputControl.triggerHandler('focus');
                        return false;
                    });

                    $("a.dnnSpinnerBotButton", objContainerDiv).click(function () {
                        if ((selectedValue - opt.typedata.interval) >= opt.typedata.min || opt.looping) {
                            if ((selectedValue - opt.typedata.interval) < opt.typedata.min) {
                                selectedValue = opt.typedata.max + opt.typedata.interval;
                            }
                            var valueData = (selectedValue - opt.typedata.interval).toFixed(opt.typedata.decimalplaces);
                            selectedValue -= opt.typedata.interval;
                            ($("div.dnnSpinnerDisplay", objContainerDiv)).html(valueData);
                            inputControl.val(valueData);
                        }
                        inputControl.triggerHandler('focus');
                        return false;
                    });
                }

                break;
            case 'list':
                if (!opt.typedata.list || opt.typedata.list.lenght == 0) {
                    return inputControl;
                }

                var listItems = opt.typedata.list.split(',');
                var selectedIndex = jQuery.inArray(opt.defaultVal, listItems);
                if (selectedIndex < 0) {
                    selectedIndex = 0;
                    opt.defaultVal = listItems[0];
                }

                inputControl.val(opt.defaultVal);
                ($("div.dnnSpinnerDisplay", objContainerDiv)).html(opt.defaultVal);

                if (listItems.length > 1) {
                    // attach events;
                    $("a.dnnSpinnerBotButton", objContainerDiv).click(function () {
                        if (selectedIndex < (listItems.length - 1) || opt.looping) {
                            if (selectedIndex == listItems.length - 1) {
                                selectedIndex = -1;
                            }
                            selectedIndex++;
                            var valueData = listItems[selectedIndex];
                            ($("div.dnnSpinnerDisplay", objContainerDiv)).html(valueData);
                            inputControl.val(valueData);
                        }
                        inputControl.triggerHandler('focus');
                        return false;
                    });

                    $("a.dnnSpinnerTopButton", objContainerDiv).click(function () {
                        if (selectedIndex > 0 || opt.looping) {
                            if (selectedIndex == 0) {
                                selectedIndex = listItems.length;
                            }
                            selectedIndex--;
                            var valueData = listItems[selectedIndex];
                            ($("div.dnnSpinnerDisplay", objContainerDiv)).html(valueData);
                            inputControl.val(valueData);
                        }
                        inputControl.triggerHandler('focus');
                        return false;
                    });
                }

                break;
        };

        // return the selected input control for the chainability
        return inputControl;
    };
})(jQuery);

// dnn customised jquery autocompleter
(function ($) {
    "use strict";
    $.fn.dnnAutocomplete = function (options) {
        var url;
        if (arguments.length > 1) {
            url = options;
            options = arguments[1];
            options.url = url;
        } else if (typeof options === 'string') {
            url = options;
            options = { url: url };
        }
        var opts = $.extend({}, $.fn.dnnAutocomplete.defaults, options);
        return this.each(function () {
            var $this = $(this);
            $this.data('autocompleter', new $.dnnAutocompleter(
                $this,
                $.meta ? $.extend({}, opts, $this.data()) : opts
            ));
        });
    };

    /**
     * Store default options
     * @type {object}
     */
    $.fn.dnnAutocomplete.defaults = {
        inputClass: 'acInput',
        loadingClass: 'acLoading',
        resultsClass: 'acResults',
        selectClass: 'acSelect',
        queryParamName: 'q',
        extraParams: {},
        remoteDataType: false,
        lineSeparator: '\n',
        cellSeparator: '|',
        minChars: 2,
        maxItemsToShow: 10,
        delay: 400,
        useCache: true,
        maxCacheLength: 10,
        matchSubset: true,
        matchCase: false,
        matchInside: true,
        mustMatch: false,
        selectFirst: false,
        selectOnly: false,
        showResult: null,
        preventDefaultReturn: 1,
        preventDefaultTab: 0,
        autoFill: false,
        filterResults: true,
        sortResults: true,
        sortFunction: null,
        onItemSelect: null,
        onNoMatch: null,
        onFinish: null,
        matchStringConverter: null,
        beforeUseConverter: null,
        autoWidth: 'min-width',
        useDelimiter: false,
        delimiterChar: ',',
        delimiterKeyCode: 188,
        processData: null,
        onError: null,
        moduleId: null // dnn Module Id context if needed
    };

    /**
     * Sanitize result
     * @param {Object} result
     * @returns {Object} object with members value (String) and data (Object)
     * @private
     */
    var sanitizeResult = function (result) {
        var value, data;
        var type = typeof result;
        if (type === 'string') {
            value = result;
            data = {};
        } else if ($.isArray(result)) {
            value = result[0];
            data = result.slice(1);
        } else if (type === 'object') {
            value = result.value;
            data = result.data;
        }
        value = String(value);
        if (typeof data !== 'object') {
            data = {};
        }
        return {
            value: value,
            data: data
        };
    };

    /**
     * Sanitize integer
     * @param {mixed} value
     * @param {Object} options
     * @returns {Number} integer
     * @private
     */
    var sanitizeInteger = function (value, stdValue, options) {
        var num = parseInt(value, 10);
        options = options || {};
        if (isNaN(num) || (options.min && num < options.min)) {
            num = stdValue;
        }
        return num;
    };

    /**
     * Create partial url for a name/value pair
     */
    var makeUrlParam = function (name, value) {
        return [name, encodeURIComponent(value)].join('=');
    };

    /**
     * Build an url
     * @param {string} url Base url
     * @param {object} [params] Dictionary of parameters
     */
    var makeUrl = function (url, params) {
        var urlAppend = [];
        $.each(params, function (index, value) {
            urlAppend.push(makeUrlParam(index, value));
        });
        if (urlAppend.length) {
            url += url.indexOf('?') === -1 ? '?' : '&';
            url += urlAppend.join('&');
        }
        return url;
    };
    /**
     * Default sort filter
     * @param {object} a
     * @param {object} b
     * @param {boolean} matchCase
     * @returns {number}
     */
    var sortValueAlpha = function (a, b, matchCase) {
        a = String(a.value);
        b = String(b.value);
        if (!matchCase) {
            a = a.toLowerCase();
            b = b.toLowerCase();
        }
        if (a > b) {
            return 1;
        }
        if (a < b) {
            return -1;
        }
        return 0;
    };

    /**
     * dnnAutocompleter class
     * @param {object} $elem jQuery object with one input tag
     * @param {object} options Settings
     * @constructor
     */
    $.dnnAutocompleter = function ($elem, options) {

        /**
         * Assert parameters
         */
        if (!$elem || !($elem instanceof $) || $elem.length !== 1 || $elem.get(0).tagName.toUpperCase() !== 'INPUT') {
            throw new Error('Invalid parameter for dnnAutocompleter, jQuery object with one element with INPUT tag expected.');
        }

        /**
         * @constant Link to this instance
         * @type object
         * @private
         */
        var self = this;

        /**
         * @property {object} Options for this instance
         * @public
         */
        this.options = options;

        /**
         * @property object Cached data for this instance
         * @private
         */
        this.cacheData_ = {};

        /**
         * @property {number} Number of cached data items
         * @private
         */
        this.cacheLength_ = 0;

        /**
         * @property {string} Class name to mark selected item
         * @private
         */
        this.selectClass_ = 'jquery-autocomplete-selected-item';

        /**
         * @property {number} Handler to activation timeout
         * @private
         */
        this.keyTimeout_ = null;

        /**
         * @property {number} Handler to finish timeout
         * @private
         */
        this.finishTimeout_ = null;

        /**
         * @property {number} Last key pressed in the input field (store for behavior)
         * @private
         */
        this.lastKeyPressed_ = null;

        /**
         * @property {string} Last value processed by the autocompleter
         * @private
         */
        this.lastProcessedValue_ = null;

        /**
         * @property {string} Last value selected by the user
         * @private
         */
        this.lastSelectedValue_ = null;

        /**
         * @property {boolean} Is this autocompleter active (showing results)?
         * @see showResults
         * @private
         */
        this.active_ = false;

        /**
         * @property {boolean} Is this autocompleter allowed to finish on blur?
         * @private
         */
        this.finishOnBlur_ = true;

        /**
         * Sanitize options
         */
        this.options.minChars = sanitizeInteger(this.options.minChars, $.fn.dnnAutocomplete.defaults.minChars, { min: 0 });
        this.options.maxItemsToShow = sanitizeInteger(this.options.maxItemsToShow, $.fn.dnnAutocomplete.defaults.maxItemsToShow, { min: 0 });
        this.options.maxCacheLength = sanitizeInteger(this.options.maxCacheLength, $.fn.dnnAutocomplete.defaults.maxCacheLength, { min: 1 });
        this.options.delay = sanitizeInteger(this.options.delay, $.fn.dnnAutocomplete.defaults.delay, { min: 0 });
        if (this.options.preventDefaultReturn != 2) {
            this.options.preventDefaultReturn = this.options.preventDefaultReturn ? 1 : 0;
        }
        if (this.options.preventDefaultTab != 2) {
            this.options.preventDefaultTab = this.options.preventDefaultTab ? 1 : 0;
        }

        /**
         * Init DOM elements repository
         */
        this.dom = {};

        /**
         * Store the input element we're attached to in the repository
         */
        this.dom.$elem = $elem;

        /**
         * Switch off the native autocomplete and add the input class
         */
        this.dom.$elem.attr('autocomplete', 'off').addClass(this.options.inputClass);

        /**
         * Create DOM element to hold results, and force absolute position
         */
        this.dom.$results = $('<div></div>').hide().addClass(this.options.resultsClass).css({
            position: 'absolute'
        });
        $('body').append(this.dom.$results);

        /**
         * Attach keyboard monitoring to $elem
         */
        $elem.keydown(function (e) {
            self.lastKeyPressed_ = e.keyCode;
            switch (self.lastKeyPressed_) {

                case self.options.delimiterKeyCode: // comma = 188
                    if (self.options.useDelimiter && self.active_) {
                        self.selectCurrent();
                    }
                    break;

                    // ignore navigational & special keys
                case 35: // end
                case 36: // home
                case 16: // shift
                case 17: // ctrl
                case 18: // alt
                case 37: // left
                case 39: // right
                    break;

                case 38: // up
                    e.preventDefault();
                    if (self.active_) {
                        self.focusPrev();
                    } else {
                        self.activate();
                    }
                    return false;

                case 40: // down
                    e.preventDefault();
                    if (self.active_) {
                        self.focusNext();
                    } else {
                        self.activate();
                    }
                    return false;

                case 9: // tab
                    if (self.active_) {
                        self.selectCurrent();
                        if (self.options.preventDefaultTab) {
                            e.preventDefault();
                            return false;
                        }
                    }
                    if (self.options.preventDefaultTab === 2) {
                        e.preventDefault();
                        return false;
                    }
                    break;

                case 13: // return
                    if (self.active_) {
                        self.selectCurrent();
                        if (self.options.preventDefaultReturn) {
                            e.preventDefault();
                            return false;
                        }
                    }
                    if (self.options.preventDefaultReturn === 2) {
                        e.preventDefault();
                        return false;
                    }
                    break;

                case 27: // escape
                    if (self.active_) {
                        e.preventDefault();
                        self.deactivate(true);
                        return false;
                    }
                    break;

                default:
                    self.activate();

            }
        });

        /**
         * Finish on blur event
         * Use a timeout because instant blur gives race conditions
         */
        var onBlurFunction = function () {
            self.deactivate(true);
        }
        $elem.blur(function () {
            if (self.finishOnBlur_) {
                self.finishTimeout_ = setTimeout(onBlurFunction, 200);
            }
        });
        /**
         * Catch a race condition on form submit
         */
        $elem.parents('form').on('submit', onBlurFunction);

    };

    /**
     * Position output DOM elements
     * @private
     */
    $.dnnAutocompleter.prototype.position = function () {
        var offset = this.dom.$elem.offset();
        var height = this.dom.$results.outerHeight();
        var totalHeight = $(window).outerHeight();
        var inputBottom = offset.top + this.dom.$elem.outerHeight();
        var bottomIfDown = inputBottom + height;
        // Set autocomplete results at the bottom of input
        var position = { top: inputBottom, left: offset.left };
        if (bottomIfDown > totalHeight) {
            // Try to set autocomplete results at the top of input
            var topIfUp = offset.top - height;
            if (topIfUp >= 0) {
                position.top = topIfUp;
            }
        }
        this.dom.$results.css(position);
    };

    /**
     * Read from cache
     * @private
     */
    $.dnnAutocompleter.prototype.cacheRead = function (filter) {
        var filterLength, searchLength, search, maxPos, pos;
        if (this.options.useCache) {
            filter = String(filter);
            filterLength = filter.length;
            if (this.options.matchSubset) {
                searchLength = 1;
            } else {
                searchLength = filterLength;
            }
            while (searchLength <= filterLength) {
                if (this.options.matchInside) {
                    maxPos = filterLength - searchLength;
                } else {
                    maxPos = 0;
                }
                pos = 0;
                while (pos <= maxPos) {
                    search = filter.substr(0, searchLength);
                    if (this.cacheData_[search] !== undefined) {
                        return this.cacheData_[search];
                    }
                    pos++;
                }
                searchLength++;
            }
        }
        return false;
    };

    /**
     * Write to cache
     * @private
     */
    $.dnnAutocompleter.prototype.cacheWrite = function (filter, data) {
        if (this.options.useCache) {
            if (this.cacheLength_ >= this.options.maxCacheLength) {
                this.cacheFlush();
            }
            filter = String(filter);
            if (this.cacheData_[filter] !== undefined) {
                this.cacheLength_++;
            }
            this.cacheData_[filter] = data;
            return this.cacheData_[filter];
        }
        return false;
    };

    /**
     * Flush cache
     * @public
     */
    $.dnnAutocompleter.prototype.cacheFlush = function () {
        this.cacheData_ = {};
        this.cacheLength_ = 0;
    };

    /**
     * Call hook
     * Note that all called hooks are passed the autocompleter object
     * @param {string} hook
     * @param data
     * @returns Result of called hook, false if hook is undefined
     */
    $.dnnAutocompleter.prototype.callHook = function (hook, data) {
        var f = this.options[hook];
        if (f && $.isFunction(f)) {
            return f(data, this);
        }
        return false;
    };

    /**
     * Set timeout to activate autocompleter
     */
    $.dnnAutocompleter.prototype.activate = function () {
        var self = this;
        if (this.keyTimeout_) {
            clearTimeout(this.keyTimeout_);
        }
        this.keyTimeout_ = setTimeout(function () {
            self.activateNow();
        }, this.options.delay);
    };

    /**
     * Activate autocompleter immediately
     */
    $.dnnAutocompleter.prototype.activateNow = function () {
        var value = this.beforeUseConverter(this.dom.$elem.val());
        if (value !== this.lastProcessedValue_ && value !== this.lastSelectedValue_) {
            this.fetchData(value);
        }
    };

    /**
     * Get autocomplete data for a given value
     * @param {string} value Value to base autocompletion on
     * @private
     */
    $.dnnAutocompleter.prototype.fetchData = function (value) {
        var self = this;
        var processResults = function (results, filter) {
            if (self.options.processData) {
                results = self.options.processData(results);
            }
            self.showResults(self.filterResults(results, filter), filter);
        };
        this.lastProcessedValue_ = value;
        if (value.length < this.options.minChars) {
            processResults([], value);
        } else if (this.options.data) {
            processResults(this.options.data, value);
        } else {
            this.fetchRemoteData(value, function (remoteData) {
                processResults(remoteData, value);
            });
        }
    };

    /**
     * Get remote autocomplete data for a given value
     * @param {string} filter The filter to base remote data on
     * @param {function} callback The function to call after data retrieval
     * @private
     */
    $.dnnAutocompleter.prototype.fetchRemoteData = function (filter, callback) {
        var data = this.cacheRead(filter);
        if (data) {
            callback(data);
        } else {
            var self = this;
            var ajaxCallback = function (data) {
                var parsed = false;
                if (data !== false) {
                    parsed = self.parseRemoteData(data);
                    self.cacheWrite(filter, parsed);
                }
                self.dom.$elem.removeClass(self.options.loadingClass);
                callback(parsed);
            };
            this.dom.$elem.addClass(this.options.loadingClass);
            var services = self.options.moduleId ? $.dnnSF(self.options.moduleId) : null;

            $.ajax({
                url: this.makeUrl(filter),
                beforeSend: services ? services.setModuleHeaders : null,
                success: ajaxCallback,
                error: function (jqXHR, textStatus, errorThrown) {
                    if ($.isFunction(self.options.onError)) {
                        self.options.onError(jqXHR, textStatus, errorThrown);
                    } else {
                        ajaxCallback(false);
                    }
                },
                type: 'GET',
                dataType: 'json',
                contentType: "application/json"
            });
        }
    };

    /**
     * Create or update an extra parameter for the remote request
     * @param {string} name Parameter name
     * @param {string} value Parameter value
     * @public
     */
    $.dnnAutocompleter.prototype.setExtraParam = function (name, value) {
        var index = $.trim(String(name));
        if (index) {
            if (!this.options.extraParams) {
                this.options.extraParams = {};
            }
            if (this.options.extraParams[index] !== value) {
                this.options.extraParams[index] = value;
                this.cacheFlush();
            }
        }
    };

    /**
     * Build the url for a remote request
     * If options.queryParamName === false, append query to url instead of using a GET parameter
     * @param {string} param The value parameter to pass to the backend
     * @returns {string} The finished url with parameters
     */
    $.dnnAutocompleter.prototype.makeUrl = function (param) {
        var self = this;
        var url = this.options.url;
        var params = {};
        params[this.options.queryParamName] = param;
        return makeUrl(url, params);
    };

    /**
     * Parse data received from server
     * @param remoteData Data received from remote server
     * @returns {array} Parsed data
     */
    $.dnnAutocompleter.prototype.parseRemoteData = function (remoteData) {
        var data = remoteData;
        if (typeof data['d'] != 'undefined') {
            data = $.parseJSON(data['d']);
        }
        return data;
    };

    /**
     * Filter result
     * @param {Object} result
     * @param {String} filter
     * @returns {boolean} Include this result
     * @private
     */
    $.dnnAutocompleter.prototype.filterResult = function (result, filter) {
        if (!result.value) {
            return false;
        }
        if (this.options.filterResults) {
            var pattern = this.matchStringConverter(filter);
            var testValue = this.matchStringConverter(result.value);
            if (!this.options.matchCase) {
                pattern = pattern.toLowerCase();
                testValue = testValue.toLowerCase();
            }
            var patternIndex = testValue.indexOf(pattern);
            if (this.options.matchInside) {
                return patternIndex > -1;
            } else {
                return patternIndex === 0;
            }
        }
        return true;
    };

    /**
     * Filter results
     * @param results
     * @param filter
     */
    $.dnnAutocompleter.prototype.filterResults = function (results, filter) {
        var filtered = [];
        var i, result;

        for (i = 0; i < results.length; i++) {
            result = sanitizeResult(results[i]);
            if (this.filterResult(result, filter)) {
                filtered.push(result);
            }
        }
        if (this.options.sortResults) {
            filtered = this.sortResults(filtered, filter);
        }
        if (this.options.maxItemsToShow > 0 && this.options.maxItemsToShow < filtered.length) {
            filtered.length = this.options.maxItemsToShow;
        }
        return filtered;
    };

    /**
     * Sort results
     * @param results
     * @param filter
     */
    $.dnnAutocompleter.prototype.sortResults = function (results, filter) {
        var self = this;
        var sortFunction = this.options.sortFunction;
        if (!$.isFunction(sortFunction)) {
            sortFunction = function (a, b, f) {
                return sortValueAlpha(a, b, self.options.matchCase);
            };
        }
        results.sort(function (a, b) {
            return sortFunction(a, b, filter, self.options);
        });
        return results;
    };

    /**
     * Convert string before matching
     * @param s
     * @param a
     * @param b
     */
    $.dnnAutocompleter.prototype.matchStringConverter = function (s, a, b) {
        var converter = this.options.matchStringConverter;
        if ($.isFunction(converter)) {
            s = converter(s, a, b);
        }
        return s;
    };

    /**
     * Convert string before use
     * @param s
     * @param a
     * @param b
     */
    $.dnnAutocompleter.prototype.beforeUseConverter = function (s, a, b) {
        s = this.getValue();
        var converter = this.options.beforeUseConverter;
        if ($.isFunction(converter)) {
            s = converter(s, a, b);
        }
        return s;
    };

    /**
     * Enable finish on blur event
     */
    $.dnnAutocompleter.prototype.enableFinishOnBlur = function () {
        this.finishOnBlur_ = true;
    };

    /**
     * Disable finish on blur event
     */
    $.dnnAutocompleter.prototype.disableFinishOnBlur = function () {
        this.finishOnBlur_ = false;
    };

    /**
     * Create a results item (LI element) from a result
     * @param result
     */
    $.dnnAutocompleter.prototype.createItemFromResult = function (result) {
        var self = this;
        var $li = $('<li>' + this.showResult(result.value, result.data) + '</li>');
        $li.data({ value: result.value, data: result.data })
            .click(function () {
                self.selectItem($li);
            })
            .mousedown(self.disableFinishOnBlur)
            .mouseup(self.enableFinishOnBlur)
        ;
        return $li;
    };

    /**
     * Get all items from the results list
     * @param result
     */
    $.dnnAutocompleter.prototype.getItems = function () {
        return $('>ul>li', this.dom.$results);
    };

    /**
     * Show all results
     * @param results
     * @param filter
     */
    $.dnnAutocompleter.prototype.showResults = function (results, filter) {
        var numResults = results.length;
        var self = this;
        var $ul = $('<ul></ul>');
        var i, result, $li, autoWidth, first = false, $first = false;

        if (numResults) {
            for (i = 0; i < numResults; i++) {
                result = results[i];
                $li = this.createItemFromResult(result);
                $ul.append($li);
                if (first === false) {
                    first = String(result.value);
                    $first = $li;
                    $li.addClass(this.options.firstItemClass);
                }
                if (i === numResults - 1) {
                    $li.addClass(this.options.lastItemClass);
                }
            }

            this.dom.$results.html($ul).show();

            // Always recalculate position since window size or
            // input element location may have changed.
            this.position();
            if (this.options.autoWidth) {
                autoWidth = this.dom.$elem.outerWidth() - this.dom.$results.outerWidth() + this.dom.$results.width();
                this.dom.$results.css(this.options.autoWidth, autoWidth);
            }
            this.getItems().hover(
                function () { self.focusItem(this); },
                function () { /* void */ }
            );
            if (this.autoFill(first, filter) || this.options.selectFirst || (this.options.selectOnly && numResults === 1)) {
                this.focusItem($first);
            }
            this.active_ = true;
        } else {
            this.hideResults();
            this.active_ = false;
        }
    };

    $.dnnAutocompleter.prototype.showResult = function (value, data) {
        if ($.isFunction(this.options.showResult)) {
            return this.options.showResult(value, data);
        } else {
            return value;
        }
    };

    $.dnnAutocompleter.prototype.autoFill = function (value, filter) {
        var lcValue, lcFilter, valueLength, filterLength;
        if (this.options.autoFill && this.lastKeyPressed_ !== 8) {
            lcValue = String(value).toLowerCase();
            lcFilter = String(filter).toLowerCase();
            valueLength = value.length;
            filterLength = filter.length;
            if (lcValue.substr(0, filterLength) === lcFilter) {
                var d = this.getDelimiterOffsets();
                var pad = d.start ? ' ' : ''; // if there is a preceding delimiter
                this.setValue(pad + value);
                var start = filterLength + d.start + pad.length;
                var end = valueLength + d.start + pad.length;
                this.selectRange(start, end);
                return true;
            }
        }
        return false;
    };

    $.dnnAutocompleter.prototype.focusNext = function () {
        this.focusMove(+1);
    };

    $.dnnAutocompleter.prototype.focusPrev = function () {
        this.focusMove(-1);
    };

    $.dnnAutocompleter.prototype.focusMove = function (modifier) {
        var $items = this.getItems();
        modifier = sanitizeInteger(modifier, 0);
        if (modifier) {
            for (var i = 0; i < $items.length; i++) {
                if ($($items[i]).hasClass(this.selectClass_)) {
                    this.focusItem(i + modifier);
                    return;
                }
            }
        }
        this.focusItem(0);
    };

    $.dnnAutocompleter.prototype.focusItem = function (item) {
        var $item, $items = this.getItems();
        if ($items.length) {
            $items.removeClass(this.selectClass_).removeClass(this.options.selectClass);
            if (typeof item === 'number') {
                if (item < 0) {
                    item = 0;
                } else if (item >= $items.length) {
                    item = $items.length - 1;
                }
                $item = $($items[item]);
            } else {
                $item = $(item);
            }
            if ($item) {
                $item.addClass(this.selectClass_).addClass(this.options.selectClass);
            }
        }
    };

    $.dnnAutocompleter.prototype.selectCurrent = function () {
        var $item = $('li.' + this.selectClass_, this.dom.$results);
        if ($item.length === 1) {
            this.selectItem($item);
        } else {
            this.deactivate(false);
        }
    };

    $.dnnAutocompleter.prototype.selectItem = function ($li) {
        var value = $li.data('value');
        var data = $li.data('data');
        var displayValue = this.displayValue(value, data);
        var processedDisplayValue = this.beforeUseConverter(displayValue);
        this.lastProcessedValue_ = processedDisplayValue;
        this.lastSelectedValue_ = processedDisplayValue;
        var d = this.getDelimiterOffsets();
        var delimiter = this.options.delimiterChar;
        var elem = this.dom.$elem;
        var extraCaretPos = 0;
        if (this.options.useDelimiter) {
            // if there is a preceding delimiter, add a space after the delimiter
            if (elem.val().substring(d.start - 1, d.start) == delimiter && delimiter != ' ') {
                displayValue = ' ' + displayValue;
            }
            // if there is not already a delimiter trailing this value, add it
            if (elem.val().substring(d.end, d.end + 1) != delimiter && this.lastKeyPressed_ != this.options.delimiterKeyCode) {
                displayValue = displayValue + delimiter;
            } else {
                // move the cursor after the existing trailing delimiter
                extraCaretPos = 1;
            }
        }
        this.setValue(displayValue);
        this.setCaret(d.start + displayValue.length + extraCaretPos);
        this.callHook('onItemSelect', { value: value, data: data });
        this.deactivate(true);
        elem.trigger('result', value);
    };

    $.dnnAutocompleter.prototype.displayValue = function (value, data) {
        if ($.isFunction(this.options.displayValue)) {
            return this.options.displayValue(value, data);
        }
        return value;
    };

    $.dnnAutocompleter.prototype.hideResults = function () {
        this.dom.$results.hide();
    };

    $.dnnAutocompleter.prototype.deactivate = function (finish) {
        if (this.finishTimeout_) {
            clearTimeout(this.finishTimeout_);
        }
        if (this.keyTimeout_) {
            clearTimeout(this.keyTimeout_);
        }
        if (finish) {
            if (this.lastProcessedValue_ !== this.lastSelectedValue_) {
                if (this.options.mustMatch) {
                    this.setValue('');
                }
                this.callHook('onNoMatch');
            }
            if (this.active_) {
                this.callHook('onFinish');
            }
            this.lastKeyPressed_ = null;
            this.lastProcessedValue_ = null;
            this.lastSelectedValue_ = null;
            this.active_ = false;
        }
        this.hideResults();
    };

    $.dnnAutocompleter.prototype.selectRange = function (start, end) {
        var input = this.dom.$elem.get(0);
        if (input.setSelectionRange) {
            input.focus();
            input.setSelectionRange(start, end);
        } else if (input.createTextRange) {
            var range = input.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
    };

    /**
     * Move caret to position
     * @param {Number} pos
     */
    $.dnnAutocompleter.prototype.setCaret = function (pos) {
        this.selectRange(pos, pos);
    };

    /**
     * Get caret position
     */
    $.dnnAutocompleter.prototype.getCaret = function () {
        var elem = this.dom.$elem;
        if ($.browser.msie) {
            // ie
            var selection = document.selection;
            if (elem[0].tagName.toLowerCase() != 'textarea') {
                var val = elem.val();
                var range = selection.createRange().duplicate();
                range.moveEnd('character', val.length);
                var s = (range.text == '' ? val.length : val.lastIndexOf(range.text));
                range = selection.createRange().duplicate();
                range.moveStart('character', -val.length);
                var e = range.text.length;
            } else {
                var range = selection.createRange();
                var stored_range = range.duplicate();
                stored_range.moveToElementText(elem[0]);
                stored_range.setEndPoint('EndToEnd', range);
                var s = stored_range.text.length - range.text.length;
                var e = s + range.text.length;
            }
        } else {
            // ff, chrome, safari
            var s = elem[0].selectionStart;
            var e = elem[0].selectionEnd;
        }
        return {
            start: s,
            end: e
        };
    };

    /**
     * Set the value that is currently being autocompleted
     * @param {String} value
     */
    $.dnnAutocompleter.prototype.setValue = function (value) {
        if (this.options.useDelimiter) {
            // set the substring between the current delimiters
            var val = this.dom.$elem.val();
            var d = this.getDelimiterOffsets();
            var preVal = val.substring(0, d.start);
            var postVal = val.substring(d.end);
            value = preVal + value + postVal;
        }
        this.dom.$elem.val(value).blur();
    };

    /**
     * Get the value currently being autocompleted
     * @param {String} value
     */
    $.dnnAutocompleter.prototype.getValue = function () {
        var val = this.dom.$elem.val();
        if (this.options.useDelimiter) {
            var d = this.getDelimiterOffsets();
            return val.substring(d.start, d.end).trim();
        } else {
            return val;
        }
    };

    /**
     * Get the offsets of the value currently being autocompleted
     */
    $.dnnAutocompleter.prototype.getDelimiterOffsets = function () {
        var val = this.dom.$elem.val();
        if (this.options.useDelimiter) {
            var preCaretVal = val.substring(0, this.getCaret().start);
            var start = preCaretVal.lastIndexOf(this.options.delimiterChar) + 1;
            var postCaretVal = val.substring(this.getCaret().start);
            var end = postCaretVal.indexOf(this.options.delimiterChar);
            if (end == -1) end = val.length;
            end += this.getCaret().start;
        } else {
            start = 0;
            end = val.length;
        }
        return {
            start: start,
            end: end
        };
    };

})(jQuery);

(function ($) {
    // dnn customized tags
    var delimiter = new Array();
    var tags_callbacks = new Array();
    $.fn.dnnDoAutosize = function (o) {
        var minWidth = $(this).data('minwidth'),
            maxWidth = $(this).data('maxwidth'),
            val = '',
            input = $(this),
            testSubject = $('#' + $(this).data('tester_id'));

        if (val === (val = input.val())) { return; }

        // Enter new content into testSubject
        var escaped = val.replace(/&/g, '&amp;').replace(/\s/g, ' ').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        testSubject.html(escaped);
        // Calculate new width + whether to change
        var testerWidth = testSubject.width(),
            newWidth = (testerWidth + o.comfortZone) >= minWidth ? testerWidth + o.comfortZone : minWidth,
            currentWidth = input.width(),
            isValidWidthChange = (newWidth < currentWidth && newWidth >= minWidth)
                                 || (newWidth > minWidth && newWidth < maxWidth);

        // Animate width
        if (isValidWidthChange) {
            input.width(newWidth);
        }
    };

    $.fn.dnnResetAutosize = function (options) {
        // alert(JSON.stringify(options));
        var minWidth = $(this).data('minwidth') || options.minInputWidth || $(this).width(),
            maxWidth = $(this).data('maxwidth') || options.maxInputWidth || ($(this).closest('.dnnTagsInput').width() - options.inputPadding),
            val = '',
            input = $(this),
            testSubject = $('<tester/>').css({
                position: 'absolute',
                top: -9999,
                left: -9999,
                width: 'auto',
                fontSize: input.css('fontSize'),
                fontFamily: input.css('fontFamily'),
                fontWeight: input.css('fontWeight'),
                letterSpacing: input.css('letterSpacing'),
                whiteSpace: 'nowrap'
            }),
            testerId = $(this).attr('id') + '_autosize_tester';
        if (!$('#' + testerId).length > 0) {
            testSubject.attr('id', testerId);
            testSubject.appendTo('body');
        }

        input.data('minwidth', minWidth);
        input.data('maxwidth', maxWidth);
        input.data('tester_id', testerId);
        input.css('width', minWidth);
    };

    $.fn.dnnAddTag = function (value, options) {
        options = jQuery.extend({ focus: false, callback: true }, options);
        this.each(function () {
            var id = $(this).attr('id');

            var tagslist = $(this).val().split(delimiter[id]);
            if (tagslist[0] == '') {
                tagslist = new Array();
            }

            value = jQuery.trim(value);

            if (options.unique) {
                var skipTag = $(this).dnnTagExist(value);
                if (skipTag == true) {
                    //Marks fake input as not_valid to let styling it
                    $('#' + id + '_tag').addClass('dnnTagsInvalid');
                }
            } else {
                var skipTag = false;
            }

            if (value != '' && skipTag != true) {
                $('<span>').addClass('tag').append(
                    $('<span>').text(value).append('&nbsp;&nbsp;'),
                    $('<a>', {
                        href: '#',
                        title: 'Removing tag'
                    }).click(function () {
                        return $('#' + id).dnnRemoveTag(escape(value));
                    })
                ).insertBefore('#' + id + '_addTag');

                tagslist.push(value);

                $('#' + id + '_tag').val('');
                if (options.focus) {
                    $('#' + id + '_tag').focus();
                } else {
                    $('#' + id + '_tag').blur();
                }

                $.fn.dnnTagsInput.updateTagsField(this, tagslist);

                if (options.callback && tags_callbacks[id] && tags_callbacks[id]['onAddTag']) {
                    var f = tags_callbacks[id]['onAddTag'];
                    f.call(this, value);
                }
                if (tags_callbacks[id] && tags_callbacks[id]['onChange']) {
                    var i = tagslist.length;
                    var f = tags_callbacks[id]['onChange'];
                    f.call(this, $(this), tagslist[i - 1]);
                }
            }

        });

        return false;
    };

    $.fn.dnnRemoveTag = function (value) {
        value = unescape(value);
        this.each(function () {
            var id = $(this).attr('id');

            var old = $(this).val().split(delimiter[id]);

            $('#' + id + '_tagsinput .tag').remove();
            str = '';
            for (i = 0; i < old.length; i++) {
                if (old[i] != value) {
                    str = str + delimiter[id] + old[i];
                }
            }

            $.fn.dnnTagsInput.importTags(this, str);

            if (tags_callbacks[id] && tags_callbacks[id]['onRemoveTag']) {
                var f = tags_callbacks[id]['onRemoveTag'];
                f.call(this, value);
            }
        });

        return false;
    };

    $.fn.dnnTagExist = function (val) {
        var id = $(this).attr('id');
        var tagslist = $(this).val().split(delimiter[id]);
        return (jQuery.inArray(val, tagslist) >= 0); //true when tag exists, false when not
    };

    // clear all existing tags and import new ones from a string
    $.fn.dnnImportTags = function (str) {
        id = $(this).attr('id');
        $('#' + id + '_tagsinput .tag').remove();
        $.fn.dnnTagsInput.importTags(this, str);
    };

    $.fn.dnnTagsInput = function (options) {

        var settings = jQuery.extend({
            interactive: true,
            defaultText: 'add a tag',
            minChars: 0,
            width: '45%',
            height: '28px',
            autocomplete: { selectFirst: false },
            'hide': true,
            'delimiter': ',',
            'unique': true,
            removeWithBackspace: true,
            placeholderColor: '#666666',
            autosize: true,
            comfortZone: 20,
            inputPadding: 6 * 2
        }, options);

        this.each(function () {
            if (settings.hide) {
                $(this).hide();
            }
            var id = $(this).attr('id');
            if (!id || delimiter[$(this).attr('id')]) {
                id = $(this).attr('id', 'tags' + new Date().getTime()).attr('id');
            }

            var data = jQuery.extend({
                pid: id,
                real_input: '#' + id,
                holder: '#' + id + '_tagsinput',
                input_wrapper: '#' + id + '_addTag',
                fake_input: '#' + id + '_tag'
            }, settings);

            delimiter[id] = data.delimiter;

            if (settings.onAddTag || settings.onRemoveTag || settings.onChange) {
                tags_callbacks[id] = new Array();
                tags_callbacks[id]['onAddTag'] = settings.onAddTag;
                tags_callbacks[id]['onRemoveTag'] = settings.onRemoveTag;
                tags_callbacks[id]['onChange'] = settings.onChange;
            }

            var markup = '<div id="' + id + '_tagsinput" class="dnnTagsInput"><div id="' + id + '_addTag">';

            if (settings.interactive) {
                markup = markup + '<input id="' + id + '_tag" value="" data-default="' + settings.defaultText + '" />';
            }

            markup = markup + '</div><div class="dnnTagsClear"></div></div>';

            $(markup).insertAfter(this);

            $(data.holder).css({
                'width': settings.width,
                'min-height': settings.height,
                'height': '100%',
                'overflow': 'hidden'
            });

            if ($(data.real_input).val() != '') {
                $.fn.dnnTagsInput.importTags($(data.real_input), $(data.real_input).val());
            }
            if (settings.interactive) {
                $(data.fake_input).val($(data.fake_input).attr('data-default'));
                $(data.fake_input).css('color', settings.placeholderColor);
                $(data.fake_input).dnnResetAutosize(settings);

                $(data.holder).bind('click', data, function (event) {
                    $(event.data.real_input).triggerHandler('focus');
                    $(event.data.fake_input).triggerHandler('focus');
                });

                $(data.fake_input).bind('focus', data, function (event) {
                    if ($(event.data.fake_input).val() == $(event.data.fake_input).attr('data-default')) {
                        $(event.data.fake_input).val('');
                    }
                    $(event.data.fake_input).css('color', '#000000');
                });

                if (settings.autocomplete_url != undefined) {
                    autocomplete_options = { source: settings.autocomplete_url };
                    for (attrname in settings.autocomplete) {
                        autocomplete_options[attrname] = settings.autocomplete[attrname];
                    }

                    if ($.dnnAutocompleter !== undefined) {
                        $(data.fake_input).dnnAutocomplete(settings.autocomplete_url, settings.autocomplete);
                        $(data.fake_input).bind('result', data, function (event, data, formatted) {
                            if (data) {
                                $('#' + id).dnnAddTag(data, { focus: true, unique: (settings.unique) });
                            }
                        });
                    }
                } else {
                    // if a user tabs out of the field, create a new tag
                    // this is only available if autocomplete is not used.
                    $(data.fake_input).bind('blur', data, function (event) {
                        var d = $(this).attr('data-default');
                        if ($(event.data.fake_input).val() != '' && $(event.data.fake_input).val() != d) {
                            if ((event.data.minChars <= $(event.data.fake_input).val().length) && (!event.data.maxChars || (event.data.maxChars >= $(event.data.fake_input).val().length)))
                                $(event.data.real_input).dnnAddTag($(event.data.fake_input).val(), { focus: true, unique: (settings.unique) });
                        } else {
                            $(event.data.fake_input).val($(event.data.fake_input).attr('data-default'));
                            $(event.data.fake_input).css('color', settings.placeholderColor);
                        }
                        return false;
                    });

                }
                // if user types a comma, create a new tag
                $(data.fake_input).bind('keypress', data, function (event) {
                    if (event.which == event.data.delimiter.charCodeAt(0) || event.which == 13) {
                        event.preventDefault();
                        if ((event.data.minChars <= $(event.data.fake_input).val().length) && (!event.data.maxChars || (event.data.maxChars >= $(event.data.fake_input).val().length)))
                            $(event.data.real_input).dnnAddTag($(event.data.fake_input).val(), { focus: true, unique: (settings.unique) });
                        $(event.data.fake_input).dnnResetAutosize(settings);
                        return false;
                    } else if (event.data.autosize) {
                        $(event.data.fake_input).dnnDoAutosize(settings);

                    }
                });
                //Delete last tag on backspace
                data.removeWithBackspace && $(data.fake_input).bind('keydown', function (event) {
                    if (event.keyCode == 8 && $(this).val() == '') {
                        event.preventDefault();
                        var last_tag = $(this).closest('.dnnTagsInput').find('.tag:last').text();
                        var id = $(this).attr('id').replace(/_tag$/, '');
                        last_tag = last_tag.replace(/[\s]+$/, '');
                        $('#' + id).dnnRemoveTag(escape(last_tag));
                        $(this).trigger('focus');
                    }
                });
                $(data.fake_input).blur();

                //Removes the not_valid class when user changes the value of the fake input
                if (data.unique) {
                    $(data.fake_input).keydown(function (event) {
                        if (event.keyCode == 8 || String.fromCharCode(event.which).match(/\w+|[áéíóúÁÉÍÓÚñÑ,/]+/)) {
                            $(this).removeClass('dnnTagsInvalid');
                        }
                    });
                }
            } // if settings.interactive
        });

        return this;
    };

    $.fn.dnnTagsInput.updateTagsField = function (obj, tagslist) {
        var id = $(obj).attr('id');
        $(obj).val(tagslist.join(delimiter[id]));
    };

    $.fn.dnnTagsInput.importTags = function (obj, val) {
        $(obj).val('');
        var id = $(obj).attr('id');
        var tags = val.split(delimiter[id]);
        for (i = 0; i < tags.length; i++) {
            $(obj).dnnAddTag(tags[i], { focus: false, callback: false });
        }
        if (tags_callbacks[id] && tags_callbacks[id]['onChange']) {
            var f = tags_callbacks[id]['onChange'];
            f.call(obj, obj, tags[i]);
        }
    };

})(jQuery);

(function ($) {
    // dnnForm customised client side validation
    $.fn.toggleErrorMessage = function (options) {
        var defaultOptions = {
            errorMessage: "Error message",
            errorCls: "dnnFormError",
            show: true,
            removeErrorMessage: true
        };

        options = $.extend(defaultOptions, options);

        return this.each(function () {
            var dnnFormItem = $(this).closest('.dnnFormItem');
            if (options.show) {
                var errorSpan = dnnFormItem.find('span.dnnFormMessage.' + options.errorCls);
                if (errorSpan.length) {
                    errorSpan.html(options.errorMessage);
                } else {
                    errorSpan = $('<span class="dnnFormMessage ' + options.errorCls + '">' + options.errorMessage + '</span>');
                    dnnFormItem.append(errorSpan);
                }

                if (this.tagName.toLowerCase() == 'div') {
                    // customised controls
                    if ($(this).hasClass('RadComboBox')) {
                        // RadComboBox
                        $(this).addClass('dnnError');
                    }
                }
                else {
                    if ($(this).parent().hasClass('RadPicker')) {
                        // RadDate Picker
                        $(this).parent().find('input.riTextBox').css('border', '1px solid red');
                    }
                    else if ($(this).hasClass('dnnSpinnerInput')) {
                        // Spinner
                        $(this).parent().css('border', '1px solid red');
                    }
                    else {
                        // normal ctrl
                        $(this).css('border', '1px solid red');
                    }
                }
            }
            else {
                if (options.removeErrorMessage)
                    dnnFormItem.find('span.' + options.errorCls).remove();
                else
                    dnnFormItem.find('span.' + options.errorCls).hide();

                if (this.tagName.toLowerCase() == 'div') {
                    // customised controls
                    if ($(this).hasClass('RadComboBox')) {
                        // RadComboBox
                        $(this).removeClass('dnnError');

                    }
                }
                else {
                    if ($(this).hasClass('dnnSpinnerInput')) {
                        // Spinner
                        $(this).parent().css('border', '1px solid #ccc');
                    }
                    else {
                        // normal ctrl
                        $(this).css('border', '1px solid #ccc');
                    }
                }
            }
        });
    };
    $.fn.dnnFormSubmit = function (options) {
        var defaultOptions = {
            validates: []
        };

        options = $.extend(defaultOptions, options);

        return this.each(function () {
            $(this).click(function () {
                var formValidate = true;
                for (var i = 0; i < options.validates.length; i++) {
                    var ele = $('#' + options.validates[i].ele);
                    var func = options.validates[i].func;
                    if (ele.length) {
                        var eleVal = ele.val();
                        var eleError = func.call(ele.get(0), eleVal);
                        if (eleError) {
                            ele.toggleErrorMessage({ errorMessage: eleError, show: true });
                            formValidate = false;
                        }
                        else {
                            ele.toggleErrorMessage({ show: false });
                        }

                        //ele focus then remove error info
                        var hideErrorInfo = function () {
                            $(this).toggleErrorMessage({ show: false });
                        };

                        ele.unbind('focus', hideErrorInfo).bind('focus', hideErrorInfo);
                    }
                }

                return formValidate;
            });
        });
    };

    // fix telerik custom datepicker popup arrow position       
    $.dnnRadPickerHack = function (sender) {
        //when click this icon, hide error info
        var hideErrorInfo = function () {
            $(this).toggleErrorMessage({ show: false, removeErrorMessage: false });
        };

        var dnnRadPickerPopupFix = function () {
            if (!$.browser.msie || $.browser.version >= 9) {
                var id = $(this).attr('id');
                var popupId = id.replace('popupButton', 'calendar_wrapper');
                var popupElement = $('#' + popupId);
                var wrapperId = id.replace('popupButton', 'wrapper');
                var wrapperElement = $('#' + wrapperId);

                var popupElementTop = popupElement.parent().position().top;
                var wrapperElementTop = wrapperElement.offset().top;
                var popupTbl = popupElement.find('.RadCalendar_Default');
                var nextEle = popupTbl.next();
                if (nextEle.hasClass('RadCalendar_Default_PopupArrow_Down') || nextEle.hasClass('RadCalendar_Default_PopupArrow_Up'))
                    nextEle.remove();

                if (popupElementTop < wrapperElementTop) { // popup above the calendar ctrl, so show arrow down
                    popupTbl.after('<div class="RadCalendar_Default_PopupArrow_Down"></div>');
                }
                else { // popup below the calendar ctrl, so show arrow up
                    popupTbl.after('<div class="RadCalendar_Default_PopupArrow_Up"></div>');
                }
            }

            $(this).toggleErrorMessage({ show: false });
        };


        $('.RadPicker_Default a.rcCalPopup').unbind('click', dnnRadPickerPopupFix).bind('click', dnnRadPickerPopupFix);
        $('.RadPicker_Default .riTextBox').unbind('focus', hideErrorInfo).bind('focus', hideErrorInfo);
    };

    // remove combobox inline style
    $.dnnComboBoxLoaded = function (sender) {
        if (sender.constructor.__typeName == "Telerik.Web.UI.RadComboBox") {
            $(sender._inputDomElement).closest(".RadComboBox").removeAttr("style");
        }
    };

    //fix combobox hide error info
    $.dnnComboBoxHack = function (sender) {
        $(('#' + sender._clientStateFieldID).replace('_ClientState', '')).toggleErrorMessage({ show: false, removeErrorMessage: false });
    };

    //fix combobox scroll
    $.dnnComboBoxScroll = function (sender) {
        if (!$.browser.msie || $.browser.version >= 9) {
            $(('#' + sender._clientStateFieldID + ' .rcbScroll').replace('ClientState', 'DropDown')).jScrollPane();
        }
    };

    // fix grid issues 
    $.dnnGridCreated = function (sender) {
        var clientID = sender.ClientID;
        var $grid = $('#' + clientID);
        $('input.rgSortDesc, input.rgSortAsc', $grid).click(function () {
            var href = $(this).parent().find('a').get(0).href;
            window.location = href;
            return false;
        });

        if ($grid.hasClass('dnnTooltipGrid')) {
            $grid.dnnHelperTipDestroy();
            $('.rgRow, .rgAltRow', $grid).each(function () {
                var info = "Here is some text will show up and explian more about this information";
                $(this).dnnHelperTip({ helpContent: info, holderId: clientID });
            });
        }

        var grid = $find(clientID);

        // fix a bug while using customised checkbox - remove onclick event then attach onchange
        var headerCheck = $('.rgCheck', $grid);
        if (headerCheck.length) {
            headerCheck.each(function () {
                var checkbox = $(this).find('input[type="checkbox"]').get(0);
                var onclick = checkbox.onclick;
                checkbox.onchange = onclick;
                checkbox.onclick = null;
            });

            // fix when use customised scrollbar and customised checkbox, the row cannot be correctly selected
            $('.rgDataDiv input[type="checkbox"]', $grid).change(function () {
                var masterTable = grid.get_masterTableView();
                var rowIndex = $(this).closest('tr').get(0).rowIndex;
                var checked = this.checked;
                if (checked)
                    masterTable.selectItem(rowIndex);
                else
                    masterTable.deselectItem(rowIndex);
            });
        }

        // initialize dnnGrid customised scrollbar
        $('.rgDataDiv').each(function () {
            var $this = $(this);
            var ele = $this.get(0);
            ele.scrollPane = $this.jScrollPane();
            if (window.__rgDataDivScrollTopPersistArray && window.__rgDataDivScrollTopPersistArray.length) {
                var y = window.__rgDataDivScrollTopPersistArray.pop();
                var api = ele.scrollPane.data('jsp');
                api.scrollToY(y);
            }
        });

    };

})(jQuery);

(function ($) {

    var types = ['DOMMouseScroll', 'mousewheel'];

    if ($.event.fixHooks) {
        for (var i = types.length; i;) {
            $.event.fixHooks[types[--i]] = $.event.mouseHooks;
        }
    }

    $.event.special.mousewheel = {
        setup: function () {
            if (this.addEventListener) {
                for (var i = types.length; i;) {
                    this.addEventListener(types[--i], handler, false);
                }
            } else {
                this.onmousewheel = handler;
            }
        },

        teardown: function () {
            if (this.removeEventListener) {
                for (var i = types.length; i;) {
                    this.removeEventListener(types[--i], handler, false);
                }
            } else {
                this.onmousewheel = null;
            }
        }
    };

    $.fn.extend({
        mousewheel: function (fn) {
            return fn ? this.bind("mousewheel", fn) : this.trigger("mousewheel");
        },

        unmousewheel: function (fn) {
            return this.unbind("mousewheel", fn);
        }
    });


    function handler(event) {
        var orgEvent = event || window.event, args = [].slice.call(arguments, 1), delta = 0, returnValue = true, deltaX = 0, deltaY = 0;
        event = $.event.fix(orgEvent);
        event.type = "mousewheel";

        // Old school scrollwheel delta
        if (orgEvent.wheelDelta) { delta = orgEvent.wheelDelta / 120; }
        if (orgEvent.detail) { delta = -orgEvent.detail / 3; }

        // New school multidimensional scroll (touchpads) deltas
        deltaY = delta;

        // Gecko
        if (orgEvent.axis !== undefined && orgEvent.axis === orgEvent.HORIZONTAL_AXIS) {
            deltaY = 0;
            deltaX = -1 * delta;
        }

        // Webkit
        if (orgEvent.wheelDeltaY !== undefined) { deltaY = orgEvent.wheelDeltaY / 120; }
        if (orgEvent.wheelDeltaX !== undefined) { deltaX = -1 * orgEvent.wheelDeltaX / 120; }

        // Add event and delta to the front of the arguments
        args.unshift(event, delta, deltaX, deltaY);

        return ($.event.dispatch || $.event.handle).apply(this, args);
    }

})(jQuery);


(function ($) {
    $.fn.dnnFileInput = function () {
        return this.each(function () {
            var $ctrl = $(this);
            if (this.wrapper)
                return;

            //if this.wrapper is undefined, then we check if parent node is a wrapper
            if (this.parentNode && this.parentNode.tagName.toLowerCase() == 'span' && this.parentNode.className == 'dnnInputFileWrapper') {
                return;
            }

            this.wrapper = $("<span class='dnnInputFileWrapper'></span>");
            $ctrl.wrap(this.wrapper);
            var text = $ctrl.data('text');
            text = text || 'Choose File';
            var btn = $("<span class='dnnSecondaryAction'>" + text + "</span>");
            btn.insertBefore($ctrl);

            $ctrl.change(function () {
                var val = $(this).val();
                if (val != '') {
                    var lastIdx = val.lastIndexOf('\\') + 1;
                    val = val.substring(lastIdx, val.length);
                } else {
                    val = text;
                }
                $(this).prev().html(val);
            });
        });
    };
})(jQuery);


(function ($) {
    var supportAjaxUpload = function () {
        var xhr = new XMLHttpRequest;
        return !!(xhr && ('upload' in xhr) && ('onprogress' in xhr.upload));
    };
    $.fn.dnnFileUpload = function (settings) {

        return this.each(function () {
            // set scope and settings, service
            var scope = $(this).attr('id');
            dnn.dnnFileUpload.setSettings(scope, settings);
            var service = $.dnnSF();

            // hide progress
            $('#' + settings.progressBarId).parent().hide();
            // detect draggable support or not
            var droppableSpan = $('#' + settings.dropZoneId + '>span');
            if ('draggable' in document.createElement('span')) {
                droppableSpan.show();
            }
            else {
                droppableSpan.hide();
            }

            // set file upload
            var url = service.getServiceRoot('internalservices') + 'fileupload/postfile';
            if (!supportAjaxUpload()) {
                var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
                url += '?__RequestVerificationToken=' + antiForgeryToken;
            }

            $('#' + scope + ' input[type="file"]').fileupload({
                url: url,
                beforeSend: service.setModuleHeaders,
                dropZone: $('#' + settings.dropZoneId),
                replaceFileInput: false,
                submit: function (e, data) {
                    data.formData = { folder: settings.folder, filter: settings.fileFilter };
                    return true;
                },
                progressall: function (e, data) {
                    var progress = parseInt(data.loaded / data.total * 100, 10);
                    if (progress < 100) {
                        $('#' + settings.progressBarId).parent().show();
                        $('#' + settings.progressBarId + '>div').css('width', progress + '%');
                    }
                    else
                        $('#' + settings.progressBarId).parent().hide();
                },
                done: function (e, data) {
                    $('#' + settings.progressBarId).parent().hide();
                    var img = new Image();
                    $(img).load(function () {
                        $('#' + settings.dropZoneId + ' img').remove();
                        $(img).css({ 'max-width': 180, 'max-height': 150 }).insertBefore($('#' + settings.dropZoneId + ' span'));
                    });
                    var src;
                    var testContent = $('pre', data.result);
                    if (testContent.length) {
                        src = testContent.text();
                    }
                    else
                        src = data.result;

                    if (src && $.trim(src)) {
                        img.src = src;

                        // set updated files into combo
                        var foldersCombo = $find(settings.foldersComboId);
                        var folderPath = foldersCombo.get_value();
                        url = service.getServiceRoot('internalservices') + 'fileupload/loadfiles';
                        $.ajax({
                            url: url,
                            type: 'POST',
                            data: { FolderPath: folderPath, FileFilter: settings.fileFilter, Required: false },
                            beforeSend: service.setModuleHeaders,
                            success: function (d) {
                                var combo = $find(settings.filesComboId);
                                combo.clearItems();
                                for (var i = 0; i < d.length; i++) {
                                    var txt = d[i].Text;
                                    var val = d[i].Value;

                                    var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                                    comboItem.set_text(txt);
                                    comboItem.set_value(val);
                                    combo.get_items().add(comboItem);
                                    if (src.indexOf(txt) > 0) {
                                        comboItem.select();
                                        $('#' + settings.fileIdId).val(val);
                                        var path = folderPath ? folderPath + '/' + txt : txt;
                                        $('#' + settings.filePathId).val(path);
                                    }
                                }
                            },
                            error: function () {
                            }
                        });
                    }
                }
            });

            // set initial thumb image
            setTimeout(function () {
                var filesCombo = $find(settings.filesComboId);
                var selectedFileId = filesCombo.get_value();
                url = service.getServiceRoot('internalservices') + 'fileupload/loadimage';
                if (selectedFileId) {
                    $.ajax({
                        url: url,
                        type: 'GET',
                        data: { fileId: selectedFileId },
                        success: function (d) {
                            var img = new Image();
                            $(img).load(function () {
                                $('#' + settings.dropZoneId + ' img').remove();
                                $(img).css({ 'max-width': 180, 'max-height': 150 }).insertBefore($('#' + settings.dropZoneId + ' span'));
                            });
                            img.src = d;
                        },
                        error: function () {
                        }
                    });
                }
            }, 500);

        });
    };

    if (typeof dnn === 'undefined') dnn = {};
    dnn.dnnFileUpload = dnn.dnnFileUpload || {};
    dnn.dnnFileUpload.settings = {};
    dnn.dnnFileUpload.setSettings = function (scope, settings) {
        dnn.dnnFileUpload.settings[scope] = settings;
    };
    dnn.dnnFileUpload.getSettings = function (sender) {
        var senderId = sender.get_id();
        var scope = $('#' + senderId).closest('.dnnFileUploadScope').attr('id');
        return dnn.dnnFileUpload.settings[scope];
    };
    dnn.dnnFileUpload.Folders_Changed = function (sender, e) {
        var settings = dnn.dnnFileUpload.getSettings(sender);
        if (!settings) return;

        var item = e.get_item();
        if (item) {
            var folderPath = item.get_value();
            settings.folder = folderPath;
            var service = $.dnnSF();
            var url = service.getServiceRoot('internalservices') + 'fileupload/loadfiles';
            $.ajax({
                url: url,
                type: 'POST',
                data: { FolderPath: folderPath, FileFilter: settings.fileFilter, Required: false },
                beforeSend: service.setModuleHeaders,
                success: function (d) {
                    var combo = $find(settings.filesComboId);
                    combo.clearItems();
                    for (var i = 0; i < d.length; i++) {
                        var txt = d[i].Text;
                        var val = d[i].Value;

                        var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text(txt);
                        comboItem.set_value(val);
                        combo.get_items().add(comboItem);
                        if (i == 0) {
                            comboItem.select();
                        }
                    }


                },
                error: function () {
                }
            });

        }
    };
    dnn.dnnFileUpload.Files_Changed = function (sender, e) {
        var settings = dnn.dnnFileUpload.getSettings(sender);
        if (!settings) return;

        var item = e.get_item();
        if (item) {
            var fileId = item.get_value();
            $('#' + settings.fileIdId).val(fileId);
            var fileName = item.get_text();
            var folderPath = settings.folder;
            var path = folderPath ? folderPath + '/' + fileName : fileName;
            $('#' + settings.filePathId).val(path);
            var service = $.dnnSF();
            var url = service.getServiceRoot('internalservices') + 'fileupload/loadimage';
            if (fileId) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    data: { fileId: fileId },
                    success: function (d) {
                        var img = new Image();
                        $(img).load(function () {
                            $('#' + settings.dropZoneId + ' img').remove();
                            $(img).css({ 'max-width': 180, 'max-height': 150 }).insertBefore($('#' + settings.dropZoneId + ' span'));
                        });
                        img.src = d;
                    },
                    error: function () {
                    }
                });
            }
            else
                $('#' + settings.dropZoneId + ' img').remove();
        }
    };

})(jQuery);

(function ($) {

    $.detectScroll = function () {
        var divs = document.getElementsByTagName('div');
        return $.grep(divs, function (n) {
            return $(n).hasClass('dnnScroll');
        });
    };

    /* BELOW jscrollPane code */
    $.fn.jScrollPane = function (settings) {
        // JScrollPane "class" - public methods are available through $('selector').data('jsp')
        function JScrollPane(elem, s) {
            var settings, jsp = this, pane, paneWidth, paneHeight, container, contentWidth, contentHeight,
				percentInViewH, percentInViewV, isScrollableV, isScrollableH, verticalDrag, dragMaxY,
				verticalDragPosition, horizontalDrag, dragMaxX, horizontalDragPosition,
				verticalBar, verticalTrack, scrollbarWidth, verticalTrackHeight, verticalDragHeight, arrowUp, arrowDown,
				horizontalBar, horizontalTrack, horizontalTrackWidth, horizontalDragWidth, arrowLeft, arrowRight,
				reinitialiseInterval, originalPadding, originalPaddingTotalWidth, previousContentWidth,
				wasAtTop = true, wasAtLeft = true, wasAtBottom = false, wasAtRight = false,
				originalElement = elem.clone(false, false).empty(),
				mwEvent = $.fn.mwheelIntent ? 'mwheelIntent.jsp' : 'mousewheel.jsp';

            originalPadding = elem.css('paddingTop') + ' ' +
								elem.css('paddingRight') + ' ' +
								elem.css('paddingBottom') + ' ' +
								elem.css('paddingLeft');
            originalPaddingTotalWidth = (parseInt(elem.css('paddingLeft'), 10) || 0) +
										(parseInt(elem.css('paddingRight'), 10) || 0);

            function initialise(s) {

                var /*firstChild, lastChild, */isMaintainingPositon, lastContentX, lastContentY,
						hasContainingSpaceChanged, originalScrollTop, originalScrollLeft,
						maintainAtBottom = false, maintainAtRight = false;

                settings = s;

                if (pane === undefined) {
                    originalScrollTop = elem.scrollTop();
                    originalScrollLeft = elem.scrollLeft();

                    elem.css(
						{
						    overflow: 'hidden',
						    padding: 0
						}
					);
                    // TODO: Deal with where width/ height is 0 as it probably means the element is hidden and we should
                    // come back to it later and check once it is unhidden...
                    paneWidth = elem.innerWidth() + originalPaddingTotalWidth;
                    paneHeight = elem.innerHeight();

                    elem.width(paneWidth);

                    pane = $('<div class="jspPane" />').css('padding', originalPadding).append(elem.children());
                    container = $('<div class="jspContainer" />')
						.css({
						    'width': paneWidth + 'px',
						    'height': paneHeight + 'px'
						}
					).append(pane).appendTo(elem);

                    /*
					// Move any margins from the first and last children up to the container so they can still
					// collapse with neighbouring elements as they would before jScrollPane 
					firstChild = pane.find(':first-child');
					lastChild = pane.find(':last-child');
					elem.css(
						{
							'margin-top': firstChild.css('margin-top'),
							'margin-bottom': lastChild.css('margin-bottom')
						}
					);
					firstChild.css('margin-top', 0);
					lastChild.css('margin-bottom', 0);
					*/
                } else {
                    elem.css('width', '');

                    maintainAtBottom = settings.stickToBottom && isCloseToBottom();
                    maintainAtRight = settings.stickToRight && isCloseToRight();

                    hasContainingSpaceChanged = elem.innerWidth() + originalPaddingTotalWidth != paneWidth || elem.outerHeight() != paneHeight;

                    if (hasContainingSpaceChanged) {
                        paneWidth = elem.innerWidth() + originalPaddingTotalWidth;
                        paneHeight = elem.innerHeight();
                        container.css({
                            width: paneWidth + 'px',
                            height: paneHeight + 'px'
                        });
                    }

                    // If nothing changed since last check...
                    if (!hasContainingSpaceChanged && previousContentWidth == contentWidth && pane.outerHeight() == contentHeight) {
                        elem.width(paneWidth);
                        return;
                    }
                    previousContentWidth = contentWidth;

                    pane.css('width', '');
                    elem.width(paneWidth);

                    container.find('>.jspVerticalBar,>.jspHorizontalBar').remove().end();
                }

                pane.css('overflow', 'auto');
                if (s.contentWidth) {
                    contentWidth = s.contentWidth;
                } else {
                    contentWidth = pane[0].scrollWidth;
                }
                contentHeight = pane[0].scrollHeight;
                pane.css('overflow', '');

                percentInViewH = contentWidth / paneWidth;
                percentInViewV = contentHeight / paneHeight;
                isScrollableV = percentInViewV > 1;

                isScrollableH = percentInViewH > 1;

                if (!(isScrollableH || isScrollableV)) {
                    elem.removeClass('jspScrollable');
                    pane.css({
                        top: 0,
                        width: container.width() - originalPaddingTotalWidth
                    });
                    removeMousewheel();
                    removeFocusHandler();
                    removeKeyboardNav();
                    removeClickOnTrack();
                } else {
                    elem.addClass('jspScrollable');

                    isMaintainingPositon = settings.maintainPosition && (verticalDragPosition || horizontalDragPosition);
                    if (isMaintainingPositon) {
                        lastContentX = contentPositionX();
                        lastContentY = contentPositionY();
                    }

                    initialiseVerticalScroll();
                    initialiseHorizontalScroll();
                    resizeScrollbars();

                    if (isMaintainingPositon) {
                        scrollToX(maintainAtRight ? (contentWidth - paneWidth) : lastContentX, false);
                        scrollToY(maintainAtBottom ? (contentHeight - paneHeight) : lastContentY, false);
                    }

                    initFocusHandler();
                    initMousewheel();
                    initTouch();

                    if (settings.enableKeyboardNavigation) {
                        initKeyboardNav();
                    }
                    if (settings.clickOnTrack) {
                        initClickOnTrack();
                    }

                    observeHash();
                    if (settings.hijackInternalLinks) {
                        hijackInternalLinks();
                    }
                }

                if (settings.autoReinitialise && !reinitialiseInterval) {
                    reinitialiseInterval = setInterval(
						function () {
						    initialise(settings);
						},
						settings.autoReinitialiseDelay
					);
                } else if (!settings.autoReinitialise && reinitialiseInterval) {
                    clearInterval(reinitialiseInterval);
                }

                originalScrollTop && elem.scrollTop(0) && scrollToY(originalScrollTop, false);
                originalScrollLeft && elem.scrollLeft(0) && scrollToX(originalScrollLeft, false);

                elem.trigger('jsp-initialised', [isScrollableH || isScrollableV]);
            }

            function initialiseVerticalScroll() {
                if (isScrollableV) {

                    container.append(
						$('<div class="jspVerticalBar" />').append(
							$('<div class="jspCap jspCapTop" />'),
							$('<div class="jspTrack" />').append(
								$('<div class="jspDrag" />').append(
									$('<div class="jspDragTop" />'),
									$('<div class="jspDragBottom" />')
								)
							),
							$('<div class="jspCap jspCapBottom" />')
						)
					);

                    verticalBar = container.find('>.jspVerticalBar');
                    verticalTrack = verticalBar.find('>.jspTrack');
                    verticalDrag = verticalTrack.find('>.jspDrag');

                    if (settings.showArrows) {
                        arrowUp = $('<a class="jspArrow jspArrowUp" />').bind(
							'mousedown.jsp', getArrowScroll(0, -1)
						).bind('click.jsp', nil);
                        arrowDown = $('<a class="jspArrow jspArrowDown" />').bind(
							'mousedown.jsp', getArrowScroll(0, 1)
						).bind('click.jsp', nil);
                        if (settings.arrowScrollOnHover) {
                            arrowUp.bind('mouseover.jsp', getArrowScroll(0, -1, arrowUp));
                            arrowDown.bind('mouseover.jsp', getArrowScroll(0, 1, arrowDown));
                        }

                        appendArrows(verticalTrack, settings.verticalArrowPositions, arrowUp, arrowDown);
                    }

                    verticalTrackHeight = paneHeight;
                    container.find('>.jspVerticalBar>.jspCap:visible,>.jspVerticalBar>.jspArrow').each(
						function () {
						    verticalTrackHeight -= $(this).outerHeight();
						}
					);


                    verticalDrag.hover(
						function () {
						    verticalDrag.addClass('jspHover');
						},
						function () {
						    verticalDrag.removeClass('jspHover');
						}
					).bind(
						'mousedown.jsp',
						function (e) {
						    // Stop IE from allowing text selection
						    $('html').bind('dragstart.jsp selectstart.jsp', nil);

						    verticalDrag.addClass('jspActive');

						    var startY = e.pageY - verticalDrag.position().top;

						    $('html').bind(
								'mousemove.jsp',
								function (e) {
								    positionDragY(e.pageY - startY, false);
								}
							).bind('mouseup.jsp mouseleave.jsp', cancelDrag);
						    return false;
						}
					);
                    sizeVerticalScrollbar();
                }
            }

            function sizeVerticalScrollbar() {
                verticalTrack.height(verticalTrackHeight + 'px');
                verticalDragPosition = 0;
                scrollbarWidth = settings.verticalGutter + verticalTrack.outerWidth();

                // Make the pane thinner to allow for the vertical scrollbar
                pane.width(paneWidth - scrollbarWidth - originalPaddingTotalWidth);

                // Add margin to the left of the pane if scrollbars are on that side (to position
                // the scrollbar on the left or right set it's left or right property in CSS)
                try {
                    if (verticalBar.position().left === 0) {
                        pane.css('margin-left', scrollbarWidth + 'px');
                    }
                } catch (err) {
                }
            }

            function initialiseHorizontalScroll() {
                if (isScrollableH) {

                    container.append(
						$('<div class="jspHorizontalBar" />').append(
							$('<div class="jspCap jspCapLeft" />'),
							$('<div class="jspTrack" />').append(
								$('<div class="jspDrag" />').append(
									$('<div class="jspDragLeft" />'),
									$('<div class="jspDragRight" />')
								)
							),
							$('<div class="jspCap jspCapRight" />')
						)
					);

                    horizontalBar = container.find('>.jspHorizontalBar');
                    horizontalTrack = horizontalBar.find('>.jspTrack');
                    horizontalDrag = horizontalTrack.find('>.jspDrag');

                    if (settings.showArrows) {
                        arrowLeft = $('<a class="jspArrow jspArrowLeft" />').bind(
							'mousedown.jsp', getArrowScroll(-1, 0)
						).bind('click.jsp', nil);
                        arrowRight = $('<a class="jspArrow jspArrowRight" />').bind(
							'mousedown.jsp', getArrowScroll(1, 0)
						).bind('click.jsp', nil);
                        if (settings.arrowScrollOnHover) {
                            arrowLeft.bind('mouseover.jsp', getArrowScroll(-1, 0, arrowLeft));
                            arrowRight.bind('mouseover.jsp', getArrowScroll(1, 0, arrowRight));
                        }
                        appendArrows(horizontalTrack, settings.horizontalArrowPositions, arrowLeft, arrowRight);
                    }

                    horizontalDrag.hover(
						function () {
						    horizontalDrag.addClass('jspHover');
						},
						function () {
						    horizontalDrag.removeClass('jspHover');
						}
					).bind(
						'mousedown.jsp',
						function (e) {
						    // Stop IE from allowing text selection
						    $('html').bind('dragstart.jsp selectstart.jsp', nil);

						    horizontalDrag.addClass('jspActive');

						    var startX = e.pageX - horizontalDrag.position().left;

						    $('html').bind(
								'mousemove.jsp',
								function (e) {
								    positionDragX(e.pageX - startX, false);
								}
							).bind('mouseup.jsp mouseleave.jsp', cancelDrag);
						    return false;
						}
					);
                    horizontalTrackWidth = container.innerWidth();
                    sizeHorizontalScrollbar();
                }
            }

            function sizeHorizontalScrollbar() {
                container.find('>.jspHorizontalBar>.jspCap:visible,>.jspHorizontalBar>.jspArrow').each(
					function () {
					    horizontalTrackWidth -= $(this).outerWidth();
					}
				);

                horizontalTrack.width(horizontalTrackWidth + 'px');
                horizontalDragPosition = 0;
            }

            function resizeScrollbars() {
                if (isScrollableH && isScrollableV) {
                    var horizontalTrackHeight = horizontalTrack.outerHeight(),
						verticalTrackWidth = verticalTrack.outerWidth();
                    verticalTrackHeight -= horizontalTrackHeight;
                    $(horizontalBar).find('>.jspCap:visible,>.jspArrow').each(
						function () {
						    horizontalTrackWidth += $(this).outerWidth();
						}
					);
                    horizontalTrackWidth -= verticalTrackWidth;
                    paneHeight -= verticalTrackWidth;
                    paneWidth -= horizontalTrackHeight;
                    horizontalTrack.parent().append(
						$('<div class="jspCorner" />').css('width', horizontalTrackHeight + 'px')
					);
                    sizeVerticalScrollbar();
                    sizeHorizontalScrollbar();
                }
                // reflow content
                if (isScrollableH) {
                    pane.width((container.outerWidth() - originalPaddingTotalWidth) + 'px');
                }
                contentHeight = pane.outerHeight();
                percentInViewV = contentHeight / paneHeight;

                if (isScrollableH) {
                    horizontalDragWidth = Math.ceil(1 / percentInViewH * horizontalTrackWidth);
                    if (horizontalDragWidth > settings.horizontalDragMaxWidth) {
                        horizontalDragWidth = settings.horizontalDragMaxWidth;
                    } else if (horizontalDragWidth < settings.horizontalDragMinWidth) {
                        horizontalDragWidth = settings.horizontalDragMinWidth;
                    }
                    horizontalDrag.width(horizontalDragWidth + 'px');
                    dragMaxX = horizontalTrackWidth - horizontalDragWidth;
                    _positionDragX(horizontalDragPosition); // To update the state for the arrow buttons
                }
                if (isScrollableV) {
                    verticalDragHeight = Math.ceil(1 / percentInViewV * verticalTrackHeight);
                    if (verticalDragHeight > settings.verticalDragMaxHeight) {
                        verticalDragHeight = settings.verticalDragMaxHeight;
                    } else if (verticalDragHeight < settings.verticalDragMinHeight) {
                        verticalDragHeight = settings.verticalDragMinHeight;
                    }
                    verticalDrag.height(verticalDragHeight + 'px');
                    dragMaxY = verticalTrackHeight - verticalDragHeight;
                    _positionDragY(verticalDragPosition); // To update the state for the arrow buttons
                }
            }

            function appendArrows(ele, p, a1, a2) {
                var p1 = "before", p2 = "after", aTemp;

                // Sniff for mac... Is there a better way to determine whether the arrows would naturally appear
                // at the top or the bottom of the bar?
                if (p == "os") {
                    p = /Mac/.test(navigator.platform) ? "after" : "split";
                }
                if (p == p1) {
                    p2 = p;
                } else if (p == p2) {
                    p1 = p;
                    aTemp = a1;
                    a1 = a2;
                    a2 = aTemp;
                }

                ele[p1](a1)[p2](a2);
            }

            function getArrowScroll(dirX, dirY, ele) {
                return function () {
                    arrowScroll(dirX, dirY, this, ele);
                    this.blur();
                    return false;
                };
            }

            function arrowScroll(dirX, dirY, arrow, ele) {
                arrow = $(arrow).addClass('jspActive');

                var eve,
					scrollTimeout,
					isFirst = true,
					doScroll = function () {
					    if (dirX !== 0) {
					        jsp.scrollByX(dirX * settings.arrowButtonSpeed);
					    }
					    if (dirY !== 0) {
					        jsp.scrollByY(dirY * settings.arrowButtonSpeed);
					    }
					    scrollTimeout = setTimeout(doScroll, isFirst ? settings.initialDelay : settings.arrowRepeatFreq);
					    isFirst = false;
					};

                doScroll();

                eve = ele ? 'mouseout.jsp' : 'mouseup.jsp';
                ele = ele || $('html');
                ele.bind(
					eve,
					function () {
					    arrow.removeClass('jspActive');
					    scrollTimeout && clearTimeout(scrollTimeout);
					    scrollTimeout = null;
					    ele.unbind(eve);
					}
				);
            }

            function initClickOnTrack() {
                removeClickOnTrack();
                if (isScrollableV) {
                    verticalTrack.bind(
						'mousedown.jsp',
						function (e) {
						    if (e.originalTarget === undefined || e.originalTarget == e.currentTarget) {
						        var clickedTrack = $(this),
									offset = clickedTrack.offset(),
									direction = e.pageY - offset.top - verticalDragPosition,
									scrollTimeout,
									isFirst = true,
									doScroll = function () {
									    var offset = clickedTrack.offset(),
											pos = e.pageY - offset.top - verticalDragHeight / 2,
											contentDragY = paneHeight * settings.scrollPagePercent,
											dragY = dragMaxY * contentDragY / (contentHeight - paneHeight);
									    if (direction < 0) {
									        if (verticalDragPosition - dragY > pos) {
									            jsp.scrollByY(-contentDragY);
									        } else {
									            positionDragY(pos);
									        }
									    } else if (direction > 0) {
									        if (verticalDragPosition + dragY < pos) {
									            jsp.scrollByY(contentDragY);
									        } else {
									            positionDragY(pos);
									        }
									    } else {
									        cancelClick();
									        return;
									    }
									    scrollTimeout = setTimeout(doScroll, isFirst ? settings.initialDelay : settings.trackClickRepeatFreq);
									    isFirst = false;
									},
									cancelClick = function () {
									    scrollTimeout && clearTimeout(scrollTimeout);
									    scrollTimeout = null;
									    $(document).unbind('mouseup.jsp', cancelClick);
									};
						        doScroll();
						        $(document).bind('mouseup.jsp', cancelClick);
						        return false;
						    }
						}
					);
                }

                if (isScrollableH) {
                    horizontalTrack.bind(
						'mousedown.jsp',
						function (e) {
						    if (e.originalTarget === undefined || e.originalTarget == e.currentTarget) {
						        var clickedTrack = $(this),
									offset = clickedTrack.offset(),
									direction = e.pageX - offset.left - horizontalDragPosition,
									scrollTimeout,
									isFirst = true,
									doScroll = function () {
									    var offset = clickedTrack.offset(),
											pos = e.pageX - offset.left - horizontalDragWidth / 2,
											contentDragX = paneWidth * settings.scrollPagePercent,
											dragX = dragMaxX * contentDragX / (contentWidth - paneWidth);
									    if (direction < 0) {
									        if (horizontalDragPosition - dragX > pos) {
									            jsp.scrollByX(-contentDragX);
									        } else {
									            positionDragX(pos);
									        }
									    } else if (direction > 0) {
									        if (horizontalDragPosition + dragX < pos) {
									            jsp.scrollByX(contentDragX);
									        } else {
									            positionDragX(pos);
									        }
									    } else {
									        cancelClick();
									        return;
									    }
									    scrollTimeout = setTimeout(doScroll, isFirst ? settings.initialDelay : settings.trackClickRepeatFreq);
									    isFirst = false;
									},
									cancelClick = function () {
									    scrollTimeout && clearTimeout(scrollTimeout);
									    scrollTimeout = null;
									    $(document).unbind('mouseup.jsp', cancelClick);
									};
						        doScroll();
						        $(document).bind('mouseup.jsp', cancelClick);
						        return false;
						    }
						}
					);
                }
            }

            function removeClickOnTrack() {
                if (horizontalTrack) {
                    horizontalTrack.unbind('mousedown.jsp');
                }
                if (verticalTrack) {
                    verticalTrack.unbind('mousedown.jsp');
                }
            }

            function cancelDrag() {
                $('html').unbind('dragstart.jsp selectstart.jsp mousemove.jsp mouseup.jsp mouseleave.jsp');

                if (verticalDrag) {
                    verticalDrag.removeClass('jspActive');
                }
                if (horizontalDrag) {
                    horizontalDrag.removeClass('jspActive');
                }
            }

            function positionDragY(destY, animate) {
                if (!isScrollableV) {
                    return;
                }
                if (destY < 0) {
                    destY = 0;
                } else if (destY > dragMaxY) {
                    destY = dragMaxY;
                }

                // can't just check if(animate) because false is a valid value that could be passed in...
                if (animate === undefined) {
                    animate = settings.animateScroll;
                }
                if (animate) {
                    jsp.animate(verticalDrag, 'top', destY, _positionDragY);
                } else {
                    verticalDrag.css('top', destY);
                    _positionDragY(destY);
                }

            }

            function _positionDragY(destY) {
                if (destY === undefined) {
                    destY = verticalDrag.position().top;
                }

                container.scrollTop(0);
                verticalDragPosition = destY;

                var isAtTop = verticalDragPosition === 0,
					isAtBottom = verticalDragPosition == dragMaxY,
					percentScrolled = destY / dragMaxY,
					destTop = -percentScrolled * (contentHeight - paneHeight);

                if (wasAtTop != isAtTop || wasAtBottom != isAtBottom) {
                    wasAtTop = isAtTop;
                    wasAtBottom = isAtBottom;
                    elem.trigger('jsp-arrow-change', [wasAtTop, wasAtBottom, wasAtLeft, wasAtRight]);
                }

                updateVerticalArrows(isAtTop, isAtBottom);
                pane.css('top', destTop);
                elem.trigger('jsp-scroll-y', [-destTop, isAtTop, isAtBottom]).trigger('scroll');
            }

            function positionDragX(destX, animate) {
                if (!isScrollableH) {
                    return;
                }
                if (destX < 0) {
                    destX = 0;
                } else if (destX > dragMaxX) {
                    destX = dragMaxX;
                }

                if (animate === undefined) {
                    animate = settings.animateScroll;
                }
                if (animate) {
                    jsp.animate(horizontalDrag, 'left', destX, _positionDragX);
                } else {
                    horizontalDrag.css('left', destX);
                    _positionDragX(destX);
                }
            }

            function _positionDragX(destX) {
                if (destX === undefined) {
                    destX = horizontalDrag.position().left;
                }

                container.scrollTop(0);
                horizontalDragPosition = destX;

                var isAtLeft = horizontalDragPosition === 0,
					isAtRight = horizontalDragPosition == dragMaxX,
					percentScrolled = destX / dragMaxX,
					destLeft = -percentScrolled * (contentWidth - paneWidth);

                if (wasAtLeft != isAtLeft || wasAtRight != isAtRight) {
                    wasAtLeft = isAtLeft;
                    wasAtRight = isAtRight;
                    elem.trigger('jsp-arrow-change', [wasAtTop, wasAtBottom, wasAtLeft, wasAtRight]);
                }

                updateHorizontalArrows(isAtLeft, isAtRight);
                pane.css('left', destLeft);
                elem.trigger('jsp-scroll-x', [-destLeft, isAtLeft, isAtRight]).trigger('scroll');
            }

            function updateVerticalArrows(isAtTop, isAtBottom) {
                if (settings.showArrows) {
                    arrowUp[isAtTop ? 'addClass' : 'removeClass']('jspDisabled');
                    arrowDown[isAtBottom ? 'addClass' : 'removeClass']('jspDisabled');
                }
            }

            function updateHorizontalArrows(isAtLeft, isAtRight) {
                if (settings.showArrows) {
                    arrowLeft[isAtLeft ? 'addClass' : 'removeClass']('jspDisabled');
                    arrowRight[isAtRight ? 'addClass' : 'removeClass']('jspDisabled');
                }
            }

            function scrollToY(destY, animate) {
                var percentScrolled = destY / (contentHeight - paneHeight);
                positionDragY(percentScrolled * dragMaxY, animate);
            }

            function scrollToX(destX, animate) {
                var percentScrolled = destX / (contentWidth - paneWidth);
                positionDragX(percentScrolled * dragMaxX, animate);
            }

            function scrollToElement(ele, stickToTop, animate) {
                var e, eleHeight, eleWidth, eleTop = 0, eleLeft = 0, viewportTop, viewportLeft, maxVisibleEleTop, maxVisibleEleLeft, destY, destX;

                // Legal hash values aren't necessarily legal jQuery selectors so we need to catch any
                // errors from the lookup...
                try {
                    e = $(ele);
                } catch (err) {
                    return;
                }
                eleHeight = e.outerHeight();
                eleWidth = e.outerWidth();

                container.scrollTop(0);
                container.scrollLeft(0);

                // loop through parents adding the offset top of any elements that are relatively positioned between
                // the focused element and the jspPane so we can get the true distance from the top
                // of the focused element to the top of the scrollpane...
                while (!e.is('.jspPane')) {
                    eleTop += e.position().top;
                    eleLeft += e.position().left;
                    e = e.offsetParent();
                    if (/^body|html$/i.test(e[0].nodeName)) {
                        // we ended up too high in the document structure. Quit!
                        return;
                    }
                }

                viewportTop = contentPositionY();
                maxVisibleEleTop = viewportTop + paneHeight;
                if (eleTop < viewportTop || stickToTop) { // element is above viewport
                    destY = eleTop - settings.verticalGutter;
                } else if (eleTop + eleHeight > maxVisibleEleTop) { // element is below viewport
                    destY = eleTop - paneHeight + eleHeight + settings.verticalGutter;
                }
                if (destY) {
                    scrollToY(destY, animate);
                }

                viewportLeft = contentPositionX();
                maxVisibleEleLeft = viewportLeft + paneWidth;
                if (eleLeft < viewportLeft || stickToTop) { // element is to the left of viewport
                    destX = eleLeft - settings.horizontalGutter;
                } else if (eleLeft + eleWidth > maxVisibleEleLeft) { // element is to the right viewport
                    destX = eleLeft - paneWidth + eleWidth + settings.horizontalGutter;
                }
                if (destX) {
                    scrollToX(destX, animate);
                }

            }

            function contentPositionX() {
                return -pane.position().left;
            }

            function contentPositionY() {
                return -pane.position().top;
            }

            function isCloseToBottom() {
                var scrollableHeight = contentHeight - paneHeight;
                return (scrollableHeight > 20) && (scrollableHeight - contentPositionY() < 10);
            }

            function isCloseToRight() {
                var scrollableWidth = contentWidth - paneWidth;
                return (scrollableWidth > 20) && (scrollableWidth - contentPositionX() < 10);
            }

            function initMousewheel() {
                container.unbind(mwEvent).bind(
					mwEvent,
					function (event, delta, deltaX, deltaY) {
					    var dX = horizontalDragPosition, dY = verticalDragPosition;
					    jsp.scrollBy(deltaX * settings.mouseWheelSpeed, -deltaY * settings.mouseWheelSpeed, false);
					    // return true if there was no movement so rest of screen can scroll
					    return dX == horizontalDragPosition && dY == verticalDragPosition;
					}
				);
            }

            function removeMousewheel() {
                container.unbind(mwEvent);
            }

            function nil() {
                return false;
            }

            function initFocusHandler() {
                pane.find(':input,a').unbind('focus.jsp').bind(
					'focus.jsp',
					function (e) {
					    scrollToElement(e.target, false);
					}
				);
            }

            function removeFocusHandler() {
                pane.find(':input,a').unbind('focus.jsp');
            }

            function initKeyboardNav() {
                var keyDown, elementHasScrolled, validParents = [];
                isScrollableH && validParents.push(horizontalBar[0]);
                isScrollableV && validParents.push(verticalBar[0]);

                // IE also focuses elements that don't have tabindex set.
                pane.focus(
					function () {
					    elem.focus();
					}
				);

                elem.attr('tabindex', 0)
					.unbind('keydown.jsp keypress.jsp')
					.bind(
						'keydown.jsp',
						function (e) {
						    if (e.target !== this && !(validParents.length && $(e.target).closest(validParents).length)) {
						        return;
						    }
						    var dX = horizontalDragPosition, dY = verticalDragPosition;
						    switch (e.keyCode) {
						        case 40: // down
						        case 38: // up
						        case 34: // page down
						        case 32: // space
						        case 33: // page up
						        case 39: // right
						        case 37: // left
						            keyDown = e.keyCode;
						            keyDownHandler();
						            break;
						        case 35: // end
						            scrollToY(contentHeight - paneHeight);
						            keyDown = null;
						            break;
						        case 36: // home
						            scrollToY(0);
						            keyDown = null;
						            break;
						    }

						    elementHasScrolled = e.keyCode == keyDown && dX != horizontalDragPosition || dY != verticalDragPosition;
						    return !elementHasScrolled;
						}
					).bind(
						'keypress.jsp', // For FF/ OSX so that we can cancel the repeat key presses if the JSP scrolls...
						function (e) {
						    if (e.keyCode == keyDown) {
						        keyDownHandler();
						    }
						    return !elementHasScrolled;
						}
					);

                if (settings.hideFocus) {
                    elem.css('outline', 'none');
                    if ('hideFocus' in container[0]) {
                        elem.attr('hideFocus', true);
                    }
                } else {
                    elem.css('outline', '');
                    if ('hideFocus' in container[0]) {
                        elem.attr('hideFocus', false);
                    }
                }

                function keyDownHandler() {
                    var dX = horizontalDragPosition, dY = verticalDragPosition;
                    switch (keyDown) {
                        case 40: // down
                            jsp.scrollByY(settings.keyboardSpeed, false);
                            break;
                        case 38: // up
                            jsp.scrollByY(-settings.keyboardSpeed, false);
                            break;
                        case 34: // page down
                        case 32: // space
                            jsp.scrollByY(paneHeight * settings.scrollPagePercent, false);
                            break;
                        case 33: // page up
                            jsp.scrollByY(-paneHeight * settings.scrollPagePercent, false);
                            break;
                        case 39: // right
                            jsp.scrollByX(settings.keyboardSpeed, false);
                            break;
                        case 37: // left
                            jsp.scrollByX(-settings.keyboardSpeed, false);
                            break;
                    }

                    elementHasScrolled = dX != horizontalDragPosition || dY != verticalDragPosition;
                    return elementHasScrolled;
                }
            }

            function removeKeyboardNav() {
                elem.attr('tabindex', '-1')
					.removeAttr('tabindex')
					.unbind('keydown.jsp keypress.jsp');
            }

            function observeHash() {
                if (location.hash && location.hash.length > 1) {
                    var e,
						retryInt,
						hash = escape(location.hash.substr(1)) // hash must be escaped to prevent XSS
                    ;
                    try {
                        e = $('#' + hash + ', a[name="' + hash + '"]');
                    } catch (err) {
                        return;
                    }

                    if (e.length && pane.find(hash)) {
                        // nasty workaround but it appears to take a little while before the hash has done its thing
                        // to the rendered page so we just wait until the container's scrollTop has been messed up.
                        if (container.scrollTop() === 0) {
                            retryInt = setInterval(
								function () {
								    if (container.scrollTop() > 0) {
								        scrollToElement(e, true);
								        $(document).scrollTop(container.position().top);
								        clearInterval(retryInt);
								    }
								},
								50
							);
                        } else {
                            scrollToElement(e, true);
                            $(document).scrollTop(container.position().top);
                        }
                    }
                }
            }

            function hijackInternalLinks() {
                // only register the link handler once
                if ($(document.body).data('jspHijack')) {
                    return;
                }

                // remember that the handler was bound
                $(document.body).data('jspHijack', true);

                // use live handler to also capture newly created links
                $(document.body).delegate('a[href*=#]', 'click', function (event) {
                    // does the link point to the same page?
                    // this also takes care of cases with a <base>-Tag or Links not starting with the hash #
                    // e.g. <a href="index.html#test"> when the current url already is index.html
                    var href = this.href.substr(0, this.href.indexOf('#')),
						locationHref = location.href,
						hash,
						element,
						container,
						jsp,
						scrollTop,
						elementTop;
                    if (location.href.indexOf('#') !== -1) {
                        locationHref = location.href.substr(0, location.href.indexOf('#'));
                    }
                    if (href !== locationHref) {
                        // the link points to another page
                        return;
                    }

                    // check if jScrollPane should handle this click event
                    hash = escape(this.href.substr(this.href.indexOf('#') + 1));

                    // find the element on the page
                    element;
                    try {
                        element = $('#' + hash + ', a[name="' + hash + '"]');
                    } catch (e) {
                        // hash is not a valid jQuery identifier
                        return;
                    }

                    if (!element.length) {
                        // this link does not point to an element on this page
                        return;
                    }

                    container = element.closest('.jspScrollable');
                    jsp = container.data('jsp');

                    // jsp might be another jsp instance than the one, that bound this event
                    // remember: this event is only bound once for all instances.
                    jsp.scrollToElement(element, true);

                    if (container[0].scrollIntoView) {
                        // also scroll to the top of the container (if it is not visible)
                        scrollTop = $(window).scrollTop();
                        elementTop = element.offset().top;
                        if (elementTop < scrollTop || elementTop > scrollTop + $(window).height()) {
                            container[0].scrollIntoView();
                        }
                    }

                    // jsp handled this event, prevent the browser default (scrolling :P)
                    event.preventDefault();
                });
            }

            // Init touch on iPad, iPhone, iPod, Android
            function initTouch() {
                var startX,
					startY,
					touchStartX,
					touchStartY,
					moved,
					moving = false;

                container.unbind('touchstart.jsp touchmove.jsp touchend.jsp click.jsp-touchclick').bind(
					'touchstart.jsp',
					function (e) {
					    var touch = e.originalEvent.touches[0];
					    startX = contentPositionX();
					    startY = contentPositionY();
					    touchStartX = touch.pageX;
					    touchStartY = touch.pageY;
					    moved = false;
					    moving = true;
					}
				).bind(
					'touchmove.jsp',
					function (ev) {
					    if (!moving) {
					        return;
					    }

					    var touchPos = ev.originalEvent.touches[0],
							dX = horizontalDragPosition, dY = verticalDragPosition;

					    jsp.scrollTo(startX + touchStartX - touchPos.pageX, startY + touchStartY - touchPos.pageY);

					    moved = moved || Math.abs(touchStartX - touchPos.pageX) > 5 || Math.abs(touchStartY - touchPos.pageY) > 5;

					    // return true if there was no movement so rest of screen can scroll
					    return dX == horizontalDragPosition && dY == verticalDragPosition;
					}
				).bind(
					'touchend.jsp',
					function (e) {
					    moving = false;
					    /*if(moved) {
							return false;
						}*/
					}
				).bind(
					'click.jsp-touchclick',
					function (e) {
					    if (moved) {
					        moved = false;
					        return false;
					    }
					}
				);
            }

            function destroy() {
                var currentY = contentPositionY(),
					currentX = contentPositionX();
                elem.removeClass('jspScrollable').unbind('.jsp');
                elem.replaceWith(originalElement.append(pane.children()));
                originalElement.scrollTop(currentY);
                originalElement.scrollLeft(currentX);

                // clear reinitialize timer if active
                if (reinitialiseInterval) {
                    clearInterval(reinitialiseInterval);
                }
            }

            // Public API
            $.extend(
				jsp,
				{
				    // Reinitialises the scroll pane (if it's internal dimensions have changed since the last time it
				    // was initialised). The settings object which is passed in will override any settings from the
				    // previous time it was initialised - if you don't pass any settings then the ones from the previous
				    // initialisation will be used.
				    reinitialise: function (s) {
				        s = $.extend({}, settings, s);
				        initialise(s);
				    },
				    // Scrolls the specified element (a jQuery object, DOM node or jQuery selector string) into view so
				    // that it can be seen within the viewport. If stickToTop is true then the element will appear at
				    // the top of the viewport, if it is false then the viewport will scroll as little as possible to
				    // show the element. You can also specify if you want animation to occur. If you don't provide this
				    // argument then the animateScroll value from the settings object is used instead.
				    scrollToElement: function (ele, stickToTop, animate) {
				        scrollToElement(ele, stickToTop, animate);
				    },
				    // Scrolls the pane so that the specified co-ordinates within the content are at the top left
				    // of the viewport. animate is optional and if not passed then the value of animateScroll from
				    // the settings object this jScrollPane was initialised with is used.
				    scrollTo: function (destX, destY, animate) {
				        scrollToX(destX, animate);
				        scrollToY(destY, animate);
				    },
				    // Scrolls the pane so that the specified co-ordinate within the content is at the left of the
				    // viewport. animate is optional and if not passed then the value of animateScroll from the settings
				    // object this jScrollPane was initialised with is used.
				    scrollToX: function (destX, animate) {
				        scrollToX(destX, animate);
				    },
				    // Scrolls the pane so that the specified co-ordinate within the content is at the top of the
				    // viewport. animate is optional and if not passed then the value of animateScroll from the settings
				    // object this jScrollPane was initialised with is used.
				    scrollToY: function (destY, animate) {
				        scrollToY(destY, animate);
				    },
				    // Scrolls the pane to the specified percentage of its maximum horizontal scroll position. animate
				    // is optional and if not passed then the value of animateScroll from the settings object this
				    // jScrollPane was initialised with is used.
				    scrollToPercentX: function (destPercentX, animate) {
				        scrollToX(destPercentX * (contentWidth - paneWidth), animate);
				    },
				    // Scrolls the pane to the specified percentage of its maximum vertical scroll position. animate
				    // is optional and if not passed then the value of animateScroll from the settings object this
				    // jScrollPane was initialised with is used.
				    scrollToPercentY: function (destPercentY, animate) {
				        scrollToY(destPercentY * (contentHeight - paneHeight), animate);
				    },
				    // Scrolls the pane by the specified amount of pixels. animate is optional and if not passed then
				    // the value of animateScroll from the settings object this jScrollPane was initialised with is used.
				    scrollBy: function (deltaX, deltaY, animate) {
				        jsp.scrollByX(deltaX, animate);
				        jsp.scrollByY(deltaY, animate);
				    },
				    // Scrolls the pane by the specified amount of pixels. animate is optional and if not passed then
				    // the value of animateScroll from the settings object this jScrollPane was initialised with is used.
				    scrollByX: function (deltaX, animate) {
				        var destX = contentPositionX() + Math[deltaX < 0 ? 'floor' : 'ceil'](deltaX),
							percentScrolled = destX / (contentWidth - paneWidth);
				        positionDragX(percentScrolled * dragMaxX, animate);
				    },
				    // Scrolls the pane by the specified amount of pixels. animate is optional and if not passed then
				    // the value of animateScroll from the settings object this jScrollPane was initialised with is used.
				    scrollByY: function (deltaY, animate) {
				        var destY = contentPositionY() + Math[deltaY < 0 ? 'floor' : 'ceil'](deltaY),
							percentScrolled = destY / (contentHeight - paneHeight);
				        positionDragY(percentScrolled * dragMaxY, animate);
				    },
				    // Positions the horizontal drag at the specified x position (and updates the viewport to reflect
				    // this). animate is optional and if not passed then the value of animateScroll from the settings
				    // object this jScrollPane was initialised with is used.
				    positionDragX: function (x, animate) {
				        positionDragX(x, animate);
				    },
				    // Positions the vertical drag at the specified y position (and updates the viewport to reflect
				    // this). animate is optional and if not passed then the value of animateScroll from the settings
				    // object this jScrollPane was initialised with is used.
				    positionDragY: function (y, animate) {
				        positionDragY(y, animate);
				    },
				    // This method is called when jScrollPane is trying to animate to a new position. You can override
				    // it if you want to provide advanced animation functionality. It is passed the following arguments:
				    //  * ele          - the element whose position is being animated
				    //  * prop         - the property that is being animated
				    //  * value        - the value it's being animated to
				    //  * stepCallback - a function that you must execute each time you update the value of the property
				    // You can use the default implementation (below) as a starting point for your own implementation.
				    animate: function (ele, prop, value, stepCallback) {
				        var params = {};
				        params[prop] = value;
				        ele.animate(
							params,
							{
							    'duration': settings.animateDuration,
							    'easing': settings.animateEase,
							    'queue': false,
							    'step': stepCallback
							}
						);
				    },
				    // Returns the current x position of the viewport with regards to the content pane.
				    getContentPositionX: function () {
				        return contentPositionX();
				    },
				    // Returns the current y position of the viewport with regards to the content pane.
				    getContentPositionY: function () {
				        return contentPositionY();
				    },
				    // Returns the width of the content within the scroll pane.
				    getContentWidth: function () {
				        return contentWidth;
				    },
				    // Returns the height of the content within the scroll pane.
				    getContentHeight: function () {
				        return contentHeight;
				    },
				    // Returns the horizontal position of the viewport within the pane content.
				    getPercentScrolledX: function () {
				        return contentPositionX() / (contentWidth - paneWidth);
				    },
				    // Returns the vertical position of the viewport within the pane content.
				    getPercentScrolledY: function () {
				        return contentPositionY() / (contentHeight - paneHeight);
				    },
				    // Returns whether or not this scrollpane has a horizontal scrollbar.
				    getIsScrollableH: function () {
				        return isScrollableH;
				    },
				    // Returns whether or not this scrollpane has a vertical scrollbar.
				    getIsScrollableV: function () {
				        return isScrollableV;
				    },
				    // Gets a reference to the content pane. It is important that you use this method if you want to
				    // edit the content of your jScrollPane as if you access the element directly then you may have some
				    // problems (as your original element has had additional elements for the scrollbars etc added into
				    // it).
				    getContentPane: function () {
				        return pane;
				    },
				    // Scrolls this jScrollPane down as far as it can currently scroll. If animate isn't passed then the
				    // animateScroll value from settings is used instead.
				    scrollToBottom: function (animate) {
				        positionDragY(dragMaxY, animate);
				    },
				    // Hijacks the links on the page which link to content inside the scrollpane. If you have changed
				    // the content of your page (e.g. via AJAX) and want to make sure any new anchor links to the
				    // contents of your scroll pane will work then call this function.
				    hijackInternalLinks: $.noop,
				    // Removes the jScrollPane and returns the page to the state it was in before jScrollPane was
				    // initialised.
				    destroy: function () {
				        destroy();
				    }
				}
			);

            initialise(s);
        }

        // Pluginifying code...
        settings = $.extend({}, $.fn.jScrollPane.defaults, settings);

        // Apply default speed
        $.each(['mouseWheelSpeed', 'arrowButtonSpeed', 'trackClickSpeed', 'keyboardSpeed'], function () {
            settings[this] = settings[this] || settings.speed;
        });

        return this.each(
			function () {
			    var elem = $(this), jspApi = elem.data('jsp');
			    if (jspApi) {
			        jspApi.reinitialise(settings);
			    } else {
			        $("script", elem).filter('[type="text/javascript"], :not([type])').remove();
			        jspApi = new JScrollPane(elem, settings);
			        elem.data('jsp', jspApi);
			    }
			}
		);
    };

    $.fn.jScrollPane.defaults = {
        showArrows: false,
        maintainPosition: true,
        stickToBottom: false,
        stickToRight: false,
        clickOnTrack: true,
        autoReinitialise: false,
        autoReinitialiseDelay: 500,
        verticalDragMinHeight: 0,
        verticalDragMaxHeight: 99999,
        horizontalDragMinWidth: 0,
        horizontalDragMaxWidth: 99999,
        contentWidth: undefined,
        animateScroll: false,
        animateDuration: 300,
        animateEase: 'linear',
        hijackInternalLinks: false,
        verticalGutter: 4,
        horizontalGutter: 4,
        mouseWheelSpeed: 0,
        arrowButtonSpeed: 0,
        arrowRepeatFreq: 50,
        arrowScrollOnHover: false,
        trackClickSpeed: 0,
        trackClickRepeatFreq: 70,
        verticalArrowPositions: 'split',
        horizontalArrowPositions: 'split',
        enableKeyboardNavigation: true,
        hideFocus: false,
        keyboardSpeed: 0,
        initialDelay: 300,        // Delay before starting repeating
        speed: 30,		// Default speed when others falsey
        scrollPagePercent: .8		// Percent of visible area scrolled when pageUp/Down or track area pressed
    };

})(jQuery);

(function ($) {
    $.fn.dnnSettingDropdown = function () {
        var clicked = function () {
            if ($(this).hasClass('dnnButtonDropdown')) {
                $(this).removeClass('dnnButtonDropdown').addClass('dnnButtonDropdown-clicked');
                $(this).next().show();
            }
        };

        var hideDropdown = function () {
            var btn = $(this).children(':first');
            if (btn.hasClass('dnnButtonDropdown-clicked')) {
                btn.removeClass('dnnButtonDropdown-clicked').addClass('dnnButtonDropdown');
                btn.next().fadeOut();
            }
        };

        var hoverConfig = {
            over: function () {
            },
            out: hideDropdown,
            timout: 600
        };

        return $(this).each(function () {
            $(this).unbind('click', clicked).bind('click', clicked);
            $(this).parent().hoverIntent(hoverConfig);
        });
    };
})(jQuery);
(function ($) {

    /* Start customised controls */
    var dnnInitCustomisedCtrls = function () {
        $('input[type="checkbox"]').dnnCheckbox();
        $('input[type="radio"]').dnnCheckbox({ cls: 'dnnRadiobutton' });
        $('.dnnTooltip').dnnTooltip();
        var divs = $.detectScroll();
        for (var i = 0; i < divs.length; i++)
            $(divs[i]).jScrollPane();


        $('input[type="text"], input[type="password"]').unbind('focus', inputFocusFix).bind('focus', inputFocusFix);
        $(':file').dnnFileInput();

    };

    var inputFocusFix = function () {
        var errorMsg = $(this).next();
        if (errorMsg.hasClass('dnnFormError'))
            errorMsg.hide();
    };

    var saveRgDataDivScrollTop = function () {
        window.__rgDataDivScrollTopPersistArray = [];
        $('.rgDataDiv').each(function () {
            var $this = $(this);
            var ele = $this.get(0);
            if (ele.scrollPane) {
                var api = ele.scrollPane.data('jsp');
                var y = api.getContentPositionY();
                window.__rgDataDivScrollTopPersistArray.push(y);
            }
        });
    };

    window.__rgDataDivScrollTopPersistArray = [];
    $(window).ajaxComplete(dnnInitCustomisedCtrls);

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(saveRgDataDivScrollTop);
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(dnnInitCustomisedCtrls);
    $(dnnInitCustomisedCtrls);

})(jQuery);