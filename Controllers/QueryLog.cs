using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace email_alerts.Models
{
    [Table("QueryLog")]
    public class QueryLog
    {
        public QueryLog()
        {
            query = new HashSet<Query>();
        }

        [Key]
        public int id { get; set; }
        [Required]
        [ForeignKey("EmailLog")]
        public int QueryID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Info { get; set; }
        public Guid? SessionID { get; set; }

        public virtual ICollection<Query> query { get; set; }
        public virtual EmailLog emailLog { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
