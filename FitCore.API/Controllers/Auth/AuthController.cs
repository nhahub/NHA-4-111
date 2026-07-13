using FitCore.BLL.Interfaces.Auth;
using FitCore.Shared.Enums;
using FitCore.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);
            return Ok(result);
        }

        
        [HttpPost("register-member")]
        //[Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> RegisterMember(RegisterMemberDto dto)
        {
            var result = await _authService.RegisterMember(dto);
            return Ok(result);
        }

       
        [HttpPost("create-staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff(CreateStaffDto dto)
        {
            var result = await _authService.CreateStaff(dto);
            return Ok(result);
        }

        
        [HttpPut("promote-to-trainer/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToTrainer(int userId)
        {
            await _authService.PromoteMemberToTrainer(userId);
            return Ok(new SimpleMessageDto { Message = "User promoted to Trainer successfully." });
        }

        
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsers();
            return Ok(result);
        }
    }
}
