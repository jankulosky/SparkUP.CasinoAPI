using Microsoft.EntityFrameworkCore;
using SparkUP.CasinoAPI.Entities;

namespace SparkUP.CasinoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PlayerBonus> PlayerBonuses { get; set; }
        public DbSet<BonusAuditLog> BonusAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerBonus>(b =>
            {
                b.Property(x => x.BonusType)
                    .HasConversion<int>();

                b.HasIndex(x => new { x.PlayerId, x.BonusType, x.IsActive });
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
