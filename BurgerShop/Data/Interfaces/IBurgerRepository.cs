using BurgerShop.Models.ApplicationModels.Burgers;
using BurgerShop.Models.DataModels.ProductsAndDishes;

namespace BurgerShop.Data
{
    public interface IBurgerRepository
    {
        public Task<Burger> GetBurgerAsync(string burgerName, CancellationToken cancellationToken = default);
        public Task<IEnumerable<BurgersListItem>> GetItemsAsync(
            string burgerName,
            int linesPerPage,
            int pageNumber,
            string sortOrder,
            CancellationToken cancellationToken = default);
    }
}
