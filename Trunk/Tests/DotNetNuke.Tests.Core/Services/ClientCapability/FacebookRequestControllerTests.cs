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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Web;

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Mobile;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

namespace DotNetNuke.Tests.Core.Services.ClientCapability
{
	/// <summary>
	///   Summary description for FacebookRequestController
	/// </summary>
	[TestFixture]
	public class FacebookRequestControllerTests
	{
		#region "Private Properties"

		private IDictionary<string, string> _requestDics;

		#endregion

		#region "Set Up"

		[SetUp]
		public void SetUp()
		{
			_requestDics = new Dictionary<string, string>();
			_requestDics.Add("Empty", string.Empty);
			_requestDics.Add("Valid", "vlXgu64BQGFSQrY0ZcJBZASMvYvTHu9GQ0YM9rjPSso.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsIjAiOiJwYXlsb2FkIiwidXNlcl9pZCI6ICIxIiwiZXhwaXJlcyI6IjEzMjUzNzU5OTkifQ==");
			_requestDics.Add("Invalid", "Invalid Content");
		}

		#endregion

		#region "Tests"

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Empty_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Empty"]);
			Assert.IsNull(request);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Invalid_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Invalid"]);
			Assert.IsNull(request);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Valid_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Valid"]);
			Assert.AreEqual(true, request.IsValid);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Empty_Request()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(null as HttpRequest);
			Assert.IsNull(request);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Get_Request()
		{
			HttpRequest httpRequest = new HttpRequest("unittest.aspx", "http://localhost/unittest.aspx", "");
			httpRequest.RequestType = "GET";

			var request = FacebookRequestController.GetFacebookDetailsFromRequest(httpRequest);
			Assert.IsNull(request);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Post_Invalid_Request()
		{
			HttpRequest httpRequest = new HttpRequest("unittest.aspx", "http://localhost/unittest.aspx", "");
			httpRequest.RequestType = "POST";
			SetReadonly(httpRequest.Form, false);
			httpRequest.Form.Add("signed_request", _requestDics["Invalid"]);

			var request = FacebookRequestController.GetFacebookDetailsFromRequest(httpRequest);
			Assert.IsNull(request);
		}

		[Test]
		public void FacebookRequestController_GetFacebookDetailsFromRequest_With_Post_Valid_Request()
		{
			HttpRequest httpRequest = new HttpRequest("unittest.aspx", "http://localhost/unittest.aspx", "");
			httpRequest.RequestType = "POST";
			SetReadonly(httpRequest.Form, false);
			httpRequest.Form.Add("signed_request", _requestDics["Valid"]);

			var request = FacebookRequestController.GetFacebookDetailsFromRequest(httpRequest);
			Assert.AreEqual(true, request.IsValid);
		}

		#endregion

		#region "Private Methods"

		private void SetReadonly(NameValueCollection collection, bool readOnly)
		{
			var readOnlyProperty = collection.GetType().GetProperty("IsReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
			if(readOnlyProperty != null)
			{
				readOnlyProperty.SetValue(collection, readOnly, null);
			}
		}

		#endregion
	}
}