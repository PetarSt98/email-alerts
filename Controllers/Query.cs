namespace email_alerts.Models
{
    public class Query
    {
        public int id { get; set; }
        public string Text { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; }
        public int? Period { get; set; }
        public int? LegacyMsg { get; set; }
        public int? Timeout { get; set; }
        public int? TemplateID { get; set; }
        public string? TemplateParameters { get; set; }
        public int? QueryType { get; set; }
        public int? ReceiverType { get; set; }
        public int MessageFormat { get; set; }
    }
}
