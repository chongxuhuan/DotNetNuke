using System;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using NUnit.Framework;

namespace DotNetNuke.Tests.Web.Api
{
    [TestFixture]
    public class HttpRequestMessageExtensionsTests
    {
        private const string ModuleIdKey = "ModuleId";
        private const string ValidTabId = "99";
        private const string ValidTabId2 = "66";
        private const string ValidModuleId = "888";
        private const string ValidModuleId2 = "777";
        private const string TabIdKey = "TabId";

        [Test]
        public void ModuleIdInHeader()
        {
            //Arrange
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add(ModuleIdKey, ValidModuleId);

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId), requestMessage.FindModuleId());
        }

        [Test]
        public void ModuleIdInHeaderTakesPriority()
        {
            //Arrange
            var requestMessage = CreateRequestMessageWithUrl(ModuleIdKey, ValidModuleId);
            requestMessage.Headers.Add(ModuleIdKey, ValidModuleId2);
            
            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId2), requestMessage.FindModuleId());
        }

        [Test]
        public void ModuleIdInParams()
        {
            //Arrange
            var requestMessage = CreateRequestMessageWithUrl(ModuleIdKey, ValidModuleId);

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidModuleId), requestMessage.FindModuleId());
        }


        [Test]
        public void NoModuleIdReturnsNullInteger()
        {
            //Arrange
            var requestMessage = new HttpRequestMessage();

            //Act & Assert
            Assert.AreEqual(Null.NullInteger, requestMessage.FindModuleId());
        }

        [Test]
        public void NoTabIdReturnsNullInteger()
        {
            //Arrange
            var requestMessage = new HttpRequestMessage();

            //Act & Assert
            Assert.AreEqual(Null.NullInteger, requestMessage.FindTabId());
        }

        [Test]
        public void TabIdInHeader()
        {
            //Arrange
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add(TabIdKey, ValidTabId);

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId), requestMessage.FindTabId());
        }

        [Test]
        public void TabIdInHeaderTakesPriority()
        {
            //Arrange
            var requestMessage = CreateRequestMessageWithUrl(TabIdKey, ValidTabId);
            requestMessage.Headers.Add(TabIdKey, ValidTabId2);

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId2), requestMessage.FindTabId());
        }

        [Test]
        public void TabIdInParams()
        {
            //Arrange
            var requestMessage = CreateRequestMessageWithUrl(TabIdKey, ValidTabId);

            //Act & Assert
            Assert.AreEqual(Convert.ToInt32(ValidTabId), requestMessage.FindTabId());
        }

        private static HttpRequestMessage CreateRequestMessageWithUrl(string key, string value)
        {
            return new HttpRequestMessage(HttpMethod.Get, string.Format("http://foo.com?{0}={1}", key, value));
        }
    }
}