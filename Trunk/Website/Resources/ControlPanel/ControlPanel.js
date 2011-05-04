jQuery(function ($) {
  $('.dnnControlPanel').wrap(function () {
    return '<div id="dnnCPWrap"><div id="dnnCPWrapLeft"><div id="dnnCPWrapRight"></div></div></div>'
  });

  function cpHoverOver() {
    $(this).find(".dnnCPContent").stop().slideDown('fast').show();
  }

  function cpHoverOut() {
    $("#dnnCPWrap").animate({ opacity: 1.0 }, 100, function () {
      if ($("#dnnCPWrap").find('.dnnCPContent.focused').size() == 0) {
        $("#dnnCPWrap").find(".dnnCPContent").stop().slideUp('fast', function () {
          $(".dnnCPContent").hide();
        });
      }
    });
  }

  function fixPadding() {
    var $wrapper = $('#dnnCPWrap');
    if ($wrapper.hasClass('Pinned')) {
      $wrapper.parent().css({ paddingBottom: '0' });
    } else {
      $wrapper.parent().css({ paddingBottom: $('.dnnCPHeader').height() + 5 + 'px' });
    }
  }

  var config = {
    sensitivity: 2,
    interval: 100,
    over: cpHoverOver,
    timeout: 500,
    out: cpHoverOut
  };

  $("#dnnCPWrap")
  	.hoverIntent(config)
  	.live('mouseenter mouseleave', function (event) {
  	  if (event.type == 'mouseenter') {
  	    $(this).addClass('moused');
  	  } else {
  	    $(this).removeClass('moused');
  	  }
  	})
    .parent()
    .css({ overflow: 'hidden', position: 'relative' });

  $('.dnnCPContent').live('focus blur', function (event) {
    if (event.type == 'focusout') {
      $(this).removeClass('focused');
      if ($("#dnnCPWrap.moused").size() == 0) cpHoverOut();

    } else {
      $(this).addClass('focused');
    }
  });

  $('.dnnCPDock').click(function () {
    $(this).toggleClass('Pinned');
    $('#dnnCPWrap').toggleClass('Pinned');
    fixPadding();
  });

  fixPadding();
});

//jQuery(document).ready(function() {
//    if (jQuery("input[value='LAYOUT'][checked]").length == 0) {

//        var paneId = jQuery("select[id$='cboPanes'] option:selected").text();
//        var pane = jQuery("td[id$='" + paneId + "'],div[id$='" + paneId + "']");
//        var isHidden = (pane.height() < 2);

//        if (isHidden) {
//            pane.removeClass("DNNEmptyPane").append("<div class='_DNNRemove' style='height:0px' />");
//            jQuery("._DNNRemove").animate({ height: 29 }, "slow");
//        }

//        var size = { width: (pane.width() < 150 ? 150 : pane.width()), height: (pane.height() < 25 ? 25 : pane.height()) };
//        var offset = pane.offset();

//        var highlight = jQuery('#dnnPaneHighlight');
//        if (highlight.length < 1) {
//            highlight = jQuery("<div id='dnnPaneHighlight' style='position:absolute;border:2px dashed #333;text-align:center;font-weight:bold;font-size:1.2em;display:none;' />")
//                    .appendTo("#Body");
//        }

//        highlight.css("top", offset.top)
//                    .css("left", offset.left)
//                    .width(size.width)
//                    .height(size.height)
//                    .text(paneId)
//                    .fadeIn(1000)
//                    .fadeOut(500);

//        if (isHidden) {
//            jQuery("._DNNRemove").animate({ opacity: 1.0 }, 100).animate({ height: 0 }, "slow", function() { jQuery(this).remove() });
//            pane.addClass("DNNEmptyPane");

//        }
//    }

//});
