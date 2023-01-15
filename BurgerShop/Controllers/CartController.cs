using BurgerShop.Data;
using BurgerShop.Models.DataModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers
{
    [Authorize]
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPurchaseRepository _purchaseRepository;

        public CartController(IMenuRepository menuRepository, IOrderRepository orderRepository, IPurchaseRepository purchaseRepository)
        {
            _menuRepository = menuRepository;
            _orderRepository = orderRepository;
            _purchaseRepository = purchaseRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult> Index()
        {
            return View(await Order.FromSession(HttpContext, _menuRepository));
        }

        [HttpPost("additem")]
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

        [HttpPost("removeitem")]
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

        [HttpGet("submit")]
        public async Task<ActionResult> Submit()
        {
            ISession session = HttpContext.Session;

            if (session.Keys.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            Order order = await Order.FromSession(HttpContext, _menuRepository);

            await _orderRepository.CreateOrderAsync(order);
            await _purchaseRepository.CreatePurchaseAsync(order);

            session.Clear();

            return RedirectToAction("Index", "Menu");
        }
    }
}
