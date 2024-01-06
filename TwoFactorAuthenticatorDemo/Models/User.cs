using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;

namespace TwoFactorAuthenticatorDemo.Models
{
    public class User: IdentityUser
    {
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorSecretKey { get; set; }
        public bool TwoFactorLoginVerified { get; set; }
    }
}
