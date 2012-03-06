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
        };

        base.isNumeric = function (n) { return !isNaN(parseFloat(n)) && isFinite(n); };

        base.findScope = function () {
            var modScope = base.$el.parents("div[class*='DnnModule-']");
            if (typeof modScope != 'undefined' && modScope.length > 0) {
                var classes = $(modScope[0]).attr('class');
                var pattern = /DnnModule-[^\s]+/gi;
                var match = classes.match(pattern);
                for(m in match) {
                    var id = match[m].replace('DnnModule-', '');
                    if (base.isNumeric(id)) {
                        return id;
                    }
                }
                return -1;
            }
        };

        base.getServiceRoot = function (moduleName) {
            var serviceRoot = dnn.getVar("sf_siteRoot", "/");
            serviceRoot += "DesktopModules/" + moduleName + "/API/";
            return serviceRoot;
        };

        base.getTabId = function () {
            return dnn.getVar("sf_tabId", -1);
        };

        base.getModuleId = function () {
            return base.findScope();
        };

        base.setModuleHeaders = function (xhr) {
            xhr.setRequestHeader("ModuleId", base.getModuleId());
            xhr.setRequestHeader("TabId", base.getTabId());
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
