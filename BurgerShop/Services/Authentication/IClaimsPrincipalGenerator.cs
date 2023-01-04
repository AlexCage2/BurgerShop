using System.Security.Claims;

namespace BurgerShop.Services.Authentication
{
    public interface IClaimsPrincipalGenerator
    {
        public ClaimsPrincipal GenerateClaimsPrincipal(string firstName, string lastName);
    }
}
