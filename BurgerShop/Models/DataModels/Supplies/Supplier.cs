using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.DataModels.Supplies
{
    public class Supplier
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string SupplierStatus { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Phone]
        [Required]
        [MaxLength(30)]
        public string Phone { get; set; }

        [Url]
        public string Site { get; set; }

        public string Information { get; set; }
    }
}
