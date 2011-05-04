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
using System.Collections;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;

#endregion

namespace DotNetNuke.Modules.Admin.Scheduler
{
    public partial class ViewSchedule : PortalModuleBase, IActionable
    {
        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "add.gif",
                            EditUrl(),
                            false,
                            SecurityAccessLevel.Host,
                            true,
                            false);
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.ContentOptions, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "icon_scheduler_16px.gif",
                            EditUrl("", "", "Status"),
                            false,
                            SecurityAccessLevel.Host,
                            true,
                            false);
                Actions.Add(GetNextActionID(),
                            Localization.GetString("ScheduleHistory.Action", LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "icon_viewScheduleHistory_16px.gif",
                            EditUrl("", "", "History"),
                            false,
                            SecurityAccessLevel.Host,
                            true,
                            false);
                return Actions;
            }
        }

        #endregion

        protected string GetTimeLapse(int TimeLapse, string TimeLapseMeasurement)
        {
            if (TimeLapse != Null.NullInteger)
            {
                string str = Null.NullString;
                string strPrefix = Localization.GetString("TimeLapsePrefix", LocalResourceFile);
                string strSec = Localization.GetString("Second", LocalResourceFile);
                string strMn = Localization.GetString("Minute", LocalResourceFile);
                string strHour = Localization.GetString("Hour", LocalResourceFile);
                string strDay = Localization.GetString("Day", LocalResourceFile);
                string strSecs = Localization.GetString("Seconds");
                string strMns = Localization.GetString("Minutes");
                string strHours = Localization.GetString("Hours");
                string strDays = Localization.GetString("Days");
                switch (TimeLapseMeasurement)
                {
                    case "s":
                        str = strPrefix + " " + TimeLapse + " " + (TimeLapse > 1 ? strSecs : strSec);
                        break;
                    case "m":
                        str = strPrefix + " " + TimeLapse + " " + (TimeLapse > 1 ? strMns : strMn);
                        break;
                    case "h":
                        str = strPrefix + " " + TimeLapse + " " + (TimeLapse > 1 ? strHours : strHour);
                        break;
                    case "d":
                        str = strPrefix + " " + TimeLapse + " " + (TimeLapse > 1 ? strDays : strDay);
                        break;
                }
                return str;
            }
            else
            {
                return Localization.GetString("n/a", LocalResourceFile);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                if (!Page.IsPostBack)
                {
                    ArrayList arrSchedule = SchedulingProvider.Instance().GetSchedule();
                    Localization.LocalizeDataGrid(ref dgSchedule, LocalResourceFile);
                    dgSchedule.DataSource = arrSchedule;
                    dgSchedule.DataBind();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}