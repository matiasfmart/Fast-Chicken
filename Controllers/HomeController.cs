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

            _mySqlRepository.startCashJournal();

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

            return PartialView("_OrderItems", items);
        }

        public IActionResult GetOrderDetail()
        {
            Order order = new Order();

            //traer las ordenes

            return PartialView("_OrderDetail", order);
        }

        public IActionResult ConfirmFinishOrder()
        {
            return PartialView("_ModalConfirmFinishOrder");
        }

        public IActionResult FinishOrder(List<OrderItem> items, int total, int orderNum)
        {
            Order newOrder = new Order();

            orderNum = _mySqlRepository.getNewNumOrder();

            newOrder.Total = total;
            newOrder.Date = DateTime.Now;
            newOrder.OrderNum = orderNum;
            newOrder.Items = items;

            var idOrder = _mySqlRepository.AddOrder(newOrder);

            foreach( var item in items)
            {
                item.OrderId = idOrder;
                _mySqlRepository.AddOrderItem(item);
            }

            //setear combos pedidos a tabla en sql...
            return PartialView("_FinishOrder", newOrder);
        }

        public IActionResult FinishDay()
        {
            //orderId = 0;
            //ViewBag.Total = 0;
            _mySqlRepository.endCashJournal();
            
            //RedirectToAction("Index", "Home");
            return View("Index");
        }
    }
}