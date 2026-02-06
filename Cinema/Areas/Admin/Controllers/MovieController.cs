
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class MovieController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(MovieFilter moviefilter, int page = 1)
        {
            var movies = _context.Movies.AsNoTracking().AsQueryable();
            movies = movies.Include(e => e.Categories).Include(e => e.Cinemas);
            if (moviefilter.Name is not null)
            {
                movies = movies.Where(x => x.Name.Contains(moviefilter.Name));
            }
            if (page < 1)
                page = 1;
            if (moviefilter.MinPrice is not null)
                movies = movies.Where(e => e.Price >= moviefilter.MinPrice);

            if (moviefilter.MaxPrice is not null)
                movies = movies.Where(e => e.Price <= moviefilter.MaxPrice);
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            //var actors=_context.Actors.AsNoTracking().AsNoTracking();
            //actors=actors.Where(e=>e.MovieId==movies.)
            if (moviefilter.CtagoryId is not null)
                movies = movies.Where(e => e.Categories.Id == moviefilter.CtagoryId);
            if (moviefilter.CinemaId is not null)
                movies = movies.Where(e => e.Cinemas.Id == moviefilter.CinemaId);
            int currentpage = page;
            double totalpages = Math.Ceiling(movies.Count() / 5.0);
            movies = movies.Skip((page - 1) * 5).Take(5);
            return View(new MovieVM
            {
                MovieModel = movies.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
                MovieFilter = moviefilter,
                Categories = categories,
                Cinemas = cinemas
            });

        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            return View(new MovieCreateVM()
            {
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable(),
            } );
        }
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile? MainImg, List<IFormFile>? SubImg)

        {
            ModelState.Remove("movie.Categories");
            ModelState.Remove("movie.MainImg");
            ModelState.Remove("movie.Cinemas");
            ModelState.Remove("movie.Actors");
            if (!ModelState.IsValid)
                return View(new MovieCreateVM
                {
                    Categories = _context.Categories.AsNoTracking().ToList(),
                    Cinemas = _context.Cinemas.AsNoTracking().ToList()
                });
            if (MainImg is not null && MainImg.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }
                movie.MainImg = newfilename;
            }
            _context.Movies.Add(movie);
            _context.SaveChanges();
            if (SubImg.Any())
            {
                foreach (var item in SubImg)
                {
                    var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs\\SubImg",
                       newfilename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }
                    _context.MovieSubImgs.Add(new()
                    {
                        MovieId = movie.Id,
                        SubImg = newfilename
                    });
                }
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null) return NotFound();

            var subImg = _context.MovieSubImgs.Where(e => e.MovieId == id);

            var categories = _context.Categories.Where(e => e.Id == movie.CategoryId);
            var cinemas = _context.Cinemas.Where(e => e.Id == movie.CinemaId);

            return View(new MovieEditVM()
            {
                Movie = movie,
                Categories = categories.ToList(),
                MovieSubImgs = subImg.ToList(),
                Cinemas = cinemas.ToList()

            });
        }
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile? MainImg, List<IFormFile>? SubImg)
        {
            ModelState.Remove(nameof(movie.MovieSubImgs));
            ModelState.Remove(nameof(movie.Categories));
            ModelState.Remove(nameof(movie.Cinemas));
            ModelState.Remove(nameof(movie.Actors));
            if(!ModelState.IsValid)
                return View(new MovieEditVM()
                {
                    Movie = movie,
                    Categories = _context.Categories.Where(e => e.Id == movie.CategoryId).ToList(),
                    MovieSubImgs =_context.MovieSubImgs.Where(e => e.MovieId == movie.Id).ToList(),
                    Cinemas = _context.Cinemas.Where(e => e.Id == movie.CinemaId).ToList()

                });
            var movieInDB = _context.Movies.AsNoTracking().FirstOrDefault(e => e.Id == movie.Id);
            if (movieInDB is null)
                return NotFound();

            if (MainImg is not null && MainImg.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }
                movie.MainImg = newfilename;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs",
                   movieInDB.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            else
            {
                movie.MainImg = movieInDB.MainImg;
            }
            _context.Movies.Update(movie);
            _context.SaveChanges();
            if (SubImg.Any())
            {
                var SubImgsInDB = _context.MovieSubImgs.Where(e => e.MovieId == movie.Id);
                foreach (var item in SubImg)
                {
                    var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs\\SubImg",
                       newfilename);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }
                    _context.MovieSubImgs.Add(new()
                    {
                        MovieId = movie.Id,
                        SubImg = newfilename
                    });
                }
                foreach (var item in SubImgsInDB)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs\\SubImg",
                  item.SubImg);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                _context.MovieSubImgs.RemoveRange(SubImgsInDB);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs",
                   movie.MainImg);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            var SubImgsInDB = _context.MovieSubImgs.Where(e => e.MovieId == movie.Id);
            foreach (var item in SubImgsInDB)
            {
                var oldPathsubImgs = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs\\SubImg",
              item.SubImg);

                if (System.IO.File.Exists(oldPathsubImgs))
                {
                    System.IO.File.Delete(oldPathsubImgs);
                }
            }
            _context.MovieSubImgs.RemoveRange(SubImgsInDB);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
