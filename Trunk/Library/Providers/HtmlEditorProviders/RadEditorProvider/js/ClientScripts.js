function OnClientCommandExecuting(editor, args) {
    var commandName = args.get_commandName();
    if (commandName == "ToggleScreenMode") {
        if (editor._isFullScreen == true ) {
            $.fn.dnnControlPanel.show();
        }
        else {
            $.fn.dnnControlPanel.hide();
        }
    }
}
