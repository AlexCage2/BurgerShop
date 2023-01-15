namespace BurgerShop.Models.ApplicationModels.Sales
{
    public class SaleItem
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public int Summary => Amount * Price;
    }
}
