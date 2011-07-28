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
using System.Collections.Generic;

using DotNetNuke.Common.Utilities.Internal;

using MbUnit.Framework;

namespace DotNetNuke.Tests.Core
{
    [TestFixture]
    public class RetryableActionTests
    {
        [Test]
        public void ActionSucceedsFirstTime()
        {
            var monitor = new ActionMonitor();
            var retryable = CreateRetryable(monitor.Action);

            retryable.TryIt();

            Assert.AreEqual(1, monitor.TimesCalled);
        }

        [Test]
        public void ActionFailsTwice()
        {
            var monitor = new ActionMonitor(2);
            var retryable = CreateRetryable(monitor.Action);

            retryable.TryIt();

            Assert.AreEqual(3, monitor.TimesCalled);
        }

        [Test]
        public void DelaysIncreaseWithEachRetry()
        {
            var monitor = new ActionMonitor(3);
            var retryable = CreateRetryable(monitor.Action, 10);

            retryable.TryIt();

            var firstRetry = (monitor.CallTime[1] - monitor.CallTime[0]).TotalMilliseconds;
            var secondRetry = (monitor.CallTime[2] - monitor.CallTime[1]).TotalMilliseconds;
            var thirdRetry = (monitor.CallTime[3] - monitor.CallTime[2]).TotalMilliseconds;

            Assert.IsTrue(secondRetry > firstRetry * 5); //factor of 5 vs 10 to allow for lots of slop in the thread.sleep
            Assert.IsTrue(thirdRetry > secondRetry * 5);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ActionNeverSucceeds()
        {
            var monitor = new ActionMonitor(-1);
            var retryable = CreateRetryable(monitor.Action);

            retryable.TryIt();
        }

        private static RetryableAction CreateRetryable(Action action)
        {
            return CreateRetryable(action, 1);
        }

        private static RetryableAction CreateRetryable(Action action, int factor)
        {
            return new RetryableAction(action, "foo", 10, TimeSpan.FromMilliseconds(5), factor);
        }
    }
    
    public class ActionMonitor
    {
        private int _failuresRemaining;
        private readonly List<DateTime> _callTimes = new List<DateTime>();

        public ActionMonitor() : this(0) {}

        public ActionMonitor(int failureCount)
        {
            _failuresRemaining = failureCount;
        }

        public int TimesCalled { get; private set; }

        public IList<DateTime> CallTime
        {
            get
            {
                return _callTimes.AsReadOnly();
            }
        }

        public void Action()
        {
            _callTimes.Add(DateTime.Now);
            TimesCalled++;

            if(_failuresRemaining != 0)
            {
                if (_failuresRemaining > 0)
                {
                    _failuresRemaining--;
                }
                
                throw new Exception("it failed");
            }
        }
    }
}