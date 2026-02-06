using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class CinemaController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(string? name, int page = 1)
        {
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            if (name is not null)
                cinemas = cinemas.Where(e => e.Name.ToLower().Contains(name.ToLower()));
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(cinemas.Count() / 5.0);
            cinemas = cinemas.Skip((page - 1) * 5).Take(5);
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
        public ActionResult Create(Cinema cinema, IFormFile? img)
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
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Not_Found()
        {
            return View();
        }
        public IActionResult Edit([FromRoute] int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
                return RedirectToAction(nameof(Not_Found));
            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? img)
        {
            var cinemainDB = _context.Cinemas.AsNoTracking().FirstOrDefault(e => e.Id == cinema.Id);
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
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (cinema is null)
                return RedirectToAction(nameof(Not_Found));
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\CinemaImgs",
                   cinema.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
