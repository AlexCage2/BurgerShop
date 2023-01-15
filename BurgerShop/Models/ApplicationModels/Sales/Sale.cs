namespace BurgerShop.Models.ApplicationModels.Sales
{
    public class Sale
    {
        public Guid OrderId { get; set; }
        public DateOnly Order_Date { get; set; }
        public string UserName { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
