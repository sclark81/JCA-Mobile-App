using JCA.Mobile.Services;

namespace JCA.Mobile;

public partial class App : Application
{
    private readonly ThemeService _themeService;
    private readonly AuthService _authService;

    public App(ThemeService themeService, AuthService authService)
    {
        InitializeComponent();
        _themeService = themeService;
        _authService = authService;
        _themeService.ApplyTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += async (sender, args) =>
        {
            // Check authentication state and navigate accordingly
            bool isValid = await _authService.IsTokenValidAsync();
            if (!isValid)
            {
                // Not authenticated or token expired - try refresh first
                bool refreshed = await _authService.RefreshTokenAsync();
                if (!refreshed)
                {
                    // Navigate to login page
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
        };

        return window;
    }
}
