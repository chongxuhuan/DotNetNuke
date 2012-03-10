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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

#endregion

namespace DotNetNuke.Authentication.Google.Components
{
    public class GoogleClient
    {
        #region Private Members

        private string _scope = @"scope=" + HttpContext.Current.Server.UrlEncode("https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email");

        #endregion

        #region Constructors

        public GoogleClient(int portalId)
        {
            TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token");
            AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth");
            MeGraphEndpoint = new Uri("https://www.googleapis.com/oauth2/v1/userinfo");
            APIKey = GoogleConfig.GetConfig(portalId).APIKey;
            APISecret = GoogleConfig.GetConfig(portalId).APISecret;
            LoginPage = new Uri(GoogleConfig.GetConfig(portalId).SiteURL);
            SessionTokenName = "UserGoogleToken";
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
            string postData = "code=" + code + "&" +
                            "client_id=" + APIKey + "&" +
                            "client_secret=" + APISecret + "&" +
                            "redirect_uri=" + HttpContext.Current.Server.UrlEncode(redirectUrl.ToString()) + "&" +
                            "grant_type=authorization_code";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            WebRequest request = WebRequest.CreateDefault(TokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        string responseText = responseReader.ReadToEnd();
                        var jsonSerializer = new JavaScriptSerializer();
                        var tokenDictionary = jsonSerializer.DeserializeObject(responseText) as Dictionary<string, object>;
                        string token = Convert.ToString(tokenDictionary["access_token"]);
                        return token;
                    }
                }
            }
        }

        public GoogleAuthorisationResult Authorize()
        {
            string errorReason = HttpContext.Current.Request.Params["error_reason"];
            bool userDenied = (errorReason != null);
            if (userDenied)
            {
                return GoogleAuthorisationResult.Denied;
            }

            if (!HaveVerificationCode())
            {
                string url = AuthorizationEndpoint + "?" + _scope + "&" +
                                "state=google&response_type=code&" +
                                "client_id=" + APIKey + "&" +
                                "redirect_uri=" + HttpContext.Current.Server.UrlEncode(LoginPage.ToString());
                HttpContext.Current.Response.Redirect(url, true);
                return GoogleAuthorisationResult.RequestingCode;
            }

            SessionToken = ExchangeCodeForToken(VerificationCode, LoginPage);

            return GoogleAuthorisationResult.Authorized;
        }

        public GoogleGraph GetCurrentUser()
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
                        GoogleGraph user = GoogleGraph.Deserialize(responseText);
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

        public bool IsGoogle()
        {
            string service = HttpContext.Current.Request.Params["state"];
            return !String.IsNullOrEmpty(service) && service == "google";
        }


    }
}