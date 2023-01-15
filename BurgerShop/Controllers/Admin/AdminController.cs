using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers.Admin
{
    [Authorize]
    [Route("admin")]
    public class AdminController : Controller
    {
        // GET: Sales
        [HttpGet("sales")]
        public IActionResult Sales()
        {
            return RedirectToAction("Index", "Sales");
        }

        // GET: Marketing
        [HttpGet("marketing")]
        public IActionResult Marketing()
        {
            return RedirectToAction("Index", "Marketing");
        }

        // GET: Warehouse
        [HttpGet("warehouse")]
        public IActionResult Warehouse()
        {
            return RedirectToAction("Index", "Warehouse");
        }

        // GET: Supplies
        [HttpGet("purchases")]
        public IActionResult Supplies()
        {
            return RedirectToAction("Index", "Supplies");
        }
    }
}
