using BurgerShop.Data;

namespace BurgerShop.Models.ApplicationModels.Sales
{
    public class SaleInfo
    {
        public Guid OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string UserName { get; set; }
        public IEnumerable<SaleItem> ProductsList { get; set; }

        public async static Task<SaleInfo> FromSaleAsync(
            Guid orderId,
            IPurchaseRepository purchaseRepository,
            IOrderRepository orderRepository,
            CancellationToken cancellationToken = default)
        {
            SaleInfo saleInfo = await orderRepository.GetSaleInfoAsync(orderId, cancellationToken);
            saleInfo.ProductsList = await purchaseRepository.GetSaleItemsListAsync(orderId, cancellationToken);

            return saleInfo;
        }
    }
}
