using BookStore.Infrastructure.Identity;
using BookStore.Web.Areas.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace BookStore.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email.Split('@')[0],
                    FullName = model.Email.Split('@')[0],
                    Email = model.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow, 
                    Status = "Active"           
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Email này đã được đăng ký.");
                return View("Register");
            }

            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();
            _memoryCache.Set(email, verificationCode, TimeSpan.FromMinutes(10));

            await _emailSender.SendEmailAsync(email, "Xác thực email", $"Mã xác thực của bạn là: {verificationCode}");
            TempData["SendSuccess"] = "Mã xác thực đã được gửi đến email của bạn.";
            return View("Register");
        }

        [HttpPost]
        public IActionResult VerifyCode(string email, string verificationCode)
        {
            if (_memoryCache.TryGetValue(email, out string? cachedCode) && cachedCode == verificationCode)
            {
                TempData["VerifySuccess"] = "Xác thực thành công.";
            }
            else
            {
                TempData["VerifyFail"] = "Mã xác thực không đúng.";
            }

            return View("Register");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        else if (roles.Contains("Customer"))
                        {
                            return LocalRedirect(returnUrl ?? Url.Content("~/"));
                        }
                    }
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau.");
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, "Tên tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại.");
            }

            return View(model);
        }


        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? error = null)
        {
            if (error != null)
                return RedirectToAction("Login");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false
            );

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            // Create new user
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (email == null)
                return RedirectToAction("Login");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var err in createResult.Errors)
                    ModelState.AddModelError("", err.Description);

                return View("Login");
            }

            await _userManager.AddLoginAsync(user, info);
            await _userManager.AddToRoleAsync(user, "Customer");
            await _signInManager.SignInAsync(user, false);

            return LocalRedirect(returnUrl ?? "/");
        }



        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
