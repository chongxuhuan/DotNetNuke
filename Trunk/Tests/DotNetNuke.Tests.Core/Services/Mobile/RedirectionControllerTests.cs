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
using DotNetNuke.Entities.Tabs;
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
		private Mock<DataProvider> _dataProvider;

		private DataTable _dtRedirections;
		private DataTable _dtRules;

		[SetUp]
		public void SetUp()
		{
			ComponentFactory.Container = new SimpleContainer();
			_dataProvider = MockComponentProvider.CreateDataProvider();

			_dtRedirections = new DataTable("Redirections");
			var pkCol = _dtRedirections.Columns.Add("Id", typeof(int));
			_dtRedirections.Columns.Add("PortalId", typeof(int));
			_dtRedirections.Columns.Add("Name", typeof(string));
			_dtRedirections.Columns.Add("Type", typeof(int));
			_dtRedirections.Columns.Add("SortOrder", typeof(int));
			_dtRedirections.Columns.Add("SourceTabId", typeof(int));
			_dtRedirections.Columns.Add("TargetType", typeof(int));
			_dtRedirections.Columns.Add("TargetValue", typeof(int));
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
								It.IsAny<int>(),
								It.IsAny<object>(),
								It.IsAny<bool>(),
								It.IsAny<int>())).Returns<int, int, string, int, int, int, int, object, bool, int>(
															(id, portalId, name, type, sortOrder, sourceTabId, targetType, targetValue, enabled, userId) =>
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
																		row["targetType"] = targetType;
																		row["targetValue"] = targetValue;
																		row["enabled"] = enabled;
																	}
																}

																return id;
															});

			_dataProvider.Setup(d => d.GetRedirections(It.IsAny<int>())).Returns<int>((portalId) => { return GetRedirectionsCallBack(portalId); });
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

			_dataProvider.Setup(d => d.GetRedirectionRules(It.IsAny<int>())).Returns<int>((rid) => { return GetRedirectionRulesCallBack(rid); });
			_dataProvider.Setup(d => d.DeleteRedirectionRule(It.IsAny<int>())).Callback<int>((id) =>
			{
				var rows = _dtRules.Select("Id = " + id);
				if (rows.Length == 1)
				{
					_dtRules.Rows.Remove(rows[0]);
				}
			});
		}

		[Test]
		public void Test_Add_Valid_Redirection()
		{
			var redirection = new Redirection { Name = "Test R", PortalId = 0, SortOrder = 1, SourceTabId = -1, Type = RedirectionType.MobilePhone, TargetType = TargetType.Portal, TargetValue = 2 };
			new RedirectionController().Save(redirection);

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
			var redirection = new Redirection { Name = "Test R", PortalId = 0, SortOrder = 1, SourceTabId = -1, Type = RedirectionType.Other, TargetType = TargetType.Portal, TargetValue = 2 };
			redirection.MatchRules.Add(new MatchRules { Capability = "Platform", Expression = "IOS" });
			redirection.MatchRules.Add(new MatchRules { Capability = "Version", Expression = "5" });
			new RedirectionController().Save(redirection);

			var dataReader = _dataProvider.Object.GetRedirections(0);
			var affectedCount = 0;
			while (dataReader.Read())
			{
				affectedCount++;
			}
			Assert.AreEqual(1, affectedCount);

			var getRe = new RedirectionController().GetRedirectionsByPortal(0)[0];
			Assert.AreEqual(2, getRe.MatchRules.Count);
		}

		[Test]
		public void Test_Get_Redirections()
		{
			PrepareData();

			IList<IRedirection> list = new RedirectionController().GetRedirectionsByPortal(0);

			Assert.AreEqual(3, list.Count);
		}

		[Test]
		public void Test_Delete_Redirections()
		{
			PrepareData();
			new RedirectionController().Delete(1);

			IList<IRedirection> list = new RedirectionController().GetRedirectionsByPortal(0);

			Assert.AreEqual(2, list.Count);
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

		private void PrepareData()
		{
			_dtRedirections.Rows.Add(1, 0, "R1", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(2, 0, "R2", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(3, 0, "R3", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(4, 1, "R4", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(5, 1, "R5", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(6, 1, "R6", (int)RedirectionType.MobilePhone, 1, -1, (int)TargetType.Portal, 2, true);
		}

	}
}