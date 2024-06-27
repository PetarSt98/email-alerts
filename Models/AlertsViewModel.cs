using System.Collections.Generic;

namespace email_alerts.Models
{
    public class AlertsViewModel
    {
        public string QueryDescription { get; set; }
        public IEnumerable<Dictionary<string, object>> EmailLogs { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalEmailLogs { get; set; }
    }
}
