using System.Collections.ObjectModel;
using System.Windows.Input;
using JCA.Mobile.Models;

namespace JCA.Mobile.ViewModels;

public class MainViewModel
{
    public ObservableCollection<Notification> Notifications { get; } = new();
    public ICommand DismissCommand { get; }

    public MainViewModel()
    { 
        DismissCommand = new Command<string>(OnDismiss);
        LoadMockData();
    }

    private void OnDismiss(string id)
    {
        var item = Notifications.FirstOrDefault(n => n.Id == id);
        if (item != null)
        {
            Notifications.Remove(item);
        }
    }

    private void LoadMockData()
    {
        Notifications.Add(new Notification { Id = "1", Category = NotificationCategory.Emergency, Title = "Snow Day Tomorrow!", Body = "All classes canceled.", Timestamp = "2 min ago" });
        Notifications.Add(new Notification { Id = "2", Category = NotificationCategory.Progress, Title = "Math Certificate", Body = "Leo earned an award.", Timestamp = "1 hour ago", StudentName = "Leo Martinez" });
        Notifications.Add(new Notification { Id = "3", Category = NotificationCategory.Update, Title = "PTA Meeting", Body = "6:30 PM in the auditorium.", Timestamp = "3 hours ago" });
    }
}
