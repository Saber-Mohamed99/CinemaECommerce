using System.ComponentModel.DataAnnotations;

namespace CinemaECommerce.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [Length(7, 20)]
        public string Name { get; set; } = string.Empty;
        [Length(7, 255)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string? Img { get; set; }
        public int MovieId { get; set; } = default!;
        public Movie Movie { get; set; }

    }
}
