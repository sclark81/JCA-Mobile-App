using JCA.Mobile.Services;

namespace JCA.Mobile;

public partial class App : Application
{
    private readonly AppShell _appShell;

    public App(PushNotificationService pushNotificationService, AppShell appShell)
    {
        _appShell = appShell;
        InitializeComponent();

#if ANDROID || IOS
        pushNotificationService.SubscribeToNotifications();
#endif
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new Window(_appShell)
        {
            Width = 430,
            Height = 920
        };

        return window;
    }
}
