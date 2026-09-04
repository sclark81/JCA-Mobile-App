using CommunityToolkit.Maui;
using JCA.Mobile.Services;
using JCA.Mobile.ViewModels;
using JCA.Mobile.Views;
using Microsoft.Extensions.Logging;
#if ANDROID || IOS
using Plugin.Firebase.Core;
#endif
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;

namespace JCA.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
#if ANDROID
                .UseFirebase()
#elif IOS
                .UseFirebase()
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
                    fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                    fonts.AddFont("Roboto-Light.ttf", "RobotoLight");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                    fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined");
                });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        //builder.Services.AddSingleton<ThemeService>();
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
            return new AnnouncementService(new HttpClient(handler), authService);
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

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<AnnouncementViewModel>();
            builder.Services.AddTransient<AthleticsViewModel>();
            builder.Services.AddTransient<EventDetailViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<AnnouncementsPage>();
            builder.Services.AddTransient<AthleticsPage>();
            builder.Services.AddTransient<EventDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
