using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class CinemaController : Controller
    {

        private readonly IRepository<Cinema> _cinemarepository;
        public CinemaController(ApplicationDbContext context, IRepository<Cinema> cinemarepository)
        {
            _cinemarepository = cinemarepository;
        }

        public async Task<IActionResult> Index(string? name, int page = 1)
        {
            
            var cinemas =await _cinemarepository.GetAsync(tracked:false);
            if (name is not null)
                cinemas = cinemas.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(cinemas.Count() / 5.0);
            cinemas = cinemas.Skip((page - 1) * 5).Take(5).ToList();
            return View(new CinemaVM
            {
                CinemaModel = cinemas.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
            });


        }
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Cinema());
        }
        [HttpPost]
        public async Task<ActionResult> Create(Cinema cinema, IFormFile? img)
        {
            ModelState.Remove("Img");
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
                return View(cinema);
            if (img is not null && img.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\CinemaImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                cinema.Img = newfilename;
            }
           await _cinemarepository.CreateAsync(cinema);
           await _cinemarepository.CommitAsync();
            TempData["notification"] = "Add cinema successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Not_Found()
        {
            return View();
        }
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var cinema =await _cinemarepository.GetOneAsync(e=>e.Id==id);
            if (cinema is null)
                return RedirectToAction(nameof(Not_Found));
            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? img)
        {
            ModelState.Remove("Movies");
            ModelState.Remove("Img");
            if (!ModelState.IsValid)
                return View(cinema);
            var cinemainDB =await _cinemarepository.GetOneAsync(e=>e.Id==cinema.Id,tracked:false);
            if (cinemainDB is null)
                return RedirectToAction(nameof(Not_Found));

            if (img is not null && img.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\CinemaImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\CinemaImgs",
                   cinemainDB.Img);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                cinema.Img = newfilename;
            }
            else
            {
                cinema.Img = cinemainDB.Img;
            }
            _cinemarepository.Update(cinema);
           await _cinemarepository.CommitAsync();
            TempData["notification"] = "Update cinema successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var cinema =await _cinemarepository.GetOneAsync(e=>e.Id == id);
            if (cinema is null)
                return RedirectToAction(nameof(Not_Found));
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\CinemaImgs",
                   cinema.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _cinemarepository.Delete(cinema);
            await _cinemarepository.CommitAsync();
            TempData["notification"] = "Delete cinema successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
