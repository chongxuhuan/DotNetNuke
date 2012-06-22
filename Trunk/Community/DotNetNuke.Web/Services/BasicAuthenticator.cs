using System;
using DotNetNuke.Framework;

namespace DotNetNuke.Web.Services
{
    internal class BasicAuthenticator : ServiceLocator<AuthenticatorBase, BasicAuthenticator>
    {
        protected override Func<AuthenticatorBase> GetFactory()
        {
            return () => new BasicAuthenticatorImpl();
        }
    }
}