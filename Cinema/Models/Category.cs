using System.ComponentModel.DataAnnotations;

namespace CinemaECommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Length(7, 20)]
        public string Name { get; set; } = string.Empty;
       
        public bool Status { get; set; }
        public List<Movie> Movies { get; set; }

    }
}
