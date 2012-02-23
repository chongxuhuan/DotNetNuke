using System;
using System.Security.Principal;
using System.Text;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;

namespace DotNetNuke.Web.Services
{
    internal class BasicAuthenticatorImpl : AuthenticatorBase
    {
        private const string AuthType = "Basic";
        private readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

        public override void TryToAuthenticate(HttpContextBase context, int portalId)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var credentials = GetCredentials(context);

            if(credentials == null)
            {
                return;
            }

            var status = UserLoginStatus.LOGIN_FAILURE;
            string ipAddress = context.Request.UserHostAddress;
            var user = UserController.ValidateUser(portalId, credentials.UserName, credentials.Password, "DNN", "", "a portal", ipAddress ?? "", ref status);

            if(user != null)
            {
                context.User = new GenericPrincipal(new GenericIdentity(credentials.UserName, AuthType), null);
            }
        }

        private UserCredentials GetCredentials(HttpContextBase context)
        {
            string authorization = context.Request.Headers["Authorization"];
            if (String.IsNullOrEmpty(authorization))
            {
                return null;
            }

            if(!authorization.StartsWith(AuthType, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var encoded = authorization.Substring(AuthType.Length).Trim();
            var decoded = _encoding.GetString(Convert.FromBase64String(encoded));

            var parts = decoded.Split(new[] {':'}, 2);
            if(parts.Length < 2)
            {
                return null;
            }

            return new UserCredentials(parts[0], parts[1]);
        }

        internal class UserCredentials
        {
            public UserCredentials(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }

            public string Password { get; set; }
            public string UserName { get; set; }
        }
    }
}