namespace BurgerShop.Models.ApplicationModels.Sales
{
    public class SaleByItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Profit { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
