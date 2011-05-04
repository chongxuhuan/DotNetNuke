//This method prevents the popup from flashing before closing, also redirects the parent window.
//parent and parent.parent needs to be assign to a varaible for Opera compatibility issues.
var windowTop = parent;
var parentTop = windowTop.parent;

if (typeof (parentTop.$find) != "undefined") {
	if (location.href.indexOf('popUp') == -1 || windowTop.location.href.indexOf("popUp") > -1) {
        if (__dnn_UrlExists(location)) {
			var popup = windowTop.$("#iPopUp");
			if (popup.dialog('isOpen') === true) {
				popup.dialog("option", {
					close: function (event, ui) { }
				}).dialog('close');
			}
            windowTop.location.href = location.href;
        }
    }
    else {
    	windowTop.$("#iPopUp").dialog({ title: document.title });
    }
}

function __dnn_ShowModalPopUp(url) {
    var modal = $("#iPopUp");
	if(modal.length == 0){
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
        close: function (event, ui) { windowTop.location.reload(); }
    }).width(950 - 10).height(550 - 10);
    return false;
}

function __dnn_UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open("HEAD", url, false);
    http.send();
    if (http.status == 404)
        return false;
    else
        return true;
}