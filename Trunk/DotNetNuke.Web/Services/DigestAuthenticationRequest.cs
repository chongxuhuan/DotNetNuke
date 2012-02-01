using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace DotNetNuke.Web.Services
{
    public class DigestAuthenticationRequest
    {
        public DigestAuthenticationRequest(string authorizationHeader, string httpMethod)
        {
            //Authorization: Digest
            //username="Mufasa",
            //realm="testrealm@host.com",
            //nonce="dcd98b7102dd2f0e8b11d0f600bfb0c093",
            //uri="/dir/index.html",
            //qop=auth,
            //nc=00000001,
            //cnonce="0a4f113b",
            //response="6629fae49393a05397450978507c4ef1",
            //opaque="5ccc069c403ebaf9f0171e9517f40e41"
            RequestParams = new NameValueCollection();
            foreach (
                Match m in
                    Regex.Matches(authorizationHeader, "\\s?(?'name'\\w+)=(\"(?'value'[^\"]+)\"|(?'value'[^,]+))"))
            {
                RequestParams.Add(m.Groups["name"].Value, m.Groups["value"].Value);
            }
            HttpMethod = httpMethod;
            RawUsername = RequestParams["username"].Replace("\\\\", "\\");
            CleanUsername = RawUsername;
            if (CleanUsername.LastIndexOf("\\", System.StringComparison.Ordinal) > 0)
            {
                CleanUsername = CleanUsername.Substring(CleanUsername.LastIndexOf("\\", System.StringComparison.Ordinal) + 2 - 1);
            }
        }

        public NameValueCollection RequestParams { get; set; }

        public string CleanUsername { get; private set; }

        public string RawUsername { get; private set; }

        public string HttpMethod { get; set; }
    }
}