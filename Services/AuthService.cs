using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using JCA.Mobile.Models;
using Newtonsoft.Json;

namespace JCA.Mobile.Services
{
    public class AuthService
    {
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string UserEmailKey = "user_email";
        private const string UserNameKey = "user_name";
        private const string TokenExpiryKey = "token_expiry";

        private const string BaseUrl = "https://tools.jcadm.org";
        private const string LoginPath = "/api/auth/mobile-login";
        private const string RefreshPath = "/api/auth/refresh";
        private const string RevokePath = "/api/auth/revoke";
        private const string CallbackScheme = "com.jca.mobileapp";

        private readonly HttpClient _httpClient;

        public AuthService()
        {
#if DEBUG
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
#else
            _httpClient = new HttpClient();
#endif
        }

        public bool IsAuthenticated
        {
            get
            {
                string storedToken = SecureStorage.GetAsync(AccessTokenKey).Result ?? string.Empty;
                return !string.IsNullOrEmpty(storedToken);
            }
        }

        public async Task<bool> IsTokenValidAsync()
        {
            string accessToken = await SecureStorage.GetAsync(AccessTokenKey) ?? string.Empty;
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            // Check if the token has expired
            string expiryString = await SecureStorage.GetAsync(TokenExpiryKey) ?? string.Empty;
            if (long.TryParse(expiryString, out long expiryTicks))
            {
                DateTime expiry = new DateTime(expiryTicks, DateTimeKind.Utc);
                if (expiry <= DateTime.UtcNow.AddMinutes(1)) // 1-minute buffer
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> LoginAsync()
        {
            try
            {
                string deviceInfo = DeviceInfo.Name ?? "unknown";
                string loginUrl = $"{BaseUrl}{LoginPath}?device_info={Uri.EscapeDataString(deviceInfo)}";

                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(loginUrl),
                    new Uri($"{CallbackScheme}://auth-callback")
                );

                string? accessToken = authResult.Get("access_token");
                string? refreshToken = authResult.Get("refresh_token");
                string? expiresIn = authResult.Get("expires_in");
                string? userEmail = authResult.Get("user_email");
                string? userName = authResult.Get("user_name");

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                // Store tokens securely
                await SecureStorage.SetAsync(AccessTokenKey, accessToken);
                await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
                await SecureStorage.SetAsync(UserEmailKey, userEmail ?? string.Empty);
                await SecureStorage.SetAsync(UserNameKey, userName ?? string.Empty);

                // Calculate and store expiry time
                if (int.TryParse(expiresIn, out int seconds))
                {
                    DateTime expiry = DateTime.UtcNow.AddSeconds(seconds);
                    await SecureStorage.SetAsync(TokenExpiryKey, expiry.Ticks.ToString());
                }

                return true;
            }
            catch (TaskCanceledException)
            {
                // User cancelled the login
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetValidAccessTokenAsync()
        {
            if (await IsTokenValidAsync())
            {
                return await SecureStorage.GetAsync(AccessTokenKey);
            }

            // Try to refresh
            bool refreshed = await RefreshTokenAsync();
            if (refreshed)
            {
                return await SecureStorage.GetAsync(AccessTokenKey);
            }

            return null;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                string refreshToken = await SecureStorage.GetAsync(RefreshTokenKey) ?? string.Empty;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                string deviceInfo = DeviceInfo.Name ?? "unknown";
                object requestBody = new { RefreshToken = refreshToken, DeviceInfo = deviceInfo };
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}{RefreshPath}", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Refresh token is invalid/expired - clear everything
                    await LogoutAsync();
                    return false;
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                TokenResponse? tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return false;
                }

                // Update stored tokens
                await SecureStorage.SetAsync(AccessTokenKey, tokenResponse.AccessToken);
                await SecureStorage.SetAsync(RefreshTokenKey, tokenResponse.RefreshToken);
                await SecureStorage.SetAsync(UserEmailKey, tokenResponse.UserEmail);
                await SecureStorage.SetAsync(UserNameKey, tokenResponse.UserName);

                DateTime expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                await SecureStorage.SetAsync(TokenExpiryKey, expiry.Ticks.ToString());

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token refresh error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                string refreshToken = await SecureStorage.GetAsync(RefreshTokenKey) ?? string.Empty;
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Revoke the refresh token on the server
                    object requestBody = new { RefreshToken = refreshToken };
                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync($"{BaseUrl}{RevokePath}", content);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Revoke error: {ex.Message}");
            }
            finally
            {
                // Clear all stored tokens
                SecureStorage.Remove(AccessTokenKey);
                SecureStorage.Remove(RefreshTokenKey);
                SecureStorage.Remove(UserEmailKey);
                SecureStorage.Remove(UserNameKey);
                SecureStorage.Remove(TokenExpiryKey);
            }
        }

        public async Task<string> GetUserEmailAsync()
        {
            return await SecureStorage.GetAsync(UserEmailKey) ?? string.Empty;
        }

        public async Task<string> GetUserNameAsync()
        {
            return await SecureStorage.GetAsync(UserNameKey) ?? string.Empty;
        }
    }
}
