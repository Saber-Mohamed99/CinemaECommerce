using System.ComponentModel.DataAnnotations;

namespace CinemaECommerce.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [Length(7, 20)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Img { get; set; }
        public List<Movie> Movies { get; set; }


    }
}
