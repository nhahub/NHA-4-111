using FitCore.BLL.Interfaces;
using FitCore.BLL.Interfaces.Membership;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.MemberShip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly FitCoreDbContext _dbContext;

        public MembershipsController(IMembershipService membershipService, FitCoreDbContext dbContext)
        {
            _membershipService = membershipService;
            _dbContext = dbContext;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User token is invalid or missing.");

            return int.Parse(userIdClaim);
        }

        private async Task<int> GetMemberProfileIdAsync(int userId)
        {
            var profile = await _dbContext.Set<MemberProfile>()
                .FirstOrDefaultAsync(m => m.UserID == userId);

            if (profile == null)
                throw new KeyNotFoundException("Member profile not found for this user.");

            return profile.MemberProfileId;
        }

        [HttpGet("my-memberships")]
        public async Task<IActionResult> GetMyMemberships()
        {
            try
            {
                //int userId = GetUserIdFromToken();
                int userId = 2;
                //int profileId = await GetMemberProfileIdAsync(userId);
                var memberships = await _membershipService.GetMemberMembershipsAsync(userId);

                return Ok(memberships);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            try
            {
                var membership = await _membershipService.GetMembershipByIdAsync(id);
                return Ok(membership);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/freeze")]
        public async Task<IActionResult> FreezeMembership(int id, [FromBody] int freezeDays)
        {
            try
            {
                var success = await _membershipService.FreezeMembershipAsync(id, freezeDays);

                if (success)
                    return Ok(new { message = $"Membership frozen successfully for {freezeDays} days." });

                return BadRequest(new { message = "Failed to freeze membership." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/unfreeze")]
        public async Task<IActionResult> UnfreezeMembership(int id)
        {
            try
            {
                var success = await _membershipService.UnfreezeMembershipAsync(id);

                if (success)
                    return Ok(new { message = "Membership unfrozen successfully." });

                return BadRequest(new { message = "Failed to unfreeze membership." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin/active-memberships")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActiveMemberships([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _membershipService.GetAllActiveMembershipsForAdminAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}