using CommunityToolkit.Maui;
using JCA.Mobile.Services;
using JCA.Mobile.ViewModels;
using JCA.Mobile.Views;
using Microsoft.Extensions.Logging;
#if ANDROID || IOS
using Plugin.Firebase.Core;
#endif

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

            // Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AnnouncementService>();
            builder.Services.AddTransient<AthleticsService>();

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
