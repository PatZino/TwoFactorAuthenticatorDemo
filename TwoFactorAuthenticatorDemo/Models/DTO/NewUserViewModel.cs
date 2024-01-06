using System.ComponentModel.DataAnnotations;

namespace TwoFactorAuthenticatorDemo.Models.DTO
{
    public class NewUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorSecretKey { get; set; }
    }
}
