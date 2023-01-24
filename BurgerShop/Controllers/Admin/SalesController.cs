using BurgerShop.Data;
using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerShop.Controllers.Admin
{
    [Authorize]
    [Route("admin/sales")]
    public class SalesController : Controller
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IOrderRepository _orderRepository;

        public SalesController(IPurchaseRepository purchaseRepository, IOrderRepository orderRepository)
        {
            _purchaseRepository = purchaseRepository;
            _orderRepository = orderRepository;
        }

        // GET: Index
        [HttpGet("overview")]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken,
            string sortOrderForItemsGroup,
            string sortOrderForDaysGroup,
            string sortOrderForOperationsGroup,
            string startDateForItemsGroup = "2020-01-01",
            string endDateForItemsGroup = "2025-01-01",
            string startDateForDaysGroup = "2020-01-01",
            string endDateForDaysGroup = "2025-01-01",
            string startDateForOperationsGroup = "2020-01-01",
            string endDateForOperationsGroup = "2025-01-01",
            string userName = "",
            int pageNumberForItemsGroup = 1,
            int linesPerPageForItemsGroup = 5,
            int pageNumberForDaysGroup = 1,
            int linesPerPageForDaysGroup = 5,
            int pageNumberForOperationsGroup = 1,
            int linesPerPageForOperationsGroup = 5
            )
        {
            var filterViewModel = new SalesFilterViewModel
            {
                StartDateForItemsGroup = startDateForItemsGroup,
                EndDateForItemsGroup = endDateForItemsGroup,
                StartDateForDaysGroup = startDateForDaysGroup,
                EndDateForDaysGroup = endDateForDaysGroup,
                StartDateForOperationsGroup = startDateForOperationsGroup,
                EndDateForOperationsGroup = endDateForOperationsGroup,
                UserName = userName
            };

            var paginatorViewModel = new SalesPaginatorViewModel
            {
                PageNumberForItemsGroup = pageNumberForItemsGroup,
                LinesPerPageForItemsGroup = linesPerPageForItemsGroup,
                PageNumberForDaysGroup = pageNumberForDaysGroup,
                LinesPerPageForDaysGroup = linesPerPageForDaysGroup,
                PageNumberForOperationsGroup = pageNumberForOperationsGroup,
                LinesPerPageForOperationsGroup = linesPerPageForOperationsGroup
            };

            var itemsSortViewModel = new SalesItemsSortViewModel(sortOrderForItemsGroup);
            var daysSortViewModel = new SalesDaysSortViewModel(sortOrderForDaysGroup);
            var operationsSortViewModel = new SalesOperationsSortViewModel(sortOrderForOperationsGroup);

            var viewModel = new SalesViewModel
            {
                Orders = await _orderRepository.GetItemsAsync(DateOnly.Parse(startDateForOperationsGroup), DateOnly.Parse(endDateForOperationsGroup), userName, linesPerPageForOperationsGroup, pageNumberForOperationsGroup, sortOrderForOperationsGroup, cancellationToken),
                SalesByItems = await _purchaseRepository.GetSalesByItemsAsync(DateOnly.Parse(startDateForItemsGroup), DateOnly.Parse(endDateForItemsGroup), linesPerPageForItemsGroup, pageNumberForItemsGroup, sortOrderForItemsGroup, cancellationToken),
                SalesByDays = await _purchaseRepository.GetSalesByDaysAsync(DateOnly.Parse(startDateForDaysGroup), DateOnly.Parse(endDateForDaysGroup), linesPerPageForDaysGroup, pageNumberForDaysGroup, sortOrderForDaysGroup, cancellationToken),
                SalesItemsSortViewModel = itemsSortViewModel,
                SalesDaysSortViewModel = daysSortViewModel,
                SalesOperationsSortViewModel = operationsSortViewModel,
                FilterViewModel = filterViewModel,
                PaginatorViewModel = paginatorViewModel
            };

            return View(viewModel);
        }

        // HttpGet: Details
        [HttpGet("details/{orderId}")]
        public async Task<IActionResult> Details(Guid? orderId, CancellationToken cancellationToken)
        {
            if (orderId is null)
            {
                return NotFound();
            }

            SaleInfo saleInfo = await SaleInfo.FromSaleAsync(orderId.Value, _purchaseRepository, _orderRepository, cancellationToken);
            return View(saleInfo);
        }
    }
}
 