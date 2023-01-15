using BurgerShop.Models.DataModels.Users;
using System.Security.Claims;

namespace BurgerShop.Services.Authentication
{
    public interface IClaimsPrincipalGenerator
    {
        public ClaimsPrincipal GenerateClaimsPrincipal(User user);
    }
}
