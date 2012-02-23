using System;
using DotNetNuke.Framework;

namespace DotNetNuke.Web.Services
{
    public class DigestAuthenticator : ControllerBase<AuthenticatorBase, DigestAuthenticator>
    {
        protected override Func<AuthenticatorBase> GetFactory()
        {
            return () => new DigestAuthenticatorImpl();
        }
    }
}