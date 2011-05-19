(function ($) {
    $.fn.dnnTabs = function (options) {
        var opts = $.extend({}, $.fn.dnnTabs.defaultOptions, options),
        $wrap = this;

        // patch for period in selector - http://jsfiddle.net/9Mst9/2/
        $.ui.tabs.prototype._sanitizeSelector = function (hash) {
            return hash.replace(/:/g, "\\:").replace(/\./g, "\\\.");
        };

        $wrap.each(function () {
            var showEvent = null;
            if (this.id) {
                var id = 'dnnTabs-' + this.id;
                if (opts.selected === -1) {
                    var tabCookie = dnn.dom.getCookie(id);
                    if (tabCookie) {
                        opts.selected = tabCookie;
                    }
                    if (opts.selected === -1) {
                        opts.selected = 0;
                    }
                }
            }

            showEvent = (function (cookieId) {
                return function (event, ui) {
                    dnn.dom.setCookie(cookieId, ui.index, 1, '/', '', false);
                }
            })(id);

            $wrap.tabs({
                show: showEvent,
                selected: opts.selected,
                fx: {
                    opacity: opts.opacity,
                    duration: opts.duration
                }
            });
        });

        return $wrap;
    };

    $.fn.dnnTabs.defaultOptions = {
        opacity: 'toggle',
        duration: 'fast',
        selected: -1
    };

})(jQuery);

(function ($) {
    $.fn.dnnConfirm = function (options) {
        var opts = $.extend({}, $.fn.dnnConfirm.defaultOptions, options),
        $wrap = this;

        $wrap.each(function(){
            var $this = $(this);
            var defaultAction = $this.attr('href');
            if (defaultAction) {
                var $dnnDialog = $("<div class='dnnDialog'></div>").html(opts.text).dialog(opts);
                $this.click(function (e) {
                    e.preventDefault();
                    $dnnDialog.dialog({
                        buttons: [
                        {
                            text: opts.yesText,
                            click: function () {
                                $(this).dialog("close");
                                if (typeof (opts.callbackTrue) === 'function') {
                                    opts.callbackTrue.call(this);
                                } else {
                                    window.location.href = defaultAction;
                                };
                                return true;
                            }
                        },
                        {
                            text: opts.noText,
                            click: function () {
                                $(this).dialog("close");
                                if (typeof (opts.callbackFalse) === 'function') {
                                    opts.callbackFalse.call(this);
                                };
                                return false;
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
        dialogClass: 'dnnForm'
    };

})(jQuery);

(function ($) {
    $.dnnAlert = function (options) {
        var opts = $.extend({}, $.dnnAlert.defaultOptions, options)
        var $dnnDialog = $("<div class='dnnDialog'></div>").html(opts.text).dialog(opts);
        $dnnDialog.dialog({
            buttons: [
                {
                    text: opts.okText,
                    click: function () {
                        $(this).dialog("close");
                        if (typeof (opts.callback) === 'function') {
                            opts.callback.call(this);
                        };
                        return false;
                    }
                }
            ]
        });
        $dnnDialog.dialog('open');
    }

    $.dnnAlert.defaultOptions = {
        okText: 'Ok',
        autoOpen: false,
        resizable: false,
        modal: true,
        dialogClass: 'dnnForm'
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
                    dnn.dom.setCookie(id, $(this).is(':visible'), 1, '/', '', false);
                });
            });

            // walk over each selector and check its cookie, expand or collapse as necessary
            $this.find(opts.sectionHeadSelector).each(function (i, v) {
                var elm = $(v);
                var id = elm.attr("id");
                var idValue = dnn.dom.getCookie(id);
                if ((idValue === null && i != 0) || idValue === "false") {
                    elm.find(opts.clickToToggleIsolatedSelector).removeClass(opts.toggleClass);
                    elm.next(opts.regionToToggleSelector).hide();
                }
                else if ((idValue === null && i === 0) || idValue === "true") {
                    elm.find(opts.clickToToggleIsolatedSelector).addClass(opts.toggleClass);
                    elm.next(opts.regionToToggleSelector).show();
                }
            });

            // page validation integration - expand collapsed panels that contain tripped validators
            $this.find(opts.validationTriggerSelector).click(function(e){
                if (typeof(Page_ClientValidate) == 'function') {
                    Page_ClientValidate(opts.validationGroup);
                    $this.find(opts.invalidItemSelector).each(function(){
                        var $parent = $(this).closest(opts.regionToToggleSelector);
                        if ($parent.is(':hidden')){
                            $parent.prev(opts.sectionHeadSelector).find(opts.clickToToggleIsolatedSelector).click();
                        }
                    });
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
        validationGroup: '' 
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
                var skin = $this.find(opts.skinSelector).val();
                var container = $this.find(opts.containerSelector).val();
                if (skin) {
                    params += "SkinSrc=" + skin;
                }
                if (container) {
                    if (skin) {
                        params += "&";
                    }
                    params += "ContainerSrc=" + container;
                }
                if(params != "?"){
                    window.open(encodeURI(opts.baseUrl + params.replace(/.ascx/gi, '')), "skinpreview");    
                }
                else {
                    $.dnnAlert({text: opts.noSelectionMessage, okText: opts.alertOkText, closeText: opts.alertCloseText });
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
        alertCloseText: 'close'
    };

})(jQuery);

(function ($) {
    $.fn.dnnExpandAll = function (options) {
        var opts = $.extend({}, $.fn.dnnExpandAll.defaultOptions, options),
        $elem = this;

        $elem.click(function(e){
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