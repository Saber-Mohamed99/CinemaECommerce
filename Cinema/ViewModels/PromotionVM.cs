namespace CinemaECommerce.ViewModels
{
    public class PromotionVM
    {
        public IEnumerable<Promotion> PromotionModel { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
