namespace BurgerShop.Models.ViewModels.Burgers
{
    public class BurgerItemsSortViewModel
    {
        public string SortOrder { get; set; }
        public string NameSort { get; set; }
        public string SummSort { get; set; }
        public string TypeSort { get; set; }

        public BurgerItemsSortViewModel(string sortOrder)
        {
            NameSort = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
            SummSort = sortOrder == "SummAsc" ? "SummDesc" : "SummAsc";
            TypeSort = sortOrder == "TypeAsc" ? "TypeDesc" : "TypeAsc";
        }
    }
}
