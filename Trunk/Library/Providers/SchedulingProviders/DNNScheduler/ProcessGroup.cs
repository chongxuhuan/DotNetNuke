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
using System.Reflection;
using System.Threading;
using System.Web.Compilation;

#endregion

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class ProcessGroup
    {
        #region Delegates

        public delegate void CompletedEventHandler();

        #endregion

        private static int numberOfProcessesInQueue;
        private static int numberOfProcesses;
        private static int processesCompleted;
        private static int ticksElapsed;

        private static int GetTicksElapsed
        {
            get
            {
                return ticksElapsed;
            }
        }

        private static int GetProcessesCompleted
        {
            get
            {
                return processesCompleted;
            }
        }

        private static int GetProcessesInQueue
        {
            get
            {
                return numberOfProcessesInQueue;
            }
        }

        public event CompletedEventHandler Completed;

        public void Run(ScheduleHistoryItem objScheduleHistoryItem)
        {
            SchedulerClient Process = null;
            try
            {
                ticksElapsed = Environment.TickCount - ticksElapsed;
                Process = GetSchedulerClient(objScheduleHistoryItem.TypeFullName, objScheduleHistoryItem);
                Process.ScheduleHistoryItem = objScheduleHistoryItem;
                Process.ProcessStarted += Scheduler.CoreScheduler.WorkStarted;
                Process.ProcessProgressing += Scheduler.CoreScheduler.WorkProgressing;
                Process.ProcessCompleted += Scheduler.CoreScheduler.WorkCompleted;
                Process.ProcessErrored += Scheduler.CoreScheduler.WorkErrored;
                Process.Started();
                try
                {
                    Process.ScheduleHistoryItem.Succeeded = false;
                    Process.DoWork();
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    if (Process != null)
                    {
                        if (Process.ScheduleHistoryItem != null)
                        {
                            Process.ScheduleHistoryItem.Succeeded = false;
                        }
                        Process.Errored(ref exc);
                    }
                }
                if (Process.ScheduleHistoryItem.Succeeded)
                {
                    Process.Completed();
                }
                if (processesCompleted == numberOfProcesses)
                {
                    if (processesCompleted == numberOfProcesses)
                    {
                        ticksElapsed = Environment.TickCount - ticksElapsed;
                        if (Completed != null)
                        {
                            Completed();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (Process != null)
                {
                    if (Process.ScheduleHistoryItem != null)
                    {
                        Process.ScheduleHistoryItem.Succeeded = false;
                    }
                    Process.Errored(ref exc);
                }
            }
            finally
            {
                numberOfProcessesInQueue -= 1;
                processesCompleted += 1;
            }
        }

        private SchedulerClient GetSchedulerClient(string strProcess, ScheduleHistoryItem objScheduleHistoryItem)
        {
            Type t = BuildManager.GetType(strProcess, true, true);
            var param = new ScheduleHistoryItem[1];
            param[0] = objScheduleHistoryItem;
            var types = new Type[1];
            types[0] = typeof (ScheduleHistoryItem);
            ConstructorInfo objConstructor;
            objConstructor = t.GetConstructor(types);
            return (SchedulerClient) objConstructor.Invoke(param);
        }

        private void RunPooledThread(object objScheduleHistoryItem)
        {
            Run((ScheduleHistoryItem) objScheduleHistoryItem);
        }

        public void AddQueueUserWorkItem(ScheduleItem s)
        {
            numberOfProcessesInQueue += 1;
            numberOfProcesses += 1;
            var obj = new ScheduleHistoryItem(s);
            try
            {
                WaitCallback callback = RunPooledThread;
                ThreadPool.QueueUserWorkItem(callback, obj);
                Thread.Sleep(1000);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.ProcessSchedulerException(exc);
            }
        }
    }
}