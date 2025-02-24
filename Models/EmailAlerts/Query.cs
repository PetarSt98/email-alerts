﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;


namespace email_alerts.Models.EmailAlerts
{
    [Table("Query")]
    public class Query
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "NTEXT")]
        public string Text { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public bool Active { get; set; }
        
        public string? Subject { get; set; }
        
        [Required]
        [Column(TypeName = "NTEXT")]
        public string Body { get; set; }
        
        public int? Period { get; set; }
        
        public int? LegacyMsg { get; set; }
        
        public int? Timeout { get; set; }
        
        public int? TemplateID { get; set; }
        
        [Column(TypeName = "NTEXT")]
        public string? TemplateParameters { get; set; }
        
        public int? QueryType { get; set; }
        
        public int? ReceiverType { get; set; }
        
        [Required]
        public int MessageFormat { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
