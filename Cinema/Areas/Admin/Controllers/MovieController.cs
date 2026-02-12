
using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie>_movieRepository;
        private readonly IRepository<Category>_categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IMovieSubImgRepository _movieSubImgRepository;

        public MovieController(ApplicationDbContext context, IRepository<Movie> movieRepository,
            IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository,
           IMovieSubImgRepository movieSubImgRepository)
            
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieSubImgRepository = movieSubImgRepository;
        }

        public async Task<IActionResult> Index(MovieFilter moviefilter, int page = 1)
        {
            var movies =await _movieRepository.GetAsync(tracked: false, includes: [e=>e.Cinemas,e=>e.Categories]);
            if (moviefilter.Name is not null)
            {
                movies = movies.Where(x => x.Name.Contains(moviefilter.Name)).ToList();
            }
            if (page < 1)
                page = 1;
            if (moviefilter.MinPrice is not null)
                movies = movies.Where(e => e.Price >= moviefilter.MinPrice).ToList();

            if (moviefilter.MaxPrice is not null)
                movies = movies.Where(e => e.Price <= moviefilter.MaxPrice).ToList();
            var categories =await _categoryRepository.GetAsync(tracked:false);
            var cinemas =await _cinemaRepository.GetAsync(tracked: false); ;
            if (moviefilter.CtagoryId is not null)
                movies = movies.Where(e => e.Categories.Id == moviefilter.CtagoryId).ToList();
            if (moviefilter.CinemaId is not null)
                movies = movies.Where(e => e.Cinemas.Id == moviefilter.CinemaId).ToList();
            int currentpage = page;
            double totalpages = Math.Ceiling(movies.Count() / 5.0);
            movies = movies.Skip((page - 1) * 5).Take(5).ToList();
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
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAsync(tracked: false);
            var cinemas = await _cinemaRepository.GetAsync(tracked: false); 
            return View(new MovieCreateVM()
            {
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable(),
            } );
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile? MainImg, List<IFormFile>? SubImg)

        {
            ModelState.Remove("movie.Categories");
            //ModelState.Remove("movie.MainImg");
            ModelState.Remove("movie.Cinemas");
            ModelState.Remove("movie.Actors");
            ModelState.Remove("movie.MovieSubImgs");
            if (!ModelState.IsValid)
                return View(new MovieCreateVM
                {
                    Categories = await _categoryRepository.GetAsync(tracked: false),
                    Cinemas = await _cinemaRepository.GetAsync(tracked: false)
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
              await _movieRepository.CreateAsync(movie);
              await _movieRepository.CommitAsync();
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
                   await _movieSubImgRepository.CreateAsync(new()
                    {
                        MovieId = movie.Id,
                        SubImg = newfilename
                       });
                }
              await  _movieSubImgRepository.CommitAsync();
            }
            TempData["notification"] = "Add movie successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie =await _movieRepository.GetOneAsync(e=>e.Id==id);
            if (movie == null) return NotFound();

            var subImg =await _movieSubImgRepository.GetAsync(e => e.MovieId == id);

            var categories = await _categoryRepository.GetAsync(tracked: false);
            var cinemas = await _cinemaRepository.GetAsync(tracked: false);
            return View(new MovieEditVM()
            {
                Movie = movie,
                Categories = categories.ToList(),
                MovieSubImgs = subImg.ToList(),
                Cinemas = cinemas.ToList()

            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? MainImg, List<IFormFile>? SubImg)
        {
            ModelState.Remove("movie.MovieSubImgs");
            ModelState.Remove("movie.Categories");
            ModelState.Remove("movie.Cinemas");
            ModelState.Remove("movie.Actors");
            if(!ModelState.IsValid)
                return View(new MovieEditVM()
                {
                    Movie = movie,
                    Categories =await _categoryRepository.GetAsync(),
                    MovieSubImgs =await _movieSubImgRepository.GetAsync(e => e.MovieId == movie.Id),
                    Cinemas =await _cinemaRepository.GetAsync()

                });
            var movieInDB =await _movieRepository.GetOneAsync(e => e.Id == movie.Id,tracked:false);
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
            _movieRepository.Update(movie);
              await _movieRepository.CommitAsync();
            if (SubImg.Any())
            {
                var SubImgsInDB = await _movieSubImgRepository.GetAsync(e => e.MovieId == movie.Id);
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
                    await _movieSubImgRepository.CreateAsync(new()
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
                _movieSubImgRepository.DeleteRange(SubImgsInDB);
                await _movieSubImgRepository.CommitAsync();

            }
                TempData["notification"] = "Update movie successfully";
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs",
                   movie.MainImg);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            var SubImgsInDB = await _movieSubImgRepository.GetAsync(e => e.MovieId == movie.Id);
            
            foreach (var item in SubImgsInDB)
            {
                var oldPathsubImgs = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\MovieImgs\\SubImg",
              item.SubImg);

                if (System.IO.File.Exists(oldPathsubImgs))
                {
                    System.IO.File.Delete(oldPathsubImgs);
                }
            }
            _movieSubImgRepository.DeleteRange(SubImgsInDB);
            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();
            TempData["notification"] = "Delete movie successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
