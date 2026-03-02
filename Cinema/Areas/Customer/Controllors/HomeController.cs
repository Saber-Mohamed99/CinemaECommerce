using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CinemaECommerce.Areas.Customer.Controllors
{
    [Area(SD.Customer_Area)]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movierepository;

        public HomeController(IRepository<Movie> movierepository)
        {
            _movierepository = movierepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies =await _movierepository.GetAsync(includes: [e=>e.Cinemas,e=>e.Categories]);
            if(movies == null) return NotFound();
            return View(movies);
        }
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movierepository.GetOneAsync(e => e.Id == id, includes: new Expression<Func<Movie, object>>[]
                    {
                        e => e.MovieSubImgs,
                        e => e.Categories,
                        e=>e.Cinemas
                    });
            return View(movie);
        }
    }
}
