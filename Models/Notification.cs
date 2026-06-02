namespace JCA.Mobile.Models;

public enum NotificationCategory
{
    Emergency,
    Progress,
    Update
}

public class Notification
{
    public string Id { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string? StudentName { get; set; }
}
