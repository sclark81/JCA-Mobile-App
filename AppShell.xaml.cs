using JCA.Mobile.Services;
using JCA.Mobile.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCA.Mobile;

public partial class AppShell : Shell
{
    private const string MaintenanceGroupName = "Maintenance";
    private readonly AuthService _authService;

    public AppShell(AuthService authService)
    {
        _authService = authService;
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("CreateTicketPage", typeof(CreateTicketPage));
        Routing.RegisterRoute("MaintenanceDetailPage", typeof(MaintenanceDetailPage));
    }

    /// <summary>
    /// Reads roles from the stored JWT and shows or hides tabs accordingly.
    /// Call this after a successful login or token refresh.
    /// </summary>
    public async Task ApplyUserRolesAsync()
    {
        IList<string> roles = await _authService.GetUserRolesAsync();
        MaintenanceTab.IsVisible = roles.Contains(MaintenanceGroupName);
    }
}
