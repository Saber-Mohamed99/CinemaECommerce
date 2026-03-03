using CinemaECommerce.Models;
using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CinemaECommerce.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    [Authorize(Roles = SD.Super_Admin_Role)]
    public class PromotionController : Controller
    {
        private readonly IRepository<Promotion> _promotionRepository;
        public PromotionController(IRepository<Promotion> promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string? code, int page = 1)
        {

            var promotions = await _promotionRepository.GetAsync(tracked: false, includes: [e=>e.Movie]);
            if (code is not null)
                promotions = promotions.Where(e => e.Code.Contains(code)).ToList();
            if (page < 1)
                page = 1;

            int currentpage = page;
            double totalpages = Math.Ceiling(promotions.Count() / 5.0);
            promotions = promotions.Skip((page - 1) * 5).Take(5).ToList();
            return View(new PromotionVM
            {
                PromotionModel = promotions.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = totalpages,
            });


        }
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Promotion());
        }
        [HttpPost]
        public async Task<ActionResult> Create(Promotion promotion)
        {
            await _promotionRepository.CreateAsync(promotion);
            await _promotionRepository.CommitAsync();
            TempData["notification"] ="Add Promotion Successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit( string id)
        {
            var promotion = await _promotionRepository.GetOneAsync(e => e.Id == id);
            if (promotion is null)
                return NotFound();
            return View(promotion);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Promotion promotion)
        {
            _promotionRepository.Update(promotion);
            await _promotionRepository.CommitAsync();
            TempData["notification"] = "Update promotion successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
        {
            var promotion = await _promotionRepository.GetOneAsync(e => e.Id == id);
            if (promotion is null)
                return NotFound();
            _promotionRepository.Delete(promotion);
            await _promotionRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
