using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Github
{
    public static class OAuthClientExtensions
    {
        public static Uri OauthDeviceCode() => "login/device/code".FormatUri();

        public static Uri FormatUri(this string pattern, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or whitespace.", nameof(pattern));
            }

            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }

        /// <summary>
        /// Makes a request to get an access token using the code returned when GitHub.com redirects back from the URL
        /// <see cref="GetGitHubLoginUrl">GitHub login url</see> to the application.
        /// </summary>
        /// <remarks>
        /// If the user accepts your request, GitHub redirects back to your site with a temporary code in a code
        /// parameter as well as the state you provided in the previous step in a state parameter. If the states don’t
        /// match, the request has been created by a third party and the process should be aborted. Exchange this for
        /// an access token using this method.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/login/oauth/access_token")]
        public static async Task<OauthToken> CreateAccessToken(this GitHubClient client, OauthTokenRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var endPoint = ApiUrls.OauthAccessToken();

            var response = await client.Connection.Post<OauthToken>(endPoint, JsonConvert.SerializeObject(new {client_id = request.ClientId, code = request.Code }), "application/json", "application/json", new Uri("https://githubproxy.bbarrett.me")).ConfigureAwait(false);
            return response.Body;
        }
    }
}
