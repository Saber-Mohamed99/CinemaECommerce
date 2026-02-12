using CinemaECommerce;
using CinemaECommerce.Models;
using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace ActorECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context;//= new();

        private readonly IStringLocalizer<LocalizationController> _localizer;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Movie> _movieRepository;
        public ActorController(IStringLocalizer<LocalizationController> localizer,
            IRepository<Actor> actorRepository,
            IRepository<Movie> movieRepository)
        {
            _localizer = localizer;
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Index(string? name, int page = 1)
        {
            var actors =await _actorRepository.GetAsync(tracked: false);
            if (name is not null)
                actors = actors.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(actors.Count() / 5.0);
            actors = actors.Skip((page - 1) * 5).Take(5).ToList();
            return View(new ActorVM
            {
                ActorModel = actors.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
            });


        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var movies=await _movieRepository.GetAsync(tracked: false);
            return View(new CreateActorVM()
            {
                Movies = movies,
                Actor =new Actor()
            });
        }
        [HttpPost]
        public async Task<ActionResult> Create(Actor actor, IFormFile? img)
        {
            ModelState.Remove("actor.Movie");
            ModelState.Remove("actor.Img");
            if(!ModelState.IsValid)
                return View(new CreateActorVM()
                {
                    Movies = await _movieRepository.GetAsync(tracked: false),
                    Actor=actor,
                });
            if (img is not null && img.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\ActorImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }
                actor.Img = newfilename;
            }
            await _actorRepository.CreateAsync(actor);
            await _actorRepository.CommitAsync();
            TempData["notification"] = _localizer["AddActor"].Value;
            return RedirectToAction(nameof(Index));
        }
      
        public IActionResult Not_Found()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var actor =await _actorRepository.GetOneAsync(e=>e.Id == id);
            var movies =await _movieRepository.GetAsync(tracked:false);
            if (actor is null)
                return RedirectToAction(nameof(Not_Found));
            return View( new ActorWithMovieVM()
            {
                Actor = actor,
                Movies=movies.AsEnumerable(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? img)
        {
            ModelState.Remove("actor.Movie");
            ModelState.Remove("actor.Img");
            if (!ModelState.IsValid)
                return View(actor);
            var actorinDB =await _actorRepository.GetOneAsync(e => e.Id == actor.Id,tracked:false);
            if (actorinDB is null)
                return RedirectToAction(nameof(Not_Found));

            if (img is not null && img.Length > 0)
            {
                var newfilename = Guid.NewGuid().ToString().Substring(0, 7) + DateTime.UtcNow.ToString("yyyy,MM,dd")
                    + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\ActorImgs",
                   newfilename);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\ActorImgs",
                   actorinDB.Img);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                actor.Img = newfilename;
            }
            else
            {
                actor.Img = actorinDB.Img;
            }
            _actorRepository.Update(actor);
           await _actorRepository.CommitAsync();
            TempData["notification"] = _localizer["UpdateActor"].Value;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var actor =await _actorRepository.GetOneAsync(e => e.Id == id);
            if (actor is null)
                return RedirectToAction(nameof(Not_Found));
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\ActorImgs",
                actor.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync();
            TempData["notification"] = _localizer["DeleteActor"].Value;
            return RedirectToAction(nameof(Index));
        }

    }
}
