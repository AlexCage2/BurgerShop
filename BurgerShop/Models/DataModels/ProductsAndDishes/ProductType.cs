using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.ProductsAndDishes
{
    public class ProductType
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
