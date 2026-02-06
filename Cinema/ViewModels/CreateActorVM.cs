namespace CinemaECommerce.ViewModels
{
    public class CreateActorVM
    {
        public IEnumerable<Movie> Movies { get; set; }
        public Actor Actor { get; set; }=new Actor();
    }
}
