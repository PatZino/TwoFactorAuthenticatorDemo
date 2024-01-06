using System.ComponentModel.DataAnnotations;

namespace TwoFactorAuthenticatorDemo.Models.DTO
{
    public class TwoFactorAuthCodeViewModel
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(7, MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Code { get; set; }

        [Display(Name = "Remember this device")]
        public bool RememberMe { get; set; }
    }
}
