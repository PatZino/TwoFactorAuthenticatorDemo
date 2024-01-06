using System.ComponentModel.DataAnnotations;

namespace TwoFactorAuthenticatorDemo.Models.DTO
{
    public class UpdateUserViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorSecretKey { get; set; }
    }
}
