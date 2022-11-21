using FastChicken.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FastChicken.Interfaces;
using FastChicken.DBConnection;

namespace FastChicken.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMySqlRepository _mySqlRepository;

        public HomeController(ILogger<HomeController> logger, IMySqlRepository mySqlRepository)
        {
            _logger = logger;
            _mySqlRepository = mySqlRepository;
        }

        public int orderId = 0;
        public int totalOrder = 0;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Order()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCombos()
        {
            List<Combo> combos = _mySqlRepository.GetCombos();

            return PartialView("_CombosList",combos);
        }

        public IActionResult LoadComboModal(Combo combo)
        {
            return PartialView("_ModalCombo", combo);
        }

        public IActionResult AddOrderItems(List<OrderItem> items) {

            var total = 0;
            foreach (var item in items)
            {
                total = total + int.Parse(item.Price);
            }

            ViewBag.Total = total;
            totalOrder = total;

            return PartialView("_OrderItems", items);
        }

        public IActionResult GetOrderDetail()
        {
            Order order = new Order();

            //traer las ordenes

            return PartialView("_OrderDetail", order);
        }

        public IActionResult FinishOrder(List<OrderItem> items)
        {
            Order newOrder = new Order();

            newOrder.Id = orderId;
            newOrder.Total = totalOrder;
            newOrder.Date = DateTime.Now;

            _mySqlRepository.AddOrder(newOrder);

            //setear combos pedidos a tabla en sql...
            totalOrder = 0;
            orderId++;
            return PartialView("_FinishOrder");
        }

        public void FinishDay()
        {
            orderId = 0;
            ViewBag.Total = 0;
            RedirectToAction("Index", "Home");
        }
    }
}