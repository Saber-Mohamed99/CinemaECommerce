using CinemaECommerce.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CinemaECommerce.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [Length(7,20)]
        public string Name { get; set; } = string.Empty;
        //[Required]
        public string MainImg { get; set; } = string.Empty;
        [Length(7,255)]
        public string Description { get; set; }= string.Empty;
        public bool Status { get; set; }
        [Range(100,400)]
        public decimal Price { get; set; }
        [CustomDate]
        public DateTime Date { get; set; }
        public int CategoryId { get; set; } = default!;
        public Category Categories { get; set; }
        public int CinemaId { get; set; } = default!;
        public Cinema Cinemas { get; set; }
        public List<MovieSubImg> MovieSubImgs { get; set; }
        public List<Actor> Actors { get; set; }
    }
}
