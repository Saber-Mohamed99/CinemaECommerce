using CinemaECommerce.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    [Authorize(Roles =SD.Super_Admin_Role)]
    public class UserController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IRepository<ApplicationUser> _apllicationUserRepository;

        public UserController(UserManager<ApplicationUser> userManager, IAccountService accountService,
            IRepository<ApplicationUser> apllicationUserRepository)
        {
            _userManager = userManager;
            _accountService = accountService;
            _apllicationUserRepository = apllicationUserRepository;
        }
        //public async Task<IActionResult> Index(string? userName, int page = 1)
        //{
           
        //    var users = await _apllicationUserRepository.GetAsync(tracked: false);
        //    foreach (var user in users)
        //    {
        //        var roles = await _userManager.GetRolesAsync(user);
        //    }
        //    if (userName is not null)
        //        users = users.Where(e => e.UserName.ToLower().Contains(userName.ToLower())).ToList();
        //    if (page < 1)
        //        page = 1;

        //    int currentpage = page;
        //    double totalpages = Math.Ceiling(users.Count() / 5.0);
        //    users = users.Skip((page - 1) * 5).Take(5).ToList();
        //    return View(new UserVM
        //    {
        //        UserModel = users.AsEnumerable(),
        //        CurrentPage = currentpage,
        //        TotalPages = totalpages,
        //    });


        //}
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUseVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUseVM createUseVM)
        {
            if (!ModelState.IsValid)
                return View(createUseVM);
            
            var applicationUser = createUseVM.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(applicationUser, createUseVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(createUseVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var ConfirmeLink = Url.Action("Confirm", "Account",
                new {area="Identity" ,id=applicationUser.Id,token, Request.Scheme });
            string msg = $"<h1>Click <a href='{ConfirmeLink}'>here</a> to confirm your account</h1>";
            await _accountService.ReSendEmailAsync(MsgType.ConfirmationEmail, msg, applicationUser);

            await _userManager.AddToRoleAsync(applicationUser, createUseVM.Role);
            TempData["notification"] = "Add account successfully";
            return RedirectToAction("Login", "Account", new {area="Identity"});
        }
    }
}
