using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


namespace email_alerts.Models.EmailAlerts
{
    [Table("EmailLog")]
    public class EmailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int QueryID { get; set; }

        [Required]
        [StringLength(200)]
        public string EMail { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string PCName { get; set; }

        [Required]
        [StringLength(2000)]
        public string? Info { get; set; }

        public int? SentStatus { get; set; }

        public Guid? SessionID { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session? Session { get; set; }

        [ForeignKey("QueryID")]
        public virtual Query? Query { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
