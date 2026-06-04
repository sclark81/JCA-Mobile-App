namespace JCA.Mobile.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Safely access the BindingContext we declared in XAML
        if (BindingContext is ViewModels.MainViewModel viewModel)
        {
            // Trigger the clean async call now that the UI is active!
            //await viewModel.RefreshCommand.ExecuteAsync(null);
        }
    }
}