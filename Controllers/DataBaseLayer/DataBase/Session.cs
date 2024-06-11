using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace email_alerts.DataBaseLayer.Data
{
    [Table("Session")]
    public partial class Session
    {
        [Key]
        //[ForeignKey("EmailLog")]
        public Guid id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Info { get; set; }

        public virtual ICollection<EmailLog> EmailLog { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }


    }
}
