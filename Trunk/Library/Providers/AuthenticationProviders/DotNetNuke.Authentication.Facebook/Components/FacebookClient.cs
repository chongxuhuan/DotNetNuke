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
using System.IO;
using System.Net;
using System.Web;

#endregion

namespace DotNetNuke.Authentication.Facebook.Components
{
    public class FacebookClient
    {
        #region Constructors

        public FacebookClient(int portalId)
        {
            TokenEndpoint = new Uri("https://graph.facebook.com/oauth/access_token");
            AuthorizationEndpoint = new Uri("https://graph.facebook.com/oauth/authorize");
            MeGraphEndpoint = new Uri("https://graph.facebook.com/me");
            APIKey = FacebookConfig.GetConfig(portalId).APIKey;
            APISecret = FacebookConfig.GetConfig(portalId).APISecret;
            LoginPage = new Uri(FacebookConfig.GetConfig(portalId).SiteURL);
            SessionTokenName = "UserFacebookToken";
        }

        #endregion

        #region Protected Properties

        protected string APIKey { get; set; }
        protected string APISecret { get; set; }
        protected Uri AuthorizationEndpoint { get; set; }
        protected Uri LoginPage { get; set; }
        protected Uri MeGraphEndpoint { get; set; }
        protected string SessionToken
        {
            get { return HttpContext.Current.Session[SessionTokenName] as string; }
            set { HttpContext.Current.Session[SessionTokenName] = value; }
        }
        protected string SessionTokenName { get; set; }
        protected Uri TokenEndpoint { get; set; }
        protected string VerificationCode
        {
            get { return HttpContext.Current.Request.Params["code"]; }
        }

        #endregion

        private String ExchangeCodeForToken(String code, Uri redirectUrl)
        {
            string url = TokenEndpoint + "?" +
                         "client_id=" + APIKey + "&" +
                         "redirect_uri=" + HttpContext.Current.Server.UrlEncode(redirectUrl.ToString()) + "&" +
                         "client_secret=" + APISecret + "&" +
                         "code=" + code;

            WebRequest request = WebRequest.CreateDefault(new Uri(url));
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        string responseText = responseReader.ReadToEnd();
                        string token = responseText.Replace("access_token=", "");
                        return token;
                    }
                }
            }
        }

        public FacebookAuthorisationResult Authorize()
        {
            string errorReason = HttpContext.Current.Request.Params["error_reason"];
            bool userDenied = (errorReason != null);
            if (userDenied)
            {
                return FacebookAuthorisationResult.Denied;
            }

            if (!HaveVerificationCode())
            {
                string url = AuthorizationEndpoint + "?" +
                             "client_id=" + APIKey + "&" +
                             "redirect_uri=" + HttpContext.Current.Server.UrlEncode(LoginPage.ToString()) + "&" +
                             "state=facebook&scope=email";
                HttpContext.Current.Response.Redirect(url, true);
                return FacebookAuthorisationResult.RequestingCode;
            }

            SessionToken = ExchangeCodeForToken(VerificationCode, LoginPage);

            return FacebookAuthorisationResult.Authorized;
        }

        public FacebookGraph GetCurrentUser()
        {
            if (!IsCurrentUserAuthorized())
            {
                return null;
            }

            string url = MeGraphEndpoint + "?" + "access_token=" + SessionToken;

            WebRequest request = WebRequest.CreateDefault(new Uri(url));
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        string responseText = responseReader.ReadToEnd();
                        FacebookGraph user = FacebookGraph.Deserialize(responseText);
                        return user;
                    }
                }
            }
        }

        public bool HaveVerificationCode()
        {
            return (VerificationCode != null);
        }

        public Boolean IsCurrentUserAuthorized()
        {
            return !String.IsNullOrEmpty(SessionToken);
        }

        public bool IsFacebook()
        {
            string service = HttpContext.Current.Request.Params["state"];
            return !String.IsNullOrEmpty(service) && service == "facebook";
        }
    }
}