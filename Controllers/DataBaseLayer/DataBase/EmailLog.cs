using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


namespace email_alerts.DataBaseLayer.Data
{
    [Table("EmailLog")]
    public class EmailLog
    {
        public EmailLog()
        {
            session = new HashSet<Session>();
            queryLog = new HashSet<QueryLog>();
        }

        [Key]
        public int ID { get; set; }
        [Required]
        public int QueryID { get; set; }
        [Required]
        public string EMail { get; set; }
        public DateTime Date { get; set; }
        public string PCName { get; set; }
        public string? Info { get; set; }
        public int? SentStatus { get; set; }
        [Required]
        public Guid? SessionID { get; set; }

        public virtual ICollection<Session> session { get; set; }
        public virtual ICollection<QueryLog> queryLog { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
