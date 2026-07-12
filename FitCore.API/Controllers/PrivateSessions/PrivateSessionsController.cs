using FitCore.BLL.Interfaces.PrivateSessions;
using FitCore.Shared.DTOs.PrivateSessions;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.PrivateSessions
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivateSessionsController(IPrivateSessionService privateSessionService) : ControllerBase
    {
        //[Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        public async Task<IActionResult> CreatePrivateSession(CreatePrivateSessionDto dto)
        {
            var result = await privateSessionService.CreatePrivateSessionAsync(dto);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{privateSessionId}/assign-trainer/{trainerId}")]
        public async Task<IActionResult> AssignTrainer(int privateSessionId, int trainerId)
        {
            var result = await privateSessionService.AssignTrainerAsync(privateSessionId, trainerId);
            return Ok(result);
        }

        [HttpGet("trainer/{trainerId}")]
        public async Task<IActionResult> GetSessionsByTrainer(int trainerId)
        {
            var result = await privateSessionService.GetSessionsByTrainerAsync(trainerId);
            return Ok(result);
        }

        [HttpGet("member/{memberUserId}")]
        public async Task<IActionResult> GetSessionsByMember(int memberUserId)
        {
            var result = await privateSessionService.GetSessionsByMemberAsync(memberUserId);
            return Ok(result);
        }

        [HttpPatch("{privateSessionId}/cancel")]
        public async Task<IActionResult> CancelSession(int privateSessionId)
        {
            var result = await privateSessionService.CancelSessionAsync(privateSessionId);
            if (!result) return BadRequest();

            return Ok(new { Message = "Private session cancelled." });
        }

        [HttpPatch("{privateSessionId}/complete")]
        public async Task<IActionResult> CompleteSession(int privateSessionId)
        {
            var result = await privateSessionService.CompleteSessionAsync(privateSessionId);
            if (!result) return BadRequest();

            return Ok(new { Message = "Private session marked as completed." });
        }
    }
}
