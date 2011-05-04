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

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      PurgeUsersOnline
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PurgeUsersOnline class provides a Scheduler for purging the Users Online
    /// data
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/14/2006	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PurgeUsersOnline : SchedulerClient
    {
        public PurgeUsersOnline(ScheduleHistoryItem objScheduleHistoryItem)
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
        }

        private void UpdateUsersOnline()
        {
            var objUserOnlineController = new UserOnlineController();
            if ((objUserOnlineController.IsEnabled()))
            {
                Status = "Updating Users Online";
                objUserOnlineController.UpdateUsersOnline();
                Status = "Update Users Online Successfully";
                ScheduleHistoryItem.Succeeded = true;
            }
        }

        public override void DoWork()
        {
            try
            {
                Progressing();
                UpdateUsersOnline();
                ScheduleHistoryItem.Succeeded = true;
                ScheduleHistoryItem.AddLogNote("UsersOnline purge completed.");
            }
            catch (Exception exc)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("UsersOnline purge failed." + exc);
                Errored(ref exc);
                Exceptions.LogException(exc);
            }
        }
    }
}
