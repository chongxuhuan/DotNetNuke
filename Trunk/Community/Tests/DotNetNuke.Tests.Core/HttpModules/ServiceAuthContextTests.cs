using System.Collections.Generic;
using System.Web;
using DotNetNuke.HttpModules.Services.Internal;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Core.HttpModules
{
    [TestFixture]
    public class ServiceAuthContextTests
    {
        [Test]
        public void DoA401Persists()
        {
            //Arrange
            var context = new Mock<HttpContextBase>();
            var items = new Dictionary<string, object>();
            context.Setup(x => x.Items).Returns(items);

            var initialContext = new ServicesContextWrapper(context.Object);

            //Act
            initialContext.DoA401 = true;

            var resultantContext = new ServicesContextWrapper(context.Object);

            //Assert
            Assert.IsTrue(resultantContext.DoA401);

        }

        [Test]
        public void DoA401Clears()
        {
            //Arrange
            var context = new Mock<HttpContextBase>();
            var items = new Dictionary<string, object>();
            context.Setup(x => x.Items).Returns(items);

            var initialContext = new ServicesContextWrapper(context.Object);

            //Act
            initialContext.DoA401 = true;
            initialContext.DoA401 = false;

            var resultantContext = new ServicesContextWrapper(context.Object);

            //Assert
            Assert.IsFalse(resultantContext.DoA401);

        }

        [Test]
        public void IsStalePersists()
        {
            //Arrange
            var context = new Mock<HttpContextBase>();
            var items = new Dictionary<string, object>();
            context.Setup(x => x.Items).Returns(items);

            var intialContext = new ServicesContextWrapper(context.Object);

            //Act
            intialContext.IsStale = true;

            var resultantContext = new ServicesContextWrapper(context.Object);

            //Assert
            Assert.IsTrue(resultantContext.IsStale);

        }

        [Test]
        public void IsStaleClears()
        {
            //Arrange
            var context = new Mock<HttpContextBase>();
            var items = new Dictionary<string, object>();
            context.Setup(x => x.Items).Returns(items);

            var intialContext = new ServicesContextWrapper(context.Object);

            //Act
            intialContext.IsStale = true;
            intialContext.IsStale = false;

            var resultantContext = new ServicesContextWrapper(context.Object);

            //Assert
            Assert.IsFalse(resultantContext.IsStale);

        }
    }
}