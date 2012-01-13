#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Core.HttpModules
{
    [TestFixture]
    public class ServicesModuleTests
    {
        [Test]
        public void FlagSetCauses401AndResponseClearedAndLocationHeaderRemoved()
        {
            var response = new Mock<HttpResponseBase>();
            var headers = new NameValueCollection {{"Location", "redirected.aspx"}};
            response.Setup(x => x.Headers).Returns(headers);
            
            var items = new Dictionary<string, object> { { "DnnReal401", true } };
            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.Items).Returns(items);

            DotNetNuke.HttpModules.Services.ServicesModule.CheckForReal401(context.Object);

            response.VerifySet(x => x.StatusCode=401);
            response.Verify(x => x.ClearContent());
            Assert.AreEqual(0, headers.Count);  
        }

        [Test]
        public void FlagClearCausesNothing()
        {
            var response = new Mock<HttpResponseBase>();
            var headers = new NameValueCollection { { "Location", "redirected.aspx" } };
            response.Setup(x => x.Headers).Returns(headers);

            var items = new Dictionary<string, object> { { "DnnReal401", false } };
            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.Items).Returns(items);

            DotNetNuke.HttpModules.Services.ServicesModule.CheckForReal401(context.Object);

            response.Verify(x => x.StatusCode, Times.Never());
            response.Verify(x => x.ClearContent(), Times.Never());
            Assert.AreEqual(1, headers.Count);  
        }

        [Test]
        public void FlagAbsentCausesNothing()
        {
            var response = new Mock<HttpResponseBase>();
            var headers = new NameValueCollection { { "Location", "redirected.aspx" } };
            response.Setup(x => x.Headers).Returns(headers);

            var items = new Dictionary<string, object>(); //don't insert the flag at all { { "DnnReal401", false } };
            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.Items).Returns(items);

            DotNetNuke.HttpModules.Services.ServicesModule.CheckForReal401(context.Object);

            response.Verify(x => x.StatusCode, Times.Never());
            response.Verify(x => x.ClearContent(), Times.Never());
            Assert.AreEqual(1, headers.Count);
        }
    }
}