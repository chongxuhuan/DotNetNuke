namespace DotNetNuke.Services.ClientCapability.Matchers.Strategy
{
    internal class SafariLongTokensProvider : RegExTokensProvider
    {
        private const string LONG_REGEX =
            @"(Mozilla/5\.0)\s\((.*;)(.*?)\)\s(AppleWebKit)/(\d+)(.*?)\s+\((KHTML,\slike\sGecko)\)\s(.*?)/(\d+)(.*?)\s(Safari)/(\d+)(.*)";

        public SafariLongTokensProvider() : base(LONG_REGEX, new int[] {25, 25, 0, 25, 5, 1, 25, 20, 5, 1, 25, 5, 1})
        {
        }
    }
}