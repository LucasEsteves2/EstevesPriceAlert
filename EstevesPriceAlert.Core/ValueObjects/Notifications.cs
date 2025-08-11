namespace EstevesPriceAlert.Core.ValueObjects
{
    public class Notifications
    {
        public string ProductName { get; set; }
        public decimal TargetPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastNotifiedAtUtc { get; set; }
        public int NotifyCount { get; set; } = 0;
        public int MinHoursBetweenAlerts { get; set; } = 24;
        public List<string> ProductUrls { get; set; } = new();
    }
}
