using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.Supplies
{
    public class City
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
