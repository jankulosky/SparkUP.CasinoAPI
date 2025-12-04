using AutoMapper;
using SparkUP.CasinoAPI.DTOs;
using SparkUP.CasinoAPI.Entities;
using SparkUP.CasinoAPI.Enums;
using SparkUP.CasinoAPI.Models;
using SparkUP.CasinoAPI.Repositories.Interfaces;
using SparkUP.CasinoAPI.Services.Interfaces;

namespace SparkUP.CasinoAPI.Services
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }

    public class BonusService : IBonusService
    {
        private readonly IBonusRepository _bonusRepository;
        private readonly IMapper _mapper;

        public BonusService(IBonusRepository repository, IMapper mapper)
        {
            _bonusRepository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PlayerBonusDto>> GetAllBonusesAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _bonusRepository.GetAllAsync(pageNumber, pageSize);

            return new PagedResult<PlayerBonusDto>
            {
                Items = _mapper.Map<List<PlayerBonusDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PlayerBonusDto> CreateBonusAsync(CreateBonusDto createDto, string operatorName)
        {
            if (!Enum.TryParse<BonusType>(createDto.BonusType, true, out var type))
                throw new BusinessRuleException("Invalid bonus type.");

            var existingBonus = await _bonusRepository.GetActiveByPlayerAndTypeAsync(
                createDto.PlayerId,
                type);

            if (existingBonus != null)
            {
                throw new BusinessRuleException(
                    $"Player {createDto.PlayerId} already has an active {createDto.BonusType} bonus.");
            }

            var bonus = _mapper.Map<PlayerBonus>(createDto);
            bonus.CreatedBy = operatorName;
            bonus.CreatedAt = DateTime.UtcNow;

            var createdBonus = await _bonusRepository.CreateAsync(bonus) ?? throw new BusinessRuleException("Failed to create bonus."); ;

            await _bonusRepository.LogAuditAsync(
                createdBonus.Id,
                "Created",
                operatorName,
                $"Bonus created: {createDto.BonusType}, Amount: {createDto.Amount}");

            return _mapper.Map<PlayerBonusDto>(createdBonus);
        }

        public async Task<PlayerBonusDto> UpdateBonusAsync(Guid id, UpdateBonusDto updateDto, string operatorName)
        {
            var bonus = await _bonusRepository.GetByIdAsync(id) ?? throw new BusinessRuleException($"Bonus with ID {id} not found.");

            if (updateDto.IsActive == true)
            {
                var existingActive = await _bonusRepository.GetActiveByPlayerAndTypeAsync(
                    bonus.PlayerId,
                    bonus.BonusType
                );

                if (existingActive != null && existingActive.Id != id)
                {
                    throw new BusinessRuleException(
                        $"Player {bonus.PlayerId} already has an active {bonus.BonusType} bonus."
                    );
                }
            }

            var changes = new List<string>();

            if (updateDto.Amount.HasValue)
            {
                changes.Add($"Amount: {bonus.Amount} → {updateDto.Amount.Value}");
                bonus.Amount = updateDto.Amount.Value;
            }

            if (updateDto.IsActive.HasValue)
            {
                changes.Add($"IsActive: {bonus.IsActive} → {updateDto.IsActive.Value}");
                bonus.IsActive = updateDto.IsActive.Value;
            }

            if (updateDto.ExpiresAt.HasValue)
            {
                changes.Add($"ExpiresAt: {bonus.ExpiresAt} → {updateDto.ExpiresAt.Value}");
                bonus.ExpiresAt = updateDto.ExpiresAt.Value;
            }

            bonus.UpdatedAt = DateTime.UtcNow;
            bonus.UpdatedBy = operatorName;

            var updatedBonus = await _bonusRepository.UpdateAsync(bonus);

            await _bonusRepository.LogAuditAsync(
                id,
                "Updated",
                operatorName,
                string.Join(", ", changes));

            return _mapper.Map<PlayerBonusDto>(updatedBonus);
        }

        public async Task<bool> DeleteBonusAsync(Guid id, string operatorName)
        {
            var bonus = await _bonusRepository.GetByIdAsync(id);

            if (bonus == null) return false;

            var result = await _bonusRepository.DeleteAsync(id);

            if (result)
            {
                await _bonusRepository.LogAuditAsync(
                    id,
                    "Deleted",
                    operatorName,
                    $"Bonus deactivated: {bonus.BonusType}");
            }

            return result;
        }
    }
}
