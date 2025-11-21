using System.ComponentModel.DataAnnotations;

namespace SparkUP.CasinoAPI.DTOs
{
    public class UpdateBonusDto
    {
        public decimal? Amount { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
