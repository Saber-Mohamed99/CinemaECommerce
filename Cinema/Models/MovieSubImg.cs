namespace CinemaECommerce.Models
{
    public class MovieSubImg
    {
        public int Id { get; set; }
        public string SubImg { get; set; }=string.Empty;
        public int MovieId { get; set; } = default!;
        public Movie Movie { get; set; }
    }
}
