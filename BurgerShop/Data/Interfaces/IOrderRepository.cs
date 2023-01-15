using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.DataModels.Orders;

namespace BurgerShop.Data
{
    public interface IOrderRepository
    {
        public Task CreateOrderAsync(Order order);
        public Task<SaleInfo> GetSaleInfoAsync(Guid orderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<Sale>> GetItemsAsync(
            DateOnly startDate,
            DateOnly endDate,
            string userName,
            int linesPerPage,
            int pageNumber,
            string sortOrder,
            CancellationToken cancellationToken = default);
    }
}
