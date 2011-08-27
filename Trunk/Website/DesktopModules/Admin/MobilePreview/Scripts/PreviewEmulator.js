(function ($) {
	$.fn.previewEmulator = function (options) {
		var handler = this;
		var hDimension = this.find(".dimension_h");
		var vDimension = this.find(".dimension_v");
		var viewContainer = this.find(".emulator_c");
		var viewer = this.find("iframe");

		var bindElement = function () {
			viewContainer = handler.find(".emulator_c");
			viewer = handler.find("iframe");

			viewContainer.bind("resize", function (e) {
				var width = $(this).width();
				var height = $(this).height();

				hDimension.width(width);
				vDimension.height(height);

				var hContentWidth = width - hDimension.find(".left").width() - hDimension.find(".right").width();
				hDimension.find(".center").width(hContentWidth).html(width + "px");

				var vContentHeight = height - vDimension.find(".top").height() - vDimension.find(".bottom").height();
				vDimension.find(".middle").height(vContentHeight).css("line-height", vContentHeight + "px").html(height + "px");

				$(this).jScrollPane();

			});

			viewer.attr("scrolling", "no").bind("load", function () {
				if (!$(this).data("loaded")) {
					var width = this.contentWindow.document.body.scrollWidth;
					var height = this.contentWindow.document.body.scrollHeight;

					$(this).width(width).height(height);

					$(this).data("loaded", true);

					viewContainer.trigger("resize");
				}

			});
		};

		bindElement();

		this.setPreview = function (width, height) {
			if (viewContainer.data("jsp")) viewContainer.data("jsp").destroy();

			bindElement();

			viewContainer.width(width).height(height);
			viewContainer.trigger("resize");
		};

		this.showDimension = function (show) {
			var visible = show ? "visible" : "hidden";
			hDimension.css("visibility", visible);
			vDimension.css("visibility", visible);
		};

		hDimension.html("<span class=\"left\"></span><span class=\"center\"></span><span class=\"right\"></span>");
		vDimension.html("<span class=\"top\"></span><span class=\"middle\"></span><span class=\"bottom\"></span>");

		viewer.attr("src", options.url);

		return this;
	};
})(jQuery);