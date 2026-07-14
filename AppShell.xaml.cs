using JCA.Mobile.Views;

namespace JCA.Mobile;

public partial class AppShell : Microsoft.Maui.Controls.Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(MaintenanceDetailPage), typeof(MaintenanceDetailPage));
		Routing.RegisterRoute(nameof(CreateTicketPage), typeof(CreateTicketPage));
	}
}
