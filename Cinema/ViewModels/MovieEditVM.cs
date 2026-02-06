namespace CinemaECommerce.ViewModels
{
    public class MovieEditVM
    {
        public Movie Movie { get; set; }
        public List<Category> Categories { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public List<MovieSubImg> MovieSubImgs { get; set; }
    }
}
