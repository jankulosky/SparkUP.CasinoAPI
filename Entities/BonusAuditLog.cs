using System.ComponentModel.DataAnnotations;

namespace SparkUP.CasinoAPI.Entities
{
    public class BonusAuditLog
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? BonusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Action { get; set; }

        public string? OperatorName { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
