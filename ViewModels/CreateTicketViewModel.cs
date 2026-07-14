using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace JCA.Mobile.ViewModels
{
    public partial class CreateTicketViewModel : ObservableObject
    {
        private readonly MaintenanceService _service = new MaintenanceService();

        [ObservableProperty]
        private string details = string.Empty;

        [ObservableProperty]
        private string room = string.Empty;

        [ObservableProperty]
        private Campus selectedCampus = Campus.Main;

        [ObservableProperty]
        private MaintenanceCategory selectedCategory = MaintenanceCategory.Other;

        [ObservableProperty]
        private TicketPriority selectedPriority = TicketPriority.Medium;

        [ObservableProperty]
        private DateTime dueDate = DateTime.Now.AddDays(7);

        [ObservableProperty]
        private string? imagePath;

        [ObservableProperty]
        private string previewImage = "placeholder_image.png";

        [ObservableProperty]
        private bool isBusy;

        public List<Campus> CampusOptions { get; } = Enum.GetValues(typeof(Campus)).Cast<Campus>().ToList();
        public List<MaintenanceCategory> CategoryOptions { get; } = Enum.GetValues(typeof(MaintenanceCategory)).Cast<MaintenanceCategory>().ToList();
        public List<TicketPriority> PriorityOptions { get; } = Enum.GetValues(typeof(TicketPriority)).Cast<TicketPriority>().ToList();

        [RelayCommand]
        public async Task TakePhotoAsync()
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                IsBusy = true;
                string? serverPath = await _service.UploadImageAsync(photo);
                if (serverPath != null)
                {
                    ImagePath = serverPath;
                    PreviewImage = $"https://tools.jcadm.org{serverPath}";
                }
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveTicketAsync()
        {
            if (string.IsNullOrWhiteSpace(Details) || string.IsNullOrEmpty(Room))
            {
                await Shell.Current.DisplayAlert("Validation", "Please provide both details and a room/location.", "OK");
                return;
            }

            IsBusy = true;
            MaintenanceTicket newTicket = new MaintenanceTicket
            { 
                Details = Details,
                Room = Room,
                Campus = SelectedCampus,
                Category = SelectedCategory,
                Priority = SelectedPriority,
                DueDate = DueDate,
                ImagePath = ImagePath,
                SubmittedBy = "Maintenance App", 
                SubmittedEmail = "utility@jcadm.org",
                DateSubmitted = DateTime.Now,
                Status = TicketStatus.Open
            };

            bool success = await _service.CreateTicketAsync(newTicket);
            IsBusy = false;

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create ticket.", "OK");
            }
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
