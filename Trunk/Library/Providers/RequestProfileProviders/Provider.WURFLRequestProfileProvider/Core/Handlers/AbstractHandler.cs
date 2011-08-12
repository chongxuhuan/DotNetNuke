/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Hanldlers
{
    /// <summary>
    /// Base Handler
    /// </summary>
    public abstract class AbstractHandler : IHandler<string>
    {
        private readonly string _id;

        public AbstractHandler(string id)
        {
            _id = id;
        }

        #region IHandler<string> Members

        public string ID
        {
            get { return _id; }
        }


        public abstract bool CanHandle(string userAgent);

        #endregion
    }
}