using BurgerShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers
{
    [Authorize]
    [Route("menu")]
    public class MenuController : Controller
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IBurgerRepository _burgerRepository;

        public MenuController(IMenuRepository menuRepository, IBurgerRepository burgerRepository)
        {
            _menuRepository = menuRepository;
            _burgerRepository = burgerRepository;
        }

        // GET: Burgers
        [HttpGet("")]
        public async Task<ActionResult> Index()
        {
            return View(await _menuRepository.GetMenuItemsAsync());
        }

        // GET: Burgers/Details/5
        [HttpGet("details/{burgerName}")]
        public async Task<ActionResult> Details(string burgerName)
        {
            return View(await _burgerRepository.GetBurgerAsync(burgerName));
        }
    }
}
