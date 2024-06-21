using email_alerts.Models.EmailAlerts;

namespace email_alerts.Models
{
    public class HistoryViewModel
    {
        public string QueryDescription { get; set; }
        public IEnumerable<EmailLog> EmailLogs { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalEmailLogs { get; set; }
    }
}
