$(function () {

    var canHide = false;

    function EnableHide() {
        canHide = true;
    }

    function megaHoverOver() {
        hideAll();
        $(this).parent().find(".megaborder").stop().fadeTo('fast', 1).show(); //Find sub and fade it in
    }

    function hideAll() {
        $(".megaborder").stop().fadeTo('fast', 0, function () { //Fade to 0 opacity
            $(this).hide();  //after fading, hide it
        });
    }


    //Set custom configurations
    var config = {
        sensitivity: 2, // number = sensitivity threshold (must be 1 or higher)
        interval: 200, // number = milliseconds for onMouseOver polling interval
        over: megaHoverOver, // function = onMouseOver callback (REQUIRED)
        timeout: 200, // number = milliseconds delay before onMouseOut
        out: function () { return; } // function = onMouseOut callback (REQUIRED)
    };

    $(".dnnadminmega > .megaborder").css({ 'opacity': '0' }); //Fade sub nav to 0 opacity on default
    
    $(".dnnadminmega > li").mouseenter(EnableHide); //Hovering over CP will re-enable hiding.
        
    $(".dnnadminmega > li > a").hoverIntent(config); //Trigger Hover intent with custom configurations
    
    //Hovering over the content area will hide the control panel
    $('#Footer, #Content').mouseenter(function () {
        if (canHide) {
            hideAll();
        }
    });

    $("#Footer, #Content").click(hideAll); //Clicking content area will force the CP to hide. ignoring canHide flag.

    //Hovering over a telerik dropdown will disable autohide
    //need this to disable hide when the drop down expands beyond the menu body
    $('.rcbSlide li').mouseover(function () {
        canHide = false;
    });
});