namespace CinemaECommerce.ViewModels
{
    public class CategoryVM
    {
        public IEnumerable<Category> CatogeryModel { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
