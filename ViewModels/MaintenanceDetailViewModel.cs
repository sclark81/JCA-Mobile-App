using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JCA.Mobile.ViewModels
{
    [QueryProperty(nameof(TicketId), "id")]
    public partial class MaintenanceDetailViewModel : ObservableObject
    {
        private readonly MaintenanceService _service = new();

        [ObservableProperty]
        private int ticketId;

        [ObservableProperty]
        private MaintenanceTicket? ticket;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? adminNotes;

        [ObservableProperty]
        private TicketStatus selectedStatus;

        public List<TicketStatus> StatusOptions { get; } = Enum.GetValues<TicketStatus>().Cast<TicketStatus>().ToList();

        partial void OnTicketIdChanged(int value)
        {
            _ = LoadTicketAsync(value);
        }

        private async Task LoadTicketAsync(int id)
        {
            IsBusy = true;
            Ticket = await _service.GetTicketByIdAsync(id);
            if (Ticket != null)
            {
                AdminNotes = Ticket.Notes;
                SelectedStatus = Ticket.Status;
            }
            IsBusy = false;
        }

        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (Ticket == null) return;

            IsBusy = true;
            var success = await _service.UpdateTicketStatusAsync(Ticket.Id, SelectedStatus, AdminNotes);
            IsBusy = false;

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to update ticket status.", "OK");
            }
        }
    }
}
