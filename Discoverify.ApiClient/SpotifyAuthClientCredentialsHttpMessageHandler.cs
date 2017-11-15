namespace Discoverify.ApiClient
{
    using Discoverify.ApiModels;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;


    public class SpotifyAuthClientCredentialsHttpMessageHandler : DelegatingHandler
    {
        private const string AuthenticationEndpoint = "https://accounts.spotify.com/api/token";
        private readonly string _clientId;
        private readonly string _clientSecret;

        public SpotifyAuthClientCredentialsHttpMessageHandler(string clientId, string clientSecret, HttpMessageHandler httpMessageHandler) : base(httpMessageHandler)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAuthenticationTokenAsync());
            }
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetAuthenticationTokenAsync()
        {

            string token = string.Empty;

            var timeBeforeRequest = DateTime.Now;

            var response = await GetAuthenticationTokenResponse();

            token = response?.AccessToken;
            if (token == null)
                throw new AuthenticationException("Spotify authentication failed");

            var expireTime = timeBeforeRequest.AddSeconds(response.ExpiresIn);

            return token;
        }

        private async Task<AuthenticationResponse> GetAuthenticationTokenResponse()
        {
            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var authHeader = BuildAuthHeader();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, AuthenticationEndpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            requestMessage.Content = content;

            var response = await client.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();

            var authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(responseString);
            return authenticationResponse;
        }

        private string BuildAuthHeader()
        {
            return Base64Encode(_clientId + ":" + _clientSecret);
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
