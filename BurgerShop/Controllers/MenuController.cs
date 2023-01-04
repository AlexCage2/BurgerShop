using BurgerShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers
{
    [Authorize]
    [Route("menu")]
    public class MenuController : Controller
    {
        private readonly MenuContext _menuContext;
        public MenuController(MenuContext menuContext)
        {
            _menuContext = menuContext;
        }

        // GET: Burgers
        [HttpGet("")]
        public async Task<ActionResult> Index()
        {
            return View(await _menuContext.GetMenuItemsAsync());
        }

        // GET: Burgers/Details/5
        [HttpGet("details/{burgerName}")]
        public async Task<ActionResult> Details(string burgerName)
        {
            return View(await _menuContext.GetBurgerAsync(burgerName));
        }
    }
}
