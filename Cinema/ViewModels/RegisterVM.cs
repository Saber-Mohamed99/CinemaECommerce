using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace CinemaECommerce.ViewModels
{
    public class RegisterVM
    {
        public int Id { get; set; }
        [Required]
        public string FName { get; set; }=string.Empty;
        [Required]
        public string LName { get; set; }=string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; }=string.Empty;
        [Required]
        public string UserName { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }=string.Empty;
        public string? Address { get; set; }

    }
}
