using System.Collections.ObjectModel;
using System.Windows.Input;
using JCA.Mobile.Models;
using JCA.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JCA.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AnnouncementService _announcementService = new();

    [ObservableProperty]
    private ObservableCollection<Announcement> announcements = new();

    [ObservableProperty]
    private bool isRefreshing;

    public ICommand RefreshCommand { get; }
    public ICommand DismissCommand { get; }

    public MainViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadAnnouncementsAsync);
        DismissCommand = new RelayCommand<int>(OnDismiss);
        
        // Initial load
        _ = LoadAnnouncementsAsync();
    }

    private async Task LoadAnnouncementsAsync()
    {
        IsRefreshing = true;
        
        var data = await _announcementService.GetAnnouncementsAsync();
        
        MainThread.BeginInvokeOnMainThread(() => {
            Announcements.Clear();
            foreach (var item in data)
            {
                Announcements.Add(item);
            }
        });

        IsRefreshing = false;
    }

    private void OnDismiss(int id)
    {
        var item = Announcements.FirstOrDefault(n => n.Id == id);
        if (item != null)
        {
            Announcements.Remove(item);
        }
    }
}