/*
  DotNetNuke® - http://www.dotnetnuke.com
  Copyright (c) 2002-2010
  by DotNetNuke Corporation
 
  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
  documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
  to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 
  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
  of the Software.
 
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
  DEALINGS IN THE SOFTWARE.

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' This script renders all Visibility widgets defined on the page.
	''' This script is designed to be called by StyleSheetWidget.js.
	''' Calling it directly will produce an error.
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	'''     Version 1.0.0: Oct. 28, 2007, Nik Kalyani, nik.kalyani@dotnetnuke.com 
    '''     Version 1.1.0: Dec. 29, 2009, Joe Brinkman, joe.brinkman@dotnetnuke.com
	''' </history>
	''' -----------------------------------------------------------------------------
*/

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// BEGIN: VisibilityWidget class                                                                              //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
DotNetNuke.UI.WebControls.Widgets.VisibilityWidget = function(widget)
{
    DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.initializeBase(this, [widget]);
}

DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.prototype = 
{        
        // BEGIN: render
        render : 
        function()
        {
			var data = 	{
							useAlternatingClasses: 'true',
							expandClassName: 'expand',
							collapseClassName: 'collapse',
							targetElement: '',
							eventSourceElement: '',
							closeElement: '',
							title: '',
							toggleFunction: DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.animate
						};
			
            var params = this._widget.childNodes;
            for(var p=0;p<params.length;p++)
            {
                try
                {
                    var paramName = params[p].name.toLowerCase();
                    switch(paramName)
                    {
                        case "usealternatingclasses" : data.useAlternatingClasses = params[p].value; break;
                        case "expandclassname" : data.expandClassName = params[p].value; break;
                        case "collapseclassname" : data.collapseClassName = params[p].value; break;
                        case "targetelementid"  : data.targetElement = jQuery('#' + params[p].value); break;
                        case "eventsourceelementid" : data.eventSourceElement = jQuery('#' + params[p].value); break;
                        case "closeelementid" : data.closeElement = jQuery('#' + params[p].value); break;
                        case "title" : data.title = params[p].value; break;
                        case "togglefunction" : data.toggleFunction = eval(params[p].value); break;
                    }
                }
                catch(e)
                {                
                }
            }
			
            if (data.targetElement.length == 1)
            {

				var input 
				if (data.eventSourceElement.length == 1)
                {
					input = data.eventSourceElement;
					if (data.closeElement.length == 1)
					{
						data.closeElement.bind('click', 
							data, 
							DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility);
					}
                }
                 

				if (input == null)
                {  
					input = jQuery('<input type="button" />')
						.addClass(data.expandClassName)
						.attr('title', data.title);
                }              
                
                if (data.useAlternatingClasses == "true")
                {
					input.addClass(data.expandClassName);
                }
                
				input.bind('click', 
					data,
					DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility );
			
                DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.callBaseMethod(this, "render", input.get(0));
            }            
        }                
        // END: render
}

DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility = function(e)
{
    var iconObject = jQuery(e.target);
    if (e.data.targetElement.length == 0) return;

    if (e.data.useAlternatingClasses == "true")
    {
		if (e.data.targetElement.is(":visible"))
		{
			iconObject
				.removeClass(e.data.collapseClassName)
				.addClass(e.data.expandClassName);
		}
		else
		{
			iconObject
				.removeClass(e.data.expandClassName)
				.addClass(e.data.collapseClassName);
		}
    }
	
	if (jQuery.isFunction(e.data.toggleFunction))
	{
		e.data.toggleFunction(e);
	}
	
}

DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.animate = function(e){
	e.data.targetElement.toggle();
}

DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.inheritsFrom(DotNetNuke.UI.WebControls.Widgets.BaseWidget);
DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.registerClass("DotNetNuke.UI.WebControls.Widgets.VisibilityWidget", DotNetNuke.UI.WebControls.Widgets.BaseWidget);
DotNetNuke.UI.WebControls.Widgets.renderWidgetType("DotNetNuke.UI.WebControls.Widgets.VisibilityWidget");
// END: VisibilityWidget class
