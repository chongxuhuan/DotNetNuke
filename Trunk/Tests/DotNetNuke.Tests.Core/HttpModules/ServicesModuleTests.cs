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

using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.HttpModules.Services;
using DotNetNuke.HttpModules.Services.Internal;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Core.HttpModules
{
    [TestFixture]
    public class ServicesModuleTests
    {
        [Test]
        public void DoA401CausesStatusCode401()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(x => x.StatusCode);
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            Assert.AreEqual(401, response.Object.StatusCode);
        }

        [Test]
        public void DoA401ClearsContent()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            response.Verify(x => x.ClearContent());
        }

        [Test]
        public void DoA401ClearsHeaders()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            response.Verify(x => x.ClearHeaders());
        }

        [Test]
        public void NoDoA401DoesNothing()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(false);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            response.Verify(x => x.StatusCode, Times.Never());
            response.Verify(x => x.ClearContent(), Times.Never());
            response.Verify(x => x.AppendHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            response.Verify(x => x.AddHeader(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            response.Verify(x => x.Headers, Times.Never());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsStaleSetsStaleNonceIndicator(bool stale)
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.IsStale).Returns(stale);
            context.Setup(x => x.SupportBasicAuth).Returns(false); //avoid basic auth header
            context.Setup(x => x.SupportDigestAuth).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            var regex = string.Format("^Digest.*, stale={0}.*", stale);
            response.Verify(x => x.AppendHeader("WWW-Authenticate", It.IsRegex(regex, RegexOptions.IgnoreCase)));
        }

        [Test]
        public void SupportsBasicAddsBasicHeader()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.SupportBasicAuth).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            response.Verify(x => x.AppendHeader("WWW-Authenticate", It.IsRegex("^Basic.*", RegexOptions.IgnoreCase)));
        }

        [Test]
        public void SupportsDigestAddsDigestHeader()
        {
            //Arrange
            var response = new Mock<HttpResponseBase>();
            var context = new Mock<IServicesContext>();
            context.Setup(x => x.DoA401).Returns(true);
            context.Setup(x => x.SupportDigestAuth).Returns(true);
            context.Setup(x => x.BaseContext.Response).Returns(response.Object);

            //Act
            ServicesModule.CheckForReal401(context.Object);

            //Assert
            response.Verify(x => x.AppendHeader("WWW-Authenticate", It.IsRegex("^Digest.*", RegexOptions.IgnoreCase)));
        }
    }
}