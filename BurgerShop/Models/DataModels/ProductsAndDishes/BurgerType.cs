using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.ProductsAndDishes
{
    public class BurgerType
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
