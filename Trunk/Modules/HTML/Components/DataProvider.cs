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

using System.Data;

using DotNetNuke.Framework;

#endregion

namespace DotNetNuke.Modules.Html
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The DataProvider is an abstract class that provides the Data Access Layer for the HtmlText module
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public abstract partial class DataProvider
    {
        #region "Shared/Static Methods"

        // singleton reference to the instantiated object 

        private static DataProvider objProvider;
        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider) Reflection.CreateObject("data", "DotNetNuke.Modules.Html", "");
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region "Abstract methods"

        public abstract IDataReader GetHtmlText(int ModuleID, int ItemID);

        public abstract IDataReader GetTopHtmlText(int ModuleID, bool IsPublished);

        public abstract IDataReader GetAllHtmlText(int ModuleID);

        public abstract int AddHtmlText(int ModuleId, string Content, int StateID, bool IsPublished, int CreatedByUserID, int History);

        public abstract void UpdateHtmlText(int ItemID, string Content, int StateID, bool IsPublished, int LastModifiedByUserID);

        public abstract void DeleteHtmlText(int ModuleID, int ItemID);

        public abstract IDataReader GetHtmlTextLog(int ItemID);

        public abstract void AddHtmlTextLog(int ItemID, int StateID, string Comment, bool Approved, int CreatedByUserID);

        public abstract IDataReader GetHtmlTextUser(int UserID);

        public abstract void AddHtmlTextUser(int ItemID, int StateID, int ModuleID, int TabID, int UserID);

        public abstract void DeleteHtmlTextUsers();

        public abstract IDataReader GetWorkflows(int PortalID);

        public abstract IDataReader GetWorkflowStates(int WorkflowID);

        public abstract IDataReader GetWorkflowStatePermissions();

        #endregion

        public abstract IDataReader GetWorkflowStatePermissionsByStateID(int StateID);
    }
}