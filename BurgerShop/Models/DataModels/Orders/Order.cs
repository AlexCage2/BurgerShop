using BurgerShop.Data;
using BurgerShop.Models.DataModels.ProductsAndDishes;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BurgerShop.Models.DataModels.Orders
{
    public class Order
    {
        public Guid Id { get; set; }

        [Required]
        public DateOnly Order_Date { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Dictionary<MenuItem, int> Purchases { get; set; }

        public async static Task<Order> FromSession(HttpContext context, IMenuRepository menuRepository)
        {
            Dictionary<MenuItem, int> purchases = new Dictionary<MenuItem, int>();

            foreach (var menuItemName in context.Session.Keys)
            {
                MenuItem menuItem = await menuRepository.GetMenuItemAsync(menuItemName);
                purchases.TryAdd(menuItem, int.Parse(context.Session.GetString(menuItemName)));
            }

            return new Order
            {
                Id = Guid.NewGuid(),
                Order_Date = DateOnly.FromDateTime(DateTime.Now),
                UserId = new Guid(context.User?.FindFirst(ClaimTypes.NameIdentifier).Value),
                Purchases = purchases
            };
        }
    }
}
