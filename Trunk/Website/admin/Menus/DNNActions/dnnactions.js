(function ($) {
    $.fn.dnnActionMenu = function (options) {
        var opts = $.extend({},
            $.fn.dnnActionMenu.defaultOptions, options),
            $moduleWrap = this;

        $moduleWrap.each(function () {

            var $module = $(this);

            if ($module.find(opts.menuSelector).size() > 0) {
                function hoverOver($m, opacity) {
                    var $border = $m.prev('.' + opts.borderClassName);
                    if (!$border.size() > 0) {
                        $border = $('<div class="' + opts.borderClassName + '"></div>')
                        .css('height', $m.height() + 10)
                        .css('width', $m.width() + 10)
                        .css('position', 'absolute')
                        .position({ my: 'left top', at: 'left top', of: $m, offset: '-5' })
                        .insertBefore($m);
                    }
                    $m.find(opts.menuActionSelector)
                        .css('position', 'absolute')
                        .position({ my: 'left top', at: 'left top', of: $m, offset: '10' })
                        .fadeTo(opts.fadeSpeed, opacity);
                    $m.prev('.' + opts.borderClassName).fadeTo(opts.fadeSpeed, opacity);
                };

                function hoverOut($m, opacity) {
                    $m.prev('.' + opts.borderClassName).stop().fadeTo(opts.fadeSpeed, opacity);
                    $m.find(opts.menuActionSelector).stop().fadeTo(opts.fadeSpeed, opacity);
                }

                $module.hoverIntent({
                    sensitivity: opts.hoverSensitivity,
                    timeout: opts.hoverTimeout,
                    interval: opts.hoverInterval,
                    over: function () {
                        hoverOver($(this).data('intentExpressed', true), 1);
                        if (opts.debug) {
                            console.log('module hover intent over');
                        }
                    },
                    out: function () {
                        hoverOut($(this).data('intentExpressed', false), 0);
                        if (opts.debug) {
                            console.log('module hover intent out');
                        }
                    }
                });

                $module.hover(function () {
                    hoverOver($(this), 0.2);
                    if (opts.debug) {
                        console.log('module hover over');
                    }
                },
                function () {
                    if (opts.debug) {
                        console.log('module hover out');
                    }
                    var $this = $(this);
                    if (!$this.data('intentExpressed')) {
                        hoverOut($this, 0);
                    }
                });

                $module.find(opts.menuActionSelector).hoverIntent({
                    sensitivity: opts.hoverSensitivity,
                    timeout: opts.hoverTimeout,
                    interval: opts.hoverInterval,
                    over: function () {
                        if (opts.debug) {
                            console.log('module action menu hover intent over');
                        }
                        $(this).find(opts.menuSelector).show().fadeTo(opts.fadeSpeed, 1);
                    },
                    out: function () {
                        if (opts.debug) {
                            console.log('module action menu hover intent out');
                        }
                        $(this).find(opts.menuSelector).stop().fadeTo(opts.fadeSpeed, 0).hide();
                    }
                });
            }

        });

        return $moduleWrap;
    };

    $.fn.dnnActionMenu.defaultOptions = {
        menuActionSelector: '.dnnActionMenu',
        menuSelector: 'ul.dnnActionMenuBody',
        fadeSpeed: 'fast',
        borderClassName: 'dnnActionMenuBorder',
        hoverSensitivity: 2,
        hoverTimeout: 200,
        hoverInterval: 200,
        debug: false
    };

})(jQuery);

$(document).ready(function () {
    $('.DnnModule').dnnActionMenu();
});