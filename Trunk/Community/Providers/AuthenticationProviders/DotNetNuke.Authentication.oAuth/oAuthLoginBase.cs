using System;

using DotNetNuke.Services.Authentication;
using System.Web;

namespace DotNetNuke.Authentication.oAuth
{
    public abstract class oAuthLoginBase : AuthenticationLoginBase
    {
        protected virtual string AuthSystemApplicationName { get { return String.Empty; } }

        protected oAuthClientBase OAuthClient { get; set; }

        protected abstract UserData GetCurrentUser();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                //Save the return Url in the cookie
                HttpContext.Current.Response.Cookies.Set(new HttpCookie("returnurl", RedirectURL) { Expires = DateTime.Now.AddMinutes(5) });
            }

            if (OAuthClient.IsCurrentService() && OAuthClient.HaveVerificationCode() || OAuthClient.IsCurrentUserAuthorized())
            {
                if (OAuthClient.Authorize() == AuthorisationResult.Authorized)
                {
                    OAuthClient.AuthenticateUser(GetCurrentUser(), PortalSettings, IPAddress,
                                     (properties) => { },
                                     OnUserAuthenticated);
                }
            }
        }

        #region Overrides of AuthenticationLoginBase

        public override bool Enabled
        {
            get
            {
                return oAuthConfigBase.GetConfig(AuthSystemApplicationName, PortalId).Enabled;
            }
        }

        #endregion

    }
}
