/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System.Collections.Generic;
using System.Threading;

namespace DotNetNuke.Services.Devices.Core.Cache
{
    internal class DictionaryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private const int DEFAULT_CAPACITY = 500;
        private const int DEFAUTL_TIMEOUT = 500; //milliseconds
        private static readonly ReaderWriterLock rwLock = new ReaderWriterLock();
        private readonly int _capacity;
        private IDictionary<TKey, TValue> _cache;

        public DictionaryCache() : this(DEFAULT_CAPACITY)
        {
        }

        public DictionaryCache(int capacity)
        {
            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>(capacity);
        }

        #region ICache<TKey,TValue> Members

        public TValue Get(TKey key)
        {
            TValue value = default(TValue);
            try
            {
                rwLock.AcquireReaderLock(DEFAUTL_TIMEOUT);
                _cache.TryGetValue(key, out value);
            }
            finally
            {
                rwLock.ReleaseLock();
            }

            return value;
        }

        public void Put(TKey key, TValue value)
        {
            try
            {
                rwLock.AcquireWriterLock(DEFAUTL_TIMEOUT);
                _cache[key] = value;
            }
            finally
            {
                rwLock.ReleaseLock();
            }
        }

        public void Clear()
        {
            try
            {
                rwLock.AcquireWriterLock(DEFAUTL_TIMEOUT);
                _cache = new Dictionary<TKey, TValue>(_capacity);
            }
            finally
            {
                rwLock.ReleaseLock();
            }
        }

        #endregion
    }
}