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

using System.Collections.Generic;
using System.Web;
#endregion

namespace DotNetNuke.Services.RequestProfile
{
    /// <summary>
    ///   RequestProfile provides capabilities supported by the web requester (e.g. Mobile Device, TV, Desktop)
    /// </summary>
    /// <remarks>
    ///   The capabilities are primarily derived based on UserAgent.  
    /// </remarks>          
    public interface IRequestProfile
    {
        /// <summary>
        ///   Unique ID of the requester.
        /// </summary>
        string ID { get; }

        /// <summary>
        ///   User Agent of the requester
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        ///   Is request coming from a mobile device.
        /// </summary>
        bool IsMobile { get; }

        /// <summary>
        ///   Is request coming from a tablet device.
        /// </summary>
        bool IsTablet { get; }

        /// <summary>
        ///   Does the requesting device supports touch screen.
        /// </summary>
        bool IsTouchScreen { get; }

        /// <summary>
        ///   Is request coming from Facebook iframe.
        /// </summary>
        /// <remarks>
        ///   Pesence of "signed_request" in the headers is used to detect of request is coming from facebook.
        ///   No further analysis is performed on the value of "signed_request".
        /// </remarks>         
        bool IsFacebook { get; }

        /// <summary>
        ///   Screen Width of the requester.
        /// </summary>
        /// <remarks>
        ///   If IsFacebook is true, then this value represents iframe width, otherwise this value is device width
        /// </remarks>          
        int Width { get; }

        /// <summary>
        ///   Screen Height of the requester.
        /// </summary>
        /// <remarks>
        ///   If IsFacebook is true, then this value represents iframe height, otherwise this value is device height
        /// </remarks>                  
        int Height { get; }

        /// <summary>
        ///   Does requester supports Flash.
        /// </summary>
        bool SupportsFlash { get; }

        /// <summary>
        /// A key-value collection containing all capabilities supported by requester
        /// </summary>        
        IDictionary<string, string> Capabilities { get; }

        /// <summary>
        /// Gets the capability.
        /// </summary>
        /// <param name="capabilityName">Name of the capability.</param>
        /// <returns></returns>
        string GetCapability(string capabilityName);
    }
}
