// '
// ' DotNetNuke® - http://www.dotnetnuke.com
// ' Copyright (c) 2002-2010
// ' by DotNetNuke Corporation
// '
// ' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// ' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// ' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// ' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// '
// ' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// ' of the Software.
// '
// ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// ' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// ' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// ' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// ' DEALINGS IN THE SOFTWARE.
// '

using System;
using System.Collections.Generic;
using System.Threading;
using DotNetNuke.Collections.Internal;
using MbUnit.Framework;
using ThreadState = System.Threading.ThreadState;

namespace DotNetNuke.Tests.Core.Collections
{
    public abstract class LockStrategyTests
    {
        internal abstract ILockStrategy GetLockStrategy();

        [Test]
        public void DoubleDisposeAllowed()
        {
            var strategy = GetLockStrategy();

            strategy.Dispose();
            strategy.Dispose();
            //no exception on 2nd dispose
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void DoubleReadLockThrows()
        {
            using (var strategy = GetLockStrategy())
            {
                using (var readLock1 = strategy.GetReadLock())
                {
                    using (var readLock2 = strategy.GetReadLock())
                    {
                        //do nothing
                    }
                }
            }
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void ReadAndWriteLockOnSameThreadThrows()
        {
            using (var strategy = GetLockStrategy())
            {
                using (var readLock1 = strategy.GetReadLock())
                {
                    using (var readLock2 = strategy.GetWriteLock())
                    {
                        //do nothing
                    }
                }
            }
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void WriteAndReadLockOnSameThreadThrows()
        {
            using (var strategy = GetLockStrategy())
            {
                using (var readLock1 = strategy.GetWriteLock())
                {
                    using (var readLock2 = strategy.GetReadLock())
                    {
                        //do nothing
                    }
                }
            }
        }

        [Test]
        public void DoubleReadLockOnDifferentThreads()
        {
            using (var strategy = GetLockStrategy())
            {
                if (strategy.SupportsConcurrentReads)
                {
                    using (var readLock1 = strategy.GetReadLock())
                    {

                        var t = new Thread(GetReadLock);
                        t.Start(strategy);

                        //sleep and let new thread run
                        t.Join(TimeSpan.FromMilliseconds(100));

                        //assert that read thread has terminated
                        Console.WriteLine(t.ThreadState.ToString());
                        Assert.IsTrue(t.ThreadState == ThreadState.Stopped);
                    }
                }
            }
        }

        [Test]
        public void DoubleWriteLockOnDifferentThreadsWaits()
        {
            using (ILockStrategy strategy = GetLockStrategy())
            {
                Thread t;
                using (var writeLock1 = strategy.GetWriteLock())
                {
                    t = new Thread(GetWriteLock);
                    t.Start(strategy);

                    //sleep and let new thread run and block
                    Thread.Sleep(50);

                    //assert that write thread has not terminated
                    Assert.IsTrue(t.IsAlive);
                } //release write lock

                //loop up to 2 seconds (40*50ms) waiting for thread to terminate
                //no termination in 2 seconds is likely a failure
                for(int i = 0; i < 40; i++)
                {
                    Thread.Sleep(50);
                    if (!t.IsAlive) break;
                }

                //assert that getwritelock did complete once first writelock was released it's call
                Assert.IsFalse(t.IsAlive);
            }
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void DoubleWriteLockThrows()
        {
            using (ILockStrategy strategy = GetLockStrategy())
            {
                using (var writeLock1 = strategy.GetWriteLock())
                {
                    using (var writeLock2 = strategy.GetWriteLock())
                    {
                        //do nothing
                    }
                }
            }
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        [Factory("GetObjectDisposedExceptionMethods")]
        internal void MethodsThrowAfterDisposed(Action<ILockStrategy> methodCall)
        {
            var strategy = GetLockStrategy();

            strategy.Dispose();
            methodCall.Invoke(strategy);
        }

        [Test]
        public void ReadLockPreventsWriteLock()
        {
            Thread t = null;

            using (var strategy = GetLockStrategy())
            {
                using (var readLock = strategy.GetReadLock())
                {
                    t = new Thread(LockStrategyTests.GetWriteLock);
                    t.Start(strategy);

                    //sleep and let new thread run
                    Thread.Sleep(100);

                    //assert that write thread is still waiting
                    Assert.IsTrue(t.IsAlive);
                }
                //release read lock

                //sleep and let write thread finish up
                Thread.Sleep(100);
                Assert.IsFalse(t.IsAlive);
            }
            //release controller
        }

        [Test]
        public void OnlyOneWriteLockAllowed()
        {
            Thread t = null;

            using (var strategy = GetLockStrategy())
            {
                using (var writeLock = strategy.GetWriteLock())
                {
                    t = new Thread(LockStrategyTests.GetWriteLock);
                    t.Start(strategy);

                    //sleep and let new thread run
                    Thread.Sleep(100);

                    //assert that write thread is still waiting
                    Assert.IsTrue(t.IsAlive);
                }
                //release write lock
                //sleep and let write thread finish up
                Thread.Sleep(100);
                Assert.IsFalse(t.IsAlive);
            }
        }

        [Test]
        public void MultipleReadLocksAllowed()
        {
            Thread t = null;

            using (var strategy = GetLockStrategy())
            {
                if (strategy.SupportsConcurrentReads)
                {
                    using (var readLock = strategy.GetReadLock())
                    {
                        t = new Thread(LockStrategyTests.GetReadLock);
                        t.Start(strategy);

                        //sleep and let new thread run
                        Thread.Sleep(100);

                        //assert that read thread has terminated
                        Assert.IsFalse(t.IsAlive);
                    }
                }
                else
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [Test]
        public void TimeSpanTimedWriteLockThrows()
        {
            using(var strategy = GetLockStrategy())
            {
                var lockHolder = new WriteLockHolder(strategy);
                lockHolder.EstablishLock();

                try
                {
                    strategy.GetWriteLock(TimeSpan.FromMilliseconds(50));
                }
                catch (ApplicationException)
                {
                    Assert.IsTrue(true);
                }

                lockHolder.ReleaseLock();
            }
        }

        [Test]
        public void TimeSpanTimedReadLockThrows()
        {
            using (var strategy = GetLockStrategy())
            {
                var lockHolder = new WriteLockHolder(strategy);
                lockHolder.EstablishLock();

                try
                {
                    strategy.GetReadLock(TimeSpan.FromMilliseconds(50));
                }
                catch (ApplicationException) //todo change to a namespace specific exception
                {
                    Assert.IsTrue(true);
                }

                lockHolder.ReleaseLock();
            }
        }

        protected virtual IEnumerable<Action<ILockStrategy>> GetObjectDisposedExceptionMethods()
        {
            var l = new List<Action<ILockStrategy>>();

            l.Add((ILockStrategy strategy) => strategy.GetReadLock());
            l.Add((ILockStrategy strategy) => strategy.GetWriteLock());
            l.Add((ILockStrategy strategy) => Console.WriteLine(strategy.ThreadCanRead));
            l.Add((ILockStrategy strategy) => Console.WriteLine(strategy.ThreadCanWrite));

            return l;
        }

        private static void GetReadLock(object obj)
        {
            var strategy = (ILockStrategy)obj;
            using (var readLock = strategy.GetReadLock())
            {
                //do nothing
            }
        }

        private static void GetWriteLock(object obj)
        {
            var strategy = (ILockStrategy)obj;
            using (var writeLock = strategy.GetWriteLock())
            {
                //do nothing
            }
        }

        private class WriteLockHolder
        {
            //this could be IDisposable and then it would work as
            //  using(var lockHolder as new WriteLockHolder)
            //  {
            //    do stuff here
            //  }
            private volatile bool _releaseWriteLock;
            private volatile bool _holdingWriteLock;
            private readonly ILockStrategy _strategy;

            public WriteLockHolder(ILockStrategy strategy)
            {
                _strategy = strategy;
            }

            /// <summary>
            /// Start a new thread and hold a write lock on that thread
            /// </summary>
            public void EstablishLock()
            {
                _releaseWriteLock = false;
                var t = new Thread(HoldWriteLock);
                t.Start(_strategy);

                do
                {
                    Thread.Sleep(10);
                } while (!_holdingWriteLock);
            }

            /// <summary>
            /// Release the write lock that was established by EstablishLock
            /// </summary>
            public void ReleaseLock()
            {
                _releaseWriteLock = true;

                do
                {
                    Thread.Sleep(10);
                } while (_holdingWriteLock);
            }

            private void HoldWriteLock(object obj)
            {
                var strategy = (ILockStrategy)obj;
                using (var writeLock = strategy.GetWriteLock())
                {
                    _holdingWriteLock = true;
                    while (!_releaseWriteLock)
                    {
                        Thread.Sleep(10);
                    }
                }

                _holdingWriteLock = false;
            }
        }

        //todo test dispose of Sharedcollectinlock after controller is disposed

    }
}
