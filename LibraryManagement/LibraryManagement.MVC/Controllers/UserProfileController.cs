using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.UserProfiles;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.MVC.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model =
                await _userProfileService.GetProfile();

            if (model == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
       UpdateUserProfileVm model)
        {
            var success =
                await _userProfileService.UpdateProfile(
                    model);

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "Cập nhật thất bại");

                var profileModel = await _userProfileService.GetProfile();
                return View("Index", profileModel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
