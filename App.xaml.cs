using JCA.Mobile.Services;

#if ANDROID || IOS
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
#endif


namespace JCA.Mobile;

public partial class App : Application
{
    private readonly PushNotificationService _pushNotificationService;

    public App(PushNotificationService pushNotificationService)//ThemeService themeService,
    {
        InitializeComponent();
#if ANDROID || IOS
        pushNotificationService.SubscribeToNotifications();
#endif
        //MainPage = new AppShell();

        //_pushNotificationService = new PushNotificationService();

#if ANDROID || IOS
        // Subscribe to FCM token refresh events so re-registration happens automatically
        Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.TokenChanged += OnFcmTokenChanged;
#endif
    }

#if ANDROID || IOS
    private async void OnFcmTokenChanged(object? sender,
        Plugin.Firebase.CloudMessaging.EventArgs.FCMTokenChangedEventArgs args)
    {
        try
        {
            if (await _authService.IsTokenValidAsync())
            {
                await _authService.RefreshDeviceRegistrationAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FCM token refresh handler error: {ex.Message}");
        }
    }
#endif

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new Window(new AppShell())
        {
            Width = 430,
            Height = 920
        };

        return window;
    }
}
