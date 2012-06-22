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

using System;
using System.Configuration;
using System.IO;
using System.Net;

using NUnit.Framework;

namespace DotNetNuke.Website.Specs.ServiceLayer
{
    [TestFixture]
    public class ServiceIntegrationTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _serviceCaller = new ServiceCaller {Module = "Test", Controller = "Test"};

            EnsureServiceIsInstalled();
        }

        private void EnsureServiceIsInstalled()
        {
            const string fileName = "DotNetNuke.Tests.TestServices.dll";

            var binPath = Path.Combine(ConfigurationManager.AppSettings["DefaultPhysicalAppPath"], "bin\\" + fileName);
            
            if(!File.Exists(binPath))
            {
                var resourcesPath = Path.Combine(Environment.CurrentDirectory, "Resources\\" + fileName);
                File.Copy(resourcesPath, binPath);
            }
        }

        #endregion

        private const string SuccessMessage = "success";
        private ServiceCaller _serviceCaller;

        [Test]
        public void AuthWithNoCredentialsGives401()
        {
            //Arrange
            _serviceCaller.Action = "Authorized";
            _serviceCaller.AuthMode = AuthMode.None;

            //Act
            bool caughtException = false;
            try
            {
                _serviceCaller.CallService();
            }
            catch (WebException e)
            {
                //Assert
                StringAssert.Contains("401", e.Message);
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
        }

        [Test]
        public void CanCallAnonymous()
        {
            //Arrange
            _serviceCaller.Action = "Anonymous";

            //Act
            string result = _serviceCaller.CallService();

            //Assert
            Assert.AreEqual(SuccessMessage, result);
        }

        [Test]
        public void CanCallAuthorizedBasic()
        {
            //Arrange
            _serviceCaller.Action = "Authorized";
            _serviceCaller.AuthMode = AuthMode.Basic;

            //Act
            string result = _serviceCaller.CallService();

            //Assert
            Assert.AreEqual(SuccessMessage, result);
        }

        [Test]
        public void CanCallAuthorizedDigest()
        {
            //Arrange
            _serviceCaller.Action = "Authorized";
            _serviceCaller.AuthMode = AuthMode.Digest;

            //Act
            string result = _serviceCaller.CallService();

            //Assert
            Assert.AreEqual(SuccessMessage, result);
        }
    }
}