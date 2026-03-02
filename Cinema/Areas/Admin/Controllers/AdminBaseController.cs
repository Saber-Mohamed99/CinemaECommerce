using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Authorize(Roles =$"{SD.Super_Admin_Role},{SD.Admin_Role},{SD.Employee_Role}")]
    public class AdminBaseController:Controller
    {
    }
}
