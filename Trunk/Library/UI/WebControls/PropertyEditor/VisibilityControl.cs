#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{

	/// <summary>
	/// The VisibilityControl control provides a base control for defining visibility
	/// options
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	///     [cnurse]	05/03/2006	created
	/// </history>
	[ToolboxData("<{0}:VisibilityControl runat=server></{0}:VisibilityControl>")]
	public class VisibilityControl : WebControl, IPostBackDataHandler, INamingContainer
	{

		#region Public Properties
		
		/// <summary>
		/// Caption
		/// </summary>
		/// <value>A string representing the Name of the property</value>
		/// <history>
		///     [cnurse]	05/08/2006	created
		/// </history>
		public string Caption { get; set; }

		/// <summary>
		/// Name is the name of the field as a string
		/// </summary>
		/// <value>A string representing the Name of the property</value>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		public string Name { get; set; }

		/// <summary>
		/// StringValue is the value of the control expressed as a String
		/// </summary>
		/// <value>A string representing the Value</value>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		public object Value { get; set; }
		
		#endregion

		#region IPostBackDataHandler Members

		/// <summary>
		/// LoadPostData loads the Post Back Data and determines whether the value has change
		/// </summary>
		/// <param name="postDataKey">A key to the PostBack Data to load</param>
		/// <param name="postCollection">A name value collection of postback data</param>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			var dataChanged = false;
			var presentValue = Convert.ToString(Value);
			var postedValue = postCollection[postDataKey];
			if (!presentValue.Equals(postedValue))
			{
				Value = postedValue;
				dataChanged = true;
			}
			return dataChanged;
		}

		/// <summary>
		/// RaisePostDataChangedEvent runs when the PostBackData has changed.  It triggers
		/// a ValueChanged Event
		/// </summary>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		public void RaisePostDataChangedEvent()
		{
			//Raise the VisibilityChanged Event
			int intValue = Convert.ToInt32(Value);
			var args = new PropertyEditorEventArgs(Name);
			args.Value = Enum.ToObject(typeof (UserVisibilityMode), intValue);
			OnVisibilityChanged(args);
		}

		#endregion
		
		#region Events

		public event PropertyChangedEventHandler VisibilityChanged;
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// OnVisibilityChanged runs when the Visibility has changed.  It raises the VisibilityChanged
		/// Event
		/// </summary>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		protected virtual void OnVisibilityChanged(PropertyEditorEventArgs e)
		{
			if (VisibilityChanged != null)
			{
				VisibilityChanged(this, e);
			}
		}

		/// <summary>
		/// Render renders the control
		/// </summary>
		/// <param name="writer">A HtmlTextWriter.</param>
		/// <history>
		///     [cnurse]	05/03/2006	created
		/// </history>
		protected override void Render(HtmlTextWriter writer)
		{
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "dnnFormVisibility");
			var propValue = (UserVisibilityMode)(Convert.ToInt32(Value));

			writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
			writer.RenderBeginTag(HtmlTextWriterTag.Select);

			writer.AddAttribute(HtmlTextWriterAttribute.Value, "0");
			if ((propValue == UserVisibilityMode.AllUsers))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Selected, "true");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Option);
			writer.Write(Localization.GetString("Public").Trim());
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Value, "1");
			if ((propValue == UserVisibilityMode.MembersOnly))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Selected, "true");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Option);
			writer.Write(Localization.GetString("MembersOnly"));
			writer.RenderEndTag();

			writer.AddAttribute(HtmlTextWriterAttribute.Value, "2");
			if ((propValue == UserVisibilityMode.AdminOnly))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Selected, "true");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Option);
			writer.Write(Localization.GetString("AdminOnly"));
			writer.RenderEndTag();

			writer.RenderEndTag();
		}
		
		#endregion
	}
}
