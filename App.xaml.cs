using JCA.Mobile.Services;

namespace JCA.Mobile;

public partial class App : Application
{
    public App(PushNotificationService pushNotificationService)
    {
        InitializeComponent();

#if ANDROID || IOS
        pushNotificationService.SubscribeToNotifications();
#endif
    }

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
