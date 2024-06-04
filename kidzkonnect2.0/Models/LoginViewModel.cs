using System.ComponentModel.DataAnnotations;

namespace kidzkonnect2._0.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Courriel { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

