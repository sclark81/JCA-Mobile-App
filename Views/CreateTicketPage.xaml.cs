namespace JCA.Mobile.Views;

public partial class CreateTicketPage : ContentPage
{
	public CreateTicketPage(ViewModels.CreateTicketViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
