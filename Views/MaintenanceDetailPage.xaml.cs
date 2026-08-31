namespace JCA.Mobile.Views;

public partial class MaintenanceDetailPage : ContentPage
{
	public MaintenanceDetailPage(ViewModels.MaintenanceDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
