namespace CinemaECommerce.ViewModels
{
    public class MovieCreateVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public Movie? Movie { get; set; }=new Movie();

    }
}
