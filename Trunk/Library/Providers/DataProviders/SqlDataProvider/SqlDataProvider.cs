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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Instrumentation;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Data
{
    public class SqlDataProvider : DataProvider
    {
        private readonly string _connectionString;
        private readonly string _coreConnectionString;
        private readonly string _databaseOwner;
        private readonly bool _isConnectionValid;
        private readonly string _objectQualifier;
        private readonly string _providerName;
        private readonly string _providerPath;
        private readonly string _upgradeConnectionString;
        private string _scriptDelimiterRegex = "(?<=(?:[^\\w]+|^))GO(?=(?: |\\t)*?(?:\\r?\\n|$))";

        public SqlDataProvider()
            : this(true)
        {
        }

        public SqlDataProvider(bool useConfig)
        {
            _providerName = Settings["providerName"];
            _providerPath = Settings["providerPath"];
            if (useConfig)
            {
                //Get Connection string from web.config
                _connectionString = Config.GetConnectionString();
            }
            if (string.IsNullOrEmpty(_connectionString))
            {
                //Use connection string specified in provider
                _connectionString = Settings["connectionString"];
            }
            _objectQualifier = Settings["objectQualifier"];
            if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }
            _databaseOwner = Settings["databaseOwner"];
            if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }
            _coreConnectionString = _connectionString;
            if (!_coreConnectionString.EndsWith(";"))
            {
                _coreConnectionString += ";";
            }
            _coreConnectionString += "Application Name=DNNCore;";
            if (!String.IsNullOrEmpty(Settings["upgradeConnectionString"]))
            {
                _upgradeConnectionString = Settings["upgradeConnectionString"];
            }
            else
            {
                _upgradeConnectionString = _coreConnectionString;
            }
            _isConnectionValid = CanConnect(ConnectionString, DatabaseOwner, ObjectQualifier);
        }

        public override string ConnectionString
        {
            get
            {
                return _coreConnectionString;
            }
        }

        public override string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        public override string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public override string ProviderName
        {
            get
            {
                return _providerName;
            }
        }

        public override Dictionary<string, string> Settings
        {
            get
            {
                return ComponentFactory.GetComponentSettings<SqlDataProvider>() as Dictionary<string, string>;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string UpgradeConnectionString
        {
            get
            {
                return _upgradeConnectionString;
            }
        }

        public bool IsConnectionValid
        {
            get
            {
                return _isConnectionValid;
            }
        }

        protected Regex SqlDelimiterRegex
        {
            get
            {
                var objRegex = (Regex)DataCache.GetCache("SQLDelimiterRegex");
                if (objRegex == null)
                {
                    objRegex = new Regex(_scriptDelimiterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    DataCache.SetCache("SQLDelimiterRegex", objRegex);
                }
                return objRegex;
            }
        }

        private void ExecuteADOScript(SqlTransaction trans, string SQL)
        {
            //Get the connection
            SqlConnection connection = trans.Connection;

            //Create a new command (with no timeout)
            var command = new SqlCommand(SQL, trans.Connection);
            command.Transaction = trans;
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
        }

        private void ExecuteADOScript(string SQL)
        {
            //Create a new connection
            var connection = new SqlConnection(UpgradeConnectionString);

            //Create a new command (with no timeout)
            var command = new SqlCommand(SQL, connection);
            command.CommandTimeout = 0;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        [Obsolete("Temporarily Added in DNN 5.4.2. This will be removed and replaced with named instance support.")]
        private void ExecuteADOScript(string ConnectionString, string SQL)
        {
            //Create a new connection
            var connection = new SqlConnection(ConnectionString);
            //Create a new command (with no timeout)
            var command = new SqlCommand(SQL, connection);
            command.CommandTimeout = 0;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        private object GetRoleNull(int RoleID)
        {
            if (RoleID.ToString() == Globals.glbRoleNothing)
            {
                return DBNull.Value;
            }
            else
            {
                return RoleID;
            }
        }

        private string GetConnectionStringUserID()
        {
            string DBUser = "public";
            string[] ConnSettings;
            string[] ConnSetting;

            ConnSettings = ConnectionString.Split(';');

            //If connection string does not use integrated security, then get user id.
            if (ConnectionString.ToUpper().Contains("USER ID") || ConnectionString.ToUpper().Contains("UID") || ConnectionString.ToUpper().Contains("USER"))
            {
                ConnSettings = ConnectionString.Split(';');

                foreach (string s in ConnSettings)
                {
                    if (s != string.Empty)
                    {
                        ConnSetting = s.Split('=');
                        if ("USER ID|UID|USER".Contains(ConnSetting[0].Trim().ToUpper()))
                        {
                            DBUser = ConnSetting[1].Trim();
                        }
                    }
                }
            }
            return DBUser;
        }

        private string GrantStoredProceduresPermission(string Permission, string LoginOrRole)
        {
            string SQL = string.Empty;
            string Exceptions = string.Empty;

            try
            {
                //grant rights to a login or role for all stored procedures
                SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
                SQL += "  begin";
                SQL += "    declare @exec nvarchar(2000) ";
                SQL += "    declare @name varchar(150) ";
                SQL += "    declare sp_cursor cursor for select o.name as name ";
                SQL += "    from dbo.sysobjects o ";
                SQL += "    where ( OBJECTPROPERTY(o.id, N'IsProcedure') = 1 or OBJECTPROPERTY(o.id, N'IsExtendedProc') = 1 or OBJECTPROPERTY(o.id, N'IsReplProc') = 1 ) ";
                SQL += "    and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "    and o.name not like N'#%%' ";
                SQL += "    and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "    open sp_cursor ";
                SQL += "    fetch sp_cursor into @name ";
                SQL += "    while @@fetch_status >= 0 ";
                SQL += "      begin";
                SQL += "        select @exec = 'grant " + Permission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "        execute (@exec)";
                SQL += "        fetch sp_cursor into @name ";
                SQL += "      end ";
                SQL += "    deallocate sp_cursor";
                SQL += "  end ";
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
            }
            catch (SqlException objException)
            {
                DnnLog.Debug(objException);

                Exceptions += objException + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
            }
            return Exceptions;
        }

        private string GrantUserDefinedFunctionsPermission(string ScalarPermission, string TablePermission, string LoginOrRole)
        {
            string SQL = string.Empty;
            string Exceptions = string.Empty;
            try
            {
                //grant EXECUTE rights to a login or role for all functions
                SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
                SQL += "  begin";
                SQL += "    declare @exec nvarchar(2000) ";
                SQL += "    declare @name varchar(150) ";
                SQL += "    declare @isscalarfunction int ";
                SQL += "    declare @istablefunction int ";
                SQL += "    declare sp_cursor cursor for select o.name as name, OBJECTPROPERTY(o.id, N'IsScalarFunction') as IsScalarFunction ";
                SQL += "    from dbo.sysobjects o ";
                SQL += "    where ( OBJECTPROPERTY(o.id, N'IsScalarFunction') = 1 OR OBJECTPROPERTY(o.id, N'IsTableFunction') = 1 ) ";
                SQL += "      and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "      and o.name not like N'#%%' ";
                SQL += "      and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "    open sp_cursor ";
                SQL += "    fetch sp_cursor into @name, @isscalarfunction ";
                SQL += "    while @@fetch_status >= 0 ";
                SQL += "      begin ";
                SQL += "        if @IsScalarFunction = 1 ";
                SQL += "          begin";
                SQL += "            select @exec = 'grant " + ScalarPermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "            execute (@exec)";
                SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "          end ";
                SQL += "        else ";
                SQL += "          begin";
                SQL += "            select @exec = 'grant " + TablePermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "            execute (@exec)";
                SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "          end ";
                SQL += "      end ";
                SQL += "    deallocate sp_cursor";
                SQL += "  end ";
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
            }
            catch (SqlException objException)
            {
                DnnLog.Debug(objException);

                Exceptions += objException + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
            }
            return Exceptions;
        }

        //Private Overloads Function TestDatabaseConnection(ByVal ConnectionString As String, ByVal Owner As String, ByVal Qualifier As String) As Boolean
        // Dim result As Boolean
        // Try
        // SqlHelper.ExecuteReader(ConnectionString, Owner & Qualifier & "GetDatabaseVersion")
        // result = True
        // Catch ex As SqlException
        // End Try
        // Return result
        //End Function
        private bool CanConnect(string ConnectionString, string Owner, string Qualifier)
        {
            bool connectionValid = true;

            try
            {
                SqlHelper.ExecuteReader(ConnectionString, Owner + Qualifier + "GetDatabaseVersion");
            }
            catch (SqlException ex)
            {
                foreach (SqlError c in ex.Errors)
                {
                    if (!(c.Number == 2812 && c.Class == 16))
                    {
                        connectionValid = false;
                        break;
                    }
                }
            }

            return connectionValid;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExecuteReader executes a stored procedure or "dynamic sql" statement, against 
        /// the database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ProcedureName">The name of the Stored Procedure to Execute</param>
        /// <param name="commandParameters">An array of parameters to pass to the Database</param>
        /// <history>
        /// 	[cnurse]	12/11/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void ExecuteNonQuery(string ProcedureName, params object[] commandParameters)
        {
            SqlHelper.ExecuteNonQuery(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }

        public override IDataReader ExecuteReader(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteReader(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }

        public override object ExecuteScalar(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }

        public override T ExecuteScalar<T>(string ProcedureName, params object[] commandParameters)
        {
            object retObject = ExecuteScalar(ProcedureName, commandParameters);
            T retValue = default(T);
            if (retObject != null)
            {
                retValue = (T)Convert.ChangeType(retObject, typeof(T));
            }
            return retValue;
        }

        public override DataSet ExecuteDataSet(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }

        public override IDataReader ExecuteSQL(string SQL)
        {
            return ExecuteSQLInternal(_connectionString, SQL, (IDataParameter)null);
        }

        [Obsolete("Temporarily Added in DNN 5.4.2. This will be removed and replaced with named instance support.")]
        public override IDataReader ExecuteSQL(string SQL, params IDataParameter[] commandParameters)
        {
            return ExecuteSQLInternal(_connectionString, SQL, commandParameters);
        }

        public override IDataReader ExecuteSQLTemp(string ConnectionString, string SQL)
        {
            return ExecuteSQLInternal(ConnectionString, SQL, (IDataParameter)null);
        }

        public IDataReader ExecuteSQLInternal(string ConnectionString, string SQL, params IDataParameter[] commandParameters)
        {
            SqlParameter[] sqlCommandParameters = null;
            if (commandParameters != null)
            {
                sqlCommandParameters = new SqlParameter[commandParameters.Length];
                for (int intIndex = 0; intIndex <= commandParameters.Length - 1; intIndex++)
                {
                    sqlCommandParameters[intIndex] = (SqlParameter)commandParameters[intIndex];
                }
            }
            SQL = SQL.Replace("{databaseOwner}", DatabaseOwner);
            SQL = SQL.Replace("{objectQualifier}", ObjectQualifier);
            try
            {
                return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, SQL, sqlCommandParameters);
            }
            catch
            {
                //error in SQL query
                return null;
            }
        }

        public override DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder();
        }

        public override object GetNull(object Field)
        {
            return Null.GetNull(Field, DBNull.Value);
        }

        public override void CommitTransaction(DbTransaction transaction)
        {
            try
            {
                transaction.Commit();
            }
            finally
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Connection.Close();
                }
            }
        }

        public override string ExecuteScript(string Script, DbTransaction transaction)
        {
            string Exceptions = "";
            string[] arrSQL = SqlDelimiterRegex.Split(Script);
            bool IgnoreErrors;
            foreach (string SQL in arrSQL)
            {
                string s = SQL;
                if (!String.IsNullOrEmpty(s.Trim()))
                {
                    //script dynamic substitution
                    s = s.Replace("{databaseOwner}", DatabaseOwner);
                    s = s.Replace("{objectQualifier}", ObjectQualifier);
                    IgnoreErrors = false;
                    if (s.Trim().StartsWith("{IgnoreError}"))
                    {
                        IgnoreErrors = true;
                        s = s.Replace("{IgnoreError}", "");
                    }
                    try
                    {
                        ExecuteADOScript((SqlTransaction)transaction, s);
                    }
                    catch (SqlException objException)
                    {
                        DnnLog.Debug(objException);

                        if (!IgnoreErrors)
                        {
                            Exceptions += objException + Environment.NewLine + Environment.NewLine + s + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }
            }
            return Exceptions;
        }

        public override DbTransaction GetTransaction()
        {
            var Conn = new SqlConnection(UpgradeConnectionString);
            Conn.Open();
            SqlTransaction transaction = Conn.BeginTransaction();
            return transaction;
        }

        public override void RollbackTransaction(DbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            finally
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Connection.Close();
                }
            }
        }

        public override string ExecuteScript(string Script)
        {
            return ExecuteScript(Script, false);
        }

        /// <summary>
        ///   This is a temporary overload until proper support for named instances is added to the core.
        /// </summary>
        /// <param name = "connectionString"></param>
        /// <param name = "script"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [Obsolete("Temporarily Added in DNN 5.4.2. This will be removed and replaced with named instance support.")]
        public override string ExecuteScript(string connectionString, string script)
        {
            string exceptions = "";
            string[] sqlStatements = SqlDelimiterRegex.Split(script);
            foreach (string statement in sqlStatements)
            {
                var sql = statement.Trim();

                if (!string.IsNullOrEmpty(sql))
                {
                    // script dynamic substitution
                    sql = sql.Replace("{databaseOwner}", DatabaseOwner).Replace("{objectQualifier}", ObjectQualifier);
                    try
                    {
                        ExecuteADOScript(connectionString, sql);
                    }
                    catch (SqlException objException)
                    {
                        DnnLog.Debug(objException);

                        exceptions += objException + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine;
                    }
                }
            }

            return exceptions;
        }

        public override string ExecuteScript(string Script, bool UseTransactions)
        {
            string Exceptions = "";
            if (UseTransactions)
            {
                DbTransaction transaction = GetTransaction();
                try
                {
                    Exceptions += ExecuteScript(Script, transaction);
                    if (String.IsNullOrEmpty(Exceptions))
                    {
                        //No exceptions so go ahead and commit
                        CommitTransaction(transaction);
                    }
                    else
                    {
                        //Found exceptions, so rollback db
                        RollbackTransaction(transaction);
                        Exceptions += "SQL Execution failed.  Database was rolled back" + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                    }
                }
                finally
                {
                    if (transaction != null && transaction.Connection != null)
                    {
                        transaction.Connection.Close();
                    }
                }
            }
            else
            {
                string[] arrSQL = SqlDelimiterRegex.Split(Script);
                foreach (string SQL in arrSQL)
                {
                    string s = SQL;
                    if (!String.IsNullOrEmpty(s.Trim()))
                    {
                        //script dynamic substitution
                        s = s.Replace("{databaseOwner}", DatabaseOwner);
                        s = s.Replace("{objectQualifier}", ObjectQualifier);
                        try
                        {
                            DnnLog.Trace("Executing SQL Script " + s);
                            ExecuteADOScript(s);
                        }
                        catch (SqlException objException)
                        {
                            DnnLog.Debug(objException);

                            Exceptions += objException + Environment.NewLine + Environment.NewLine + s + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }
            }

            //if the upgrade connection string is specified or or db_owner setting is not set to dbo
            if (UpgradeConnectionString != ConnectionString || DatabaseOwner.Trim().ToLower() != "dbo.")
            {
                try
                {
                    //grant execute rights to the public role or userid for all stored procedures. This is
                    //necesary because the UpgradeConnectionString will create stored procedures
                    //which restrict execute permissions for the ConnectionString user account. This is also
                    //necessary when db_owner is not set to "dbo" 
                    Exceptions += GrantStoredProceduresPermission("EXECUTE", GetConnectionStringUserID());
                }
                catch (SqlException objException)
                {
                    DnnLog.Debug(objException);

                    Exceptions += objException + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                }

                try
                {
                    //grant execute or select rights to the public role or userid for all user defined functions based
                    //on what type of function it is (scalar function or table function). This is
                    //necesary because the UpgradeConnectionString will create user defined functions
                    //which restrict execute permissions for the ConnectionString user account.  This is also
                    //necessary when db_owner is not set to "dbo" 
                    Exceptions += GrantUserDefinedFunctionsPermission("EXECUTE", "SELECT", GetConnectionStringUserID());
                }
                catch (SqlException objException)
                {
                    DnnLog.Debug(objException);

                    Exceptions += objException + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                }
            }
            return Exceptions;
        }

        public override IDataReader GetDatabaseServer()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseServer");
        }

        public override Version GetDatabaseEngineVersion()
        {
            string version = "0.0";
            IDataReader dr = null;
            try
            {
                dr = GetDatabaseServer();
                if (dr.Read())
                {
                    version = dr["Version"].ToString();
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return new Version(version);
        }

        public override IDataReader FindDatabaseVersion(int Major, int Minor, int Build)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "FindDatabaseVersion", Major, Minor, Build);
        }

        public override IDataReader GetDatabaseVersion()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseVersion");
        }

        public override Version GetVersion()
        {
            Version version = null;
            try
            {
                IDataReader dr = GetDatabaseVersion();
                if (dr.Read())
                {
                    version = new Version(Convert.ToInt32(dr["Major"]), Convert.ToInt32(dr["Minor"]), Convert.ToInt32(dr["Build"]));
                }
            }
            catch (SqlException ex)
            {
                bool noStoredProc = false;
                for (int i = 0; i <= ex.Errors.Count - 1; i++)
                {
                    SqlError sqlError = ex.Errors[i];
                    if (sqlError.Number == 2812 && sqlError.Class == 16) //2812 - 16 means SP could not be found
                    {
                        noStoredProc = true;
                        break;
                    }
                }

                if (!noStoredProc)
                {
                    throw;
                }
            }
            return version;
        }

        public override string GetProviderPath()
        {
            string path = ProviderPath;
            if (!String.IsNullOrEmpty(path))
            {
                path = HostingEnvironment.MapPath(path);

                if (Directory.Exists(path))
                {
                    if (!IsConnectionValid)
                    {
                        path = "ERROR: Could not connect to database specified in connectionString for SqlDataProvider";
                    }
                }
                else
                {
                    path = "ERROR: providerPath folder " + path + " specified for SqlDataProvider does not exist on web server";
                }
            }
            else
            {
                path = "ERROR: providerPath folder value not specified in web.config for SqlDataProvider";
            }
            return path;
        }

        public override string TestDatabaseConnection(DbConnectionStringBuilder builder, string Owner, string Qualifier)
        {
            var sqlBuilder = builder as SqlConnectionStringBuilder;
            string connectionString = Null.NullString;
            if (sqlBuilder != null)
            {
                connectionString = sqlBuilder.ToString();
                IDataReader dr = null;
                try
                {
                    dr = SqlHelper.ExecuteReader(connectionString, Owner + Qualifier + "GetDatabaseVersion");
                }
                catch (SqlException ex)
                {
                    string message = "ERROR:";
                    bool bError = true;
                    int i;
                    var errorMessages = new StringBuilder();
                    for (i = 0; i <= ex.Errors.Count - 1; i++)
                    {
                        SqlError sqlError = ex.Errors[i];
                        if (sqlError.Number == 2812 && sqlError.Class == 16)
                        {
                            bError = false;
                            break;
                        }
                        else
                        {
                            string filteredMessage = String.Empty;
                            switch (sqlError.Number)
                            {
                                case 17:
                                    filteredMessage = "Sql server does not exist or access denied";
                                    break;
                                case 4060:
                                    filteredMessage = "Invalid Database";
                                    break;
                                case 18456:
                                    filteredMessage = "Sql login failed";
                                    break;
                                case 1205:
                                    filteredMessage = "Sql deadlock victim";
                                    break;
                            }
                            errorMessages.Append("<b>Index #:</b> " + i + "<br/>" + "<b>Source:</b> " + sqlError.Source + "<br/>" + "<b>Class:</b> " + sqlError.Class + "<br/>" + "<b>Number:</b> " +
                                                 sqlError.Number + "<br/>" + "<b>Message:</b> " + filteredMessage + "<br/><br/>");
                        }
                    }
                    if (bError)
                    {
                        connectionString = message + errorMessages;
                    }
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            else
            {
                //Invalid DbConnectionStringBuilder
            }
            return connectionString;
        }

        public override void UpgradeDatabaseSchema(int Major, int Minor, int Build)
        {
            //not necessary for SQL Server - use Transact-SQL scripts
        }

        public override void UpdateDatabaseVersion(int Major, int Minor, int Build, string Name)
        {
            if ((Major >= 5 || (Major == 4 && Minor == 9 && Build > 0)))
            {
                //If the version > 4.9.0 use the new sproc, which is added in 4.9.1 script
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDatabaseVersionAndName", Major, Minor, Build, Name);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDatabaseVersion", Major, Minor, Build);
            }
        }

        //host
        public override IDataReader GetHostSettings()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSettings");
        }

        public override IDataReader GetHostSetting(string SettingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSetting", SettingName);
        }

        public override void AddHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int CreatedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddHostSetting", SettingName, SettingValue, SettingIsSecure, CreatedByUserID);
        }

        public override void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateHostSetting", SettingName, SettingValue, SettingIsSecure, LastModifiedByUserID);
        }

        public override IDataReader GetServers()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetServers");
        }

        public override IDataReader GetServerConfiguration()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetServerConfiguration");
        }

        public override void UpdateServer(int ServerId, string Url, bool Enabled)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateServer", ServerId, Url, Enabled);
        }

        public override void DeleteServer(int ServerId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteServer", ServerId);
        }

        public override void UpdateServerActivity(string ServerName, string IISAppName, DateTime CreatedDate, DateTime LastActivityDate)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateServerActivity", ServerName, IISAppName, CreatedDate, LastActivityDate);
        }

        //portal
        public override int AddPortalInfo(string portalname, string currency, string firstname, string lastname, string username, string password, string email, DateTime expirydate, double hostfee,
                                          double hostspace, int pagequota, int userquota, int siteloghistory, string homedirectory, int createdbyuserid)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "addportalinfo",
                                                        portalname,
                                                        currency,
                                                        GetNull(expirydate),
                                                        hostfee,
                                                        hostspace,
                                                        pagequota,
                                                        userquota,
                                                        GetNull(siteloghistory),
                                                        homedirectory,
                                                        createdbyuserid));
        }

        public override int CreatePortal(string PortalName, string Currency, DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory,
                                         string HomeDirectory, int CreatedByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddPortalInfo",
                                                        PortalName,
                                                        Currency,
                                                        GetNull(ExpiryDate),
                                                        HostFee,
                                                        HostSpace,
                                                        PageQuota,
                                                        UserQuota,
                                                        GetNull(SiteLogHistory),
                                                        HomeDirectory,
                                                        CreatedByUserID));
        }

        public override void DeletePortalInfo(int PortalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalInfo", PortalId);
        }

        public override void DeletePortalSetting(int PortalId, string SettingName, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalSetting", PortalId, SettingName, CultureCode);
        }

        public override void DeletePortalSettings(int PortalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalSettings", PortalId);
        }

        public override IDataReader GetExpiredPortals()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetExpiredPortals");
        }

        public override IDataReader GetPortal(int PortalId, string CultureCode)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortal", PortalId, CultureCode);
        }

        public override IDataReader GetPortalByAlias(string PortalAlias)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByAlias", PortalAlias);
        }

        public override IDataReader GetPortalByTab(int TabId, string PortalAlias)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByTab", TabId, PortalAlias);
        }

        public override int GetPortalCount()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalCount"));
        }

        //public override IDataReader GetPortals(string CultureCode)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortals", CultureCode);
        //}
        public override IDataReader GetPortals()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortals");
        }

        public override IDataReader GetPortalsByName(string nameToMatch, int pageIndex, int pageSize)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalsByName", nameToMatch, pageIndex, pageSize);
        }

        public override IDataReader GetPortalSettings(int PortalId, string CultureCode)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalSettings", PortalId, CultureCode);
        }

        public override IDataReader GetPortalSpaceUsed(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalSpaceUsed", GetNull(PortalId));
        }

        public override void UpdatePortalInfo(int portalId, int portalGroupId, string portalName, string logoFile, string footerText, DateTime expiryDate, int userRegistration, int bannerAdvertising, string currency,
                                              int administratorId, double hostFee, double hostSpace, int pageQuota, int userQuota, string paymentProcessor, string processorUserId,
                                              string processorPassword, string description, string keyWords, string backgroundFile, int siteLogHistory, int splashTabId, int homeTabId, int loginTabId,
                                              int registerTabId, int userTabId, int searchTabId, string defaultLanguage, string homeDirectory, int lastModifiedByUserID, string cultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdatePortalInfo",
                                      portalId,
                                      portalGroupId,
                                      portalName,
                                      GetNull(logoFile),
                                      GetNull(footerText),
                                      GetNull(expiryDate),
                                      userRegistration,
                                      bannerAdvertising,
                                      currency,
                                      GetNull(administratorId),
                                      hostFee,
                                      hostSpace,
                                      pageQuota,
                                      userQuota,
                                      GetNull(paymentProcessor),
                                      GetNull(processorUserId),
                                      GetNull(processorPassword),
                                      GetNull(description),
                                      GetNull(keyWords),
                                      GetNull(backgroundFile),
                                      GetNull(siteLogHistory),
                                      GetNull(splashTabId),
                                      GetNull(homeTabId),
                                      GetNull(loginTabId),
                                      GetNull(registerTabId),
                                      GetNull(userTabId),
                                      GetNull(searchTabId),
                                      GetNull(defaultLanguage),
                                      homeDirectory,
                                      lastModifiedByUserID,
                                      cultureCode);
        }

        public override void UpdatePortalSetting(int PortalId, string SettingName, string SettingValue, int UserID, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalSetting", PortalId, SettingName, SettingValue, UserID, CultureCode);
        }

        public override void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId,
                                               int UserTabId, int SearchTabId, int AdminTabId, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdatePortalSetup",
                                      PortalId,
                                      AdministratorId,
                                      AdministratorRoleId,
                                      RegisteredRoleId,
                                      SplashTabId,
                                      HomeTabId,
                                      LoginTabId,
                                      RegisterTabId,
                                      UserTabId,
                                      SearchTabId,
                                      AdminTabId,
                                      CultureCode);
        }

        public override IDataReader VerifyPortal(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortal", PortalId);
        }

        public override IDataReader VerifyPortalTab(int PortalId, int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortalTab", PortalId, TabId);
        }

        // tab
        public override int AddTab(int ContentItemId, int PortalId, Guid UniqueId, Guid VersionGuid, Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string TabName, bool IsVisible,
                                   bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description, string KeyWords, string Url, string SkinSrc,
                                   string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText, bool IsSecure, bool PermanentRedirect,
                                   float SiteMapPriority, int CreatedByUserID, string CultureCode)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddTab",
                                                        ContentItemId,
                                                        GetNull(PortalId),
                                                        UniqueId,
                                                        VersionGuid,
                                                        GetNull(DefaultLanguageGuid),
                                                        LocalizedVersionGuid,
                                                        TabName,
                                                        IsVisible,
                                                        DisableLink,
                                                        GetNull(ParentId),
                                                        IconFile,
                                                        IconFileLarge,
                                                        Title,
                                                        Description,
                                                        KeyWords,
                                                        Url,
                                                        GetNull(SkinSrc),
                                                        GetNull(ContainerSrc),
                                                        TabPath,
                                                        GetNull(StartDate),
                                                        GetNull(EndDate),
                                                        GetNull(RefreshInterval),
                                                        GetNull(PageHeadText),
                                                        IsSecure,
                                                        PermanentRedirect,
                                                        SiteMapPriority,
                                                        CreatedByUserID,
                                                        GetNull(CultureCode)));
        }

        [Obsolete("This method is used for legacy support during the upgrade process (pre v3.1.1). It has been replaced by one that adds the RefreshInterval and PageHeadText variables.")]
        public override void UpdateTab(int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted,
                                       string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateTab",
                                      TabId,
                                      TabName,
                                      IsVisible,
                                      DisableLink,
                                      GetNull(ParentId),
                                      IconFile,
                                      Title,
                                      Description,
                                      KeyWords,
                                      IsDeleted,
                                      Url,
                                      GetNull(SkinSrc),
                                      GetNull(ContainerSrc),
                                      TabPath,
                                      GetNull(StartDate),
                                      GetNull(EndDate),
                                      GetNull(CultureCode));
        }

        public override void UpdateTab(int TabId, int ContentItemId, int PortalId, Guid VersionGuid, Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string TabName, bool IsVisible,
                                       bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description, string KeyWords, bool IsDeleted, string Url,
                                       string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText, bool IsSecure,
                                       bool PermanentRedirect, float SiteMapPriority, int LastModifiedByUserID, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateTab",
                                      TabId,
                                      ContentItemId,
                                      GetNull(PortalId),
                                      VersionGuid,
                                      GetNull(DefaultLanguageGuid),
                                      LocalizedVersionGuid,
                                      TabName,
                                      IsVisible,
                                      DisableLink,
                                      GetNull(ParentId),
                                      IconFile,
                                      IconFileLarge,
                                      Title,
                                      Description,
                                      KeyWords,
                                      IsDeleted,
                                      Url,
                                      GetNull(SkinSrc),
                                      GetNull(ContainerSrc),
                                      TabPath,
                                      GetNull(StartDate),
                                      GetNull(EndDate),
                                      GetNull(RefreshInterval),
                                      GetNull(PageHeadText),
                                      IsSecure,
                                      PermanentRedirect,
                                      SiteMapPriority,
                                      LastModifiedByUserID,
                                      GetNull(CultureCode));
        }

        public override void UpdateTabTranslationStatus(int TabId, Guid LocalizedVersionGuid, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabTranslationStatus", TabId, LocalizedVersionGuid, LastModifiedByUserID);
        }

        public override void UpdateTabOrder(int TabId, int TabOrder, int Level, int ParentId, string TabPath, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabOrder", TabId, TabOrder, Level, GetNull(ParentId), TabPath, LastModifiedByUserID);
        }

        public override void UpdateTabVersion(int TabId, Guid VersionGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabVersion", TabId, VersionGuid);
        }

        public override void DeleteTab(int TabId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTab", TabId);
        }

        public override IDataReader GetTabs(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabs", GetNull(PortalId));
        }

        public override IDataReader GetAllTabs()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabs");
        }

        public override IDataReader GetTabPaths(int PortalId, string cultureCode)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPaths", GetNull(PortalId), cultureCode);
        }

        public override IDataReader GetTab(int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTab", TabId);
        }

        public override IDataReader GetTabByUniqueID(Guid UniqueId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabByUniqueID", UniqueId);
        }

        public override IDataReader GetTabByName(string TabName, int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabByName", TabName, GetNull(PortalId));
        }

        public override int GetTabCount(int PortalId)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabCount", PortalId));
        }

        public override IDataReader GetTabsByParentId(int ParentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByParentId", ParentId);
        }

        public override IDataReader GetTabsByModuleID(int moduleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByModuleID", moduleID);
        }

        public override IDataReader GetTabsByPackageID(int portalID, int packageID, bool forHost)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByPackageID", GetNull(portalID), packageID, forHost);
        }

        public override IDataReader GetTabPanes(int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPanes", TabId);
        }

        public override IDataReader GetPortalTabModules(int PortalId, int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId);
        }

        public override IDataReader GetTabModule(int TabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModule", TabModuleId);
        }

        public override IDataReader GetTabModules(int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId);
        }

        //module
        public override IDataReader GetAllModules()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllModules");
        }

        public override IDataReader GetModules(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModules", PortalId);
        }

        public override IDataReader GetAllTabsModules(int PortalId, bool AllTabs)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabsModules", PortalId, AllTabs);
        }

        public override IDataReader GetAllTabsModulesByModuleID(int ModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabsModulesByModuleID", ModuleId);
        }

        public override IDataReader GetModule(int ModuleId, int TabId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModule", ModuleId, GetNull(TabId));
        }

        public override IDataReader GetModuleByUniqueID(Guid UniqueID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleByUniqueID", UniqueID);
        }

        public override IDataReader GetModuleByDefinition(int PortalId, string FriendlyName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleByDefinition", GetNull(PortalId), FriendlyName);
        }

        public override int AddModule(int ContentItemID, int PortalID, int ModuleDefID, bool AllTabs, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted,
                                      int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddModule",
                                                        ContentItemID,
                                                        GetNull(PortalID),
                                                        ModuleDefID,
                                                        AllTabs,
                                                        GetNull(StartDate),
                                                        GetNull(EndDate),
                                                        InheritViewPermissions,
                                                        IsDeleted,
                                                        createdByUserID));
        }

        public override void UpdateModule(int ModuleId, int ContentItemId, bool AllTabs, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateModule",
                                      ModuleId,
                                      ContentItemId,
                                      AllTabs,
                                      GetNull(StartDate),
                                      GetNull(EndDate),
                                      InheritViewPermissions,
                                      IsDeleted,
                                      lastModifiedByUserID);
        }

        public override void DeleteModule(int ModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModule", ModuleId);
        }

        public override IDataReader GetTabModuleOrder(int TabId, string PaneName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleOrder", TabId, PaneName);
        }

        public override void UpdateModuleOrder(int TabId, int ModuleId, int ModuleOrder, string PaneName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleOrder", TabId, ModuleId, ModuleOrder, PaneName);
        }

        public override void AddTabModule(int TabId, int ModuleId, string ModuleTitle, string Header, string Footer, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod,
                                          string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint,
                                          bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, Guid UniqueId, Guid VersionGuid,
                                          Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string CultureCode, int createdByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "AddTabModule",
                                      TabId,
                                      ModuleId,
                                      ModuleTitle,
                                      GetNull(Header),
                                      GetNull(Footer),
                                      ModuleOrder,
                                      PaneName,
                                      CacheTime,
                                      GetNull(CacheMethod),
                                      GetNull(Alignment),
                                      GetNull(Color),
                                      GetNull(Border),
                                      GetNull(IconFile),
                                      Visibility,
                                      GetNull(ContainerSrc),
                                      DisplayTitle,
                                      DisplayPrint,
                                      DisplaySyndicate,
                                      IsWebSlice,
                                      WebSliceTitle,
                                      GetNull(WebSliceExpiryDate),
                                      WebSliceTTL,
                                      UniqueId,
                                      VersionGuid,
                                      GetNull(DefaultLanguageGuid),
                                      LocalizedVersionGuid,
                                      CultureCode,
                                      createdByUserID);
        }

        public override void DeleteTabModule(int TabId, int ModuleId, bool softDelete)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModule", TabId, ModuleId, softDelete);
        }

        public override void MoveTabModule(int fromTabId, int moduleId, int toTabId, string toPaneName, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "MoveTabModule", fromTabId, moduleId, toTabId, toPaneName, lastModifiedByUserID);
        }

        public override void RestoreTabModule(int TabId, int ModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "RestoreTabModule", TabId, ModuleId);
        }

        public override void UpdateTabModule(int TabModuleId, int TabId, int ModuleId, string ModuleTitle, string Header, string Footer, int ModuleOrder, string PaneName, int CacheTime,
                                             string CacheMethod, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle,
                                             bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, Guid VersionGuid,
                                             Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string CultureCode, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateTabModule",
                                      TabModuleId,
                                      TabId,
                                      ModuleId,
                                      ModuleTitle,
                                      GetNull(Header),
                                      GetNull(Footer),
                                      ModuleOrder,
                                      PaneName,
                                      CacheTime,
                                      GetNull(CacheMethod),
                                      GetNull(Alignment),
                                      GetNull(Color),
                                      GetNull(Border),
                                      GetNull(IconFile),
                                      Visibility,
                                      GetNull(ContainerSrc),
                                      DisplayTitle,
                                      DisplayPrint,
                                      DisplaySyndicate,
                                      IsWebSlice,
                                      WebSliceTitle,
                                      GetNull(WebSliceExpiryDate),
                                      WebSliceTTL,
                                      VersionGuid,
                                      GetNull(DefaultLanguageGuid),
                                      LocalizedVersionGuid,
                                      CultureCode,
                                      lastModifiedByUserID);
        }

        public override void UpdateTabModuleTranslationStatus(int TabModuleId, Guid LocalizedVersionGuid, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleTranslationStatus", TabModuleId, LocalizedVersionGuid, LastModifiedByUserID);
        }

        public override void UpdateModuleLastContentModifiedOnDate(int moduleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleLastContentModifiedOnDate", moduleId);
        }

        public override void UpdateTabModuleVersion(int TabModuleId, Guid VersionGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleVersion", TabModuleId, VersionGuid);
        }

        public override void UpdateTabModuleVersionByModule(int ModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleVersionByModule", ModuleId);
        }

        public override IDataReader GetSearchModules(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchModules", PortalId);
        }

        public override IDataReader GetModuleSettings(int ModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSettings", ModuleId);
        }

        public override IDataReader GetModuleSetting(int ModuleId, string SettingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSetting", ModuleId, SettingName);
        }

        public override void AddModuleSetting(int ModuleId, string SettingName, string SettingValue, int createdByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleSetting", ModuleId, SettingName, SettingValue, createdByUserID);
        }

        public override void UpdateModuleSetting(int ModuleId, string SettingName, string SettingValue, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleSetting", ModuleId, SettingName, SettingValue, lastModifiedByUserID);
        }

        public override void DeleteModuleSetting(int ModuleId, string SettingName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSetting", ModuleId, SettingName);
        }

        public override void DeleteModuleSettings(int ModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSettings", ModuleId);
        }

        public override IDataReader GetTabSettings(int TabID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabSettings", TabID);
        }

        public override IDataReader GetTabSetting(int TabID, string SettingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabSetting", TabID, SettingName);
        }

        public override void UpdateTabSetting(int TabId, string SettingName, string SettingValue, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabSetting", TabId, SettingName, SettingValue, lastModifiedByUserID);
        }

        public override void AddTabSetting(int TabId, string SettingName, string SettingValue, int createdByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabSetting", TabId, SettingName, SettingValue, createdByUserID);
        }

        public override void DeleteTabSetting(int TabId, string SettingName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabSetting", TabId, SettingName);
        }

        public override void DeleteTabSettings(int TabId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabSettings", TabId);
        }

        public override IDataReader GetTabModuleSettings(int TabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSettings", TabModuleId);
        }

        public override IDataReader GetTabModuleSetting(int TabModuleId, string SettingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSetting", TabModuleId, SettingName);
        }

        public override void AddTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int createdByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabModuleSetting", TabModuleId, SettingName, SettingValue, createdByUserID);
        }

        public override void UpdateTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleSetting", TabModuleId, SettingName, SettingValue, lastModifiedByUserID);
        }

        public override void DeleteTabModuleSetting(int TabModuleId, string SettingName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSetting", TabModuleId, SettingName);
        }

        public override void DeleteTabModuleSettings(int TabModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSettings", TabModuleId);
        }

        //Desktop Modules
        public override int AddDesktopModule(int packageID, string moduleName, string folderName, string friendlyName, string description, string version, bool isPremium, bool isAdmin,
                                             string businessControllerClass, int supportedFeatures, string compatibleVersions, string dependencies, string permissions,
                                             int contentItemId, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddDesktopModule",
                                                        packageID,
                                                        moduleName,
                                                        folderName,
                                                        friendlyName,
                                                        GetNull(description),
                                                        GetNull(version),
                                                        isPremium,
                                                        isAdmin,
                                                        businessControllerClass,
                                                        supportedFeatures,
                                                        GetNull(compatibleVersions),
                                                        GetNull(dependencies),
                                                        GetNull(permissions),
                                                        contentItemId,
                                                        createdByUserID));
        }

        public override void DeleteDesktopModule(int desktopModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModule", desktopModuleId);
        }

        public override IDataReader GetDesktopModules()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModules");
        }

        public override IDataReader GetDesktopModulesByPortal(int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulesByPortal", PortalId);
        }


        public override void UpdateDesktopModule(int desktopModuleId, int packageID, string moduleName, string folderName, string friendlyName, string description, string version, bool isPremium,
                                                 bool isAdmin, string businessControllerClass, int supportedFeatures, string compatibleVersions, string dependencies, string permissions,
                                                 int contentItemId, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateDesktopModule",
                                      desktopModuleId,
                                      packageID,
                                      moduleName,
                                      folderName,
                                      friendlyName,
                                      GetNull(description),
                                      GetNull(version),
                                      isPremium,
                                      isAdmin,
                                      businessControllerClass,
                                      supportedFeatures,
                                      GetNull(compatibleVersions),
                                      GetNull(dependencies),
                                      GetNull(permissions),
                                      contentItemId,
                                      lastModifiedByUserID);
        }


        //Portal Desktop Modules
        public override int AddPortalDesktopModule(int PortalId, int DesktopModuleId, int createdByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalDesktopModule", PortalId, DesktopModuleId, createdByUserID));
        }

        public override void DeletePortalDesktopModules(int PortalId, int DesktopModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalDesktopModules", GetNull(PortalId), GetNull(DesktopModuleId));
        }

        public override IDataReader GetPortalDesktopModules(int PortalId, int DesktopModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalDesktopModules", GetNull(PortalId), GetNull(DesktopModuleId));
        }

        //Module Definitions
        public override int AddModuleDefinition(int DesktopModuleId, string FriendlyName, int DefaultCacheTime, int createdByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleDefinition", DesktopModuleId, FriendlyName, DefaultCacheTime, createdByUserID));
        }

        public override void DeleteModuleDefinition(int ModuleDefId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleDefinition", ModuleDefId);
        }

        public override IDataReader GetModuleDefinitions()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinitions");
        }

        public override void UpdateModuleDefinition(int ModuleDefId, string FriendlyName, int DefaultCacheTime, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleDefinition", ModuleDefId, FriendlyName, DefaultCacheTime, lastModifiedByUserID);
        }


        public override IDataReader GetModuleControls()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControls");
        }

        public override IDataReader GetModuleControl(int ModuleControlId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControl", ModuleControlId);
        }

        public override IDataReader GetModuleControlsByKey(string ControlKey, int ModuleDefId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlsByKey", GetNull(ControlKey), GetNull(ModuleDefId));
        }

        public override IDataReader GetModuleControlByKeyAndSrc(int ModuleDefID, string ControlKey, string ControlSrc)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlByKeyAndSrc", GetNull(ModuleDefID), GetNull(ControlKey), GetNull(ControlSrc));
        }

        public override int AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl,
                                             bool SupportsPartialRendering, bool SupportsPopUps, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddModuleControl",
                                                        GetNull(ModuleDefId),
                                                        GetNull(ControlKey),
                                                        GetNull(ControlTitle),
                                                        ControlSrc,
                                                        GetNull(IconFile),
                                                        ControlType,
                                                        GetNull(ViewOrder),
                                                        GetNull(HelpUrl),
                                                        SupportsPartialRendering,
                                                        SupportsPopUps,
                                                        createdByUserID));
        }

        public override void UpdateModuleControl(int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder,
                                                 string HelpUrl, bool SupportsPartialRendering, bool SupportsPopUps, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateModuleControl",
                                      ModuleControlId,
                                      GetNull(ModuleDefId),
                                      GetNull(ControlKey),
                                      GetNull(ControlTitle),
                                      ControlSrc,
                                      GetNull(IconFile),
                                      ControlType,
                                      GetNull(ViewOrder),
                                      GetNull(HelpUrl),
                                      SupportsPartialRendering,
                                      SupportsPopUps,
                                      lastModifiedByUserID);
        }

        public override void DeleteModuleControl(int ModuleControlId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleControl", ModuleControlId);
        }

        public override int AddSkinControl(int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int CreatedByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddSkinControl",
                                                        GetNull(packageID),
                                                        GetNull(ControlKey),
                                                        ControlSrc,
                                                        SupportsPartialRendering,
                                                        CreatedByUserID));
        }

        public override void DeleteSkinControl(int skinControlID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkinControl", skinControlID);
        }

        public override IDataReader GetSkinControls()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControls");
        }

        public override IDataReader GetSkinControl(int skinControlID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControl", skinControlID);
        }

        public override IDataReader GetSkinControlByKey(string controlKey)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControlByKey", controlKey);
        }

        public override IDataReader GetSkinControlByPackageID(int packageID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControlByPackageID", packageID);
        }

        public override void UpdateSkinControl(int skinControlID, int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateSkinControl",
                                      skinControlID,
                                      GetNull(packageID),
                                      GetNull(ControlKey),
                                      ControlSrc,
                                      SupportsPartialRendering,
                                      LastModifiedByUserID);
        }

        //files
        public override IDataReader GetFiles(int FolderID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFiles", FolderID);
        }

        public override IDataReader GetFile(string FileName, int FolderID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFile", FileName, FolderID);
        }

        public override IDataReader GetFileById(int FileId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileById", FileId);
        }

        public override IDataReader GetFileByUniqueID(Guid UniqueID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileByUniqueID", UniqueID);
        }

        public override void DeleteFile(int PortalId, string FileName, int FolderID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFile", GetNull(PortalId), FileName, FolderID);
        }

        public override void DeleteFiles(int PortalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFiles", GetNull(PortalId));
        }

        public override int AddFile(int PortalId, Guid UniqueId, Guid VersionGuid, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID,
                                    int createdByUserID, string hash)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddFile",
                                                        GetNull(PortalId),
                                                        UniqueId,
                                                        VersionGuid,
                                                        FileName,
                                                        Extension,
                                                        Size,
                                                        GetNull(Width),
                                                        GetNull(Height),
                                                        ContentType,
                                                        Folder,
                                                        FolderID,
                                                        createdByUserID,
                                                        hash));
        }

        public override void UpdateFile(int FileId, Guid VersionGuid, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID,
                                        int lastModifiedByUserID, string hash)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateFile",
                                      FileId,
                                      VersionGuid,
                                      FileName,
                                      Extension,
                                      Size,
                                      GetNull(Width),
                                      GetNull(Height),
                                      ContentType,
                                      Folder,
                                      FolderID,
                                      lastModifiedByUserID,
                                      hash);
        }

        public override DataTable GetAllFiles()
        {
            return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllFiles").Tables[0];
        }

        public override IDataReader GetFileContent(int FileId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileContent", FileId);
        }

        public override void UpdateFileContent(int FileId, byte[] Content)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFileContent", FileId, GetNull(Content));
        }

        public override void UpdateFileVersion(int FileId, Guid VersionGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFileVersion", FileId, VersionGuid);
        }

        //site log
        public override void AddSiteLog(DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId,
                                        int AffiliateId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "AddSiteLog",
                                      DateTime,
                                      PortalId,
                                      GetNull(UserId),
                                      GetNull(Referrer),
                                      GetNull(URL),
                                      GetNull(UserAgent),
                                      GetNull(UserHostAddress),
                                      GetNull(UserHostName),
                                      GetNull(TabId),
                                      GetNull(AffiliateId));
        }

        public override IDataReader GetSiteLog(int PortalId, string PortalAlias, string ReportName, DateTime StartDate, DateTime EndDate)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ReportName, PortalId, PortalAlias, StartDate, EndDate);
        }

        public override IDataReader GetSiteLogReports()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSiteLogReports");
        }

        public override void DeleteSiteLog(DateTime DateTime, int PortalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSiteLog", DateTime, PortalId);
        }

        //database
        public override IDataReader GetTables()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTables");
        }

        public override IDataReader GetFields(string TableName)
        {
            string SQL = "SELECT * FROM {objectQualifier}" + TableName + " WHERE 1 = 0";
            return ExecuteSQL(SQL);
        }

        //vendors
        public override IDataReader GetVendors(int PortalId, bool UnAuthorized, int PageIndex, int PageSize)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendors", GetNull(PortalId), UnAuthorized, GetNull(PageSize), GetNull(PageIndex));
        }

        public override IDataReader GetVendorsByEmail(string Filter, int PortalId, int PageIndex, int PageSize)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByEmail", Filter, GetNull(PortalId), GetNull(PageSize), GetNull(PageIndex));
        }

        public override IDataReader GetVendorsByName(string Filter, int PortalId, int PageIndex, int PageSize)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByName", Filter, GetNull(PortalId), GetNull(PageSize), GetNull(PageIndex));
        }

        public override IDataReader GetVendor(int VendorId, int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendor", VendorId, GetNull(PortalId));
        }

        public override void DeleteVendor(int VendorId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendor", VendorId);
        }

        public override int AddVendor(int PortalId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
                                      string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddVendor",
                                                        GetNull(PortalId),
                                                        VendorName,
                                                        Unit,
                                                        Street,
                                                        City,
                                                        Region,
                                                        Country,
                                                        PostalCode,
                                                        Telephone,
                                                        Fax,
                                                        Cell,
                                                        Email,
                                                        Website,
                                                        FirstName,
                                                        LastName,
                                                        UserName,
                                                        LogoFile,
                                                        KeyWords,
                                                        bool.Parse(Authorized)));
        }

        public override void UpdateVendor(int VendorId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
                                          string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateVendor",
                                      VendorId,
                                      VendorName,
                                      Unit,
                                      Street,
                                      City,
                                      Region,
                                      Country,
                                      PostalCode,
                                      Telephone,
                                      Fax,
                                      Cell,
                                      Email,
                                      Website,
                                      FirstName,
                                      LastName,
                                      UserName,
                                      LogoFile,
                                      KeyWords,
                                      bool.Parse(Authorized));
        }

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public override IDataReader GetVendorClassifications(int VendorId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorClassifications", GetNull(VendorId));
        }

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public override void DeleteVendorClassifications(int VendorId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendorClassifications", VendorId);
        }

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public override int AddVendorClassification(int VendorId, int ClassificationId)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddVendorClassification", VendorId, ClassificationId));
        }

        public override IDataReader GetBanners(int VendorId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanners", VendorId);
        }

        public override IDataReader GetBanner(int BannerId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanner", BannerId);
        }

        public override DataTable GetBannerGroups(int PortalId)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBannerGroups", GetNull(PortalId)).Tables[0];
        }

        public override void DeleteBanner(int BannerId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteBanner", BannerId);
        }

        public override int AddBanner(string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName,
                                      int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddBanner",
                                                        BannerName,
                                                        VendorId,
                                                        GetNull(ImageFile),
                                                        GetNull(URL),
                                                        Impressions,
                                                        CPM,
                                                        GetNull(StartDate),
                                                        GetNull(EndDate),
                                                        UserName,
                                                        BannerTypeId,
                                                        GetNull(Description),
                                                        GetNull(GroupName),
                                                        Criteria,
                                                        Width,
                                                        Height));
        }

        public override void UpdateBanner(int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName,
                                          int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateBanner",
                                      BannerId,
                                      BannerName,
                                      GetNull(ImageFile),
                                      GetNull(URL),
                                      Impressions,
                                      CPM,
                                      GetNull(StartDate),
                                      GetNull(EndDate),
                                      UserName,
                                      BannerTypeId,
                                      GetNull(Description),
                                      GetNull(GroupName),
                                      Criteria,
                                      Width,
                                      Height);
        }

        public override IDataReader FindBanners(int PortalId, int BannerTypeId, string GroupName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "FindBanners", GetNull(PortalId), GetNull(BannerTypeId), GetNull(GroupName));
        }

        public override void UpdateBannerViews(int BannerId, DateTime StartDate, DateTime EndDate)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerViews", BannerId, GetNull(StartDate), GetNull(EndDate));
        }

        public override void UpdateBannerClickThrough(int BannerId, int VendorId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerClickThrough", BannerId, VendorId);
        }

        public override IDataReader GetAffiliates(int VendorId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliates", VendorId);
        }

        public override IDataReader GetAffiliate(int AffiliateId, int VendorId, int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliate", AffiliateId, VendorId, GetNull(PortalId));
        }

        public override void DeleteAffiliate(int AffiliateId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteAffiliate", AffiliateId);
        }

        public override int AddAffiliate(int VendorId, DateTime StartDate, DateTime EndDate, double CPC, double CPA)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddAffiliate", VendorId, GetNull(StartDate), GetNull(EndDate), CPC, CPA));
        }

        public override void UpdateAffiliate(int AffiliateId, DateTime StartDate, DateTime EndDate, double CPC, double CPA)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliate", AffiliateId, GetNull(StartDate), GetNull(EndDate), CPC, CPA);
        }

        public override void UpdateAffiliateStats(int AffiliateId, int Clicks, int Acquisitions)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliateStats", AffiliateId, Clicks, Acquisitions);
        }

        public override bool CanDeleteSkin(string SkinType, string SkinFoldername)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "CanDeleteSkin", SkinType, SkinFoldername)) == 1;
        }

        public override int AddSkin(int skinPackageID, string skinSrc)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkin", skinPackageID, skinSrc));
        }

        public override int AddSkinPackage(int packageID, int portalID, string skinName, string skinType, int CreatedByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkinPackage", packageID, GetNull(portalID), skinName, skinType, CreatedByUserID));
        }

        public override void DeleteSkin(int skinID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkin", skinID);
        }

        public override void DeleteSkinPackage(int skinPackageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkinPackage", skinPackageID);
        }

        public override IDataReader GetSkinByPackageID(int packageID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinPackageByPackageID", packageID);
        }

        public override IDataReader GetSkinPackage(int portalID, string skinName, string skinType)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinPackage", GetNull(portalID), skinName, skinType);
        }

        public override void UpdateSkin(int skinID, string skinSrc)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSkin", skinID, skinSrc);
        }

        public override void UpdateSkinPackage(int skinPackageID, int packageID, int portalID, string skinName, string skinType, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSkinPackage", skinPackageID, packageID, GetNull(portalID), skinName, skinType, LastModifiedByUserID);
        }

        public override IDataReader GetAllProfiles()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllProfiles");
        }

        public override IDataReader GetProfile(int UserId, int PortalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetProfile", UserId, PortalId);
        }

        public override void AddProfile(int UserId, int PortalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddProfile", UserId, PortalId);
        }

        public override void UpdateProfile(int UserId, int PortalId, string ProfileData)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateProfile", UserId, PortalId, ProfileData);
        }

        //profile property definitions
        public override int AddPropertyDefinition(int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required,
                                                  string ValidationExpression, int ViewOrder, bool Visible, int Length, int DefaultVisibility, int CreatedByUserID)
        {
            int retValue = Null.NullInteger;
            try
            {
                retValue =
                    Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                            DatabaseOwner + ObjectQualifier + "AddPropertyDefinition",
                                                            GetNull(PortalId),
                                                            ModuleDefId,
                                                            DataType,
                                                            DefaultValue,
                                                            PropertyCategory,
                                                            PropertyName,
                                                            Required,
                                                            ValidationExpression,
                                                            ViewOrder,
                                                            Visible,
                                                            Length,
                                                            DefaultVisibility,
                                                            CreatedByUserID));
            }
            catch (SqlException ex)
            {
                DnnLog.Debug(ex);

                //If not a duplicate (throw an Exception)
                retValue = -ex.Number;
                if (ex.Number != 2601)
                {
                    throw ex;
                }
            }
            return retValue;
        }

        public override void DeletePropertyDefinition(int definitionId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePropertyDefinition", definitionId);
        }

        public override IDataReader GetPropertyDefinition(int definitionId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinition", definitionId);
        }

        public override IDataReader GetPropertyDefinitionByName(int portalId, string name)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionByName", GetNull(portalId), name);
        }

        public override IDataReader GetPropertyDefinitionsByPortal(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionsByPortal", GetNull(portalId));
        }

        public override void UpdatePropertyDefinition(int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required,
                                                      string ValidationExpression, int ViewOrder, bool Visible, int Length, int DefaultVisibility, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdatePropertyDefinition",
                                      PropertyDefinitionId,
                                      DataType,
                                      DefaultValue,
                                      PropertyCategory,
                                      PropertyName,
                                      Required,
                                      ValidationExpression,
                                      ViewOrder,
                                      Visible,
                                      Length,
                                      DefaultVisibility,
                                      LastModifiedByUserID);
        }

        public override IDataReader GetUrls(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrls", PortalID);
        }

        public override IDataReader GetUrl(int PortalID, string Url)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrl", PortalID, Url);
        }

        public override void AddUrl(int PortalID, string Url)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrl", PortalID, Url);
        }

        public override void DeleteUrl(int PortalID, string Url)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrl", PortalID, Url);
        }

        public override IDataReader GetUrlTracking(int PortalID, string Url, int ModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlTracking", PortalID, Url, GetNull(ModuleID));
        }

        public override void AddUrlTracking(int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlTracking", PortalID, Url, UrlType, LogActivity, TrackClicks, GetNull(ModuleID), NewWindow);
        }

        public override void UpdateUrlTracking(int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTracking", PortalID, Url, LogActivity, TrackClicks, GetNull(ModuleID), NewWindow);
        }

        public override void DeleteUrlTracking(int PortalID, string Url, int ModuleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrlTracking", PortalID, Url, GetNull(ModuleID));
        }

        public override void UpdateUrlTrackingStats(int PortalID, string Url, int ModuleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTrackingStats", PortalID, Url, GetNull(ModuleID));
        }

        public override IDataReader GetUrlLog(int UrlTrackingID, DateTime StartDate, DateTime EndDate)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlLog", UrlTrackingID, GetNull(StartDate), GetNull(EndDate));
        }

        public override void AddUrlLog(int UrlTrackingID, int UserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlLog", UrlTrackingID, GetNull(UserID));
        }

        public override IDataReader GetPermissionsByModuleDefID(int ModuleDefID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleDefID", ModuleDefID);
        }

        public override IDataReader GetPermissionsByModuleID(int ModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleID", ModuleID);
        }

        public override IDataReader GetPermissionsByPortalDesktopModule()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByPortalDesktopModule");
        }

        public override IDataReader GetPermissionsByFolder()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByFolder");
        }

        public override IDataReader GetPermissionByCodeAndKey(string PermissionCode, string PermissionKey)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionByCodeAndKey", GetNull(PermissionCode), GetNull(PermissionKey));
        }

        public override IDataReader GetPermissionsByTab()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByTab");
        }

        public override IDataReader GetPermission(int permissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermission", permissionID);
        }

        public override void DeletePermission(int permissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePermission", permissionID);
        }

        public override int AddPermission(string permissionCode, int moduleDefID, string permissionKey, string permissionName, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPermission", moduleDefID, permissionCode, permissionKey, permissionName, createdByUserID));
        }

        public override void UpdatePermission(int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdatePermission",
                                      permissionID,
                                      permissionCode,
                                      moduleDefID,
                                      permissionKey,
                                      permissionName,
                                      lastModifiedByUserID);
        }

        public override IDataReader GetModulePermission(int modulePermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermission", modulePermissionID);
        }

        public override IDataReader GetModulePermissionsByModuleID(int moduleID, int PermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByModuleID", moduleID, PermissionID);
        }

        public override IDataReader GetModulePermissionsByPortal(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByPortal", PortalID);
        }

        public override IDataReader GetModulePermissionsByTabID(int TabID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByTabID", TabID);
        }

        public override void DeleteModulePermissionsByModuleID(int ModuleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermissionsByModuleID", ModuleID);
        }

        public override void DeleteModulePermissionsByUserID(int PortalID, int UserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermissionsByUserID", PortalID, UserID);
        }

        public override void DeleteModulePermission(int modulePermissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermission", modulePermissionID);
        }

        public override int AddModulePermission(int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddModulePermission",
                                                        moduleID,
                                                        PermissionID,
                                                        GetRoleNull(roleID),
                                                        AllowAccess,
                                                        GetNull(UserID),
                                                        createdByUserID));
        }

        public override void UpdateModulePermission(int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateModulePermission",
                                      modulePermissionID,
                                      moduleID,
                                      PermissionID,
                                      GetRoleNull(roleID),
                                      AllowAccess,
                                      GetNull(UserID),
                                      lastModifiedByUserID);
        }

        public override IDataReader GetTabPermissionsByPortal(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByPortal", GetNull(PortalID));
        }

        public override IDataReader GetTabPermissionsByTabID(int TabID, int PermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByTabID", TabID, PermissionID);
        }

        public override void DeleteTabPermissionsByTabID(int TabID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermissionsByTabID", TabID);
        }

        public override void DeleteTabPermissionsByUserID(int PortalID, int UserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermissionsByUserID", PortalID, UserID);
        }

        public override void DeleteTabPermission(int TabPermissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermission", TabPermissionID);
        }

        public override int AddTabPermission(int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddTabPermission",
                                                        TabID,
                                                        PermissionID,
                                                        GetRoleNull(roleID),
                                                        AllowAccess,
                                                        GetNull(UserID),
                                                        createdByUserID));
        }

        public override void UpdateTabPermission(int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateTabPermission",
                                      TabPermissionID,
                                      TabID,
                                      PermissionID,
                                      GetRoleNull(roleID),
                                      AllowAccess,
                                      GetNull(UserID),
                                      lastModifiedByUserID);
        }

        public override IDataReader GetDesktopModulePermission(int desktopModulePermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermission", desktopModulePermissionID);
        }

        public override IDataReader GetDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermissionsByPortalDesktopModuleID", portalDesktopModuleID);
        }

        public override IDataReader GetDesktopModulePermissions()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermissions");
        }

        public override void DeleteDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermissionsByPortalDesktopModuleID", portalDesktopModuleID);
        }

        public override void DeleteDesktopModulePermissionsByUserID(int userID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermissionsByUserID", userID);
        }

        public override void DeleteDesktopModulePermission(int desktopModulePermissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermission", desktopModulePermissionID);
        }

        public override int AddDesktopModulePermission(int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddDesktopModulePermission",
                                                        portalDesktopModuleID,
                                                        permissionID,
                                                        GetRoleNull(roleID),
                                                        allowAccess,
                                                        GetNull(userID),
                                                        createdByUserID));
        }

        public override void UpdateDesktopModulePermission(int desktopModulePermissionID, int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID,
                                                           int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateDesktopModulePermission",
                                      desktopModulePermissionID,
                                      portalDesktopModuleID,
                                      permissionID,
                                      GetRoleNull(roleID),
                                      allowAccess,
                                      GetNull(userID),
                                      lastModifiedByUserID);
        }

        public override IDataReader GetFoldersByPortal(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolders", GetNull(PortalID));
        }

        public override IDataReader GetFoldersByPortalAndPermissions(int PortalID, string Permissions, int UserID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFoldersByPermissions", GetNull(PortalID), GetNull(Permissions), GetNull(UserID), -1, "");
        }

        public override IDataReader GetFolder(int FolderID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderID", FolderID);
        }

        public override IDataReader GetFolder(int PortalID, string FolderPath)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderPath", GetNull(PortalID), FolderPath);
        }

        public override IDataReader GetFolderByUniqueID(Guid UniqueID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByUniqueID", UniqueID);
        }

        public override int AddFolder(int PortalID, Guid UniqueId, Guid VersionGuid, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated, int createdByUserID, int folderMappingID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddFolder",
                                                        GetNull(PortalID),
                                                        UniqueId,
                                                        VersionGuid,
                                                        FolderPath,
                                                        StorageLocation,
                                                        IsProtected,
                                                        IsCached,
                                                        GetNull(LastUpdated),
                                                        createdByUserID,
                                                        folderMappingID));
        }

        public override void UpdateFolder(int PortalID, Guid VersionGuid, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated, int lastModifiedByUserID, int folderMappingID)
        {
            SqlHelper.ExecuteScalar(ConnectionString,
                                    DatabaseOwner + ObjectQualifier + "UpdateFolder",
                                    GetNull(PortalID),
                                    VersionGuid,
                                    FolderID,
                                    FolderPath,
                                    StorageLocation,
                                    IsProtected,
                                    IsCached,
                                    GetNull(LastUpdated),
                                    lastModifiedByUserID,
                                    folderMappingID);
        }

        public override void DeleteFolder(int PortalID, string FolderPath)
        {
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolder", GetNull(PortalID), FolderPath);
        }

        public override void UpdateFolderVersion(int FolderId, Guid VersionGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolderVersion", FolderId, VersionGuid);
        }

        public override IDataReader GetFolderPermission(int FolderPermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermission", FolderPermissionID);
        }

        public override IDataReader GetFolderPermissionsByPortal(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermissionsByPortal", GetNull(PortalID));
        }

        public override IDataReader GetFolderPermissionsByFolderPath(int PortalID, string FolderPath, int PermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermissionsByFolderPath", GetNull(PortalID), FolderPath, PermissionID);
        }

        public override void DeleteFolderPermissionsByFolderPath(int PortalID, string FolderPath)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermissionsByFolderPath", GetNull(PortalID), FolderPath);
        }

        public override void DeleteFolderPermissionsByUserID(int PortalID, int UserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermissionsByUserID", PortalID, UserID);
        }

        public override void DeleteFolderPermission(int FolderPermissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermission", FolderPermissionID);
        }

        public override int AddFolderPermission(int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddFolderPermission",
                                                        FolderID,
                                                        PermissionID,
                                                        GetRoleNull(roleID),
                                                        AllowAccess,
                                                        GetNull(UserID),
                                                        createdByUserID));
        }

        public override void UpdateFolderPermission(int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateFolderPermission",
                                      FolderPermissionID,
                                      FolderID,
                                      PermissionID,
                                      GetRoleNull(roleID),
                                      AllowAccess,
                                      GetNull(UserID),
                                      lastModifiedByUserID);
        }

        public override IDataReader GetSearchIndexers()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchIndexers");
        }

        public override IDataReader GetSearchResultModules(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResultModules", PortalID);
        }

        public override void DeleteSearchItems(int ModuleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItems", ModuleID);
        }

        public override void DeleteSearchItem(int SearchItemId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItem", SearchItemId);
        }

        public override void DeleteSearchItemWords(int SearchItemId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItemWords", SearchItemId);
        }

        public override int AddSearchItem(string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddSearchItem",
                                                        Title,
                                                        Description,
                                                        GetNull(Author),
                                                        GetNull(PubDate),
                                                        ModuleId,
                                                        Key,
                                                        Guid,
                                                        ImageFileId));
        }

        public override IDataReader GetSearchCommonWordsByLocale(string Locale)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchCommonWordsByLocale", Locale);
        }

        public override IDataReader GetDefaultLanguageByModule(string ModuleList)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDefaultLanguageByModule", ModuleList);
        }

        public override IDataReader GetSearchSettings(int ModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchSettings", ModuleId);
        }

        public override IDataReader GetSearchWords()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchWords");
        }

        public override int AddSearchWord(string Word)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchWord", Word));
        }

        public override int AddSearchItemWord(int SearchItemId, int SearchWordsID, int Occurrences)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWord", SearchItemId, SearchWordsID, Occurrences));
        }

        public override void AddSearchItemWordPosition(int SearchItemWordID, string ContentPositions)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWordPosition", SearchItemWordID, ContentPositions);
        }

        public override IDataReader GetSearchResults(int PortalID, string Word)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResultsByWord", PortalID, Word);
        }

        public override IDataReader GetSearchItems(int PortalID, int TabID, int ModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItems", GetNull(PortalID), GetNull(TabID), GetNull(ModuleID));
        }

        public override IDataReader GetSearchResults(int PortalID, int TabID, int ModuleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResults", GetNull(PortalID), GetNull(TabID), GetNull(ModuleID));
        }

        public override IDataReader GetSearchItem(int ModuleID, string SearchKey)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItem", GetNull(ModuleID), SearchKey);
        }

        public override void UpdateSearchItem(int SearchItemId, string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId)
        {
            SqlHelper.ExecuteScalar(ConnectionString,
                                    DatabaseOwner + ObjectQualifier + "UpdateSearchItem",
                                    SearchItemId,
                                    Title,
                                    Description,
                                    GetNull(Author),
                                    GetNull(PubDate),
                                    ModuleId,
                                    Key,
                                    Guid,
                                    HitCount,
                                    ImageFileId);
        }

        public override IDataReader GetLists(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLists", PortalID);
        }

        public override IDataReader GetList(string ListName, string ParentKey, int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetList", ListName, ParentKey, PortalID);
        }

        public override IDataReader GetListEntry(string ListName, string Value)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntry", ListName, Value, -1);
        }

        public override IDataReader GetListEntry(int EntryID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntry", "", "", EntryID);
        }

        public override IDataReader GetListEntriesByListName(string ListName, string ParentKey, int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntries", ListName, ParentKey, GetNull(PortalID));
        }

        public override int AddListEntry(string ListName, string Value, string Text, int ParentID, int Level, bool EnableSortOrder, int DefinitionID, string Description, int PortalID, bool SystemList,
                                         int CreatedByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddListEntry",
                                                        ListName,
                                                        Value,
                                                        Text,
                                                        ParentID,
                                                        Level,
                                                        EnableSortOrder,
                                                        DefinitionID,
                                                        Description,
                                                        PortalID,
                                                        SystemList,
                                                        CreatedByUserID));
        }

        public override void UpdateListEntry(int EntryID, string Value, string Text, string Description, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListEntry", EntryID, Value, Text, Description, LastModifiedByUserID);
        }

        public override void DeleteListEntryByID(int EntryID, bool DeleteChild)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByID", EntryID, DeleteChild);
        }

        public override void DeleteList(string ListName, string ParentKey)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteList", ListName, ParentKey);
        }

        public override void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByListName", ListName, Value, DeleteChild);
        }

        public override void UpdateListSortOrder(int EntryID, bool MoveUp)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListSortOrder", EntryID, MoveUp);
        }

        public override IDataReader GetPortalAlias(string PortalAlias, int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAlias", PortalAlias, PortalID);
        }

        public override IDataReader GetPortalByPortalAliasID(int PortalAliasId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByPortalAliasID", PortalAliasId);
        }

        public override void UpdatePortalAlias(string PortalAlias, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAliasOnInstall", PortalAlias, lastModifiedByUserID);
        }

        public override void UpdatePortalAliasInfo(int PortalAliasID, int PortalID, string HTTPAlias, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAlias", PortalAliasID, PortalID, HTTPAlias, lastModifiedByUserID);
        }

        public override int AddPortalAlias(int PortalID, string HTTPAlias, int createdByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalAlias", PortalID, HTTPAlias, createdByUserID));
        }

        public override void DeletePortalAlias(int PortalAliasID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalAlias", PortalAliasID);
        }

        public override IDataReader GetPortalAliasByPortalID(int PortalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalID", PortalID);
        }

        public override IDataReader GetPortalAliasByPortalAliasID(int PortalAliasID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalAliasID", PortalAliasID);
        }

        public override int AddEventMessage(string eventName, int priority, string processorType, string processorCommand, string body, string sender, string subscriberId, string authorizedRoles,
                                            string exceptionMessage, DateTime sentDate, DateTime expirationDate, string attributes)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString,
                                             DatabaseOwner + ObjectQualifier + "AddEventMessage",
                                             eventName,
                                             priority,
                                             processorType,
                                             processorCommand,
                                             body,
                                             sender,
                                             subscriberId,
                                             authorizedRoles,
                                             exceptionMessage,
                                             sentDate,
                                             expirationDate,
                                             attributes);
        }

        public override IDataReader GetEventMessages(string eventName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventMessages", eventName);
        }

        public override IDataReader GetEventMessagesBySubscriber(string eventName, string subscriberId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventMessagesBySubscriber", eventName, subscriberId);
        }

        public override void SetEventMessageComplete(int eventMessageId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "SetEventMessageComplete", eventMessageId);
        }

        public override int AddAuthentication(int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int CreatedByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddAuthentication",
                                                        packageID,
                                                        authenticationType,
                                                        isEnabled,
                                                        settingsControlSrc,
                                                        loginControlSrc,
                                                        logoffControlSrc,
                                                        CreatedByUserID));
        }

        public override int AddUserAuthentication(int userID, string authenticationType, string authenticationToken, int CreatedByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUserAuthentication", userID, authenticationType, authenticationToken, CreatedByUserID));
        }

        public override void DeleteAuthentication(int authenticationID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteAuthentication", authenticationID);
        }

        public override IDataReader GetAuthenticationService(int authenticationID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationService", authenticationID);
        }

        public override IDataReader GetAuthenticationServiceByPackageID(int packageID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServiceByPackageID", packageID);
        }

        public override IDataReader GetAuthenticationServiceByType(string authenticationType)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServiceByType", authenticationType);
        }

        public override IDataReader GetAuthenticationServices()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServices");
        }

        public override IDataReader GetEnabledAuthenticationServices()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEnabledAuthenticationServices");
        }

        public override void UpdateAuthentication(int authenticationID, int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc,
                                                  string logoffControlSrc, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateAuthentication",
                                      authenticationID,
                                      packageID,
                                      authenticationType,
                                      isEnabled,
                                      settingsControlSrc,
                                      loginControlSrc,
                                      logoffControlSrc,
                                      LastModifiedByUserID);
        }

        public override int AddPackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner,
                                       string organization, string url, string email, string releaseNotes, bool isSystemPackage, int createdByUserID, string folderName, string iconFile)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddPackage",
                                                        GetNull(portalID),
                                                        name,
                                                        friendlyName,
                                                        description,
                                                        type,
                                                        version,
                                                        license,
                                                        manifest,
                                                        owner,
                                                        organization,
                                                        url,
                                                        email,
                                                        releaseNotes,
                                                        isSystemPackage,
                                                        createdByUserID,
                                                        folderName,
                                                        iconFile));
        }

        public override void DeletePackage(int packageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePackage", packageID);
        }

        public override IDataReader GetPackage(int packageID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackage", packageID);
        }

        public override IDataReader GetPackageByName(int portalID, string name)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageByName", GetNull(portalID), name);
        }

        public override IDataReader GetPackages(int portalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackages", GetNull(portalID));
        }

        public override IDataReader GetPackagesByType(int portalID, string type)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackagesByType", GetNull(portalID), type);
        }

        public override IDataReader GetPackageType(string type)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageType", type);
        }

        public override IDataReader GetPackageTypes()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageTypes");
        }

        public override IDataReader GetModulePackagesInUse(int portalID, bool forHost)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePackagesInUse", portalID, forHost);
        }

        public override int RegisterAssembly(int packageID, string assemblyName, string version)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "RegisterAssembly", packageID, assemblyName, version));
        }

        public override bool UnRegisterAssembly(int packageID, string assemblyName)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UnRegisterAssembly", packageID, assemblyName)) == 1;
        }

        public override void UpdatePackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner,
                                           string organization, string url, string email, string releaseNotes, bool isSystemPackage, int lastModifiedByUserID, string folderName, string iconFile)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdatePackage",
                                      GetNull(portalID),
                                      name,
                                      friendlyName,
                                      description,
                                      type,
                                      version,
                                      license,
                                      manifest,
                                      owner,
                                      organization,
                                      url,
                                      email,
                                      releaseNotes,
                                      isSystemPackage,
                                      lastModifiedByUserID,
                                      folderName,
                                      iconFile);
        }

        public override int AddLanguage(string cultureCode, string cultureName, string fallbackCulture, int CreatedByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddLanguage", cultureCode, cultureName, fallbackCulture, CreatedByUserID));
        }

        public override void DeleteLanguage(int languageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteLanguage", languageID);
        }

        public override IDataReader GetLanguages()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguages");
        }

        public override void UpdateLanguage(int languageID, string cultureCode, string cultureName, string fallbackCulture, int LastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateLanguage", languageID, cultureCode, cultureName, fallbackCulture, LastModifiedByUserID);
        }

        public override int AddPortalLanguage(int portalID, int languageID, bool IsPublished, int CreatedByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalLanguage", portalID, languageID, IsPublished, CreatedByUserID));
        }

        public override void DeletePortalLanguages(int portalID, int languageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalLanguages", GetNull(portalID), GetNull(languageID));
        }

        public override IDataReader GetLanguagesByPortal(int portalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguagesByPortal", portalID);
        }

        public override void UpdatePortalLanguage(int portalID, int languageID, bool IsPublished, int UpdatedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalLanguage", portalID, languageID, IsPublished, UpdatedByUserID);
        }

        public override int AddLanguagePack(int packageID, int languageID, int dependentPackageID, int CreatedByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddLanguagePack", packageID, languageID, dependentPackageID, CreatedByUserID));
        }

        public override void DeleteLanguagePack(int languagePackID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteLanguagePack", languagePackID);
        }

        public override IDataReader GetLanguagePackByPackage(int packageID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguagePackByPackage", packageID);
        }

        public override int UpdateLanguagePack(int languagePackID, int packageID, int languageID, int dependentPackageID, int LastModifiedByUserID)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateLanguagePack", languagePackID, packageID, languageID, dependentPackageID, LastModifiedByUserID);
        }

        //localisation
        public override string GetPortalDefaultLanguage(int portalID)
        {
            return Convert.ToString(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalDefaultLanguage", portalID));
        }

        public override void UpdatePortalDefaultLanguage(int portalID, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalDefaultLanguage", portalID, CultureCode);
        }

        public override void EnsureLocalizationExists(int portalID, string CultureCode)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "EnsureLocalizationExists", portalID, CultureCode);
        }

        //folder mappings
        public override int AddFolderMapping(int portalID, string mappingName, string folderProviderType, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolderMapping", GetNull(portalID), mappingName, folderProviderType, createdByUserID));
        }

        public override void AddFolderMappingSetting(int folderMappingID, string settingName, string settingValue, int createdByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolderMappingsSetting", folderMappingID, settingName, settingValue, createdByUserID);
        }

        public override void DeleteFolderMapping(int folderMappingID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderMapping", folderMappingID);
        }

        public override IDataReader GetFolderMapping(int folderMappingID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderMapping", folderMappingID);
        }

        public override IDataReader GetFolderMappingByMappingName(int portalID, string mappingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderMappingByMappingName", portalID, mappingName);
        }

        public override IDataReader GetFolderMappings(int portalID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderMappings", GetNull(portalID));
        }

        public override IDataReader GetFolderMappingSettings(int folderMappingID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderMappingsSettings", folderMappingID);
        }

        public override void UpdateFolderMapping(int folderMappingID, string mappingName, int priority, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolderMapping", folderMappingID, mappingName, priority, lastModifiedByUserID);
        }

        public override void UpdateFolderMappingSetting(int folderMappingID, string settingName, string settingValue, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolderMappingsSetting", folderMappingID, settingName, settingValue, lastModifiedByUserID);
        }

        public override IDataReader GetFolderMappingSetting(int folderMappingID, string settingName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderMappingsSetting", folderMappingID, settingName);
        }

        public override void AddDefaultFolderTypes(int portalID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddDefaultFolderTypes", portalID);
        }

        //SystemDateTime Utility
        public override DateTime GetDatabaseTimeUtc()
        {
            return Convert.ToDateTime(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseTimeUtc"));
        }

        public override DateTime GetDatabaseTime()
        {
            return Convert.ToDateTime(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseTime"));
        }

		#region Mobile Stuff

		public override void DeletePreviewProfile(int id)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_DeletePreviewProfile", id);
		}

		public override void DeleteRedirection(int id)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_DeleteRedirection", id);
		}

		public override void DeleteRedirectionRule(int id)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_DeleteRedirectionRule", id);
		}

		public override IDataReader GetPreviewProfiles(int portalId)
		{
			return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_GetPreviewProfiles", portalId);
		}

		public override IDataReader GetAllRedirections()
		{
			return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_GetAllRedirections");
		}

        public override IDataReader GetRedirections(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_GetRedirections", portalId);
        }

		public override IDataReader GetRedirectionRules(int redirectionId)
		{
			return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_GetRedirectionRules", redirectionId);
		}

		public override int SaveRedirection(int id, int portalId, string name, int type, int sortOrder, int sourceTabId, bool includeChildTabs, int targetType, object targetValue, bool enabled, int userId)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_SaveRedirection", id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled, userId));
		}

		public override void SaveRedirectionRule(int id, int redirectionId, string capbility, string expression)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_SaveRedirectionRule", id, redirectionId, capbility, expression);
		}

		public override int SavePreviewProfile(int id, int portalId, string name, int width, int height, string userAgent, int sortOrder, int userId)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "Mobile_SavePreviewProfile", id, portalId, name, width, height, userAgent, sortOrder, userId));
		}

		#endregion
	}
}
