using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwoFactorAuthenticatorDemo.DataContext;
using TwoFactorAuthenticatorDemo.Models;
using TwoFactorAuthenticatorDemo.Models.DTO;
using TwoFactorAuthenticatorDemo.Utility;

namespace TwoFactorAuthenticatorDemo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly TwoFactorAuthDbContext _dbContext;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            TwoFactorAuthDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewData["Message"] = Messages.INVALID_LOGIN;
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (user.TwoFactorEnabled)
                    {
                        return RedirectToAction("EnterTwoFactorCode", new { UserId = user.Id });
                    }
                }

                ViewData["Message"] = Messages.INVALID_LOGIN;
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CONTACT_ADMIN;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EnterTwoFactorCode(string userId)
        {
            return View(new TwoFactorAuthCodeViewModel { UserId = userId });
        }

        // Validate 2FA code for a user
        [HttpPost]
        public async Task<IActionResult> EnterTwoFactorCode(TwoFactorAuthCodeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    ViewData["Message"] = Messages.ACCESS_DENIED;
                    return View(nameof(EnterTwoFactorCode));
                }

                var twoFactorAuthenticator = new TwoFactorAuthenticator();
                var isCodeValid = twoFactorAuthenticator.ValidateTwoFactorPIN(user.TwoFactorSecretKey, model.Code, true);

                if (isCodeValid)
                {
                    user.TwoFactorLoginVerified = true;
                    _dbContext.Users.Update(user);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }

                ViewData["Message"] = Messages.CODE_INVALID;
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CODE_VALIDATION;
            }
            return View(nameof(EnterTwoFactorCode));
        }

        public async Task<IActionResult> Logout()
        {
            var currentUser = await _signInManager.UserManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { errorMessage = Messages.ACCESS_DENIED });
            }

            currentUser.TwoFactorLoginVerified = false; // Update the property as needed
            _dbContext.Update(currentUser);
            await _dbContext.SaveChangesAsync();

            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }

}
