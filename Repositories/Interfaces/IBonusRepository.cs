using SparkUP.CasinoAPI.Entities;
using SparkUP.CasinoAPI.Enums;

namespace SparkUP.CasinoAPI.Repositories.Interfaces
{
    public interface IBonusRepository
    {
        Task<PlayerBonus> GetByIdAsync(Guid id);

        Task<(List<PlayerBonus> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);

        Task<PlayerBonus> GetActiveByPlayerAndTypeAsync(Guid playerId, BonusType bonusType);

        Task<PlayerBonus> CreateAsync(PlayerBonus bonus);

        Task<PlayerBonus> UpdateAsync(PlayerBonus bonus);

        Task<bool> DeleteAsync(Guid id);

        Task LogAuditAsync(Guid bonusId, string action, string operatorName, string details);
    }
}
