using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.DataModels.Orders;

namespace BurgerShop.Data
{
    public interface IPurchaseRepository
    {
        public Task CreatePurchaseAsync(Order order);
        public Task<IEnumerable<SaleItem>> GetSaleItemsListAsync(Guid orderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<SaleByItem>> GetSalesByItemsAsync(DateOnly startDate, DateOnly endDate, int linesPerPage, int pageNumber, string sortOrder, CancellationToken cancellationToken = default);
        public Task<IEnumerable<SaleByDay>> GetSalesByDaysAsync(DateOnly startDate, DateOnly endDate, int linesPerPage, int pageNumber, string sortOrder, CancellationToken cancellationToken = default);
    }
}
