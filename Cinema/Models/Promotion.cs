namespace CinemaECommerce.Models
{
    public class Promotion
    {
       public string Id { get; set; }=Guid.NewGuid().ToString();
        public string Code { get; set; }=string.Empty;
        public decimal Discount { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int MaxUsage { get; set; } = 1;
        public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddDays(30);
        public bool IsValid => MaxUsage >= 1 && ExpireAt > DateTime.UtcNow;
    }
}
