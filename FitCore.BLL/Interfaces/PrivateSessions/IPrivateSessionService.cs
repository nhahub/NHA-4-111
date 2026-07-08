using FitCore.Shared.DTOs.PrivateSessions;

namespace FitCore.BLL.Interfaces.PrivateSessions
{
    public interface IPrivateSessionService
    {
        Task<PrivateSessionDto> CreatePrivateSessionAsync(CreatePrivateSessionDto dto);
        Task<PrivateSessionDto> AssignTrainerAsync(int privateSessionId, int trainerId);
        Task<ICollection<PrivateSessionDto>> GetSessionsByTrainerAsync(int trainerId);
        Task<ICollection<PrivateSessionDto>> GetSessionsByMemberAsync(int memberUserId);
        Task<bool> CancelSessionAsync(int privateSessionId);
        Task<bool> CompleteSessionAsync(int privateSessionId);
    }
}
