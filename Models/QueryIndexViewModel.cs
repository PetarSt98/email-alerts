using email_alerts.Models.EmailAlerts;

namespace email_alerts.Models
{
    public class QueryIndexViewModel
    {
        public IEnumerable<Query> Queries { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalQueries { get; set; }
    }
}
