using BurgerShop.Models.DataModels.ProductsAndDishes;

namespace BurgerShop.Data
{
    public interface IMenuRepository
    {
        public Task<IEnumerable<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default);
        public Task<MenuItem> GetMenuItemAsync(string menuItemName, CancellationToken cancellationToken = default);
    }
}
