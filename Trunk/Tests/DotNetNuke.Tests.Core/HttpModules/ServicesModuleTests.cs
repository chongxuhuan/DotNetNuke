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