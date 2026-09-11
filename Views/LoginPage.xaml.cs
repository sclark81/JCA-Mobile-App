using JCA.Mobile.Services;

namespace JCA.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly PushNotificationService _pushNotificationService;

        public LoginPage(AuthService authService, PushNotificationService pushNotificationService)
        {
            InitializeComponent();
            _authService = authService;
            _pushNotificationService = pushNotificationService;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            ErrorLabel.IsVisible = false;

            try
            {
                bool success = await _authService.LoginAsync();

                if (success)
                {
#if ANDROID || IOS
                    await _pushNotificationService.RegisterDeviceAsync();
#endif
                    // Navigate to the main app shell
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    ErrorLabel.Text = "Login failed. Please try again.";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = $"Error: {ex.Message}";
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
    }
}
