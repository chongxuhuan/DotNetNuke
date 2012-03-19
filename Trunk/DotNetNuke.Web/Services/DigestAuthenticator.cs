using System;
using DotNetNuke.Framework;

namespace DotNetNuke.Web.Services
{
    internal class DigestAuthenticator : ServiceLocator<AuthenticatorBase, DigestAuthenticator>
    {
        protected override Func<AuthenticatorBase> GetFactory()
        {
            return () => new DigestAuthenticatorImpl();
        }
    }
}