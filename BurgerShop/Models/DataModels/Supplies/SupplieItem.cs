using BurgerShop.Models.DataModels.ProductsAndDishes;
using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.Supplies
{
    public class SupplieItem
    {
        public Guid Id { get; set; }

        [Required]
        public DateOnly OrderDate { get; set; }

        [Required]
        public string SupplierName { get; set; }

        [Required]
        public List<Product> Supplies { get; set; }
    }
}
