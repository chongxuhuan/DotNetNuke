(function ($, window, console) {
    $.fn.dnnActionMenu = function (options) {
        var opts = $.extend({},
            $.fn.dnnActionMenu.defaultOptions, options),
            $moduleWrap = this;

        $moduleWrap.each(function () {

            var $module = $(this);

            //hover event
            function hoverOver($m, opacity, effectMenu) {
                var $border = $m.children('.' + opts.borderClassName);
                if ($border.size() === 0) {
                    $border = $('<div class="' + opts.borderClassName + '"></div>').prependTo($m).css({ opacity: 0 });
                }
                $m.attr('style', 'z-index:904;');
                if (effectMenu) {
                    $m.find(opts.menuActionSelector).fadeTo(opts.fadeSpeed, opacity);
                }
                $m.children('.' + opts.borderClassName).fadeTo(opts.fadeSpeed, opacity);
            }

            //hover out event
            function hoverOut($m, opacity, effectMenu) {
                $m.removeAttr('style');
                $m.children('.' + opts.borderClassName).stop().fadeTo(opts.fadeSpeed, 0);
                if (effectMenu) {
                    $m.find(opts.menuActionSelector).stop().fadeTo(opts.fadeSpeed, opacity);
                }
            }

            function setMenuPosition($menuContainer) {
                var $menuBody = $module.find(opts.menuSelector).show(),
                    menuHeight = $menuBody.height(),
                    windowHeight = $(window).height(),
                    menuTop = $menuContainer.offset().top,
                    availableRoomBelow = (windowHeight - ((menuTop - $(window).scrollTop()) + $menuContainer.height()));

                // place the menu "above" if there's not enough room on the bottom
                // but always place the menu "below" if there's not explicitly enough room above
                // collision none allows us to overlap the window see:
                // http://stackoverflow.com/questions/5256619/why-position-of-div-is-different-when-browser-is-resized-to-a-certain-dimension
                if ((menuHeight > availableRoomBelow) && (menuHeight <= menuTop)) {
                    $menuBody.position({ my: 'left bottom', at: 'left top', of: $menuContainer, collision: 'none' });
                } else {
                    $menuBody.position({ my: 'left top', at: 'left bottom', of: $menuContainer, collision: 'none' });
                }
            }

            if ($module.find(opts.menuSelector).size() > 0) {

                $module.hoverIntent({
                    sensitivity: opts.hoverSensitivity,
                    timeout: opts.hoverTimeout,
                    interval: opts.hoverInterval,
                    over: function () {
                        hoverOver($(this).data('intentExpressed', true), 1, true);
                    },
                    out: function () {
                        hoverOut($(this).data('intentExpressed', false), opts.defaultOpacity, true);
                    }
                });

                $module.hover(function () {
                    hoverOver($(this), opts.defaultOpacity, false);
                },
                function () {
                    var $this = $(this);
                    if (!$this.data('intentExpressed')) {
                        hoverOut($this, 0, false);
                    }
                });

                $module.find(opts.menuActionSelector).css({ opacity: opts.defaultOpacity });

                $module.find(opts.menuWrapSelector).hoverIntent({
                    sensitivity: opts.hoverSensitivity,
                    timeout: opts.hoverTimeout,
                    interval: opts.hoverInterval,
                    over: function () {
                        setMenuPosition($(this));
                        $module.find(opts.menuSelector).fadeTo(opts.fadeSpeed, 1);
                    },
                    out: function () {
                        $module.find(opts.menuSelector).stop().fadeTo(opts.fadeSpeed, 0).hide();
                    }
                });

                $module.find(opts.menuSelector).children().css({ opacity: 1 }); //Compact IE7

                $module.find(opts.menuWrapSelector).draggable({
                    containment: $module.children().eq(1),
                    start: function (event, ui) {
                        $module.find(opts.menuSelector).hide();
                    },
                    stop: function (event, ui) {
                        setMenuPosition($(this));
                        $module.find(opts.menuSelector).show();
                    }
                });

            }

        });

        return $moduleWrap;
    };

    $.fn.dnnActionMenu.defaultOptions = {
        menuWrapSelector: '.dnnActionMenu',
        menuActionSelector: '.dnnActionMenuTag',
        menuSelector: 'ul.dnnActionMenuBody',
        defaultOpacity: 0.3,
        fadeSpeed: 'fast',
        borderClassName: 'dnnActionMenuBorder',
        hoverSensitivity: 2,
        hoverTimeout: 200,
        hoverInterval: 200
    };

    $(document).ready(function () {
        $('.DnnModule').dnnActionMenu();
    });

})(jQuery, window, console);