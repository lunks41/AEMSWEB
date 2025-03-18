using AEMSWEB.Data;
using AEMSWEB.Extensions;
using AEMSWEB.Models;
using AEMSWEB.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<AdmUser> _signInManager;
        private readonly UserManager<AdmUser> _userManager;
        private readonly RoleManager<AdmUserGroup> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            SignInManager<AdmUser> signInManager,
            UserManager<AdmUser> userManager,
            RoleManager<AdmUserGroup> roleManager,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Invalid model state in Login request for {Email}", model.Email);
                return View(model);
            }

            try
            {
                _logger.LogInformation("Login attempt for {Email}", model.Email);
                // Normalize the username for lookup
                var normalizedUserName = model.Email.ToUpperInvariant();
                var user = await _userManager.FindByNameAsync(normalizedUserName);
                //var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive account: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Account is inactive.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        AllowRefresh = true
                    };

                    await _signInManager.SignInAsync(user, authProperties);
                    _logger.LogInformation("Successful login for {Email}", model.Email);

                    //Get all Companies
                    //var companies = await _context.AdmCompany.ToListAsync();
                    //HttpContext.Session.SetObject("AvailableCompanies", companies);

                    // Get the company details from AdmCompany for those company IDs.
                    var companies = await _context.AdmCompany
                        .Where(company => _context.AdmUserRights
                            .Any(rights => rights.CompanyId == company.CompanyId && rights.UserId == user.Id))
                        .ToListAsync();

                    HttpContext.Session.SetObject("AvailableCompanies", companies);

                    return companies.Count switch
                    {
                        0 => RedirectToAction("NoCompanyAccess"),
                        1 => RedirectToAction("Index", "Dashboard", new { companyId = companies.First().CompanyId }),
                        //1 => RedirectToCompanyDashboard(companies.First().CompanyCode),
                        _ => RedirectToAction("SelectCompany")
                    };
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account locked out: {Email}", model.Email);
                    return RedirectToAction("Lockout");
                }

                _logger.LogWarning("Invalid password attempt for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                throw;
            }
        }

        [Authorize]
        public IActionResult SelectCompany()
        {
            try
            {
                var companies = HttpContext.Session.GetObject<List<AdmCompany>>("AvailableCompanies");
                return View(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SelectCompany");
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectCompany(string companyCode)
        {
            try
            {
                await SetCompanyContext(companyCode);
                return RedirectToCompanyDashboard(companyCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SelectCompany POST for companyCode: {CompanyCode}", companyCode);
                throw;
            }
        }

        private async Task SetCompanyContext(string companyCode)
        {
            _logger.LogInformation("SetCompanyContext for companyCode: {CompanyCode}", companyCode);

            var company = await _context.AdmCompany
                .FirstOrDefaultAsync(c => c.CompanyCode == companyCode);

            if (company == null)
            {
                _logger.LogError("Company not found for companyCode: {CompanyCode}", companyCode);
                throw new Exception("Company not found.");
            }

            HttpContext.Session.SetString("CurrentCompanyId", company.CompanyId.ToString());
            HttpContext.Session.SetString("CurrentCompanyCode", company.CompanyCode);
            HttpContext.Session.SetString("CurrentCompanyName", company.CompanyName);
        }

        private IActionResult RedirectToCompanyDashboard(string companyCode)
        {
            try
            {
                _logger.LogInformation("Redirecting to Company Dashboard for companyCode: {CompanyCode}", companyCode);
                return Redirect($"/{companyCode}/Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting to Company Dashboard for companyCode: {CompanyCode}", companyCode);
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCompany(string companyId)
        {
            try
            {
                var validCompanies = HttpContext.Session.GetObject<List<AdmCompany>>("AvailableCompanies");

                if (validCompanies == null || !validCompanies.Any(c => c.CompanyId == Convert.ToByte(companyId ?? "0")))
                {
                    _logger.LogWarning("Invalid companyId: {CompanyId}", companyId);
                    return Forbid();
                }

                // Set company context
                HttpContext.Session.SetString("CurrentCompany", companyId);

                // Refresh authentication: ensure the user is present
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("User is null in SetCompany; redirecting to Login.");
                    return RedirectToAction("Login", "Account");
                }

                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index", "Dashboard", new { companyId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetCompany for {CompanyId}", companyId);
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult SwitchCompany([FromForm] string companyId, [FromForm] string originalPath)
        {
            try
            {
                var companies = HttpContext.Session.GetObject<List<AdmCompany>>("AvailableCompanies");
                if (companies == null || !companies.Any(c => c.CompanyId.ToString() == companyId))
                {
                    _logger.LogWarning("Invalid company in SwitchCompany: {CompanyId}", companyId);
                    return Json(new { success = false, error = "Invalid company" });
                }

                // Update session with the new company
                HttpContext.Session.SetString("CurrentCompany", companyId);

                var pathSegments = originalPath.TrimStart('/').Split('/').ToList();
                if (pathSegments.Any())
                {
                    pathSegments[0] = companyId;
                }
                else
                {
                    pathSegments.Add(companyId);
                }

                var newPath = string.Join('/', pathSegments);
                var newUrl = $"/{newPath}";

                return Json(new { success = true, url = newUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SwitchCompany for companyId: {CompanyId}", companyId);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Register() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Invalid model state in registration for {Email}", model.Email);
                return View(model);
            }

            try
            {
                _logger.LogInformation("Starting user registration for {Email}", model.Email);
                var user = new AdmUser
                {
                    FullName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    UserCode = model.UserCode,
                    IsActive = true,
                    CreateById = 1,
                    CreateDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} registered successfully", model.Email);
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        var role = new AdmUserGroup
                        {
                            Name = "User",
                            UserGroupCode = "USER",
                            IsActive = true,
                            CreateById = 1,
                            CreateDate = DateTime.Now
                        };
                        await _roleManager.CreateAsync(role);
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Registration error for {Email}: {Error}", model.Email, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", model.Email);
                throw;
            }
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found!");
                    return View(model);
                }
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyEmail for {Email}", model.Email);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("VerifyEmail", "Account");

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found!");
                    return View(model);
                }

                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    return Redirect("/Account/Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangePassword for {Email}", model.Email);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();

            Response.Headers.Append("Cache-Control", "no-cache, no-store");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("X-Logout-Event", "true");

            return Redirect("/Account/Login");
        }
    }
}