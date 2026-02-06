using System.ComponentModel.DataAnnotations;
namespace CinemaECommerce.ValidationAttributes
{
    public class CustomDateAttribute :ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is DateTime valueString)
            return valueString.Date>=DateTime.Today;

            return false;

        }
        public override string FormatErrorMessage(string Date)
        {
            return $"The date must be greater than or equal to today's date.";
        }
    }
}
 