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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DotNetNuke.Collections;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace DotNetNuke.Tests.Core.Collections
{
    public abstract class SharedDictionaryTests
    {

        public abstract LockingStrategy LockingStrategy { get; }

        [Test]
        public void TryAdd()
        {
            const string KEY = "key";
            const string VALUE = "value";

            var sharedDictionary = new SharedDictionary<string, string>(this.LockingStrategy);

            bool doInsert = false;
            using (ISharedCollectionLock l = sharedDictionary.GetReadLock())
            {
                if (!sharedDictionary.ContainsKey(KEY))
                {
                    doInsert = true;
                }
            }

            if (doInsert)
            {
                using (ISharedCollectionLock l = sharedDictionary.GetWriteLock())
                {

                    if (!sharedDictionary.ContainsKey(KEY))
                    {
                        sharedDictionary.Add(KEY, VALUE);
                    }

                }
            }

            Assert.AreElementsEqual(new Dictionary<string, string> 
                            { 
                                { KEY, VALUE } 
                            }, 
                            sharedDictionary.BackingDictionary);

        }

        [Test, ExpectedException(typeof(WriteLockRequiredException)), Factory("GetWriteMethods")]
        public void WriteRequiresLock(Action<SharedDictionary<string, string>> writeAction)
        {
            writeAction.Invoke(InitSharedDictionary<string, string>("key", "value"));
        }

        [Test, ExpectedException(typeof(ReadLockRequiredException)), Factory("GetReadMethods")]
        public void ReadRequiresLock(Action<SharedDictionary<string, string>> readAction)
        {
            readAction.Invoke(InitSharedDictionary<string, string>("key", "value"));
        }

        [Test, ExpectedException(typeof(ReadLockRequiredException))]
        public void DisposedReadLockDeniesRead()
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);

            ISharedCollectionLock l = d.GetReadLock();
            l.Dispose();

            d.ContainsKey("foo");
        }

        [Test, ExpectedException(typeof(ReadLockRequiredException))]
        public void DisposedWriteLockDeniesRead()
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);

            ISharedCollectionLock l = d.GetWriteLock();
            l.Dispose();

            d.ContainsKey("foo");
        }

        [Test, ExpectedException(typeof(WriteLockRequiredException))]
        public void DisposedWriteLockDeniesWrite()
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);

            ISharedCollectionLock l = d.GetWriteLock();
            l.Dispose();

            d["ke"] = "foo";
        }

        [Test]
        public void WriteLockEnablesRead()
        {
            var d = InitSharedDictionary<string, string>("key", "value");

            string actualValue = null;
            using (ISharedCollectionLock l = d.GetWriteLock())
            {
                actualValue = d["key"];
            }

            Assert.AreEqual("value", actualValue);
        }

        [Test]
        public void CanGetAnotherLockAfterDisposingLock()
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);
            ISharedCollectionLock l = d.GetReadLock();
            l.Dispose();

            l = d.GetReadLock();
            l.Dispose();
        }

        [Test]
        public void DoubleDispose()
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);

            d.Dispose();
            d.Dispose();
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        [Factory("GetObjectDisposedExceptionMethods")]
        public void MethodsThrowAfterDisposed(Action<SharedDictionary<string, string>> methodCall)
        {
            var d = new SharedDictionary<string, string>(this.LockingStrategy);

            d.Dispose();
            methodCall.Invoke(d);
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void TwoDictsShareALockWriteTest()
        {
            ILockStrategy ls = new ReaderWriterLockStrategy();
            var d1 = new SharedDictionary<string, string>(ls);
            var d2 = new SharedDictionary<string, string>(ls);

            using (ISharedCollectionLock readLock = d1.GetReadLock())
            {
                using (ISharedCollectionLock writeLock = d2.GetWriteLock())
                {
                    //do nothing
                }
            }

        }

        private IEnumerable<Action<SharedDictionary<string, string>>> GetObjectDisposedExceptionMethods()
        {
            var l = new List<Action<SharedDictionary<string, string>>>
                        {
                            (SharedDictionary<string, string> d) => d.GetReadLock(),
                            (SharedDictionary<string, string> d) => d.GetWriteLock()
                        };

            l.AddRange(GetReadMethods());
            l.AddRange(GetWriteMethods());

            return l;
        }

        private static IEnumerable<Action<SharedDictionary<string, string>>> GetReadMethods()
        {
            var l = new List<Action<SharedDictionary<string, string>>>();

            l.Add(d => d.ContainsKey("key"));
            l.Add(d => d.Contains(new KeyValuePair<string, string>("key", "value")));
            l.Add(d => Console.WriteLine(d.Count));
            l.Add(d => d.GetEnumerator());
            l.Add(d => ((IEnumerable)d).GetEnumerator());
            l.Add(d => Console.WriteLine(d.IsReadOnly));
            l.Add(d => Console.WriteLine(d["key"]));
            l.Add(d => Console.WriteLine(d.Keys));
            l.Add(d => Console.WriteLine(d.Values));

            l.Add(d =>
            {
                var arr = new KeyValuePair<string, string>[1];
                d.CopyTo(arr, 0);
            });

            l.Add((SharedDictionary<string, string> d) =>
            {
                string value = "";
                d.TryGetValue("key", out value);
            });

            return l;
        }

        private static IEnumerable<Action<SharedDictionary<string, string>>> GetWriteMethods()
        {
            var l = new List<Action<SharedDictionary<string, string>>>();

            l.Add(d => d.Add("more", "value"));
            l.Add(d => d.Add(new KeyValuePair<string, string>("more", "value")));
            l.Add(d => d.Clear());
            l.Add(d => d.Remove(new KeyValuePair<string, string>("more", "value")));
            l.Add(d => d.Remove("key"));
            l.Add(d => d["key"] = "different");

            return l;
        }

        private SharedDictionary<TKey, TValue> InitSharedDictionary<TKey, TValue>(TKey key, TValue value)
        {
            var sharedDict = new SharedDictionary<TKey, TValue>(this.LockingStrategy);
            sharedDict.BackingDictionary.Add(key, value);
            return sharedDict;
        }
    }
}
