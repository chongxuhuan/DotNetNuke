$(document).ready(function(){
    $('.dnnActionMenu').hoverIntent({
        sensitivity: 2,
        timeout: 200,
        interval: 200,
        over: function(){
            $(this).find('ul.first.last').fadeTo('fast', 1);
        },
        out: function(){
            $(this).find('ul.first.last').fadeOut('fast', 0);
        }
    });
});