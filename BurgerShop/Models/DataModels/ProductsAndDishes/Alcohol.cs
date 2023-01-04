using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.ProductsAndDishes
{
    public class Alcohol
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
