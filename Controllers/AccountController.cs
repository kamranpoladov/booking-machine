using System;
using System.Linq;
using System.Threading.Tasks;
using BookingMachine.Auth;
using BookingMachine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingMachine.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);
            
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Username or password is wrong");
            return View(loginViewModel);
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
                return View(registrationViewModel);

            var user = new ApplicationUser
            {
                UserName = registrationViewModel.UserName
            };
            var result = await _userManager.CreateAsync(user, registrationViewModel.Password);
            if (result.Succeeded)
            {
                Console.WriteLine("=== NEW USER ===");
                Console.WriteLine("Username: {0}\nPassword: {1}", registrationViewModel.UserName, registrationViewModel.Password);
                return RedirectToAction("Login", "Account");
            }

            var message = string.Join(", ", result.Errors.Select(x => "Error: " + x.Description));
            ModelState.AddModelError("", message);
            return View(registrationViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}