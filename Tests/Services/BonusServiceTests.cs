using AutoMapper;
using Moq;
using SparkUP.CasinoAPI.DTOs;
using SparkUP.CasinoAPI.Entities;
using SparkUP.CasinoAPI.Enums;
using SparkUP.CasinoAPI.Mapping;
using SparkUP.CasinoAPI.Repositories.Interfaces;
using SparkUP.CasinoAPI.Services;
using Xunit;

namespace SparkUP.CasinoAPI.Tests.Services
{
    public class BonusServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IBonusRepository> _mockRepository;
        private readonly BonusService _service;

        public BonusServiceTests()
        {
            var loggerFactory = new LoggerFactory();

            var configExpr = new MapperConfigurationExpression();
            configExpr.AddProfile(new BonusMappingProfile());

            var cfg = new MapperConfiguration(configExpr, loggerFactory);

            _mapper = cfg.CreateMapper();
            _mockRepository = new Mock<IBonusRepository>();
            _service = new BonusService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllBonusesAsync_Returns_PagedResult()
        {
            var page = 1;
            var size = 10;

            _mockRepository.Setup(r => r.GetAllAsync(page, size))
                .ReturnsAsync((
                [
                new PlayerBonus {
                    Id = Guid.NewGuid(),
                    PlayerId = Guid.NewGuid(),
                    BonusType = BonusType.Welcome }
                ], 1));

            var result = await _service.GetAllBonusesAsync(page, size);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task CreateBonusAsync_Throws_WhenPlayerHasActiveBonusOfSameType()
        {
            var playerId = Guid.NewGuid();

            _mockRepository.Setup(r =>
                r.GetActiveByPlayerAndTypeAsync(playerId, BonusType.Welcome))
                .ReturnsAsync(new PlayerBonus
                {
                    Id = Guid.NewGuid(),
                    PlayerId = playerId,
                    BonusType = BonusType.Welcome,
                    IsActive = true
                });

            var req = new CreateBonusDto
            {
                PlayerId = playerId,
                BonusType = "Welcome",
                Amount = 50
            };

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _service.CreateBonusAsync(req, "operator"));

            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<PlayerBonus>()), Times.Never);
        }


        [Fact]
        public async Task CreateBonusAsync_Successfully_Creates_Bonus()
        {
            var playerId = Guid.NewGuid();

            _mockRepository.Setup(r =>
                r.GetActiveByPlayerAndTypeAsync(playerId, BonusType.Welcome))
                .ReturnsAsync((PlayerBonus)null);

            var req = new CreateBonusDto
            {
                PlayerId = playerId,
                BonusType = "Welcome",
                Amount = 100
            };

            PlayerBonus captured = null;

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<PlayerBonus>()))
                .Callback<PlayerBonus>(b => captured = b);

            var result = await _service.CreateBonusAsync(req, "admin");

            Assert.NotNull(result);
            Assert.Equal(100, result.Amount);
            Assert.Equal(BonusType.Welcome.ToString(), result.BonusType.ToString());
            Assert.Equal(playerId, result.PlayerId);
            Assert.NotNull(captured);
        }

        [Fact]
        public async Task UpdateBonusAsync_Throws_When_Activating_Bonus_That_Would_Conflict()
        {
            var bonusId = Guid.NewGuid();
            var playerId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(bonusId))
                .ReturnsAsync(new PlayerBonus
                {
                    Id = bonusId,
                    PlayerId = playerId,
                    BonusType = BonusType.Reload,
                    IsActive = false
                });

            _mockRepository.Setup(r =>
                r.GetActiveByPlayerAndTypeAsync(playerId, BonusType.Reload))
                .ReturnsAsync(new PlayerBonus
                {
                    Id = Guid.NewGuid(),
                    PlayerId = playerId,
                    BonusType = BonusType.Reload,
                    IsActive = true
                });

            var updateBonusDto = new UpdateBonusDto
            {
                Amount = 100,
                IsActive = true
            };

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _service.UpdateBonusAsync(bonusId, updateBonusDto, "op2"));
        }

        [Fact]
        public async Task UpdateBonusAsync_Updates_And_Logs()
        {
            var bonusId = Guid.NewGuid();

            var existing = new PlayerBonus
            {
                Id = bonusId,
                PlayerId = Guid.NewGuid(),
                BonusType = BonusType.Reload,
                IsActive = true,
                Amount = 10
            };

            _mockRepository.Setup(r => r.GetByIdAsync(bonusId))
                .ReturnsAsync(existing);

            _mockRepository.Setup(r =>
                r.GetActiveByPlayerAndTypeAsync(existing.PlayerId, existing.BonusType))
                .ReturnsAsync(existing);

            var req = new UpdateBonusDto
            {
                Amount = 999,
                IsActive = true
            };

            var result = await _service.UpdateBonusAsync(bonusId, req, "admin");

            Assert.Equal(999, result.Amount);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task DeleteBonusAsync_Returns_False_When_NotFound()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PlayerBonus)null);

            var result = await _service.DeleteBonusAsync(Guid.NewGuid(), "admin");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBonusAsync_Returns_True()
        {
            var bonus = new PlayerBonus
            {
                Id = Guid.NewGuid(),
                PlayerId = Guid.NewGuid(),
                BonusType = BonusType.Cashback,
                IsActive = true
            };

            _mockRepository.Setup(r => r.GetByIdAsync(bonus.Id))
                .ReturnsAsync(bonus);

            _mockRepository.Setup(r => r.DeleteAsync(bonus.Id))
                .ReturnsAsync(true);

            var result = await _service.DeleteBonusAsync(bonus.Id, "admin");

            Assert.True(result);
        }
    }
}
