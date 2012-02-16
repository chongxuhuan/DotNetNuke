(function ($) {
    $.dnnSF = function (el, options) {
        // To avoid scope issues, use 'base' instead of 'this'
        // to reference this class from internal events and functions.
        var base = this;

        // Access to jQuery and DOM versions of element
        base.$el = $(el);
        base.el = el;

        // Add a reverse reference to the DOM object
        base.$el.data("dnnSF", base);

        base.init = function () {
            base.options = $.extend({}, $.dnnSF.defaultOptions, options);
            // Put your initialization code here
            base.options.ModuleID = base.findScope();
            base.options.TabID = dnn.getVar("sf_tabId", "-1");
            base.options.SiteRoot = dnn.getVar("sf_siteRoot", "/");
        };

        base.startsWith = function (text, searchString) { return (text.indexOf(searchString) === 0); };
        base.isNumeric = function (n) { return !isNaN(parseFloat(n)) && isFinite(n); };

        base.findScope = function () {
            var modScope = base.$el.parents("div[class*='DnnModule-']");
            if (typeof modScope != 'undefined' && modScope.length > 0) {
                var id = 0;
                var classList = modScope[0].classList;
                var count = classList.length - 1;
                for (var x = count; x > 0; x--) {
                    var cl = classList[x];
                    if (base.startsWith(cl, "DnnModule-")) {
                        id = cl.replace("DnnModule-", "");
                        if (base.isNumeric(id)) break;
                    }
                }
                return id;
            }
        };

        //more method definitions here
        base.ajax = function (module, controller, action, params, successCB, failCB, completeCB) {
            var url = base.options.SiteRoot + "DesktopModules/" + module + "/API/" + controller + "/" + action + "/";
            var async = true;
            if (typeof successCB == 'undefined') async = false;
            return $.ajax({
                type: "POST",
                url: url,
                async: async,
                dataType: "json",
                data: params,
                contentType: "application/x-www-form-urlencoded",
                beforeSend: function (jqXHR, settings) {
                    jqXHR.setRequestHeader("ModuleID", base.options.ModuleID);
                    jqXHR.setRequestHeader("tabid", base.options.TabID);
                },
                success: successCB,
                error: failCB,
                complete: completeCB
            });
        };


        // Run initializer
        base.init();
        return base;
    };

    $.dnnSF.defaultOptions = {
    };

    $.fn.dnnSF = function (options) {
        var jThis = $(this);
        if (typeof jThis.data("dnnSF") != 'undefined') return jThis.data("dnnSF");
        var b = new $.dnnSF(jThis, options)
        jThis.data("dnnSF", b);
        return b;
    };

})(jQuery);
//----------------------------