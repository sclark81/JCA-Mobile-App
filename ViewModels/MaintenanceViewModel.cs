using System.Collections.ObjectModel;
using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JCA.Mobile.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCA.Mobile.ViewModels
{
    public partial class MaintenanceViewModel : ObservableObject
    {
        private readonly MaintenanceService _service = new MaintenanceService();

        [ObservableProperty]
        private ObservableCollection<MaintenanceTicket> tickets = new ObservableCollection<MaintenanceTicket>();

        [ObservableProperty]
        private bool isRefreshing;

        public MaintenanceViewModel()
        {
            Task _ = LoadTicketsAsync();
        }

        [RelayCommand]
        public async Task LoadTicketsAsync()
        {
            IsRefreshing = true;
            List<MaintenanceTicket> data = await _service.GetTicketsAsync();
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Tickets.Clear();
                foreach (MaintenanceTicket ticket in data)
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

        [RelayCommand]
        public async Task GoToCreateTicketAsync()
        {
            await Shell.Current.GoToAsync(nameof(CreateTicketPage));
        }
    }
}
