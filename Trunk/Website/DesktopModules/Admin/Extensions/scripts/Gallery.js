


function Gallery(params) {

    //defaults
    var options = {
        rowId : 0,
        index : 1,
        pageSize : 10,
        sortBy : "",
        sortDir: "",
        animationSpeed : "fast",
        action: "none",
        pageIdx: 1,
        pageSze: 10,
        smoothScrolling: true,
        extensions : new Object(),
        extensionFilter : "",
        tagFilter : "",
        ownerFilter : "",
        tags : new Object(),
        loadTags : true,
        pagedExtensions : new Object(),    
        protocol : ('https:' == location.protocol ? 'https://' : 'http://'),
        host: location.host,
        ServiceRoot : "/AppGalleryService.svc",
        ExtensionServiceName : "/Extensions",
        TagsServiceName : "/Tags",
        ExtensionTagsServiceName: "/ExtensionTags",
        error : false,
        extensionDetailDialog : $("#extensionDetail").dialog(this.DefaultDialogOptions),
        ExtensionPane : $(".ExtensionPane"),
        loading :  $("#loading")
    };

    //extend defaults with ctor params
    if (params) {
        $.extend(options, params);
    }
    
    console.log(options);

    //load up our object with default options
    for (var i in options) {
        if (options.hasOwnProperty(i)) this[i] = options[i];
    }

    //setup smooth scrolling pager
//    if (this.smoothScrolling) {
//        var s = new Scroller(100, false, function (scroller) {
//            _gallery.index++;
//            _gallery.action = "page";
//            _gallery.Search();
//        }).watch();
//    }

    //hide our extensions pane, for now
    this.ExtensionPane.hide();
    
    //load up our urls
    this.ExtensionsUrl = this.getServiceUrl(this.ExtensionServiceName);
    this.ExtensionTagsUrl = this.getServiceUrl(this.ExtensionTagsServiceName);
    this.TagsUrl = this.getServiceUrl(this.TagsServiceName);
    
    //wire up the ajax events
    this.loading.ajaxStart(function () { _gallery.ajaxStart(arguments) }).ajaxError(function () { _gallery.ajaxError(arguments) }).ajaxStop(function () { _gallery.ajaxStop(arguments) });

    //bind to our document events

    $(document).bind("scroll", function () {
        _gallery.reposition();
    });

    $(document).bind("resize", function () {
        _gallery.reposition();
    });
    this.reposition();

    $("#typeDDL").change(function (event) {
        var e = event || window.event;
        _gallery.FilterGallery(e, this);
    });

    $("#tag-list").click(function (event) {
        var e = event || window.event;
        _gallery.TagFilterGallery(e, this);
    });

    $("#search-reset").click(function () {
        $('#searchText').val('');
        _gallery.SearchGallery();
    });

    $("#searchText").change(function () {
        $("#search-go").click();
    });
    $("#searchText").keyup(function () {
        if (arguments[0].keyCode == 27) $("#search-reset").click();
    });
    $("#search-go").click(function () {
        _gallery.SearchGallery($('#searchText').val());
    });

    $("#NameSorter").click(function () {
        _gallery.SortExtensions('extensionName');
    });

    $("#PriceSorter").click(function () {
        _gallery.SortExtensions('price');
    });

}
Gallery.prototype.ajaxStart = function(a) {
        this.loading.text("Loading...");
        this.loading.show();
}
Gallery.prototype.ajaxStop = function(a) {
        if (!_gallery.error) this.loading.hide();
}
Gallery.prototype.ajaxError = function(a) {
        this.loading.css("background-color", "red");
        this.loading.text("Error...");
        this.loading.attr("title", a[3]);
        _gallery.error = true;
}

Gallery.prototype.reposition = function () {
    var wnd = $(window);
    this.loading.css("top", wnd.scrollTop());
    this.loading.css("left", (wnd.width() / 2) - (this.loading.width() / 2));
}

Gallery.prototype.SortExtensions = function (fld, order) {
    this.index = 1;
    this.action = "sort";
    if (!order)
        this.ToggleSort(fld);
    else {
        this.sortBy = fld;
        this.sortDir = order;
    }

    if (this.sortBy && this.sortDir) {
        var NameSorter = $("#NameSorter");
        var PriceSorter = $("#PriceSorter");
        if (this.sortBy == "extensionName") {
            if (this.sortDir == "asc")
                NameSorter.text("Name: Z-A");
            else
                NameSorter.text("Name: A-Z");
        } else if (this.sortBy == "price") {
            if (this.sortDir == "asc")
                PriceSorter.text("Price: High to Low");
            else
                PriceSorter.text("Price: Low to High");
        }
    }
    return this.Search();
}
Gallery.prototype.SearchGallery = function (search) {
    this.action = "search";
    if (search) {
        this.searchText = search;
    } else {
        this.searchText = '';
    }
    this.index = 1;
    return this.Search();
}

Gallery.prototype.OwnerFilterGallery = function (owner) {
    this.action = "filter";
    if (owner)
        this.ownerFilter = owner;

    this.index = 1;
    var extensionList = $("#extensionList");
    return this.Search();
}

Gallery.prototype.TagFilterGallery = function(event, caller) {
    var e = event || window.event;
    var filter = $((e.srcElement || e.target)).attr('value');
    this.action = "filter";
    if (filter)
        this.tagFilter = filter;
    this.index = 1;
    var extensionList = $("#extensionList");
    return this.Search();
}

Gallery.prototype.FilterGallery = function(e, ddl) {
    var filter = $((e.srcElement || e.target)).attr('value');

    this.action = "filter";
    if (filter)
        this.extensionFilter = filter;
    this.index = 1;
    var extensionList = $("#extensionList");
    return this.Search();
}

Gallery.prototype.Search = function () {

    this.getExtensions(function (msg) {
        _gallery.pagedExtensions = msg.d.results;
        if (_gallery.extensions && _gallery.extensions.d && _gallery.extensions.d.results && !(_gallery.action == "search" || _gallery.action == "filter" || _gallery.action == "sort"))
            _gallery.extensions.d.results = _gallery.extensions.d.results.concat(msg.d.results);
        else
            _gallery.extensions = msg;

        _gallery.showExtensions(function () {
            if (_gallery.loadTags) {
                _gallery.getTags(function (msg) {
                    _gallery.loadTags = false;
                    _gallery.tags = msg;
                    _gallery.showTags(function () {
                    });
                });
            }
        });
    });
    return this;
}


Gallery.prototype.getTags = function (callback) {

    var url = this.TagsUrl + "?&$inlinecount=allpages&$skip=0&$top=20";
    url = url + "&callback=?"

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //fix up the tag size data

            var fixedTags = {
                __count: msg.d._count,
                results: []
            };

            var max = 0;
            for (var r in msg.d.results) {
                var count = Math.floor(Math.random() * 10);
                //var count = msg.d.results[r]["count"];
                msg.d.results[r]["count"] = count;
                if (count > max) max = count;
            }
            var maxFontSize = 2;
            var diff = maxFontSize / max;
            var minThreshold = 0.5;
            for (var r in msg.d.results) {
                var count = msg.d.results[r]["count"];
                var fs = (count * diff);
                msg.d.results[r]["fontSize"] = fs;
                if (fs >= minThreshold) fixedTags.results.push(msg.d.results[r]);
            }

            if (callback)
                callback(fixedTags);
            return msg;
        }
    });
}

Gallery.prototype.ToggleSort = function (field) {
    this.sortBy = field;

    if (!this.sortDir || this.sortDir == "" || this.sortDir == "desc") {
        this.sortDir = 'asc';
    } else {
        this.sortDir = 'desc';
    }
}

Gallery.prototype.getServiceUrl = function (ServiceName) {
    return this.protocol + this.host + this.ServiceRoot + ServiceName;
}

Gallery.prototype.getByTagID = function (tagID, callback) {
    var query = this.ExtensionTagsUrl +
	    	    			"?&$inlinecount=allpages" + 			// get total number of records
	    	    			"&$skip=" + (this.index - 1) * this.pageSize + 	// skip to first record of page
	    	    			"&$top=" + this.pageSize +
	    	    			"&$filter=tagID%20eq%20" + tagID;
    query = query + "&callback=?"

    $.ajax({
        type: "GET",
        url: query,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var extensionIDs = [];
            for (var x = 0; x <= msg.d.results.length; x++) {
                extensionIDs.push(msg.d.results["extensionID"]);
            }

            if (callback)
                callback(extensionIDs);

            return extensionIDs;
        }
    });

}

Gallery.prototype.getExtensions = function (callback) {

    var query = this.ExtensionsUrl +
	    	    			"?&$inlinecount=allpages" + 			// get total number of records
	    	    			"&$skip=" + (this.index - 1) * this.pageSize + 	// skip to first record of page
	    	    			"&$top=" + this.pageSize;

    if (this.searchText && this.searchText != "")
        query = query + "&$filter=(substringof('" + this.searchText + "', extensionName) eq true or substringof('" + this.searchText + "', Description) eq true or substringof('" + this.searchText + "', Title) eq true)";

    if (this.extensionFilter && this.extensionFilter != "") {
        if (query.indexOf("$filter") < 0)
            query = query + "&$filter=";
        else
            query = query + "and ";

        query = query + "extensionType eq '" + this.extensionFilter + "'";
    }

    if (this.tagFilter && this.tagFilter != "") {
        if (query.indexOf("$filter") < 0)
            query = query + "&$filter=";
        else
            query = query + "and ";

        query = query + "tagid eq '" + this.tagFilter + "'";
    }
    if (this.ownerFilter && this.ownerFilter != "") {
        if (query.indexOf("$filter") < 0)
            query = query + "&$filter=";
        else
            query = query + "and ";

        query = query + "ownerName eq '" + this.ownerFilter + "'";
    }



    if (this.sortBy != "") {
        query = query + "&$orderby=" + this.sortBy + " " + this.sortDir;
    }

    query = query + "&callback=?"

    $.ajax({
        type: "GET",
        url: query,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (callback)
                callback(msg);
            return msg;
        }
    });
}
Gallery.prototype.showTags = function (callback) {
    // show tags in template
    var taglist = $("#tag-list");
    var tagTmpl = $("#tag-tmpl");

    taglist.empty();

    if (this.tags.results.length > 0) {
        tagTmpl.tmpl(this.tags.results).appendTo(taglist);
        taglist.fadeIn(this.animationSpeed);
    } else {
        taglist.fadeOut(this.animationSpeed);
    }
    if (callback) callback(this);
}
Gallery.prototype.showExtensions = function (callback) {
    this.pageCount = Math.ceil(this.extensions.d.__count / this.pageSize);

    var extensionList = $("#extensionList");

    if (this.smoothScrolling) {
    } else {
        extensionList.empty();
    }
    if (this.action == "search" || this.action == "filter" || this.action == "sort") extensionList.empty();


    if (this.pagedExtensions.length > 0) {
        $("#eTmpl")
			.tmpl(this.pagedExtensions)
            .find("div")
			.appendTo(extensionList).fadeIn(this.animationSpeed);
        this.ExtensionPane.fadeIn(this.animationSpeed);


    } else {
        if (!this.smoothScrolling) this.ExtensionPane.fadeOut(this.animationSpeed);
    }
    this.pagedExtensions = [];
    if (callback) callback(this);
}

Gallery.prototype.getExtensionById = function (extensionID) {
    if (!this.extensions || !this.extensions.d) return;
    var list = this.extensions.d.results;
    for (var x = list.length; x--; x >= 0) {
        if (list[x].extensionID == extensionID) return list[x];
    }
    return;
}

Gallery.prototype.GetRowClass = function () {
    this.rowId++;
    var rowClass = this.rowId % 2 == 0 ? "even-row" : "odd-row";
    if (this.rowId == 1) {
        rowClass += " selected";
    }
    return rowClass;
}

Gallery.prototype.FormatCurrency = function (num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + '$' + num + '.' + cents);
}



Gallery.prototype.DefaultDialogOptions = {
    modal: true,
    autoOpen: false,
    width: 800,
    height: 600,
    resizable: true,
    closeOnEscape: true
}


Gallery.prototype.ShowDetails = function (extensionID) {

    var ext = this.getExtensionById(extensionID);
    if (ext) {
        var extensionDetailInner = $("#extensionDetailInner");
        extensionDetailInner.empty();
        $("#extDetailTmpl").tmpl(ext).appendTo(extensionDetailInner);

        $("#extensionDetail-tabs").tabs();

        this.extensionDetailDialog.dialog({ title: ext.extensionName });
        this.extensionDetailDialog.dialog('open');


    }
    return false;
}


Scroller = function (maxPage, loadScroll, scrollcallback) {
    this.page = 1;
    this.maxPage = (maxPage) ? maxPage : 100;
    this.loadScroll = (typeof loadScroll != 'undefined') ? loadScroll : false;
    this.scrollcallback = scrollcallback;
    if (this.loadScroll) this.loadScroller();
}

Scroller.prototype.handleScroll = function () {
    this.page++;

    if (this.scrollcallback) this.scrollcallback(this);
    if (this.page >= this.maxPage) this.unwatch();
}

Scroller.prototype.loadScroller = function () {
    var more = true;
    while (more) {
        more = ($(window).scrollTop() >= ($(document).height() - $(window).height()));
        if (more) this.handleScroll();
        if (this.page >= this.maxPage) more = false;
    }
}

Scroller.prototype.watch = function () {
    window.Scroller = this;
    //var root = $(document);
    var root = $(window);
    if (!root.scroll) root = $(document);
    root.scroll(function () {
        var s = window.Scroller;
        if ($(window).scrollTop() >= ($(document).height() - $(window).height())) {
            s.handleScroll();
        }
    });
}
Scroller.prototype.unwatch = function () {
    $(document).unbind("scroll");
}
