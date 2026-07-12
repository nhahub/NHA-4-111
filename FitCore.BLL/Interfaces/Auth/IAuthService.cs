using FitCore.Shared.DTOs.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitCore.BLL.Interfaces.Auth
{
    public interface IAuthService
    {
        /// <summary>تسجيل الدخول. بيشتغل لأي Role (Member/Trainer/Receptionist/Admin).</summary>
        Task<AuthResponseDto> Login(LoginDto loginDto);

        /// <summary>بيعمل حساب Member جديد. بينفذها Receptionist أو Admin بس (Authorize على الـ Controller).</summary>
        Task<AuthResponseDto> RegisterMember(RegisterMemberDto dto);

        /// <summary>بيعمل حساب Staff (Trainer/Receptionist). بينفذها Admin بس.</summary>
        Task<AuthResponseDto> CreateStaff(CreateStaffDto dto);

        /// <summary>بيرقّي عضو (Member) لمدرب (Trainer). بينفذها Admin بس.</summary>
        Task PromoteMemberToTrainer(int userId);

        /// <summary>عرض كل المستخدمين لصفحة إدارة المستخدمين (Admin بس).</summary>
        Task<List<ManageUserDto>> GetAllUsers();
    }
}
