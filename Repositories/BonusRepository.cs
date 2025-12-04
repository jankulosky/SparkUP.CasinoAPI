using Microsoft.EntityFrameworkCore;
using SparkUP.CasinoAPI.Data;
using SparkUP.CasinoAPI.Entities;
using SparkUP.CasinoAPI.Enums;
using SparkUP.CasinoAPI.Repositories.Interfaces;

namespace SparkUP.CasinoAPI.Repositories
{
    public class BonusRepository : IBonusRepository
    {
        private readonly AppDbContext _context;

        public BonusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerBonus> GetByIdAsync(Guid id)
        {
            return await _context.PlayerBonuses
                .FirstAsync(b => b.Id == id);
        }

        public async Task<(List<PlayerBonus> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.PlayerBonuses.AsQueryable();
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<PlayerBonus> GetActiveByPlayerAndTypeAsync(Guid playerId, BonusType bonusType)
        {
            return await _context.PlayerBonuses
                .FirstAsync(b =>
                    b.PlayerId == playerId &&
                    b.BonusType == bonusType &&
                    b.IsActive);
        }

        public async Task<PlayerBonus> CreateAsync(PlayerBonus bonus)
        {
            _context.PlayerBonuses.Add(bonus);

            await _context.SaveChangesAsync();

            return bonus;
        }

        public async Task<PlayerBonus> UpdateAsync(PlayerBonus bonus)
        {
            _context.PlayerBonuses.Update(bonus);

            await _context.SaveChangesAsync();

            return bonus;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bonus = await GetByIdAsync(id);
            if (bonus == null) return false;

            bonus.IsActive = false;
            bonus.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task LogAuditAsync(Guid bonusId, string action, string operatorName, string details)
        {
            var auditLog = new BonusAuditLog
            {
                BonusId = bonusId,
                Action = action,
                OperatorName = operatorName,
                Timestamp = DateTime.UtcNow,
                Details = details
            };

            _context.BonusAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
