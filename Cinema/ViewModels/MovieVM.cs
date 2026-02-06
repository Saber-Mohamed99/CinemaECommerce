namespace CinemaECommerce.ViewModels
{
    public class MovieVM
    {
        public IEnumerable<Movie> MovieModel { get; set; }
        public IEnumerable<Category>Categories { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
        public MovieFilter MovieFilter { get; set; }
    }
}
