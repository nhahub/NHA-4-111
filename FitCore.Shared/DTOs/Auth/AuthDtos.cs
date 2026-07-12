using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;

namespace FitCore.Shared.DTOs.Auth
{
    /// <summary>
    /// فورم تسجيل الدخول. نفس الفورم يخدم Member / Staff (Trainer, Receptionist) / Admin،
    /// والـ Backend هو اللي بيقرر أدوار المستخدم من الداتابيز مش من اختيار الفرونت.
    /// </summary>
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// الرد اللي بيرجع بعد Login أو إنشاء حساب ناجح: JWT Token + بيانات مبسطة عن المستخدم وأدواره.
    /// </summary>
    public class AuthResponseDto
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// مفيش صفحة Signup عامة مفتوحة للجمهور. الحساب ده بيتعمل بواسطة الـ Receptionist (أو الـ Admin)
    /// بعد ما يعمل تسجيل دخول، وبيبقى Role المستخدم دايمًا Member.
    /// </summary>
    public class RegisterMemberDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// حسابات الـ Staff (Trainer / Receptionist) بتتعمل بواسطة الـ Admin بس من لوحة التحكم.
    /// اختيار "Admin" ممنوع هنا، الـ Service بيرفضه بـ BusinessRuleException.
    /// </summary>
    public class CreateStaffDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRoles Role { get; set; }
    }

    /// <summary>
    /// عرض مبسّط للمستخدمين في صفحة "إدارة المستخدمين" الخاصة بالـ Admin.
    /// </summary>
    public class ManageUserDto
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public DateTime JoinDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class SimpleMessageDto
    {
        public string Message { get; set; } = string.Empty;
    }
}
