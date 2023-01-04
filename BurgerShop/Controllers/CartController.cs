using BurgerShop.Data;
using BurgerShop.Models.DataModels.Orders;
using BurgerShop.Models.DataModels.ProductsAndDishes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers
{
    [Authorize]
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly MenuContext _menuContext;

        public CartController(MenuContext menuContext)
        {
            _menuContext = menuContext;
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            Dictionary<MenuItem, int> purchases = new Dictionary<MenuItem, int>();

            foreach (var menuItemName in HttpContext.Session.Keys)
            {
                MenuItem menuItem = await _menuContext.GetMenuItemAsync(menuItemName);
                purchases.TryAdd(menuItem, int.Parse(HttpContext.Session.GetString(menuItemName)));
            }

            Order order = new Order
            {
                Id = new Guid(),
                Order_Date = DateOnly.FromDateTime(DateTime.Now),
                UserLogin = User.Identity.Name,
                Purchases = purchases
            };

            return View(order);
        }

        [Route("additem")]
        public ActionResult AddItem(string menuItemName, int count)
        {
            ISession session = HttpContext.Session; 

            if (string.IsNullOrEmpty(menuItemName) || count < 1) 
            {
                return RedirectToAction("Index", "Menu");
            }

            if (string.IsNullOrEmpty(session.GetString(menuItemName)))
            {
                session.SetString(menuItemName, count.ToString());
                return RedirectToAction("Index", "Menu");
            }

            int currentCount = Convert.ToInt32(session.GetString(menuItemName)) + count;
            session.SetString(menuItemName, currentCount.ToString());

            return RedirectToAction("Index", "Menu");
        }

        [Route("removeitem")]
        public ActionResult RemoveItem(string menuItemName)
        {
            ISession session = HttpContext.Session;

            if (string.IsNullOrEmpty(menuItemName) || string.IsNullOrEmpty(session.GetString(menuItemName)))
            {
                return RedirectToAction("Index", "Cart");
            }

            session.Remove(menuItemName);

            return RedirectToAction("Index", "Cart");
        }
    }
}
