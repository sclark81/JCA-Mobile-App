using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

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

        [ObservableProperty]
        private DateTime dueDate;

        [ObservableProperty]
        private string? selectedImagePath;

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
                DueDate = Ticket.DueDate ?? DateTime.Now.AddDays(7);
                SelectedImagePath = Ticket.FullImagePath;
            }
            IsBusy = false;
        }

        [RelayCommand]
        public async Task TakePhotoAsync()
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                IsBusy = true;
                var serverPath = await _service.UploadImageAsync(photo);
                if (serverPath != null && Ticket != null)
                {
                    Ticket.ImagePath = serverPath;
                    SelectedImagePath = Ticket.FullImagePath;
                }
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (Ticket == null) return;

            IsBusy = true;
            Ticket.Status = SelectedStatus;
            Ticket.Notes = AdminNotes;
            Ticket.DueDate = DueDate;

            var success = await _service.UpdateTicketAsync(Ticket);
            IsBusy = false;

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to update ticket.", "OK");
            }
        }
    }
}
