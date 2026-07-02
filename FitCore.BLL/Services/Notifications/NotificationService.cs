using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.Notifications;
using FitCore.DAL.Data.Models;
using FitCore.DAL.Interfaces;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Notification;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitCore.BLL.Services.Notifications
{
    public class NotificationService(IUnitOfWork _unitOfWork) : INotificationService
    {
        public async Task<PaginationResponseDto<NotificationDto>> GetAllNotifications(int page, int pageSize)
        {
            int userId = 1;
            //int userId = _currentService.UserId ?? throw new UnauthorizedAccessException("No user id assigned");
            if (page <= 0) page = 1;

            const int maxPageSize = 20;

            if (pageSize > maxPageSize) pageSize = maxPageSize;

            var query = _unitOfWork.GetRepository<Notification>().GetAllAsIQueryable()
                .OrderByDescending(x => x.CreatedAt).Where(x => x.UserID == userId);

            var rowsCount = query.Count();

            var messages = query.Skip((page - 1) * pageSize)
                .Take(pageSize);

            var messageDtos = await messages.Select(x => new NotificationDto
            {
                Id = x.NotificationID,
                Title = x.Title,
                IsRead = x.IsRead,
                CreatedAt = x.CreatedAt,
                Message = x.Content,
                Type = x.Type,
            }).ToListAsync();


            return new PaginationResponseDto<NotificationDto>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = rowsCount,
                Data = messageDtos
            };
        }
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            //int userId = _currentService.UserId ?? throw new UnauthorizedAccessException("No user id assigned");
            int userId = 1;

            var notification = await _unitOfWork.GetRepository<Notification>().GetByIdAsync(notificationId);


            if (notification == null || notification.UserID != userId) throw new KeyNotFoundException("no notification with this id");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _unitOfWork.GetRepository<Notification>().Update(notification);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task MarkAllAsReadAsync()
        {
            int userId = 1;
            //int userId = _currentService.UserId ?? throw new UnauthorizedAccessException("No branch id assigned");

            var unreadNotifications = await _unitOfWork.GetRepository<Notification>()
                .GetAllAsIQueryable()
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;                    
                }
                _unitOfWork.GetRepository<Notification>().UpdateRange(unreadNotifications);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SendNotification(RequestNotificationDto notificationDto)
        {
            int userId = 3;
            //int userId = _currentService.UserId ?? throw new UnauthorizedAccessException("No user id assigned");
            
            var SentUserRoles =await _unitOfWork.GetRepository<User>().GetByIdAsIQueryable(userId)
                .Select(x=> x.UserRoles)
                .FirstOrDefaultAsync();
            
            if (SentUserRoles == null)
            {
                return false;
            }
            
            foreach (var role in SentUserRoles)
            {
                if (role.Role == UserRoles.Member)
                {
                    throw new BusinessRuleException("Member can't push notifications");
                }
            }

            if (notificationDto == null)
            {
                throw new ArgumentNullException("Notification fields are empty, please fill required fields");
            }

            var users = _unitOfWork.GetRepository<User>().GetAllAsIQueryable().Include(x=> x.UserRoles);

            List<Notification> notifications = new List<Notification>();
            
            foreach (var user in users)
            { 
                foreach(var UserRole in user.UserRoles)
                {
                    foreach (var role in notificationDto.RecieveUserRoles)
                    {
                        if (role == UserRole.Role)
                        {                           
                            Notification notification = new Notification()
                            {
                                CreatedAt = DateTime.UtcNow,
                                Content = notificationDto.Message,
                                Title = notificationDto.Title,
                                IsRead = false,
                                Type = NotificationTypeEnum.Announcement,
                                UserID = user.UserID,
                            };
                            notifications.Add(notification);
                        }
                    }
                }
            }
            await _unitOfWork.GetRepository<Notification>().AddRangeAsync(notifications);
            int affectedRows= await _unitOfWork.SaveChangesAsync();    

            if (affectedRows <= 0)
            {
                return false;
            }
            return true;
            
        }

        public async Task MemberExpiryNotification()
        {
            var memberShips = await _unitOfWork.GetRepository<Membership>()
                .GetAllAsIQueryable()
                .ToListAsync();

            foreach (var membership in memberShips)
            {
                if(membership.EndDate <= DateTime.UtcNow)
                {
                    //Notification notification = new Notification()
                    //{
                    //    CreatedAt = DateTime.UtcNow,
                    //    Content = "Your MemberShip",
                    //    Title = notificationDto.Title,
                    //    IsRead = false,
                    //    Type = NotificationTypeEnum.Announcement,
                    //    UserID = user.UserID,
                    //};
                }
            }

        }
    }
}
