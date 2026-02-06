using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActorECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class ActorController : Controller
    {
        private ApplicationDbContext _context = new();
        public IActionResult Index(string? name, int page = 1)
        {
            var actors = _context.Actors.AsNoTracking().AsQueryable();
            if (name is not null)
                actors = actors.Where(e => e.Name.ToLower().Contains(name.ToLower()));
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(actors.Count() / 5.0);
            actors = actors.Skip((page - 1) * 5).Take(5);
            return View(new ActorVM
            {
                ActorModel = actors.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
            });


        }
        [HttpGet]
        public ActionResult Create()
        {
            var movies=_context.Movies.AsNoTracking().AsQueryable();
            return View(new CreateActorVM()
            {
                Movies = movies,
            });
        }
        [HttpPost]
        public ActionResult Create(Actor actor, IFormFile? img)
        {
            ModelState.Remove("actor.Movie");
            ModelState.Remove("actor.Img");
            if(!ModelState.IsValid)
                return View(new CreateActorVM()
                {
                    Movies = _context.Movies.AsNoTracking().AsQueryable()
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
            _context.Actors.Add(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
      
        public IActionResult Not_Found()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var actor = _context.Actors.Find(id);
            var movies = _context.Movies.AsNoTracking().AsQueryable();
            if (actor is null)
                return RedirectToAction(nameof(Not_Found));
            return View( new ActorWithMovieVM()
            {
                Actor = actor,
                Movies=movies.AsEnumerable(),
            });
        }

        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile? img)
        {
            var actorinDB = _context.Actors.AsNoTracking().FirstOrDefault(e => e.Id == actor.Id);
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
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
                return RedirectToAction(nameof(Not_Found));
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Imgs\\ActorImgs",
                   actor.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
