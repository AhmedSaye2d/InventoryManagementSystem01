namespace Domain.Entities.ActivityTracker
{
    public class Tracker
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public bool OperationStatus { get; set; }
    }
}

