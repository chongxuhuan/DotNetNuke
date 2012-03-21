﻿#region Copyright

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

namespace DotNetNuke.Authentication.Facebook.Components
{
    public class FacebookClient : oAuthClientBase
    {
        #region Constructors

        public FacebookClient(int portalId)
        {
            TokenEndpoint = new Uri("https://graph.facebook.com/oauth/access_token");
            TokenMethod = HttpMethod.GET;
            AuthorizationEndpoint = new Uri("https://graph.facebook.com/oauth/authorize");
            MeGraphEndpoint = new Uri("https://graph.facebook.com/me");

            Service = "Facebook";
            Scope = "email";

            APIKey = oAuthConfigBase.GetConfig(Service, portalId).APIKey;
            APISecret = oAuthConfigBase.GetConfig(Service, portalId).APISecret;
            CallbackUri = new Uri(oAuthConfigBase.GetConfig(Service, portalId).SiteURL);
            AuthTokenName = "FacebookUserToken";

            OAuthVersion = "2.0";

            LoadTokenCookie(String.Empty);
        }

        #endregion

        protected override TimeSpan GetExpiry(string responseText)
        {
            TimeSpan expiry = TimeSpan.MinValue;
            foreach (string token in responseText.Split('&'))
            {
                if (token.StartsWith("expires"))
                {
                    expiry = new TimeSpan(0, 0, Convert.ToInt32(token.Replace("expires=", "")));
                }
            }
            return expiry;
        }

        protected override string GetToken(string responseText)
        {
            string authToken = String.Empty;
            foreach (string token in responseText.Split('&'))
            {
                if (token.StartsWith("access_token"))
                {
                    authToken = token.Replace("access_token=", "");
                }
            }
            return authToken;
        }
    }
}