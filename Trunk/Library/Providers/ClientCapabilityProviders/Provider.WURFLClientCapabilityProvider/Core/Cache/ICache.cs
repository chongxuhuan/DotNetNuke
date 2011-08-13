/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Core.Cache
{
    public interface ICache<TKey, TValue>
    {
        TValue Get(TKey key);


        void Put(TKey key, TValue value);


        void Clear();
    }
}