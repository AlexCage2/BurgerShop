using BurgerShop.Data;
using BurgerShop.Models.ViewModels.Burgers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers.Admin
{
    [Authorize]
    [Route("admin/burgers")]
    public class BurgersController : Controller
    {
        private readonly IBurgerRepository _burgerRepository;
        private readonly IRecipesRepository _recipesRepository;

        public BurgersController(IBurgerRepository burgerRepository, IRecipesRepository recipesRepository)
        {
            _burgerRepository = burgerRepository;
            _recipesRepository = recipesRepository;
        }

        // GET: Index
        [HttpGet("overview")]
        public async Task<ActionResult> Index(
            string burgerName,
            int linesPerPage = 5,
            int pageNumber = 1,
            string sortOrder = "NameAsc"
            )
        {
            var filterViewModel = new BurgerFilterViewModel
            {
                BurgerName = burgerName
            };

            var paginatorViewModel = new BurgerPaginatorViewModel
            {
                LinesPerPage = linesPerPage,
                PageNumber = pageNumber
            };

            var sortViewModel = new BurgerItemsSortViewModel(sortOrder);

            var viewModel = new BurgerViewModel
            {
                Burgers = _burgerRepository.GetBurgersAsync(),
                FilterViewModel = filterViewModel,
                PaginatorViewModel = paginatorViewModel,
                SortViewModel = sortViewModel
            };
            return View(viewModel);
        }
    }
}
