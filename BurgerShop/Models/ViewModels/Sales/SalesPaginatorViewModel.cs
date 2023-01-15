namespace BurgerShop.Models.ViewModels.Sales
{
    public class SalesPaginatorViewModel
    {
        public int PageNumberForItemsGroup { get; set; }
        public int LinesPerPageForItemsGroup { get; set; }
        public int PageNumberForDaysGroup { get; set; }
        public int LinesPerPageForDaysGroup { get; set; }
        public int PageNumberForOperationsGroup { get; set; }
        public int LinesPerPageForOperationsGroup { get; set; }
    }
}
