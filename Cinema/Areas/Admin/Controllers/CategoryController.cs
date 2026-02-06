using CinemaECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context=new();
        public IActionResult Index(string? name,int page=1)
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            if(name is not null)
                categories= categories.Where(e => e.Name.ToLower().Contains(name.ToLower()));
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5);
            return View(new CategoryVM
            {
                CatogeryModel = categories.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
            });

           
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View( new Category());
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid) return View(category);
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null)
                return RedirectToAction(nameof(Not_Found));
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null)
                return RedirectToAction(nameof(Not_Found));
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Not_Found()
        {
            return View();
        }
    }
}
