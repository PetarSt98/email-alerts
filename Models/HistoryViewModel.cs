using email_alerts.Models.EmailAlerts;

namespace email_alerts.Models
{
    public class HistoryViewModel
    {
        public string QueryDescription { get; set; }
        public IEnumerable<EmailLog> EmailLogs { get; set; }
    }
}
