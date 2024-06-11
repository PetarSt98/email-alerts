using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


namespace email_alerts.Models.EmailAlerts
{
    [Table("QueryLog")]
    public class QueryLog
    {
        [Key]
        public int id { get; set; }
        [Required]
        [ForeignKey("Query")]
        public int QueryID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Info { get; set; }
        public Guid? SessionID { get; set; }

        public virtual Query Query { get; set; }

        public virtual Session Session { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
