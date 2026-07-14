using System;

namespace JCA.Mobile.Models
{
    public class MaintenanceTicket
    {
        public int Id { get; set; }
        public string Details { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public MaintenanceCategory Category { get; set; }
        public string? Notes { get; set; }
        public string SubmittedEmail { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public Campus Campus { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateCompleted { get; set; }
        public TicketStatus Status { get; set; }
        public string? ImagePath { get; set; }

        public string PriorityColor => Priority switch
        {
            TicketPriority.High => "#CC0000",
            TicketPriority.Medium => "#FF8C00",
            _ => "#4B5563"
        };

        // Added for easy access in the app
        public string FullImagePath => string.IsNullOrEmpty(ImagePath) 
            ? "placeholder_image.png" 
            : $"https://tools.jcadm.org{ImagePath}"; // Update with actual prod domain later
    }
}
