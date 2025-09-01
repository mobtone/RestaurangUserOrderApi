using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Authorization
{
    public class LoginDto
    {
        [Required]
        public string? LoginIdentifier { get; set; } //för att kunna använda både email och/eller telefonnummer vid inloggning
        [Required]
        public string? Password { get; set; }   
    }
}
