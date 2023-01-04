using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.ProductsAndDishes
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string ProductType { get; set; }
    }
}
