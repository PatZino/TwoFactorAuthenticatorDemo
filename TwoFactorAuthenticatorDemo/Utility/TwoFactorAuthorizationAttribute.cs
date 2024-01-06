using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwoFactorAuthenticatorDemo.DataContext;

namespace TwoFactorAuthenticatorDemo.Utility
{
    public class TwoFactorAuthorizationAttribute : TypeFilterAttribute
    {
        public TwoFactorAuthorizationAttribute() : base(typeof(TwoFactorAuthorizationFilter))
        {
        }
    }

    public class TwoFactorAuthorizationFilter : IAuthorizationFilter
    {
        private readonly TwoFactorAuthDbContext _dbContext; 

        public TwoFactorAuthorizationFilter(TwoFactorAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context?.HttpContext?.User?.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("ErrorPage", "Home", new { errorMessage = Messages.ACCESS_DENIED });
                return;
            }
            
            string userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Result = new RedirectToActionResult("ErrorPage", "Home", new { errorMessage = Messages.ACCESS_DENIED });
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                if (!user.TwoFactorLoginVerified)
                {
                    context.Result = new RedirectToActionResult("ErrorPage", "Home", new { errorMessage = Messages.TWO_FACTOR_REQUIRED });
                    return;
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("ErrorPage", "Home", new { errorMessage = Messages.ACCESS_DENIED });
                return;
            }
        }
    }

}
