namespace CinemaECommerce.ViewModels
{
    public class UserVM
    {
        public IEnumerable<ApplicationUser> UserModel { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
