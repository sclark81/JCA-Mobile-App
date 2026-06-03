using System;

namespace JCA.Mobile.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Campus Campus { get; set; } = Campus.All;
    public TargetAudience Audience { get; set; } = TargetAudience.Parents;
    public DateTime PublishDate { get; set; } = DateTime.Now;
    public bool IsPriority { get; set; } = false;
}
