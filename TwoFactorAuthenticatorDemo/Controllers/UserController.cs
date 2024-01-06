using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TwoFactorAuthenticatorDemo.DataContext;
using TwoFactorAuthenticatorDemo.Models;
using TwoFactorAuthenticatorDemo.Models.DTO;
using TwoFactorAuthenticatorDemo.Utility;

namespace TwoFactorAuthenticatorDemo.Controllers
{
    [Authorize]
    [TwoFactorAuthorization]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly TwoFactorAuthDbContext _dbContext;

        public UserController(IConfiguration configuration, UserManager<User> userManager, TwoFactorAuthDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _userManager = userManager;
        }


        // GET: /User
        public IActionResult Index()
        {
            var users = _dbContext.Users.ToList();
            return View(users);
        }

        // GET: /User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        public async Task<IActionResult> Create(NewUserViewModel newUser)
        {
            try
            {
                var key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10); // Generate a random key
                var base32Bytes = Encoding.UTF8.GetBytes(key);
                string secretKey = Base32Encoding.ToString(base32Bytes);

                var user = new User
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    TwoFactorEnabled = newUser.TwoFactorEnabled,
                    TwoFactorSecretKey = secretKey,
                };
                var password = newUser.Password;
                var existingEmail = await _userManager.FindByEmailAsync(user.Email);
                if (existingEmail != null)
                {
                    ViewData["Message"] = string.Format(Messages.EXISTING_EMAIL, user.Email);
                    return View(newUser);
                }

                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    ViewData["Message"] = string.Format(Messages.EXISTING_USERNAME, user.UserName);
                    return View(newUser);
                }
                var createUser = await _userManager.CreateAsync(user, password);

                if (!createUser.Succeeded)
                {
                    ViewData["Message"] = createUser.Errors;
                    return View(newUser);
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CONTACT_ADMIN;
                return View(newUser);
            }
        }

        // GET: /User/Edit/5
        public IActionResult Edit(string id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /User/Edit/5
        [HttpPost]
        public IActionResult Edit(string id, UpdateUserViewModel updatedUser)
        {
            try
            {
                var user = _dbContext.Users.Find(id);
                if (user == null)
                {
                    return RedirectToAction(nameof(Edit), id);
                }

                if (ModelState.IsValid)
                {
                    user.UserName = updatedUser?.UserName;
                    user.NormalizedUserName = updatedUser?.UserName?.ToUpper();
                    user.Email = updatedUser?.Email;
                    user.NormalizedEmail = updatedUser?.Email?.ToUpper();
                    _dbContext.Users.Update(user);
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CONTACT_ADMIN;
            }

            return View(updatedUser);
        }

        // GET: /User/Delete/5
        public IActionResult Delete(string id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /User/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                var user = _dbContext.Users.Find(id);

                if (user == null)
                {
                    ViewData["Message"] = Messages.ACCESS_DENIED;
                    return RedirectToAction(nameof(Delete), id);
                }

                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CONTACT_ADMIN;
                return RedirectToAction(nameof(Delete), id);
            }
        }

        // Enable 2FA for a user
        public IActionResult EnableTwoFactorAuth(string userId)
        {
            try
            {
                var user = _dbContext.Users.Find(userId);
                if (user != null)
                {
                    var twoFactorAuthenticator = new TwoFactorAuthenticator();
                    var setupInfo = twoFactorAuthenticator.GenerateSetupCode("TwoFactorAuthDemoApp", user.UserName, user.TwoFactorSecretKey, true, 300);

                    ViewData["QRCodeSetup"] = setupInfo;

                    user.TwoFactorEnabled = true;
                    _dbContext.Users.Update(user);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                ViewData["Message"] = Messages.CONTACT_ADMIN;
            }

            return View();
        }

    }
}
