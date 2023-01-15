namespace BurgerShop.Models.ViewModels.Sales
{
    public class SalesOperationsSortViewModel
    {
        public string SortOrder { get; set; }
        public string IdSort { get; set; }
        public string DateSort { get; set; }
        public string UserSort { get; set; }

        public SalesOperationsSortViewModel(string sortOrder)
        {
            IdSort = sortOrder == "IdAsc" ? "IdDesc" : "IdAsc";
            DateSort = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";
            UserSort = sortOrder == "UserAsc" ? "UserDesc" : "UserAsc";
        }
    }
}
