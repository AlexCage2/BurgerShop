using BurgerShop.Models.DataModels.ProductsAndDishes;

namespace BurgerShop.Data
{
    public interface IBurgerRepository
    {
        public Task<Burger> GetBurgerAsync(string burgerName, CancellationToken cancellationToken = default);
    }
}
