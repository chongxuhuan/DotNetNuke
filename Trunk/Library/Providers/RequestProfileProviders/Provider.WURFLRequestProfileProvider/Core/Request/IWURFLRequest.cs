/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;

namespace DotNetNuke.Services.Devices.Core.Request
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWURFLRequest
    {
        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        String UserAgent { get; }

        /// <summary>
        /// Gets the user agent profile.
        /// </summary>
        /// <value>The user agent profile.</value>
        String UserAgentProfile { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is XHTML device.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is XHTML device; otherwise, <c>false</c>.
        /// </value>
        bool IsXHTMLDevice { get; }
    }
}