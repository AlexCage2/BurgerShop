namespace BurgerShop.Models.ViewModels.Sales
{
    public class SalesDaysSortViewModel
    {
        public string SortOrder { get; set; }
        public string IndexSort { get; set; }
        public string DateSort { get; set; }
        public string SummSort { get; set; }

        public SalesDaysSortViewModel(string sortOrder)
        {
            IndexSort = sortOrder == "IndexAsc" ? "IndexDesc" : "IndexAsc";
            DateSort = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";
            SummSort = sortOrder == "SummAsc" ? "SummDesc" : "SummAsc";
        }
    }
}
