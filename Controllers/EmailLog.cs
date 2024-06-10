namespace email_alerts.Models
{
    public class EmailLog
    {
        public int ID { get; set; }
        public int QueryID { get; set; }
        public string EMail { get; set; }
        public DateTime Date { get; set; }
        public string PCName { get; set; }
        public string? Info { get; set; }
        public int? SentStatus { get; set; }
        public Guid? SessionID { get; set; }
    }
}
