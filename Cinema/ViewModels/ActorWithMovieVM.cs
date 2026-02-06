namespace CinemaECommerce.ViewModels
{
    public class ActorWithMovieVM
    {
        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}
