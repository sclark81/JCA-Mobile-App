using System.Collections.ObjectModel;
using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JCA.Mobile.Views;

namespace JCA.Mobile.ViewModels
{
    public partial class MaintenanceViewModel : ObservableObject
    {
        private readonly MaintenanceService _service = new();

        [ObservableProperty]
        private ObservableCollection<MaintenanceTicket> tickets = new();

        [ObservableProperty]
        private bool isRefreshing;

        public MaintenanceViewModel()
        {
            _ = LoadTicketsAsync();
        }

        [RelayCommand]
        public async Task LoadTicketsAsync()
        {
            IsRefreshing = true;
            var data = await _service.GetTicketsAsync();
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Tickets.Clear();
                foreach (var ticket in data)
                {
                    Tickets.Add(ticket);
                }
            });
            
            IsRefreshing = false;
        }

        [RelayCommand]
        public async Task GoToDetailsAsync(MaintenanceTicket ticket)
        {
            if (ticket == null) return;

            await Shell.Current.GoToAsync($"{nameof(MaintenanceDetailPage)}?id={ticket.Id}");
        }
    }
}
