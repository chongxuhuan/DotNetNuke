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

using System;

using DotNetNuke.Services.Authentication.OAuth;

#endregion

namespace DotNetNuke.Authentication.LinkedIn.Components
{
    public class LinkedInClient : oAuthClientBase
    {
        public LinkedInClient(int portalId)
        {
            AuthorizationEndpoint = new Uri("https://www.linkedin.com/uas/oauth/authorize");
            RequestTokenEndpoint = new Uri("https://api.linkedin.com/uas/oauth/requestToken");
            TokenEndpoint = new Uri("https://api.linkedin.com/uas/oauth/accessToken");
            MeGraphEndpoint = new Uri("http://api.linkedin.com/v1/people/~");

            Service = "LinkedIn";

            APIKey = oAuthConfigBase.GetConfig(Service, portalId).APIKey;
            APISecret = oAuthConfigBase.GetConfig(Service, portalId).APISecret;
            CallbackUri = new Uri(oAuthConfigBase.GetConfig(Service, portalId).SiteURL);
            AuthTokenName = "LinkedInUserToken";

            OAuthVersion = "1.0";

            LoadTokenCookie(String.Empty);
        }
    }
}