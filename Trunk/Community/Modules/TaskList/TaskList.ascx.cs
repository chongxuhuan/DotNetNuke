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

#region Usings

using System.Web.UI.WebControls;

using DotNetNuke.Caching;
using DotNetNuke.Data;
using DotNetNuke.Modules.TaskList.Models;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Modules.TaskList
{
    public partial class TaskList : ModuleUserControlBase
    {
        private readonly IDataContext _dataContext = new PetaPocoDataContext("SiteSqlServer", 
                                                                            DataProvider.Instance().ObjectQualifier, 
                                                                            new NoCache());

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            string editUrl = ModuleContext.EditUrl("TaskID", "{{KEY}}", "Edit");
            editUrl = editUrl.Replace("{{KEY}}", "{0}");

            var editColumn = tasksGrid.Columns[0] as HyperLinkColumn;
            if (editColumn != null)
            {
                editColumn.DataNavigateUrlFormatString = editUrl;
            }

            addButton.NavigateUrl = ModuleContext.EditUrl("Edit");
        }


        protected override void OnLoad(System.EventArgs e)
        {
            base.OnInit(e);

            using (_dataContext)
            {
                var repo = _dataContext.GetRepository<Task>();

                tasksGrid.DataSource = repo.GetAll();
                tasksGrid.DataBind();
            }
        }
    }
}