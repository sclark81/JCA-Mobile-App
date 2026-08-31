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
#if DEBUG
        builder.Services.AddSingleton<AnnouncementService>(sp =>
        {
            AuthService authService = sp.GetRequiredService<AuthService>();
            HttpClientHandler sslHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService, sslHandler);
            return new AnnouncementService(new HttpClient(handler));
        });
        builder.Services.AddSingleton<MaintenanceService>(sp =>
        {
            AuthService authService = sp.GetRequiredService<AuthService>();
            HttpClientHandler sslHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService, sslHandler);
            return new MaintenanceService(new HttpClient(handler));
        });
#else
        builder.Services.AddSingleton<AnnouncementService>(sp =>
        {
            AuthService authService = sp.GetRequiredService<AuthService>();
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService);
            return new AnnouncementService(new HttpClient(handler));
        });
        builder.Services.AddSingleton<MaintenanceService>(sp =>
        {
            AuthService authService = sp.GetRequiredService<AuthService>();
            AuthenticatedHttpClientHandler handler = new AuthenticatedHttpClientHandler(authService);
            return new MaintenanceService(new HttpClient(handler));
        });
#endif

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MaintenancePage>();
        builder.Services.AddTransient<CreateTicketPage>();
        builder.Services.AddTransient<MaintenanceDetailPage>();

        return builder.Build();
    }
}
