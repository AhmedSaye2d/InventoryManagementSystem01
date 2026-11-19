namespace Application.DTO.Response.Activity
{
    public class BaseActivityTracker
    {
        public string Title { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool OperationStatus { get; set; }
    }
}
