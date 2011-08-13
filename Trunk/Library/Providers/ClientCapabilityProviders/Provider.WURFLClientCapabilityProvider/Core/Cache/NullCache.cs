/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Core.Cache
{
    internal class NullCache<TKey, TValue> : ICache<TKey, TValue>
    {
        #region ICache<TKey,TValue> Members

        public TValue Get(TKey key)
        {
            return default(TValue);
        }

        public void Put(TKey key, TValue value)
        {
            // DO Nothing
        }

        public void Clear()
        {
            // DO Nothing
        }

        #endregion
    }
}