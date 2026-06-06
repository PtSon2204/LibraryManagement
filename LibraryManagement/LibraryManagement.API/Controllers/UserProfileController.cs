using System.Security.Claims;
using LibraryManagement.Business.DTOs.UserProfileDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var profile = await _userProfileService.GetUserProfileAsync(userId, role);

            return Ok(profile);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto model)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            await _userProfileService.UpdateUserProfileAsync(userId, role, model);   

            return Ok("Cập nhật thành công!");
        }
    }
}
