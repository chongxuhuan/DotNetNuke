(function (window, $) {
    dnnModal = { //global scope
        load: function () {
            //This method prevents the popup from flashing before closing, also redirects the parent window.
            //parent and parent.parent needs to be assign to a varaible for Opera compatibility issues.
            var windowTop = parent;
            var parentTop = windowTop.parent;

            if (typeof (parentTop.$find) != "undefined") {
                if (location.href.indexOf('popUp') == -1 || windowTop.location.href.indexOf("popUp") > -1) {
                    windowTop.location.href = location.href;

                    var popup = windowTop.$("#iPopUp");
                    if (popup.dialog('isOpen') === true) {
                        popup.dialog("option", {
                            close: function (event, ui) { }
                        }).dialog('close').remove();
                    }

                }
                else {
                    windowTop.$("#iPopUp").dialog({ title: document.title });
                }
            }
        },
        show: function (url, showReturn, height, width) {
            var $modal = $("#iPopUp");
            if ($modal.length == 0) {
                $modal = $("<iframe id=\"iPopUp\" src=\"about:blank\" scrolling=\"auto\" frameborder=\"0\" ></iframe>");
                $(document).find('html').css('overflow', 'hidden');
                $(document).append($modal);
            }
            $modal[0].src = url;
            var windowTop = parent; //needs to be assign to a varaible for Opera compatibility issues.

            $modal.dialog({
                modal: true,
                autoOpen: true,
                dialogClass: "dnnFormPopup",
                position: "center",
                minWidth: width,
                minHeight: height,
                maxWidth: 1920,
                maxHeight: 1080,
                resizable: true,
                closeOnEscape: true,
                close: function (event, ui) {
                    windowTop.location.reload();
                    $(this).remove();
                }
            })
        .width(width - 11)
        .height(height - 11);

            var $dnnToggleMax = $('<a href="#" class="dnnToggleMax"><span>Max</span></a>');
            $('.ui-dialog-title').after($dnnToggleMax);
            $dnnToggleMax.click(function (e) {
                e.preventDefault();

                var $window = $(window),
                $this = $(this),
                newHeight,
                newWidth,
                newPosition;

                if ($modal.data('isMaximized')) {
                    newHeight = $modal.data('height');
                    newWidth = $modal.data('width');
                    newPosition = $modal.data('position');
                    $modal.data('isMaximized', false);
                }
                else {
                    $modal.data('height', $modal.dialog("option", "minHeight"))
                        .data('width', $modal.dialog("option", "minWidth"))
                        .data('position', $modal.dialog("option", "position"));

                    newHeight = $window.height() - 11;
                    newWidth = $window.width() - 11;
                    newPosition = [0, 0];
                    $modal.data('isMaximized', true);
                }

                $this.toggleClass('ui-dialog-titlebar-max');
                $modal.dialog({ height: newHeight, width: newWidth });
                $modal.dialog({ position: 'center' });

            });

            if (showReturn.toString() == "true") {
                return false;
            }
        }
    };
    dnnModal.load();
}(this, jQuery))