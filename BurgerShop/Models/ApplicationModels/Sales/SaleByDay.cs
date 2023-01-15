namespace BurgerShop.Models.ApplicationModels.Sales
{
    public class SaleByDay
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int Profit { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
