/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Core.Classifiers
{
    public interface IClassificationHandler<T>
    {
        string ID { get; }
        string GetClassID(T userAgent);
    }
}