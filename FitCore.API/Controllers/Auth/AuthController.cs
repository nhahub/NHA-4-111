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
        /// <summary>
        /// تسجيل الدخول. متاح للجميع (Member/Trainer/Receptionist/Admin) — نفس الفورم لأي Role.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء حساب Member جديد من الاستقبال. بينفذها الـ Receptionist أو الـ Admin بس.
        /// مفيش Signup عام مفتوح للجمهور.
        /// </summary>
        [HttpPost("register-member")]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> RegisterMember(RegisterMemberDto dto)
        {
            var result = await _authService.RegisterMember(dto);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء حساب Staff (Trainer/Receptionist). بينفذها الـ Admin بس من لوحة التحكم.
        /// </summary>
        [HttpPost("create-staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff(CreateStaffDto dto)
        {
            var result = await _authService.CreateStaff(dto);
            return Ok(result);
        }

        /// <summary>
        /// ترقية Member لـ Trainer. بينفذها الـ Admin بس من صفحة إدارة المستخدمين.
        /// </summary>
        [HttpPut("promote-to-trainer/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToTrainer(int userId)
        {
            await _authService.PromoteMemberToTrainer(userId);
            return Ok(new SimpleMessageDto { Message = "User promoted to Trainer successfully." });
        }

        /// <summary>
        /// عرض كل المستخدمين لصفحة إدارة المستخدمين. بينفذها الـ Admin بس.
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsers();
            return Ok(result);
        }
    }
}
