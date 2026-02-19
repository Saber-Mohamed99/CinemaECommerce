using CinemaECommerce.Models;
using CinemaECommerce.Repositories.IRepositories;
using CinemaECommerce.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;
namespace CinemaECommerce.Areas.Identity.Controllors
{
    [Area(SD.Identity_Area)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAccountService _accountService;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPrepository;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IAccountService accountService,
            IRepository<ApplicationUserOTP> applicationUserOTPrepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _accountService = accountService;
            _applicationUserOTPrepository = applicationUserOTPrepository;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);
            ApplicationUser applicationUser = new()
            {
                FName = registerVM.FName,
                Email = registerVM.Email,
                LName = registerVM.LName,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
            };
            //var applicationUser =registerVM.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var ConfirmeLink = Url.Action("Confirm", "Account",
                new { applicationUser.Id, token }, Request.Scheme);
            string msg = $"<h1>Click <a href='{ConfirmeLink}'>here</a> to confirm your account</h1>";
            await _accountService.ReSendEmailAsync(MsgType.ConfirmationEmail, msg, applicationUser);
            TempData["notification"] = "Add account successfully";
            return View("Login");
        }
        public async Task<IActionResult> Confirm(string id, string token)
        {
            var User = await _userManager.FindByIdAsync(id);
            if (User is null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(User, token);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                TempData["error-notification"] = "Invalid confirmation link ,please try agin";
            }
            else
                TempData["notification"] = "Confirm account successfully";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);
            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ??
               await _userManager.FindByNameAsync(loginVM.EmailOrUserName);
            if (user == null)
            {
                ModelState.AddModelError("EmailOrUserName", "Invalid email or username");
                ModelState.AddModelError("Password", "Invalid password");
                return View(loginVM);
            }
            //var result=await _userManager.CheckPasswordAsync(user, loginVM.Password);
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("EmailOrUserName", "Confirme your email first");
                    return View(loginVM);

                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Please try agin later");
                    return View(loginVM);
                }

                ModelState.AddModelError("EmailOrUserName", "Invalid email or username");
                ModelState.AddModelError("Password", "Invalid password");
                return View(loginVM);
            }
            TempData["notification"] = $"Welcome back {user.UserName}";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmation)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmation);
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmation.EmailOrUserName) ??
               await _userManager.FindByNameAsync(resendEmailConfirmation.EmailOrUserName);
            if (user is not null && !user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var ConfirmeLink = Url.Action("Confirm", "Account",
                    new { user.Id, token }, Request.Scheme);
                string msg = $"<h1>Click <a href='{ConfirmeLink}'>here</a> to confirm your account</h1>";
                await _accountService.ReSendEmailAsync(MsgType.ResendEmail, msg, user);
            }

            TempData["notification"] = "Resend email successfully, if email is exist and not confirm";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Forgetpassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Forgetpassword(ForgetpasswordVM forgetpassword)
        {
            if (!ModelState.IsValid)
                return View(forgetpassword);

            var user = await _userManager.FindByEmailAsync(forgetpassword.EmailOrUserName) ??
               await _userManager.FindByNameAsync(forgetpassword.EmailOrUserName);
            var userOtpCount = (await _applicationUserOTPrepository.GetAsync(e =>e.ApplictionUserId==user.Id&& (e.CreateAt >= DateTime.UtcNow.AddHours(-24))))
                .Count();
            if (user is not null && userOtpCount <= 5)
            {
                string otp = new Random().Next(1000, 9999).ToString();
                string msg = $"<h1>OTP: {otp},Don’t share it</h1>";
                await _accountService.ReSendEmailAsync(MsgType.ForgetPassword, msg, user);
                await _applicationUserOTPrepository.CreateAsync(new()
                {
                    ApplictionUserId = user.Id,
                    OTP = otp,
                });
                await _applicationUserOTPrepository.CommitAsync();

                TempData["notification"] = "Resend number to confirm successfully, if email is exist";
            }
            else if (userOtpCount > 5)
            {
                TempData["error-notification"] = "Too many attemps , Please try agin after 24 hours";
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            }
            return RedirectToAction("ValidateOTP", "Account", new { area = "Identity", applicationUserId = user.Id });
        }
        [HttpGet]
        public IActionResult ValidateOTP(string applicationUserId)
        {
            return View(new ValidateOTPVM()
            {
                ApplicationUserId = applicationUserId,
            });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
                return View(validateOTPVM);
            var user = await _userManager.FindByIdAsync(validateOTPVM.ApplicationUserId);
            if (user == null) return NotFound();
            var otp = (await _applicationUserOTPrepository.GetAsync()).Where(e => e.ApplictionUserId == user.Id && e.IsValid)
            .OrderBy(e => e.Id).LastOrDefault();
            if (otp == null)
            {
                TempData["error-notificatio"] = "Invalid OTP,Please try agin";
                return View(validateOTPVM);
            }
            otp.IsUsed = true;
            _applicationUserOTPrepository.Update(otp);
           await _applicationUserOTPrepository.CommitAsync();

            return RedirectToAction("ResetPassword", "Account", new { area = "Identity", applicationUserId = user.Id });
        }
        [HttpGet]
        public IActionResult ResetPassword(string applicationUserId)
        {
            return View(new ResetPasswordVM()
            {
                ApplicationUserId = applicationUserId,
            });
        }
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordVM);
            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);
            if (user == null) return NotFound();
            var usertoken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, usertoken, resetPasswordVM.Password);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("Password",string.Join(", ", result.Errors.Select(e=>e.Description)));
                return View(resetPasswordVM);
            }
            TempData["notification"] = "Change password successfully";
            return RedirectToAction("Login");

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");

        }
    }
}
