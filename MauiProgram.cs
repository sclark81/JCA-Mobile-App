using CommunityToolkit.Maui;
using JCA.Mobile.Services;
using JCA.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace JCA.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App, AppShell>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AnnouncementService>();
        builder.Services.AddSingleton<MaintenanceService>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MaintenancePage>();
        builder.Services.AddTransient<CreateTicketPage>();
        builder.Services.AddTransient<MaintenanceDetailPage>();

        return builder.Build();
    }
}
