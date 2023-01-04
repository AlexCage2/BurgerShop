using BurgerShop.Data;
using BurgerShop.Models.AuthenticationRequests;
using BurgerShop.Models.DataModels.Users;
using BurgerShop.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BurgerShop.Controllers
{
    [Route("auth")]
    public sealed class AuthenticationController : Controller
    {
        private IClaimsPrincipalGenerator _claimsPrincipalGenerator;
        private IPasswordHandler _passwordHandler;
        private UserContext _userContext;

        public AuthenticationController(
            IClaimsPrincipalGenerator claimsPrincipalGenerator, 
            IPasswordHandler passwordHandler,
            UserContext userContext)
        {
            _claimsPrincipalGenerator = claimsPrincipalGenerator;
            _passwordHandler = passwordHandler;
            _userContext = userContext;
        }

        // GET: Login
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken cancelationToken)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return View(nameof(Login), loginRequest);
            }

            // Generate hash and salt for password
            _passwordHandler.TryCreatePasswordHash(loginRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Get user
            User user = await _userContext.GetUserAsync(loginRequest.Login, cancelationToken);

            // Check if user exists
            if (user is null)
            {
                ModelState.AddModelError("IncorrectLogin", "Логин не существует, проверьте корректность написания");
                return View(nameof(Login), loginRequest);
            }

            // Check if password correct
            if (!_passwordHandler.VerifyPasswordHash(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("IncorrectPassword", "Неверный пароль");
                return View(nameof(Login), loginRequest);
            }

            // Authentication
            string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            ClaimsPrincipal claimsPrincipal = _claimsPrincipalGenerator.GenerateClaimsPrincipal(user.FirstName, user.LastName);

            // Authorization
            await HttpContext.SignInAsync(scheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        // GET: Registration
        [HttpGet("registration")]
        public IActionResult Registration()
        {
            return View();
        }

        // POST: Registration
        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationRequest registrationRequest) 
        {
            // Create user (Mapping)
            User user = (User)registrationRequest;

            // Check login in db
            if (await _userContext.IsUserExistsAsync(user.Login))
            {
                ModelState.AddModelError("Login exists", $"Логин {registrationRequest.Login} занят, используйте другой логин");
                return View(nameof(Registration), registrationRequest);
            }

            // Validation
            if (!ModelState.IsValid)
            {
                return View(nameof(Registration), registrationRequest);
            }

            // Generate hash and salt for password
            _passwordHandler.TryCreatePasswordHash(registrationRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Try add user in db
            if (!await _userContext.TryCreateUserAsync(user))
            {
                // If can't add user
                ModelState.AddModelError("Unknown", "Не удалось добавить пользователя");
                return View(nameof(Registration), registrationRequest);
            }

            // Authentication
            string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            ClaimsPrincipal claimsPrincipal = _claimsPrincipalGenerator.GenerateClaimsPrincipal(user.FirstName, user.LastName);

            // Authorization
            await HttpContext.SignInAsync(scheme, claimsPrincipal);

            // If OK
            return RedirectToAction("Index", "Home");
        }

        // GET: Logout
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            await HttpContext.SignOutAsync(scheme);
            return RedirectToAction("Login");
        }
    }
}
