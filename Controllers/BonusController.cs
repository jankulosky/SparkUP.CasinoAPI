using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SparkUP.CasinoAPI.DTOs;
using SparkUP.CasinoAPI.Models;
using SparkUP.CasinoAPI.Services.Interfaces;
using System.Security.Claims;

namespace SparkUP.CasinoAPI.Controllers
{
    [ApiController]
    [Route("api/bonus")]
    [Authorize]
    public class BonusController : ControllerBase
    {
        private readonly IBonusService _bonusService;

        public BonusController(IBonusService bonusService)
        {
            _bonusService = bonusService;
        }

        private string GetOperatorName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<PagedResult<PlayerBonusDto>>> GetAllBonuses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _bonusService.GetAllBonusesAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<PlayerBonusDto>> CreateBonus([FromBody] CreateBonusDto createDto)
        {
            try
            {
                var operatorName = GetOperatorName();
                var bonus = await _bonusService.CreateBonusAsync(createDto, operatorName);
                return CreatedAtAction(nameof(GetAllBonuses), new { id = bonus.Id }, bonus);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<PlayerBonusDto>> UpdateBonus(Guid id, [FromBody] UpdateBonusDto updateDto)
        {
            try
            {
                var operatorName = GetOperatorName();
                var bonus = await _bonusService.UpdateBonusAsync(id, updateDto, operatorName);
                return Ok(bonus);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult> DeleteBonus(Guid id)
        {
            try
            {
                var operatorName = GetOperatorName();
                var result = await _bonusService.DeleteBonusAsync(id, operatorName);

                if (!result)
                {
                    return NotFound(new { message = $"Bonus with ID {id} not found." });
                }

                return Ok(new { message = "Bonus successfully deactivated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
