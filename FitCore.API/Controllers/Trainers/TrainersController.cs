using FitCore.BLL.Interfaces.Trainers;
using FitCore.Shared.DTOs.Trainers;
using FitCore.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.Trainers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainersController(ITrainerService trainerService) : ControllerBase
    {
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaff(CreateStaffDto dto)
        {
            var result = await trainerService.CreateStaffAsync(dto);
            return CreatedAtAction(nameof(GetTrainerById), new { trainerId = result.TrainerID }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrainers([FromQuery(Name = "Page_Size")] int pageSize = 20, [FromQuery(Name = "Page")] int page = 1)
        {
            var result = await trainerService.GetAllTrainersAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{trainerId}")]
        public async Task<IActionResult> GetTrainerById(int trainerId)
        {
            var result = await trainerService.GetTrainerByIdAsync(trainerId);
            return Ok(result);
        }

        [HttpPut("{trainerId}/working-hours")]
        public async Task<IActionResult> SetWorkingHours(int trainerId, SetWorkingHoursDto dto)
        {
            var result = await trainerService.SetWorkingHoursAsync(trainerId, dto);
            return Ok(result);
        }

        [HttpGet("{trainerId}/working-hours")]
        public async Task<IActionResult> GetWorkingHours(int trainerId)
        {
            var result = await trainerService.GetWorkingHoursAsync(trainerId);
            return Ok(result);
        }

        [HttpPut("{trainerId}/assign-class/{classId}")]
        public async Task<IActionResult> AssignTrainerToClass(int trainerId, int classId)
        {
            var result = await trainerService.AssignTrainerToClassAsync(classId, trainerId);
            if (!result) return BadRequest();

            return Ok(new { Message = "Trainer assigned to class." });
        }
    }
}
