using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace email_alerts.Models
{
    [Table("Session")]
    public partial class Session
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Info { get; set; }
        public virtual ICollection<Session> session { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }


    }
}
