using System.Net;
using System.Net.Http.Headers;

namespace JCA.Mobile.Services
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly AuthService _authService;

        public AuthenticatedHttpClientHandler(AuthService authService) : base(new HttpClientHandler())
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get a valid access token (refreshes if expired)
            string? accessToken = await _authService.GetValidAccessTokenAsync();

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // If we get a 401, try refreshing once more and retry
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _authService.RefreshTokenAsync();
                if (refreshed)
                {
                    string? newToken = await _authService.GetValidAccessTokenAsync();
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        // Clone the request (original can't be resent)
                        HttpRequestMessage retryRequest = await CloneRequestAsync(request);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                        response = await base.SendAsync(retryRequest, cancellationToken);
                    }
                }
                else
                {
                    // Refresh failed - trigger re-login on main thread
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync("//LoginPage");
                    });
                }
            }

            return response;
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage originalRequest)
        {
            HttpRequestMessage clone = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            if (originalRequest.Content != null)
            {
                byte[] contentBytes = await originalRequest.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);

                if (originalRequest.Content.Headers.ContentType != null)
                {
                    clone.Content.Headers.ContentType = originalRequest.Content.Headers.ContentType;
                }
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in originalRequest.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }
}
