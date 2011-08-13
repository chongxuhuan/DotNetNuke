namespace DotNetNuke.Services.ClientCapability.Matchers.Strategy
{
    internal class SafariShortTokensProvider : RegExTokensProvider
    {
        private const string SHORT_REGEX =
            @"(Mozilla/5\.0)\s\((.*;)(.*?)\)\s(AppleWebKit)/(\d+)(.*?)\s+\((KHTML,\slike\sGecko)\)\s(Safari)/(\d+)(.*)";

        public SafariShortTokensProvider() : base(SHORT_REGEX, new int[] {24, 24, 1, 24, 4, 1, 24, 24, 4, 1})
        {
        }
    }
}