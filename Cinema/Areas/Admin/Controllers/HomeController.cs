using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class HomeController : Controller
    {
        private ApplicationDbContext _context = new(); 
        public IActionResult Index()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var movies = _context.Movies.AsNoTracking().ToList();
            var cinemas = _context.Cinemas.AsNoTracking().ToList();
            var actors = _context.Actors.AsNoTracking().ToList();


            return View(new HomePage()
            {
                Movies = movies,
                Categories = categories,
                Actors = actors,
                Cinemas = cinemas,
            });
        }
    }
}
