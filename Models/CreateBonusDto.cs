using System.ComponentModel.DataAnnotations;

namespace SparkUP.CasinoAPI.DTOs
{
    public class CreateBonusDto
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BonusType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
