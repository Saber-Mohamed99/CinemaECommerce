namespace CinemaECommerce.ViewModels
{
    public class ActorVM
    {
        public IEnumerable<Actor> ActorModel { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
