using SparkUP.CasinoAPI.DTOs;
using SparkUP.CasinoAPI.Models;

namespace SparkUP.CasinoAPI.Services.Interfaces
{
    public interface IBonusService
    {
        Task<PagedResult<PlayerBonusDto>> GetAllBonusesAsync(int pageNumber, int pageSize);

        Task<PlayerBonusDto> CreateBonusAsync(CreateBonusDto createDto, string operatorName);

        Task<PlayerBonusDto> UpdateBonusAsync(Guid id, UpdateBonusDto updateDto, string operatorName);

        Task<bool> DeleteBonusAsync(Guid id, string operatorName);
    }
}
