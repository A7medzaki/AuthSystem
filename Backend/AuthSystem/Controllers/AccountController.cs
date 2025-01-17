using AuthSystem.Helper;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region SignUp Controller
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email.Split("@")[0],
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsAgree = model.IsAgree
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                    return RedirectToAction("LogIn");

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        #endregion


        #region Login Controller
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Traditional Login
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Incorrect Email or Password");
                    return View(model);
                }
            }
            return View(model);
        }

        // Handle Google & Facebook Callback
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in if the user already has a login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            // If the user does not have an account, create one
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser { UserName = email?.Split("@")[0], Email = email };

            var createUserResult = await _userManager.CreateAsync(user);
            if (createUserResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                //return LocalRedirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in createUserResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(LogIn));
        }

        // Google Login Redirect
        [HttpGet]
        public IActionResult LoginWithGoogle(string returnUrl = "/Home/Index")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, Url.Action(nameof(ExternalLoginCallback), new { returnUrl }));
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Facebook Login Redirect
        [HttpGet]
        public IActionResult LoginWithFacebook(string returnUrl = "/Home/Index")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                "Facebook",
                Url.Action(nameof(ExternalLoginCallback), new { returnUrl })
            );
            return Challenge(properties, "Facebook");
        }

        #endregion


        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new
                    { Email = model.Email, Token = token }, Request.Scheme);

                    var email = new Email
                    {
                        Body = url,
                        Subject = "Reset Password",
                        To = model.Email
                    };

                    EmailSettings.SendEmail(email);

                    return RedirectToAction(nameof(CheckYourInbox));
                }
            }

            return View(model);
        }

        public IActionResult CheckYourInbox()
        {
            return View();
        }

        public IActionResult ResetPassword(string Email, string Token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);

                if (user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(LogIn));
                    }

                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(input);
        }
    }

}
