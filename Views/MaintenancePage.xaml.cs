namespace JCA.Mobile.Views;

public partial class MaintenancePage : ContentPage
{
	public MaintenancePage(ViewModels.MaintenanceViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
