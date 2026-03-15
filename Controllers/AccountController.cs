using ELearningPlatform.Models;
using ELearningPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELearningPlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Student", Text = "Student" },
                new SelectListItem { Value = "Instructor", Text = "Instructor" }
            };
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Default to student if somehow an invalid role was submitted
                    var roleToAssign = model.Role == "Instructor" ? "Instructor" : "Student";
                    await _userManager.AddToRoleAsync(user, roleToAssign);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Student", Text = "Student" },
                new SelectListItem { Value = "Instructor", Text = "Instructor" }
            };
            ViewBag.Roles = roles;
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, bool firebaseApproved = false, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();

                // 1. Handle Firebase-approved users (Auto-sync with local DB)
                if (firebaseApproved)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        // Create a local identity for the external Firebase user
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = "Learner",
                            LastName = "User",
                            EmailConfirmed = true
                        };
                        
                        // We use the provided password. If it fails, we fall back to a random one 
                        // as they are already authenticated via Firebase.
                        var createResult = await _userManager.CreateAsync(user, model.Password);
                        if (!createResult.Succeeded)
                        {
                            // If creation failed (likely password complexity), try with a random secure password
                            createResult = await _userManager.CreateAsync(user, Guid.NewGuid().ToString() + "A1!");
                        }

                        if (createResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "Student");
                        }
                    }

                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                }

                // 2. Standard login check (for seeded accounts and synced users)
                var userFound = await _userManager.FindByEmailAsync(email);
                if (userFound == null)
                {
                    ModelState.AddModelError(string.Empty, "Account not found. Please register first.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Login not allowed. Please confirm your email.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid password. Please try again.");
                }
            }
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
