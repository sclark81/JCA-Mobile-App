using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
#if ANDROID || IOS
using Plugin.Firebase.CloudMessaging;
#endif

namespace JCA.Mobile.Services
{
    public class PushNotificationService
    {
        private const string FcmTokenKey = "fcm_token";

#if DEBUG
        private const string BaseUrl = "https://10.0.2.2:7777";
#else
        private const string BaseUrl = "https://tools.jcadm.org";
#endif

        private const string RegisterPath = "/api/deviceregistration/register";
        private const string UnregisterPath = "/api/deviceregistration/unregister";

        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public PushNotificationService(AuthService authService)
        {
            _authService = authService;

#if DEBUG
            HttpClientHandler sslHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService, sslHandler);
            _httpClient = new HttpClient(handler);
#else
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService);
            _httpClient = new HttpClient(handler);
#endif
        }

        /// <summary>
        /// Gets the current FCM token and registers it with the server.
        /// Call this after successful login or token refresh.
        /// </summary>
        public async Task RefreshDeviceRegistrationAsync()
        {
            try
            {
#if ANDROID || IOS
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                string fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

                if (string.IsNullOrEmpty(fcmToken))
                {
                    System.Diagnostics.Debug.WriteLine("PushNotification: FCM token is empty.");
                    return;
                }

                // Check if token has changed
                string? storedToken = await SecureStorage.GetAsync(FcmTokenKey);
                if (storedToken == fcmToken)
                {
                    System.Diagnostics.Debug.WriteLine("PushNotification: Token unchanged, skipping registration.");
                    return;
                }

                // Store the new token locally
                await SecureStorage.SetAsync(FcmTokenKey, fcmToken);

                // Register with the server
                await RegisterTokenWithServerAsync(fcmToken);
#else
                System.Diagnostics.Debug.WriteLine("PushNotification: FCM not supported on this platform.");
                await Task.CompletedTask;
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PushNotification: Registration error - {ex.Message}");
            }
        }

        /// <summary>
        /// Unregisters the current device from push notifications on the server.
        /// Call this on logout.
        /// </summary>
        public async Task UnregisterDeviceAsync()
        {
            try
            {
                string? fcmToken = await SecureStorage.GetAsync(FcmTokenKey);
                if (string.IsNullOrEmpty(fcmToken))
                {
                    return;
                }

                string json = JsonConvert.SerializeObject(new { fcmToken });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    BaseUrl + UnregisterPath, content);

                if (response.IsSuccessStatusCode)
                {
                    SecureStorage.Remove(FcmTokenKey);
                    System.Diagnostics.Debug.WriteLine("PushNotification: Device unregistered successfully.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"PushNotification: Unregister failed - {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PushNotification: Unregister error - {ex.Message}");
            }
        }

        private async Task RegisterTokenWithServerAsync(string fcmToken)
        {
            try
            {
                string platform = DeviceInfo.Current.Platform == DevicePlatform.Android
                    ? "Android"
                    : DeviceInfo.Current.Platform == DevicePlatform.iOS
                        ? "iOS"
                        : "Unknown";

                string deviceInfo = $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}";

                object payload = new
                {
                    fcmToken,
                    deviceInfo,
                    platform
                };

                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    BaseUrl + RegisterPath, content);

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("PushNotification: Device registered successfully.");
                }
                else
                {
                    string body = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(
                        $"PushNotification: Registration failed - {response.StatusCode}: {body}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"PushNotification: Server registration error - {ex.Message}");
            }
        }

#if ANDROID || IOS
        /// <summary>
        /// Subscribes to the FCM token refresh event to auto-re-register when the token changes.
        /// </summary>
        public void SubscribeToTokenRefresh()
        {
            CrossFirebaseCloudMessaging.Current.TokenChanged += async (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("PushNotification: Token refreshed, re-registering.");
                string newToken = args.Token;
                await SecureStorage.SetAsync(FcmTokenKey, newToken);
                await RegisterTokenWithServerAsync(newToken);
            };
        }
#endif
    }
}
