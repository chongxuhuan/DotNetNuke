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
using System.Data;
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
		private IDictionary<string, string> _requestDics;

		[SetUp]
		public void SetUp()
		{
			_requestDics = new Dictionary<string, string>();
			_requestDics.Add("Empty", string.Empty);
			_requestDics.Add("Valid", "vlXgu64BQGFSQrY0ZcJBZASMvYvTHu9GQ0YM9rjPSso.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsIjAiOiJwYXlsb2FkIiwidXNlcl9pZCI6ICIxIiwiZXhwaXJlcyI6IjEzMjUzNzU5OTkifQ==");
			_requestDics.Add("Invalid", "Invalid Content");
		}

		[Test]
		public void Test_With_Empty_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Empty"]);
			Assert.IsNull(request);
		}

		[Test]
		public void Test_With_Invalid_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Invalid"]);
			Assert.IsNull(request);
		}

		[Test]
		public void Test_With_Valid_Request_String()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(_requestDics["Valid"]);
			Assert.AreEqual(true, request.IsValid);
		}

		[Test]
		public void Test_With_Empty_Request()
		{
			var request = FacebookRequestController.GetFacebookDetailsFromRequest(null as HttpRequest);
			Assert.IsNull(request);
		}

		[Test]
		public void Test_With_Get_Request()
		{
			
		}

		[Test]
		public void Test_With_Post_Invalid_Request()
		{

		}

		[Test]
		public void Test_With_Post_Valid_Request()
		{

		}
	}
}