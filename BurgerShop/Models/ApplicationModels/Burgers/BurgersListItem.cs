namespace BurgerShop.Models.ApplicationModels.Burgers
{
    public class BurgersListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string BurgerType { get; set; }

        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }

    }
}
