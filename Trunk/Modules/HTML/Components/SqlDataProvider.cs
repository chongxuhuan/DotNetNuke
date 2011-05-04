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

#region Usings

using System;
using System.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Modules.Html
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The SqlDataProvider is a concrete class that provides the SQL Server implementation of the Data Access Layer for the HtmlText module
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class SqlDataProvider : DataProvider
    {
        private const string ProviderType = "data";

        private readonly string _connectionString;

        private readonly string _databaseOwner;
        private readonly string _objectQualifier;

        private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);

        private readonly string _providerPath;

        #region "Constructors"

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            var objProvider = (Provider) _providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            _connectionString = Config.GetConnectionString();

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }
        }

        #endregion

        #region "Properties"

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        #endregion

        #region "Public Methods"

        private object GetNull(object Field)
        {
            return Null.GetNull(Field, DBNull.Value);
        }

        public override IDataReader GetHtmlText(int ModuleID, int ItemID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHtmlText", ModuleID, ItemID);
        }

        public override IDataReader GetTopHtmlText(int ModuleID, bool IsPublished)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTopHtmlText", ModuleID, IsPublished);
        }

        public override IDataReader GetAllHtmlText(int ModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllHtmlText", ModuleID);
        }

        public override int AddHtmlText(int ModuleID, string Content, int StateID, bool IsPublished, int CreatedByUserID, int History)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddHtmlText", ModuleID, Content, StateID, IsPublished, CreatedByUserID, History));
        }

        public override void UpdateHtmlText(int ItemID, string Content, int StateID, bool IsPublished, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateHtmlText", ItemID, Content, StateID, IsPublished, LastModifiedByUserID);
        }

        public override void DeleteHtmlText(int ModuleID, int ItemID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteHtmlText", ModuleID, ItemID);
        }

        public override IDataReader GetHtmlTextLog(int ItemID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHtmlTextLog", ItemID);
        }

        public override void AddHtmlTextLog(int ItemID, int StateID, string Comment, bool Approved, int CreatedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddHtmlTextLog", ItemID, StateID, Comment, Approved, CreatedByUserID);
        }

        public override IDataReader GetHtmlTextUser(int UserID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHtmlTextUser", UserID);
        }

        public override void AddHtmlTextUser(int ItemID, int StateID, int ModuleID, int TabID, int UserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddHtmlTextUser", ItemID, StateID, ModuleID, TabID, UserID);
        }

        public override void DeleteHtmlTextUsers()
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteHtmlTextUsers");
        }

        public override IDataReader GetWorkflows(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflows", PortalID);
        }

        public override IDataReader GetWorkflowStates(int WorkflowID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflowStates", WorkflowID);
        }

        public override IDataReader GetWorkflowStatePermissions()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflowStatePermissions");
        }

        public override IDataReader GetWorkflowStatePermissionsByStateID(int StateID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflowStatePermissionsByStateID", StateID);
        }

        #endregion
    }
}