using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace email_alerts.Models
{
    public class QueryLog
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int QueryID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Info { get; set; }
        public Guid? SessionID { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QueryLog> queryLog { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
