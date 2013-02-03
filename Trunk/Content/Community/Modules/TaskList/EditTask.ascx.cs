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

using System;

using DotNetNuke.Caching;
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Modules.TaskList.Models;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Modules.TaskList
{
    public partial class EditTask : ModuleUserControlBase
    {
        private readonly IDataContext _dataContext = new PetaPocoDataContext("SiteSqlServer",
                                                                             DataProvider.Instance().ObjectQualifier,
                                                                             new NoCache());

        private int TaskId = -1;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            save.Click += save_Click;
            delete.Click += delete_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!String.IsNullOrEmpty(Request.QueryString["TaskID"]))
            {
                TaskId = Int32.Parse(Request.QueryString["TaskID"]);
            }

            using (_dataContext)
            {
                Task task;
                if(TaskId == -1)
                {
                    task = new Task();
                }
                else
                {
                    IRepository<Task> repo = _dataContext.GetRepository<Task>();

                    task = repo.GetById(TaskId);
                }

                editTask.DataSource = task;
                if(!IsPostBack)
                {
                    editTask.DataBind();
                }
            }

            cancel.NavigateUrl = Globals.NavigateURL();
        }

        void delete_Click(object sender, EventArgs e)
        {
            try
            {
                var task = (Task)editTask.DataSource;

                using (_dataContext)
                {
                    IRepository<Task> repo = _dataContext.GetRepository<Task>();

                    repo.Delete(task);

                    _dataContext.Commit();
                }
            }
            catch (Exception exc)
            {
                
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            Response.Redirect(Globals.NavigateURL(), true);
        }

        void save_Click(object sender, EventArgs e)
        {
            try
            {
                var task = (Task)editTask.DataSource;

                using (_dataContext)
                {
                    IRepository<Task> repo = _dataContext.GetRepository<Task>();

                    if (TaskId == -1)
                    {
                        repo.Add(task);
                    }
                    else
                    {
                        repo.Update(task);
                    }

                    _dataContext.Commit();
                }
            }
            catch (Exception exc)
            {

                Exceptions.ProcessModuleLoadException(this, exc);
            }

            Response.Redirect(Globals.NavigateURL(), true);
        }
    }
}