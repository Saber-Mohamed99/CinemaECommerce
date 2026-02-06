namespace CinemaECommerce.ViewModels
{
    public class CinemaVM
    {
        public IEnumerable<Cinema> CinemaModel { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
