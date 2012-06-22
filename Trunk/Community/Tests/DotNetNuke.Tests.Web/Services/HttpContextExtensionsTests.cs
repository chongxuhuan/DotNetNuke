using System;
using System.Collections.Specialized;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Web.Services;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Web.Services
{
    [TestFixture]
    public class HttpContextExtensionsTests
    {
        private const string ModuleIdKey = "ModuleId";
        private const string ValidTabId = "99";
        private const string ValidTabId2 = "66";
        private const string ValidModuleId = "888";
        private const string ValidModuleId2 = "777";
        private const string TabIdKey = "TabId";

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mockHttpContext = HttpContextHelper.RegisterMockHttpContext();
            _httpContext = _mockHttpContext.Object;
        }

        #endregion

        private Mock<HttpContextBase> _mockHttpContext;
        private HttpContextBase _httpContext;

        [Test]
        public void ModuleIdInHeader()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection {{ModuleIdKey, ValidModuleId}});
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection());

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId), _httpContext.FindModuleId());
        }

        [Test]
        public void ModuleIdInHeaderTakesPriority()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection {{ModuleIdKey, ValidModuleId}});
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection { { ModuleIdKey, ValidModuleId2 } });

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId), _httpContext.FindModuleId());
        }

        [Test]
        public void ModuleIdInParams()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection());
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection {{ModuleIdKey, ValidModuleId}});

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId), _httpContext.FindModuleId());
        }

        [Test]
        public void NoModuleIdReturnsNullInteger()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection());
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection());

            //Act & Assert
            Assert.AreEqual(Null.NullInteger, _httpContext.FindModuleId());
        }

        [Test]
        public void NoTabIdReturnsNullInteger()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection());
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection());

            //Act & Assert
            Assert.AreEqual(Null.NullInteger, _httpContext.FindTabId());
        }

        [Test]
        public void TabIdInHeader()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection {{TabIdKey, ValidTabId}});
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection());

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId), _httpContext.FindTabId());
        }

        [Test]
        public void TabIdInHeaderTakesPriority()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection { { TabIdKey, ValidTabId } });
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection {{TabIdKey, ValidTabId2}});

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId), _httpContext.FindTabId());
        }

        [Test]
        public void TabIdInParams()
        {
            //Arrange
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(new NameValueCollection());
            _mockHttpContext.Setup(x => x.Request.Params).Returns(new NameValueCollection {{TabIdKey, ValidTabId}});

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId), _httpContext.FindTabId());
        }
    }
}