using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.ProductsAndDishes
{
    public class Burger
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string BurgerType { get; set; }

        public Dictionary<string, int> Recipe { get; set; }
    }
}
