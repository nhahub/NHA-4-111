using FitCore.BLL.Interfaces.Profile;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IProfileService _profileService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _profileService.GetProfile();
            return Ok(result);
        }
    }
}
