using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.Auth;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs.Auth;
using FitCore.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitCore.BLL.Services.Auth
{
    public class AuthService(
        FitCoreDbContext dbContext,
        IJwtTokenGenerator _jwtTokenGenerator,
        IPasswordHasher<User> _passwordHasher) : IAuthService
    {
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ValidationException(new List<string> { "Email and password are required." });
            }

            var user = await dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && !u.IsDeleted);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Not user found.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            Console.WriteLine("_______________________________________");
            Console.WriteLine(user.PasswordHash);
            Console.WriteLine("_______________________________________");
            Console.WriteLine(verifyResult);
            Console.WriteLine("_______________________________________");
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (user.Status == UserStatus.Blocked || user.Status == UserStatus.Suspended)
            {
                throw new BusinessRuleException("This account is blocked/suspended. Please contact the gym administration.");
            }

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> RegisterMember(RegisterMemberDto dto)
        {
            var errors = ValidateBasicInfo(dto.FullName, dto.Email, dto.PhoneNumber, dto.Password);
            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            if (await dbContext.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            {
                throw new BusinessRuleException("An account with this email already exists.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Status = UserStatus.Active,
                JoinDate = DateTime.UtcNow,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            user.UserRoles.Add(new UserRole { Role = UserRoles.Member });
            user.MemberProfile = new MemberProfile { QRCodeData = Guid.NewGuid().ToString("N") };
            user.Cart = new Cart();

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> CreateStaff(CreateStaffDto dto)
        {
            //if (dto.Role != UserRoles.Trainer && dto.Role != UserRoles.Receptionist)
            //{
            //    throw new BusinessRuleException("Staff accounts can only be created with the Trainer or Receptionist role.");
            //}

            var errors = ValidateBasicInfo(dto.FullName, dto.Email, dto.PhoneNumber, dto.Password);
            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            if (await dbContext.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            {
                throw new BusinessRuleException("An account with this email already exists.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Status = UserStatus.Active,
                JoinDate = DateTime.UtcNow,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            user.UserRoles.Add(new UserRole { Role = dto.Role });

            if (dto.Role == UserRoles.Trainer)
            {
                user.Trainer = new Trainer
                {

                    Specialization = "N/A",
                    Bio = "N/A",
                    WorkingHours = "N/A",
                };
            }

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task PromoteMemberToTrainer(int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.UserRoles)
                .Include(u => u.Trainer)
                .FirstOrDefaultAsync(u => u.UserID == userId && !u.IsDeleted);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            bool isMember = user.UserRoles.Any(r => r.Role == UserRoles.Member && !r.IsDeleted);
            if (!isMember)
            {
                throw new BusinessRuleException("Only Member accounts can be promoted to Trainer.");
            }

            bool isAlreadyTrainer = user.UserRoles.Any(r => r.Role == UserRoles.Trainer && !r.IsDeleted);
            if (isAlreadyTrainer)
            {
                throw new BusinessRuleException("This user is already a Trainer.");
            }

            user.UserRoles.Add(new UserRole { Role = UserRoles.Trainer });

            if (user.Trainer == null)
            {
                user.Trainer = new Trainer
                {
                    Specialization = "N/A",
                    Bio = "N/A",
                    WorkingHours = "N/A",
                };
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<ManageUserDto>> GetAllUsers()
        {
            var users = await dbContext.Users
                .Include(u => u.UserRoles)
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.JoinDate)
                .ToListAsync();

            return users.Select(u => new ManageUserDto
            {
                UserID = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                JoinDate = u.JoinDate,
                Roles = u.UserRoles.Where(r => !r.IsDeleted).Select(r => r.Role.ToString()).ToList(),
            }).ToList();
        }

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------
        private static List<string> ValidateBasicInfo(string fullName, string email, string phoneNumber, string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(fullName))
                errors.Add("Full name is required.");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                errors.Add("A valid email is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                errors.Add("Phone number is required.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                errors.Add("Password must be at least 6 characters.");

            return errors;
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var roleNames = user.UserRoles.Where(r => !r.IsDeleted).Select(r => r.Role.ToString()).ToList();
            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roleNames);

            return new AuthResponseDto
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roleNames,
                Token = token,
                ExpiresAt = expiresAt,
            };
        }
    }
}
