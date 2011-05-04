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
using System.Collections.Generic;
using System.Data;
using System.Linq;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

namespace DotNetNuke.Tests.Core.Controllers
{
    [TestFixture]
    public class HostControllerTest
    {
        private DataTable _hostSettingsTable;
        private Mock<CachingProvider> _mockCache;
        private Mock<DataProvider> _mockData;


        [SetUp]
        public void SetUp()
        {
            _mockCache = MockComponentProvider.CreateDataCacheProvider();
            MockComponentProvider.CreateEventLogController();


            _hostSettingsTable = new DataTable("HostSettings");

            var nameCol = _hostSettingsTable.Columns.Add("SettingName");
            _hostSettingsTable.Columns.Add("SettingValue");
            _hostSettingsTable.Columns.Add("SettingIsSecure");
            _hostSettingsTable.PrimaryKey = new[] {nameCol};

            _hostSettingsTable.Rows.Add("String_1_S", "String_1_S", true);
            _hostSettingsTable.Rows.Add("String_2_S", "String_1_S", true);
            _hostSettingsTable.Rows.Add("String_3_U", "Value_3_U", false);
            _hostSettingsTable.Rows.Add("String_4_U", "Value_4_U", false);
            _hostSettingsTable.Rows.Add("Int_5_U", "5", false);
            _hostSettingsTable.Rows.Add("Int_6_S", "6", true);
            _hostSettingsTable.Rows.Add("Double_7_S", "7", true);
            _hostSettingsTable.Rows.Add("Double_8_U", "8", false);
            _hostSettingsTable.Rows.Add("Bool_9_U", false, false);
            _hostSettingsTable.Rows.Add("Bool_10_S", false, true);


            _mockData = MockComponentProvider.CreateDataProvider();
            _mockData.Setup(c => c.GetHostSettings()).Returns(_hostSettingsTable.CreateDataReader());
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }

        private string GetValue(string key)
        {
            return _hostSettingsTable.Rows.Find(key)["SettingValue"].ToString();
        }

        #region Get Dictionaries

        [Test]
        public void HostController_GetSettings_GetList()
        {
            //Arrange
            var excpectedDic = new Dictionary<string, ConfigurationSetting>();

            foreach (DataRow row in _hostSettingsTable.Rows)
            {
                var conf = new ConfigurationSetting();
                conf.Key = row["SettingName"].ToString();
                conf.Value = row["SettingValue"].ToString();
                bool IsSecure;
                Boolean.TryParse(row["SettingIsSecure"].ToString(), out IsSecure);
                conf.IsSecure = IsSecure;
                excpectedDic.Add(conf.Key, conf);
            }

            //Act
            var settingsDic = HostController.Instance.GetSettings();


            //Assert
            Assert.AreElementsEqualIgnoringOrder(excpectedDic.Values, settingsDic.Values, new ConfigurationSettingsComparer());

            //Make sure the keys in the dic correspond to keys in value

            foreach (var currentConfig in settingsDic)
            {
                Assert.AreEqual(currentConfig.Key, currentConfig.Value.Key);
            }
        }

        [Test]
        public void HostController_GetSettingsDictionary_GetList()
        {
            //Arrange
            //Convert table to Dictionary<string,string>
            var excpectedDic = _hostSettingsTable.Rows.Cast<DataRow>().ToDictionary(row => row["SettingName"].ToString(), row => row["SettingValue"].ToString());

            //Act
            var settingsDic = HostController.Instance.GetSettingsDictionary();


            //Assert
            Assert.AreElementsEqualIgnoringOrder(excpectedDic.Values, settingsDic.Values);
        }

        #endregion

        #region Update Value

        [Test]
        public void HostController_Update_ExistingValue()
        {
            //Arrange
            const string key = "String_1_S";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(key).Read()).Returns(true);

            //Act
            HostController.Instance.Update(key, value);

            //Assert
            _mockData.Verify(c => c.UpdateHostSetting(key, value, false, It.IsAny<int>()), Times.Exactly(1));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_ExistingValue_ResetCache()
        {
            //Arrange
            const string key = "String_1_S";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(key).Read()).Returns(true);

            //Act
            HostController.Instance.Update(key, value);

            //Assert
            _mockData.Verify(c => c.UpdateHostSetting(key, value, false, It.IsAny<int>()), Times.Exactly(1));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_ExistingValue_ResetCache_With_Overload()
        {
            //Arrange
            const string key = "String_1_S";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(key).Read()).Returns(true);

            //Act
            HostController.Instance.Update(key, value, true);

            //Assert
            _mockData.Verify(c => c.UpdateHostSetting(key, value, false, It.IsAny<int>()), Times.Exactly(1));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_ExistingValue_Dont_Reset_Cache()
        {
            //Arrange
            const string key = "String_1_S";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(key).Read()).Returns(true);

            //Act
            HostController.Instance.Update(key, value, false);

            //Assert
            _mockData.Verify(c => c.UpdateHostSetting(key, value, false, It.IsAny<int>()));
            _mockCache.Verify(c => c.Clear(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void HostController_Update_Dictionary()
        {
            //Arrange
            var settings = new Dictionary<string, string>
                                                      {
                                                          {"String_1_S", "MyValue"},
                                                          {"String_2_S", "MyNextValue"},
                                                          {"String_3_U", "MyOtherValue"},
                                                      };

            //Act
            HostController.Instance.Update(settings);

            //Assert
            _mockData.Verify(c => c.UpdateHostSetting("String_1_S", "MyValue", false, It.IsAny<int>()), Times.Exactly(1));
            _mockData.Verify(c => c.UpdateHostSetting("String_2_S", "MyNextValue", false, It.IsAny<int>()), Times.Exactly(1));
            _mockData.Verify(c => c.UpdateHostSetting("String_3_U", "MyOtherValue", false, It.IsAny<int>()), Times.Exactly(1));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
          
        }

        #endregion

        #region Create Value

        [Test]
        public void HostController_Update_NewValue()
        {
            //Arrange
            const string key = "MyKey";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(It.IsAny<string>()).Read()).Returns(false);

            //Act
            HostController.Instance.Update(key, value);

            //Assert
            _mockData.Verify(c => c.AddHostSetting(key, value, false, It.IsAny<int>()));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_NewValue_ResetCache_With_Overload()
        {
            //Arrange
            const string key = "MyKey";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(It.IsAny<string>()).Read()).Returns(false);

            //Act
            HostController.Instance.Update(key, value, true);

            //Assert
            _mockData.Verify(c => c.AddHostSetting(key, value, false, It.IsAny<int>()));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_NewValue_ResetCache()
        {
            //Arrange
            const string key = "MyKey";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(It.IsAny<string>()).Read()).Returns(false);

            //Act
            HostController.Instance.Update(key, value);

            //Assert
            _mockData.Verify(c => c.AddHostSetting(key, value, false, It.IsAny<int>()));
            _mockCache.Verify(c => c.Clear("Host", ""), Times.Exactly(1));
        }

        [Test]
        public void HostController_Update_NewValue_Dont_Reset_Cache()
        {
            //Arrange
            const string key = "MyKey";
            const string value = "MyValue";
            _mockData.Setup(c => c.GetHostSetting(It.IsAny<string>()).Read()).Returns(false);

            //Act
            HostController.Instance.Update(key, value, false);

            //Assert
            _mockData.Verify(c => c.AddHostSetting(key, value, false, It.IsAny<int>()));
            _mockCache.Verify(c => c.Clear(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region GetString()

        [Test]
        [Row("String_1_S")]
        [Row("String_2_S")]
        [Row("String_3_U")]
        [Row("String_4_U")]
        public void HostController_GetString_If_Key_Exists(string key)
        {
            Assert.AreEqual(HostController.Instance.GetString(key), GetValue(key));
            Assert.AreEqual(HostController.Instance.GetString(key, "Hello Default"), GetValue(key));
        }


        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetString_InvalidKey_Returns_Null_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetString(key), Null.NullString);
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetString_InvalidKey_Returns_Default_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetString(key, "Hello Default"), "Hello Default");
        }

        [Test]
        [Row("")]
        [Row(null)]
        [ExpectedArgumentException]
        public void HostController_GetString_NullEmpty(string key)
        {
            HostController.Instance.GetString(key);
        }

        #endregion

        #region GetInteger()

        [Test]
        [Row("Int_5_U")]
        [Row("Int_6_S")]
        public void HostController_GetInteger_If_Key_Exists(string key)
        {
            int s = HostController.Instance.GetInteger(key);
            Assert.AreEqual(s.ToString(), GetValue(key));
            Assert.AreEqual(HostController.Instance.GetInteger(key, 12).ToString(), GetValue(key));
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetInteger_InvalidKey_Returns_Null_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetInteger(key), Null.NullInteger);
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetInteger_InvalidKey_Returns_Default_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetInteger(key, 6969), 6969);
        }

        [Test]
        [Row("")]
        [Row(null)]
        [ExpectedArgumentException]
        public void HostController_GetInteger_NullEmpty(string key)
        {
            HostController.Instance.GetInteger(key);
        }

        #endregion

        #region GetBoolean()

        [Test]
        [Row("Bool_9_U")]
        [Row("Bool_10_S")]
        public void HostController_GetBoolean_If_Key_Exists(string key)
        {
            Assert.AreEqual(HostController.Instance.GetBoolean(key).ToString(), GetValue(key));
            Assert.AreEqual(HostController.Instance.GetBoolean(key, false).ToString(), GetValue(key));
            Assert.AreEqual(HostController.Instance.GetBoolean(key, true).ToString(), GetValue(key));
        }


        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetBoolean_InvalidKey_Returns_Null_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetBoolean(key), Null.NullBoolean);
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetBoolean_InvalidKey_Returns_Default_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetBoolean(key, true), true);
            Assert.AreEqual(HostController.Instance.GetBoolean(key, false), false);
        }


        [Test]
        [Row("")]
        [Row(null)]
        [ExpectedArgumentException]
        public void HostController_GetBoolean_NullEmpty(string key)
        {
            HostController.Instance.GetBoolean(key);
        }

        #endregion

        #region GetDouble()

        [Test]
        [Row("Double_7_S")]
        [Row("Double_8_U")]
        public void HostController_GetDouble_If_Key_Exists(string key)
        {
            Double s = HostController.Instance.GetDouble(key);
            Assert.AreEqual(s.ToString(), GetValue(key));
            Assert.AreEqual(HostController.Instance.GetDouble(key, 54.54).ToString(), GetValue(key));
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetDouble_InvalidKey_Returns_Null_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetDouble(key), Null.NullDouble);
        }

        [Test]
        [Row("BadKey1")]
        [Row("AAAAAAA")]
        public void HostController_GetDouble_InvalidKey_Returns_Default_Value(string key)
        {
            Assert.AreEqual(HostController.Instance.GetDouble(key, 21.58), 21.58);
        }

        [Test]
        [Row("")]
        [Row(null)]
        [ExpectedArgumentException]
        public void HostController_GetDouble_NullEmpty(string key)
        {
            HostController.Instance.GetDouble(key);
        }

        #endregion
    }
}