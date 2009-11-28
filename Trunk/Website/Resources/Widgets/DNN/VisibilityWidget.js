/*
  DotNetNuke® - http://www.dotnetnuke.com
  Copyright (c) 2002-2007
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
            var widgetHtml = "";
            var params = this._widget.childNodes;
            var useAlternatingClasses = "true";
            var expandClassName = "expand";
            var collapseClassName = "collapse";
            var targetElementId = "";
            var eventSourceElementId = "";
            var closeElementId = "";
			var visibilityState = "closed";
            var title = "";
            for(var p=0;p<params.length;p++)
            {
                try
                {
                    var paramName = params[p].name.toLowerCase();
                    switch(paramName)
                    {
                        case "usealternatingclasses" : useAlternatingClasses = params[p].value; break;
                        case "expandclassname" : expandClassName = params[p].value; break;
                        case "collapseclassname" : collapseClassName = params[p].value; break;
                        case "targetelementid"  : targetElementId = params[p].value; break;
                        case "eventsourceelementid" : eventSourceElementId = params[p].value; break;
                        case "closeelementid" : closeElementId = params[p].value; break;
                        case "title" : title = params[p].value; break;
                    }
                }
                catch(e)
                {                
                }
            }
            if (targetElementId != "")
            {
                var input 
                if (eventSourceElementId != "")
                {
                  input = $get(eventSourceElementId);

				  if (closeElementId != "")
				  {
					  var close = $get(closeElementId)
					  close.setAttribute("visibilityState", "open");
					  close.setAttribute("useAlternatingClasses", "false");
					  close.setAttribute("targetElementId", targetElementId);
					  $addHandler(close, "click", DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility);
				  }
                }
                 
                if (input == null)
                {  
                  input = document.createElement("input");
                  input.setAttribute("type", "button");
                  input.className = expandClassName;
                  input.title = title;
                }              
                
                if (useAlternatingClasses == "true")
                {
                  input.className = expandClassName;
                  input.setAttribute("expandClassName", expandClassName);
                  input.setAttribute("collapseClassName", collapseClassName);
                }
                
                input.setAttribute("useAlternatingClasses", useAlternatingClasses);
                input.setAttribute("targetElementId", targetElementId);
				input.setAttribute("visibilityState", "closed");
                $addHandler(input, "click", DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility);
                
                
                if (eventSourceElementId == "")
                {
                  DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.callBaseMethod(this, "render", [input]);
                }              
            }            
        }                
        // END: render
}

DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.toggleVisibility = function(sender)
{
    var iconObject = sender.target;
    if (iconObject.getAttribute("targetElementId") == null) return;

    var toolbox = $get(iconObject.getAttribute("targetElementId"));
    var visibilityState = iconObject.getAttribute("visibilityState");
    var expandClassName = iconObject.getAttribute("expandClassName");
    var collapseClassName = iconObject.getAttribute("collapseClassName");
    var useAlternatingClasses = iconObject.getAttribute("useAlternatingClasses");
   
   
    if (useAlternatingClasses == "true")
    {
      if (visibilityState == "closed")
      {
          iconObject.className = collapseClassName;
          toolbox.style.display = "block";
		  iconObject.setAttribute("visibilityState", "open");
      }
      else
      {
          iconObject.className = expandClassName;
          toolbox.style.display = "none";
		  iconObject.setAttribute("visibilityState", "closed");
      }
    }
    else
    {
		if (visibilityState == "closed")
		{
			toolbox.style.display = "block";
		}
		else
		{
			toolbox.style.display = "none";
		}
    }
}
DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.inheritsFrom(DotNetNuke.UI.WebControls.Widgets.BaseWidget);
DotNetNuke.UI.WebControls.Widgets.VisibilityWidget.registerClass("DotNetNuke.UI.WebControls.Widgets.VisibilityWidget", DotNetNuke.UI.WebControls.Widgets.BaseWidget);
DotNetNuke.UI.WebControls.Widgets.renderWidgetType("DotNetNuke.UI.WebControls.Widgets.VisibilityWidget");
// END: VisibilityWidget class
