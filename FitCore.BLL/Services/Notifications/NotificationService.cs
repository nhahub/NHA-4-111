using FitCore.BLL.Interfaces.Notifications;
using FitCore.DAL.Data.Models;
using FitCore.DAL.Interfaces;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Notification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
