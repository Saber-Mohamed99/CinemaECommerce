using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Mapster;
namespace CinemaECommerce.Areas.Identity.Controllors
{
    [Area(SD.Identity_Area)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
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
         var result=  await _userManager.CreateAsync(applicationUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }
            TempData["notificatio"] = "Add user successfully";
            return View("Login");
        }
    }
}
