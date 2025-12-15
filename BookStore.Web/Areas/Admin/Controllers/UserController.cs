using BookStore.Infrastructure.Identity;
using BookStore.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var lockoutStatus = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                    ? "Đang bị khóa"
                    : "Hoạt động";

                userList.Add(new UserViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = string.Join(", ", roles),
                    LockoutStatus = lockoutStatus
                });
            }

            return View(userList);
        }

        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList(); var model = new EditRolesViewModel { UserId = user.Id, UserName = user.UserName, Roles = allRoles.Select(role => new RoleViewModel { RoleName = role.Name, IsSelected = userRoles.Contains(role.Name) }).ToList() }; return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRolesViewModel model, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return View(model);
            }

            if (!string.IsNullOrEmpty(selectedRole))
            {
                var addResult = await _userManager.AddToRoleAsync(user, selectedRole);
                //await _userManager.RefreshSignInAsync(user);

                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add user role");
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageLockout(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                user.LockoutEnd = null;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Tài khoản đã được mở khóa thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi mở khóa tài khoản!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Tài khoản này không bị khóa hoặc không tồn tại!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var lockoutEnd = DateTime.UtcNow.AddHours(24);
            user.LockoutEnd = lockoutEnd;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tài khoản đã được khóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi khóa tài khoản!";
            }

            return RedirectToAction("Index");
        }


    }
}
