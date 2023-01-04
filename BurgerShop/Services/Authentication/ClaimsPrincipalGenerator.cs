using System.Security.Claims;

namespace BurgerShop.Services.Authentication
{
    public sealed class ClaimsPrincipalGenerator : IClaimsPrincipalGenerator
    {
        public ClaimsPrincipal GenerateClaimsPrincipal(string firstName, string lastName)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name, firstName), 
                new Claim(ClaimTypes.Surname, lastName) 
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
