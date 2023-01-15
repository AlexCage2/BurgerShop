namespace BurgerShop.Models.ViewModels.Sales
{
    public class SalesItemsSortViewModel
    {
        public string SortOrder { get; set; }
        public string IndexSort { get; set; }
        public string NameSort { get; set; }
        public string SummSort { get; set; }

        public SalesItemsSortViewModel(string sortOrder)
        {
            IndexSort = sortOrder == "IndexAsc" ? "IndexDesc" : "IndexAsc";
            NameSort = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
            SummSort = sortOrder == "SummAsc" ? "SummDesc" : "SummAsc";
        }
    }
}
