var dnnModal = {
    load: function () {
        //This method prevents the popup from flashing before closing, also redirects the parent window.
        //parent and parent.parent needs to be assign to a varaible for Opera compatibility issues.
        var windowTop = parent;
        var parentTop = windowTop.parent;

        if (typeof (parentTop.$find) != "undefined") {
            if (location.href.indexOf('popUp') == -1 || windowTop.location.href.indexOf("popUp") > -1) {
                var popup = windowTop.$("#iPopUp");
                if (popup.dialog('isOpen') === true) {
                    popup.dialog("option", {
                        close: function (event, ui) { }
                    }).dialog('close');
                }
                windowTop.location.href = location.href;
            }
            else {
                windowTop.$("#iPopUp").dialog({ title: document.title });
            }
        }
    },

    show: function (url, showReturn) {
        var modal = $("#iPopUp");
        if (modal.length == 0) {
            modal = $("<iframe id=\"iPopUp\" src=\"about:blank\" scrolling=\"auto\" frameborder=\"0\" ></iframe>");
            $(document).append(modal);
        }
        modal[0].src = url;
        var windowTop = parent; //needs to be assign to a varaible for Opera compatibility issues.
        modal.dialog({
            modal: true,
            autoOpen: true,
            dialogClass: "dnnFormPopup",
            position: "center",
            minWidth: 950,
            minHeight: 550,
            maxWidth: 1920,
            maxHeight: 1080,
            resizable: true,
            closeOnEscape: true,
            close: function (event, ui) { windowTop.location.reload(); $(this).close(); }
        }).width(950 - 11).height(550 - 10);

        if (showReturn.toString() == "true") {
            return false;
        }
    }
};

dnnModal.load();