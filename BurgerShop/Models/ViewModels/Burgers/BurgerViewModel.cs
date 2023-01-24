using BurgerShop.Models.ApplicationModels.Burgers;

namespace BurgerShop.Models.ViewModels.Burgers
{
    public class BurgerViewModel
    {
        public IEnumerable<BurgersListItem> Burgers { get; set; }
        public BurgerFilterViewModel FilterViewModel { get; set; }
        public BurgerPaginatorViewModel PaginatorViewModel { get; set; }
        public BurgerItemsSortViewModel SortViewModel { get; set; }
    }
}
