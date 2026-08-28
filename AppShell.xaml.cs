using JCA.Mobile.Views;

namespace JCA.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("CreateTicketPage", typeof(CreateTicketPage));
        Routing.RegisterRoute("MaintenanceDetailPage", typeof(MaintenanceDetailPage));
    }
}
