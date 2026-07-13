using System.ComponentModel.DataAnnotations;

namespace JCA.Mobile.Models
{
    public enum Campus
    {
        Main = 0,
        West = 1,
        South = 2,
        All = 3
    }

    public enum TargetAudience
    {
        All = 0,
        Faculty = 1,
        Students = 2,
        Parents = 3
    }

    public enum MaintenanceCategory
    {
        [Display(Name = "Safety Issue")] Safety = 0,
        Repair = 1,
        Improvement = 2,
        [Display(Name = "Quality of Life")] QualityofLife = 3,
        Electrical = 4,
        Plumbing = 5,
        [Display(Name = "General Construction")] Construction = 6,
        Other = 7
    }

    public enum TicketPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum TicketStatus
    {
        Open = 0,
        [Display(Name = "In Progress")] InProgress = 1,
        [Display(Name = "On Hold")] OnHold = 2,
        Completed = 3,
        Cancelled = 4
    }
}
