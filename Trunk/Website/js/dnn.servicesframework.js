﻿(function (a) { a.dnnSF = function (c, b) { var d = this; d.$el = a(c); d.el = c; d.$el.data("dnnSF", d); d.init = function () { d.options = a.extend({}, a.dnnSF.defaultOptions, b); d.options.ModuleID = d.findScope(); d.options.TabID = dnn.getVar("sf_tabId", "-1"); d.options.SiteRoot = dnn.getVar("sf_siteRoot", "/") }; d.startsWith = function (f, e) { return (f.indexOf(e) === 0) }; d.isNumeric = function (e) { return !isNaN(parseFloat(e)) && isFinite(e) }; d.findScope = function () { var i = d.$el.parents("div[class*='DnnModule-']"); if (typeof i != "undefined" && i.length > 0) { var j = 0; var h = i[0].classList; var g = h.length - 1; for (var e = g; e > 0; e--) { var f = h[e]; if (d.startsWith(f, "DnnModule-")) { j = f.replace("DnnModule-", ""); if (d.isNumeric(j)) { break } } } return j } }; d.ajax = function (f, j, i, h, m, l, k) { var e = d.options.SiteRoot + "DesktopModules/" + f + "/API/" + j + "/" + i + "/"; var g = true; if (typeof m == "undefined") { g = false } return a.ajax({ type: "POST", url: e, async: g, dataType: "json", data: h, contentType: "application/x-www-form-urlencoded", beforeSend: function (o, n) { o.setRequestHeader("ModuleID", d.options.ModuleID); o.setRequestHeader("tabid", d.options.TabID) }, success: m, error: l, complete: k }) }; d.init(); return d }; a.dnnSF.defaultOptions = {}; a.fn.dnnSF = function (d) { var e = a(this); if (typeof e.data("dnnSF") != "undefined") { return e.data("dnnSF") } var c = new a.dnnSF(e, d); e.data("dnnSF", c); return c } })(jQuery);