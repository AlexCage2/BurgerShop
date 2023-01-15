using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.DataModels.Orders;

namespace BurgerShop.Models.ViewModels.Sales
{
    public class SalesViewModel
    {
        public IEnumerable<Sale> Orders { get; set; }
        public IEnumerable<SaleByItem> SalesByItems { get; set; }
        public IEnumerable<SaleByDay> SalesByDays { get; set; }
        public SalesFilterViewModel FilterViewModel { get; set; }
        public SalesPaginatorViewModel PaginatorViewModel { get; set; }
        public SalesItemsSortViewModel SalesItemsSortViewModel { get; set; }
        public SalesDaysSortViewModel SalesDaysSortViewModel { get; set; }
        public SalesOperationsSortViewModel SalesOperationsSortViewModel { get; set; }
    }
}
