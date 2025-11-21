namespace SparkUP.CasinoAPI.Models
{
    public class PlayerBonusDto
    {
        public Guid Id { get; set; }

        public Guid PlayerId { get; set; }

        public string BonusType { get; set; }

        public decimal Amount { get; set; }

        public decimal WageringRequirement { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string CreatedBy { get; set; }
    }
}
