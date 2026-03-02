using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CinemaECommerce.Areas.Customer.Controllors
{
    [Area(SD.Customer_Area)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;

        public CartController(UserManager<ApplicationUser> userManager,
            IRepository<Movie> movieRepository, IRepository<Cart> cartRepository,IRepository<Promotion> promotionRepository)
        {
            _userManager = userManager;
            _movieRepository = movieRepository;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
        }
        public async Task<IActionResult> Index(string? code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);
            if(code is not null)
            {
                var promotions =await _promotionRepository.GetAsync(e => e.Code == code);
                var movieIds = promotions.Select(e => e.MovieId);
                var discount= promotions.Select(e=>e.Discount).FirstOrDefault()/100m;
                bool movieFound = false;
                foreach(var item in carts)
                {
                    if (movieIds.Contains(item.MovieId))
                    {
                        item.ListPrice-=(item.Movie.Price*discount);
                        movieFound = true;
                        promotions.Where(e => e.MovieId == item.MovieId).FirstOrDefault().MaxUsage--;
                        await _promotionRepository.CommitAsync();
                       await _cartRepository.CommitAsync();
                        TempData["notification"] = $"Apply {code} successfully";
                        break;
                    }
                }
                if(!movieFound)
                {
                    TempData["error-notification"] = $"Can not apply {code} to this movie";
                }
            }
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int movieId, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            var movie = await _movieRepository.GetOneAsync(e => e.Id == movieId);
            if (movie == null || user == null)
                return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movie.Id);
            if (cartInDb == null)
            {
                await _cartRepository.CreateAsync(new Cart()
                {
                    ApplicationUserId = user.Id,
                    MovieId = movie.Id,
                    Count = count,
                    ListPrice = movie.Price,
                });

            }
            else
            {
                cartInDb.Count += count;
                cartInDb.ListPrice += (count * movie.Price);
            }
            await _cartRepository.CommitAsync();
            TempData["notification"] = "Add movie to cart successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Increment(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
           var cart= await _cartRepository.GetOneAsync(e=>e.ApplicationUserId == user.Id&&e.MovieId== movieId
           ,includes: [e=>e.Movie]);
            if(cart == null) return NotFound();
            cart.Count++;
            cart.ListPrice += cart.Movie.Price;
           await _cartRepository.CommitAsync();
            TempData["notification"] = "Add movie to cart successfully";
            return  RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Decrement(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId
            , includes: [e => e.Movie]);
            if (cart == null) return NotFound();
            if (cart.Count > 1)
            {
                cart.Count--;
                cart.ListPrice -= cart.Movie.Price;
            }
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if (cart == null) return NotFound();
            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();
            TempData["notification"] = "Delete cart successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
