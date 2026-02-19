namespace CinemaECommerce.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; }=string.Empty;
        public DateTime CreateAt { get; set; }=DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; }=DateTime.UtcNow.AddHours(20);
        public bool IsUsed { get; set; }
        public bool IsValid => ExpiredAt > DateTime.UtcNow&&!IsUsed;
        public string ApplictionUserId { get; set; } = string.Empty;
        public ApplicationUser ApplictionUser { get; set; } = null!;

    }
}
