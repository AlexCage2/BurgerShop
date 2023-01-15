using BurgerShop.Models.DataModels.Users;
using System.Security.Claims;

namespace BurgerShop.Services.Authentication
{
    public sealed class ClaimsPrincipalGenerator : IClaimsPrincipalGenerator
    {
        public ClaimsPrincipal GenerateClaimsPrincipal(User user)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name, user.Login), 
                new Claim(ClaimTypes.Upn, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
