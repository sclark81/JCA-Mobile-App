using System;
using System.Net.Http;
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
        /// Subscribes to FCM notification-received, notification-tapped, and token-refresh events.
        /// Call once at app startup after Firebase is initialized.
        /// </summary>
        public void SubscribeToNotifications()
        {
#if ANDROID || IOS
            CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificationReceived;
            CrossFirebaseCloudMessaging.Current.NotificationTapped += OnNotificationTapped;
            CrossFirebaseCloudMessaging.Current.TokenChanged += OnTokenChanged;
#endif
        }

        /// <summary>
        /// Gets the current FCM token and registers it with the server.
        /// Call after successful login.
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
                    System.Diagnostics.Debug.WriteLine("FCM: Token is empty, skipping registration.");
                    return;
                }

                string? storedToken = await SecureStorage.GetAsync(FcmTokenKey);
                if (storedToken == fcmToken)
                {
                    System.Diagnostics.Debug.WriteLine("FCM: Token unchanged, skipping registration.");
                    return;
                }

                await SecureStorage.SetAsync(FcmTokenKey, fcmToken);
                await RegisterTokenWithServerAsync(fcmToken);
#else
                await Task.CompletedTask;
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FCM: Registration error - {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the device FCM token from the server. Call on logout.
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

                object payload = new { fcmToken };
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl + UnregisterPath, content);

                if (response.IsSuccessStatusCode)
                {
                    SecureStorage.Remove(FcmTokenKey);
                    System.Diagnostics.Debug.WriteLine("FCM: Device unregistered.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"FCM: Unregister failed - {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FCM: Unregister error - {ex.Message}");
            }
        }

        private async Task RegisterTokenWithServerAsync(string fcmToken)
        {
            try
            {
                string platform = DeviceInfo.Current.Platform == DevicePlatform.Android ? "Android" : "iOS";

                object payload = new { fcmToken, platform };
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl + RegisterPath, content);

                System.Diagnostics.Debug.WriteLine(
                    $"FCM: Device registration {(response.IsSuccessStatusCode ? "succeeded" : "failed")} - {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FCM: Server registration error - {ex.Message}");
            }
        }

#if ANDROID || IOS
        private void OnNotificationReceived(object? sender, FCMNotificationReceivedEventArgs args)
        {
            string title = args.Notification?.Title ?? "New Announcement";
            string body = args.Notification?.Body ?? string.Empty;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    bool navigate = await Application.Current.MainPage.DisplayAlert(
                        title, body, "View", "Dismiss");

                    if (navigate)
                    {
                        // TODO: Update route to //AnnouncementsPage once registered in AppShell.xaml
                        await Shell.Current.GoToAsync("//MainPage");
                    }
                }
            });
        }

        private void OnNotificationTapped(object? sender, FCMNotificationTappedEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // TODO: Update route to //AnnouncementsPage once registered in AppShell.xaml
                await Shell.Current.GoToAsync("//MainPage");
            });
        }

        private async void OnTokenChanged(object? sender, FCMTokenChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("FCM: Token refreshed, re-registering.");
            string newToken = args.Token;
            await SecureStorage.SetAsync(FcmTokenKey, newToken);
            await RegisterTokenWithServerAsync(newToken);
        }
#endif
    }
}
