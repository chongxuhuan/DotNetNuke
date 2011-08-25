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

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mobile;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

namespace DotNetNuke.Tests.Core.Services.Mobile
{
	/// <summary>
	///   Summary description for RedirectionControllerTests
	/// </summary>
	[TestFixture]
	public class RedirectionControllerTests
	{
		#region "Private Properties"

		private Mock<DataProvider> _dataProvider;
		private RedirectionController _controller;

		private DataTable _dtRedirections;
		private DataTable _dtRules;
		private IDictionary<string, string> _userAgents;

		private String wurflDataFilePath = "..\\..\\..\\..\\Website\\App_Data\\WURFLDeviceDatabase\\wurfl-latest.zip";
		private String wurflPatchFilePath = "..\\..\\..\\..\\Website\\App_Data\\WURFLDeviceDatabase\\web_browsers_patch.xml";

		#endregion

		#region "Set Up"

		[SetUp]
		public void SetUp()
		{
			ComponentFactory.Container = new SimpleContainer();
			_dataProvider = MockComponentProvider.CreateDataProvider();
			MockComponentProvider.CreateDataCacheProvider();
			MockComponentProvider.CreateEventLogController();

			_controller = new RedirectionController();

			SetupDataProvider();
			SetupClientCapabilityProvider();
			SetupUserAgents();
		}

		#endregion

		#region "Tests"

		#region "CURD API Tests"

		[Test]
		public void Test_Add_Valid_Redirection()
		{
			var redirection = new Redirection { Name = "Test R", PortalId = 0, SortOrder = 1, SourceTabId = -1, Type = RedirectionType.MobilePhone, TargetType = TargetType.Portal, TargetValue = 2 };
			_controller.Save(redirection);

			var dataReader = _dataProvider.Object.GetRedirections(0);
			var affectedCount = 0;
			while (dataReader.Read())
			{
				affectedCount++;
			}
			Assert.AreEqual(1, affectedCount);
		}

		[Test]
		public void Test_Add_ValidRedirection_With_Rules()
		{
			var redirection = new Redirection { Name = "Test R", PortalId = 0, SortOrder = 1, SourceTabId = -1, IncludeChildTabs = true, Type = RedirectionType.Other, TargetType = TargetType.Portal, TargetValue = 2 };
			redirection.MatchRules.Add(new MatchRules { Capability = "Platform", Expression = "IOS" });
			redirection.MatchRules.Add(new MatchRules { Capability = "Version", Expression = "5" });
			_controller.Save(redirection);

			var dataReader = _dataProvider.Object.GetRedirections(0);
			var affectedCount = 0;
			while (dataReader.Read())
			{
				affectedCount++;
			}
			Assert.AreEqual(1, affectedCount);

			var getRe = _controller.GetRedirectionsByPortal(0)[0];
			Assert.AreEqual(2, getRe.MatchRules.Count);
		}

		[Test]
		public void Test_Get_Redirections()
		{
			PrepareData();

			IList<IRedirection> list = _controller.GetRedirectionsByPortal(0);

			Assert.AreEqual(7, list.Count);
		}

		[Test]
		public void Test_Delete_Redirections()
		{
			PrepareData();
			_controller.Delete(0, 1);

			IList<IRedirection> list = _controller.GetRedirectionsByPortal(0);

			Assert.AreEqual(6, list.Count);
		}

		#endregion

		#region "Get Redirections URL Tests"

		[Test]
		public void Test_GetRedirectionUrl_For_iPhone_Request()
		{
		}

		#endregion

		#endregion

		#region "Private Methods"

		private void SetupUserAgents()
		{
			_userAgents = new Dictionary<string, string>();
			_userAgents.Add("iPhone4", "Mozilla/5.0 (iPod; U; CPU iPhone OS 4_0 like Mac OS X; en-us) AppleWebKit/532.9 (KHTML, like Gecko) Version/4.0.5 Mobile/8A293 Safari/6531.22.7");
		}

		private void SetupDataProvider()
		{
			_dataProvider.Setup(d => d.GetProviderPath()).Returns("");

			_dtRedirections = new DataTable("Redirections");
			var pkCol = _dtRedirections.Columns.Add("Id", typeof(int));
			_dtRedirections.Columns.Add("PortalId", typeof(int));
			_dtRedirections.Columns.Add("Name", typeof(string));
			_dtRedirections.Columns.Add("Type", typeof(int));
			_dtRedirections.Columns.Add("SortOrder", typeof(int));
			_dtRedirections.Columns.Add("SourceTabId", typeof(int));
			_dtRedirections.Columns.Add("IncludeChildTabs", typeof(bool));
			_dtRedirections.Columns.Add("TargetType", typeof(int));
			_dtRedirections.Columns.Add("TargetValue", typeof(object));
			_dtRedirections.Columns.Add("Enabled", typeof(bool));

			_dtRedirections.PrimaryKey = new[] { pkCol };

			_dtRules = new DataTable("Rules");
			var pkCol1 = _dtRules.Columns.Add("Id", typeof(int));
			_dtRules.Columns.Add("RedirectionId", typeof(int));
			_dtRules.Columns.Add("Capability", typeof(string));
			_dtRules.Columns.Add("Expression", typeof(string));

			_dtRules.PrimaryKey = new[] { pkCol1 };

			_dataProvider.Setup(d =>
								d.SaveRedirection(It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<string>(),
								It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<bool>(),
								It.IsAny<int>(),
								It.IsAny<object>(),
								It.IsAny<bool>(),
								It.IsAny<int>())).Returns<int, int, string, int, int, int, bool, int, object, bool, int>(
															(id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled, userId) =>
															{
																if (id == -1)
																{
																	if (_dtRedirections.Rows.Count == 0)
																	{
																		id = 1;
																	}
																	else
																	{
																		id = Convert.ToInt32(_dtRedirections.Select("", "Id Desc")[0]["Id"]) + 1;
																	}

																	var row = _dtRedirections.NewRow();
																	row["Id"] = id;
																	row["PortalId"] = portalId;
																	row["name"] = name;
																	row["type"] = type;
																	row["sortOrder"] = sortOrder;
																	row["sourceTabId"] = sourceTabId;
																	row["includeChildTabs"] = includeChildTabs;
																	row["targetType"] = targetType;
																	row["targetValue"] = targetValue;
																	row["enabled"] = enabled;

																	_dtRedirections.Rows.Add(row);
																}
																else
																{
																	var rows = _dtRedirections.Select("Id = " + id);
																	if (rows.Length == 1)
																	{
																		var row = rows[0];

																		row["name"] = name;
																		row["type"] = type;
																		row["sortOrder"] = sortOrder;
																		row["sourceTabId"] = sourceTabId;
																		row["includeChildTabs"] = includeChildTabs;
																		row["targetType"] = targetType;
																		row["targetValue"] = targetValue;
																		row["enabled"] = enabled;
																	}
																}

																return id;
															});

			_dataProvider.Setup(d => d.GetRedirections(It.IsAny<int>())).Returns<int>(GetRedirectionsCallBack);
			_dataProvider.Setup(d => d.DeleteRedirection(It.IsAny<int>())).Callback<int>((id) =>
			{
				var rows = _dtRedirections.Select("Id = " + id);
				if (rows.Length == 1)
				{
					_dtRedirections.Rows.Remove(rows[0]);
				}
			});

			_dataProvider.Setup(d => d.SaveRedirectionRule(It.IsAny<int>(),
				It.IsAny<int>(),
				It.IsAny<string>(),
				It.IsAny<string>())).Callback<int, int, string, string>((id, rid, capbility, expression) =>
				{
					if (id == -1)
					{
						if (_dtRules.Rows.Count == 0)
						{
							id = 1;
						}
						else
						{
							id = Convert.ToInt32(_dtRules.Select("", "Id Desc")[0]["Id"]) + 1;
						}

						var row = _dtRules.NewRow();
						row["Id"] = id;
						row["RedirectionId"] = rid;
						row["capability"] = capbility;
						row["expression"] = expression;

						_dtRules.Rows.Add(row);
					}
					else
					{
						var rows = _dtRules.Select("Id = " + id);
						if (rows.Length == 1)
						{
							var row = rows[0];

							row["capability"] = capbility;
							row["expression"] = expression;
						}
					}
				});

			_dataProvider.Setup(d => d.GetRedirectionRules(It.IsAny<int>())).Returns<int>(GetRedirectionRulesCallBack);
			_dataProvider.Setup(d => d.DeleteRedirectionRule(It.IsAny<int>())).Callback<int>((id) =>
			{
				var rows = _dtRules.Select("Id = " + id);
				if (rows.Length == 1)
				{
					_dtRules.Rows.Remove(rows[0]);
				}
			});

			_dataProvider.Setup(d => d.GetPortal(It.IsAny<int>(), It.IsAny<string>())).Returns<int, string>(GetPortalCallBack);
		}

		private void SetupClientCapabilityProvider()
		{
			var provider = MockComponentProvider.CreateNew<ClientCapabilityProvider>();

			provider.Setup(p => p.GetClientCapability(It.IsAny<string>())).Returns<string>(GetClientCapabilityCallBack);
		}

		private IDataReader GetRedirectionsCallBack(int portalId)
		{
			var dtCheck = _dtRedirections.Clone();
			foreach (var row in _dtRedirections.Select("PortalId = " + portalId))
			{
				dtCheck.Rows.Add(row.ItemArray);
			}

			return dtCheck.CreateDataReader();
		}

		private IDataReader GetRedirectionRulesCallBack(int rid)
		{
			var dtCheck = _dtRules.Clone();
			foreach (var row in _dtRules.Select("RedirectionId = " + rid))
			{
				dtCheck.Rows.Add(row.ItemArray);
			}

			return dtCheck.CreateDataReader();
		}

		private IDataReader GetPortalCallBack(int portalId, string culture)
		{
			DataTable table = new DataTable("Portal");

			var cols = new string[]
			           	{
			           		"PortalID", "PortalGroupID", "PortalName", "LogoFile", "FooterText", "ExpiryDate", "UserRegistration", "BannerAdvertising", "AdministratorId", "Currency", "HostFee",
			           		"HostSpace", "PageQuota", "UserQuota", "AdministratorRoleId", "RegisteredRoleId", "Description", "KeyWords", "BackgroundFile", "GUID", "PaymentProcessor", "ProcessorUserId",
			           		"ProcessorPassword", "SiteLogHistory", "Email", "DefaultLanguage", "TimezoneOffset", "AdminTabId", "HomeDirectory", "SplashTabId", "HomeTabId", "LoginTabId", "RegisterTabId",
			           		"UserTabId", "SearchTabId", "SuperTabId", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "CultureCode"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}

			table.Rows.Add(portalId,null,"My Website","Logo.png","Copyright 2011 by DotNetNuke Corporation",null,"2","0","2","USD","0","0","0","0","0","1","My Website","DotNetNuke, DNN, Content, Management, CMS",null,"1057AC7A-3C08-4849-A3A6-3D2AB4662020",null,null,null,"0","admin@change.me","en-US","-8","58","Portals/0",null,"55",null,null,"57","56","7","-1","2011-08-25 07:34:11","-1","2011-08-25 07:34:29",culture);

			return table.CreateDataReader();
		}

		private IClientCapability GetClientCapabilityCallBack(string userAgent)
		{
			return null;
		}

		private void PrepareData()
		{
			//id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled
			_dtRedirections.Rows.Add(1, 0, "R4", 4, 4, -1, 0, 1, "1", 1);
			_dtRedirections.Rows.Add(2, 0, "R2", 2, 2, -1, 0, 1, "1", 1);
			_dtRedirections.Rows.Add(3, 0, "R3", 3, 3, -1, 0, 1, "1", 1);
			_dtRedirections.Rows.Add(4, 0, "R1", 1, 1, -1, 0, 1, "1", 1);
			_dtRedirections.Rows.Add(5, 0, "R5", 1, 5, 55, 1, 1, "1", 1);
			_dtRedirections.Rows.Add(6, 0, "R6", 1, 6, -1, 0, 2, 55, 1);
			_dtRedirections.Rows.Add(7, 0, "R7", 1, 7, -1, 0, 3, "http://www.dotnetnuke.com", 1);

			//id, redirectionId, capability, expression
			_dtRules.Rows.Add(1, 1, "mobile_browser", "Safari");
			_dtRules.Rows.Add(2, 1, "device_os_version", "4.0");

			_dtRedirections.Rows.Add(8, 1, "R8", (int)RedirectionType.MobilePhone, 1, -1, true, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(9, 1, "R9", (int)RedirectionType.Tablet, 1, -1, true, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(10, 1, "R10", (int)RedirectionType.AllMobile, 1, -1, true, (int)TargetType.Portal, 2, true);
		}

		#endregion
	}
}