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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        [BeforeScenario("MustHaveServiceWithFee")]
        public void MustHaveServiceWithFee()
        {
            var roleName = "Paid Subscription";
            var role = TestableRoleController.Instance.GetRole(0, r => r.RoleName == roleName);
            if (role == null)
            {
                var subscription = new RoleInfo
                                       {
                                           PortalID = 0,
                                           RoleGroupID = Null.NullInteger,
                                           RoleName = roleName,
                                           Description = "Test Paid Subscription",
                                           ServiceFee = 0.01F,
                                           BillingPeriod = 1,
                                           BillingFrequency = "Y",
                                           IsPublic = true,
                                           Status = RoleStatus.Approved,
                                           SecurityMode= SecurityMode.SecurityRole
                                       };
                TestableRoleController.Instance.AddRole(subscription);
            }
        }
    }
}
