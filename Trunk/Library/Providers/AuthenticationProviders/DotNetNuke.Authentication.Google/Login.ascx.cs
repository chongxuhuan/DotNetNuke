using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using DotNetNuke.Authentication.Google.Components;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Authentication.Google
{
    public partial class Login : AuthenticationLoginBase
    {
        private GoogleClient _googleClient;
        private const string AuthSystemApplicationName = "Google";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            loginButton.Click += loginButton_Click;

            _googleClient = new GoogleClient(PortalId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                //Save the return Url in the cookie
                HttpContext.Current.Response.Cookies.Set(new HttpCookie("returnurl", RedirectURL) { Expires = DateTime.Now.AddMinutes(5) });
            }

            if (_googleClient.IsGoogle() && _googleClient.HaveVerificationCode() || _googleClient.IsCurrentUserAuthorized())
            {
                var result = _googleClient.Authorize();

                if (result == GoogleAuthorisationResult.Authorized)
                {
                    var user = _googleClient.GetCurrentUser();
                    UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;

                    string userName = AuthSystemApplicationName + "-" + user.Id;

                    UserInfo objUserInfo = UserController.ValidateUser(PortalId, userName, "",
                                                                        AuthSystemApplicationName, "",
                                                                        PortalSettings.PortalName, IPAddress,
                                                                        ref loginStatus);


                    //Raise UserAuthenticated Event
                    UserAuthenticatedEventArgs eventArgs = new UserAuthenticatedEventArgs(objUserInfo, userName, loginStatus,
                                                                                          AuthSystemApplicationName);
                    eventArgs.AutoRegister = true;

                    NameValueCollection profileProperties = new NameValueCollection();

                    profileProperties.Add("FirstName", user.FirstName);
                    profileProperties.Add("LastName", user.LastName);
                    profileProperties.Add("Email", user.Email);
                    profileProperties.Add("DisplayName", user.Name);
                    profileProperties.Add("PreferredLocale", user.Locale.Replace('_', '-'));

                    #pragma warning disable 612,618
                    int timeZone;
                    if (Int32.TryParse(user.Timezone, out timeZone))
                    {
                        TimeZoneInfo timeZoneInfo = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(timeZone);

                        profileProperties.Add("PreferredTimeZone", timeZoneInfo.Id);
                    }
                    #pragma warning restore 612,618

                    eventArgs.Profile = profileProperties;

                    //eventArgs.Authenticated = authenticated
                    //eventArgs.Message = message
                    OnUserAuthenticated(eventArgs);
                }
            }


        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            var result = _googleClient.Authorize();
        }

        #region Overrides of AuthenticationLoginBase

        public override bool Enabled
        {
            get { return GoogleConfig.GetConfig(PortalId).Enabled; }
        }

        #endregion

        
    }
}