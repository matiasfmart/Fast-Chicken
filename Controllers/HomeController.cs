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
            return View();
        }

        [HttpGet]
        public IActionResult GetCombos()
        {
            List<Combo> combos = _mySqlRepository.GetCombos();

            return PartialView("_CombosList",combos);
        }

        public IActionResult LoadComboModal(string name, string type, string price)
        {
            Combo combo = new Combo();
            combo.Name = name;
            combo.Type = type;
            combo.Price = price;

            return PartialView("_ModalCombo", combo);
        }

        public IActionResult AddOrderItem(List<OrderItem> items) {
            return PartialView("_OrderItems", items);
        }

        //[HttpGet]
        //public IActionResult GetComboDetail()
        //{
        //    List<Combo> combos = _mySqlRepository.GetCombos();

        //    return PartialView("_CombosList", combos);
        //}

        public IActionResult GetOrderDetail()
        {
            Order order = new Order();

            //traer las ordenes

            return PartialView("_OrderDetail", order);
        }

        public bool TerminarPedido()
        {
            bool success = false;

            //terminar pedido

            return success;
        }

    }
}