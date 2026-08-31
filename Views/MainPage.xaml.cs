namespace JCA.Mobile.Views;

public partial class MainPage : ContentPage
{
	public MainPage(ViewModels.MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModels.MainViewModel viewModel)
        {
        }
    }
}
