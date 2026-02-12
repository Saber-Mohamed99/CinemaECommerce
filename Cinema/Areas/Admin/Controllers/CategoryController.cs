using CinemaECommerce.Models;
using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class CategoryController : Controller
    {
        private readonly IStringLocalizer<LocalizationController> _localizer;
        private IRepository<Category> _categoryRepository;
        public CategoryController(IStringLocalizer<LocalizationController> localizer
               , IRepository<Category> categoryRepository)
        {
            _localizer = localizer;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string? name,int page=1)
        {
            //var categories = _context.Categories.AsNoTracking().AsQueryable();
            var categories =await _categoryRepository.GetAsync(tracked: false);
            if (name is not null)
                categories= categories.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5).ToList();
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
        public async Task<ActionResult> Create(Category category)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid) return View(category);
            //_context.Categories.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.CreateAsync(category);
           await _categoryRepository.CommitAsync();
            TempData["notification"] = _localizer["AddCategory"].Value;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var category =await _categoryRepository.GetOneAsync(e=>e.Id==id);
            if (category is null)
                return RedirectToAction(nameof(Not_Found));
            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            return View(category);
            //_context.Categories.Update(category);
            //_context.SaveChanges();
            _categoryRepository.Update(category);
           await _categoryRepository.CommitAsync();
            TempData["notification"] = _localizer["UpdateCategory"].Value;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var category =await _categoryRepository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return RedirectToAction(nameof(Not_Found));
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
            _categoryRepository.Delete(category);
           await _categoryRepository.CommitAsync();
            TempData["notification"] = _localizer["DeleteCategory"].Value;
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Not_Found()
        {
            return View();
        }
    }
}
