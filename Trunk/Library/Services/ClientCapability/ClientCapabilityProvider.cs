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

using System.Linq;
using System.Web;

using DotNetNuke.ComponentModel;

#endregion

namespace DotNetNuke.Services.ClientCapability
{
    public abstract class ClientCapabilityProvider
    {
        #region Abstract Methods
        /// <summary>
        ///   Returns ClientCapability based on userAgent
        /// </summary>
        public abstract IClientCapability GetClientCapability(string userAgent);

        /// <summary>
        ///   Returns ClientCapability based on ClientCapabilityId
        /// </summary>
        public abstract IClientCapability GetClientCapabilityById(string clientId);

        /// <summary>
        ///   Returns All ClientCapabilitys available
        /// </summary>
        public abstract IQueryable<IClientCapability> GeAllClientCapabilitys();
        #endregion

        #region Virtual Methods
        /// <summary>
        ///   Returns ClientCapability based on HttpRequest
        /// </summary>
        public virtual IClientCapability GetClientCapability(HttpRequest httpRequest)
        {
            IClientCapability clientCapability = GetClientCapability(httpRequest.UserAgent);

        	var fbRequest = FacebookRequest.RequestToSignedRequest(httpRequest);
			clientCapability.IsFacebook = fbRequest != null && fbRequest.IsValid;

            return clientCapability;
        }

        public virtual bool IsFacebook(HttpRequest httpRequest)
        {
            if (httpRequest == null) return false;
            if (httpRequest.RequestType != "POST") return false;
            return httpRequest["signed_request"] != null;
        }
        #endregion


        #region static methods
        public static ClientCapabilityProvider Instance()
        {
            return ComponentFactory.GetComponent<ClientCapabilityProvider>();
        }

        public static IClientCapability CurrentClientCapability
        {
            get
            {
                return ClientCapabilityProvider.Instance().GetClientCapability(HttpContext.Current.Request);                
            }            
        }
        #endregion
    }
}
