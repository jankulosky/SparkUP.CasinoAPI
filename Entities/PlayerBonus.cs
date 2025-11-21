using SparkUP.CasinoAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace SparkUP.CasinoAPI.Entities
{
    public class PlayerBonus
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public BonusType BonusType { get; set; }

        public decimal Amount { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }
        

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [MaxLength(100)]
        public string UpdatedBy { get; set; }
    }
}
