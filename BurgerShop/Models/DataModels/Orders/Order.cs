using BurgerShop.Models.DataModels.ProductsAndDishes;
using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.Orders
{
    public class Order
    {
        public Guid Id { get; set; }

        [Required]
        public DateOnly Order_Date { get; set; }

        [Required]
        public string UserLogin { get; set; }

        public Dictionary<MenuItem, int> Purchases { get; set; }
    }
}
