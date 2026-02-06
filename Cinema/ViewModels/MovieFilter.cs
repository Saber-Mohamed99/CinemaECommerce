namespace CinemaECommerce.ViewModels
{
    public class MovieFilter
    {
        public string? Name { get; set; }
        public long? MinPrice { get; set; }
        public long? MaxPrice { get; set; }
        public int? CtagoryId { get; set; }
        public int? CinemaId { get; set; }
    }
}
