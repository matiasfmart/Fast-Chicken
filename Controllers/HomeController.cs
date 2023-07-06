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
            Combo combo = null;
            List<Combo> combos = _mySqlRepository.GetCombos();

            IList<Side> sides = _mySqlRepository.getSides();
            IList<Product> products = _mySqlRepository.getProducts();
            //IList<Drink> drinks = _mySqlRepository.getDrinks();

            foreach ( var side in sides)
            {
                combo = new Combo();

                combo.Name = side.Name;
                combo.ComboId = side.idSide.ToString();
                combo.Description = "";
                combo.Type = "ES";
                combo.Price = side.Price.ToString();

                combos.Add(combo);
            }

            foreach (var product in products)
            {
                combo = new Combo();

                combo.Name = product.Name;
                combo.ComboId = product.idProduct.ToString();
                combo.Description = "";
                combo.Type = "EP";

                combo.Price = product.Price.ToString();

                combos.Add(combo);
            }

            Combo comboBebidaChica = new Combo();

            comboBebidaChica.Name = "Bebidas Chicas";
            comboBebidaChica.ComboId = "D";
            comboBebidaChica.Description = "";
            comboBebidaChica.Type = "E";
            comboBebidaChica.Price = "350";

            combos.Add(comboBebidaChica);

            Combo comboBebidaGrande = new Combo();

            comboBebidaGrande.Name = "Bebidas Grandes";
            comboBebidaGrande.ComboId = "DG";
            comboBebidaGrande.Description = "";
            comboBebidaGrande.Type = "E";
            comboBebidaGrande.Price = "900";

            combos.Add(comboBebidaGrande);

            return PartialView("_CombosList",combos);
        }

        public IActionResult LoadComboModal(Combo combo)
        {

            ViewBag.SideOutStock = false;
            ViewBag.DrinkOutStock = false;
            ViewBag.ProductOutStock = false;

            switch ( combo.Type )
            {
                case "PO":
                case "BG":
                    ViewBag.Drinks = this._mySqlRepository.getDrinksByCombo(combo.ComboId);
                    ViewBag.Sides = this._mySqlRepository.getSidesByCombo(combo.ComboId);

                    IList<Product> products = this._mySqlRepository.getProductsByCombo(combo.ComboId);

                    for(int i=0; i < products.Count; i++)
                    {
                        int quantity = this._mySqlRepository.GetComboQuantity(combo.ComboId, products[i].idProduct, "P");

                        if( quantity > products[i].Quantity )
                        {
                            ViewBag.ProductOutStock = true;
                        }
                    }

                    break;
                case "E":
                    if (combo.ComboId == "D")
                    {
                        ViewBag.Drinks = this._mySqlRepository.getDrinks();
                        ViewBag.Sides = new List<Side>();
                    }
                    else
                    {
                        ViewBag.Drinks = this._mySqlRepository.getBigDrinks();
                        ViewBag.Sides = new List<Side>();
                    }
            break;
                case "ES":
                    Side side = this._mySqlRepository.getSide(Convert.ToInt32(combo.ComboId));

                    if(side != null && side.Quantity <= 0 )
                    {
                        ViewBag.SideOutStock = true;
                    }
                    ViewBag.Drinks = new List<Drink>();
                    ViewBag.Sides = new List<Side>();
                    break;
                case "EP":
                    Product product = this._mySqlRepository.getProduct(Convert.ToInt32(combo.ComboId));

                    if (product!= null && product.Quantity <= 0)
                    {
                        ViewBag.ProductOutStock = true;
                    }
                    ViewBag.Drinks = new List<Drink>();
                    ViewBag.Sides = new List<Side>();
                    break;
            }

            return PartialView("_ModalCombo", combo);
        }

        public IActionResult AddOrderItems(List<OrderItem> items)
        {

            var total = 0;
            foreach (var item in items)
            {
                total = total + (item.Count * int.Parse(item.Price));
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

        public IActionResult FinishOrder(List<OrderItem> items, int total, int orderNum, bool hereToGo)
        {
            Order newOrder = new Order();

            orderNum = _mySqlRepository.getNewNumOrder();

            newOrder.Total = total;
            newOrder.Date = DateTime.Now;
            newOrder.OrderNum = orderNum;
            newOrder.Items = items;
            newOrder.HereToGo = hereToGo;

            var idOrder = _mySqlRepository.AddOrder(newOrder);

            foreach( var item in items)
            {
                item.OrderId = idOrder;
                _mySqlRepository.AddOrderItem(item);

                switch(item.Type)
                {
                    case "BG":
                    case "PO":
                        int quantity = this._mySqlRepository.GetComboQuantity(item.ComboId, item.idDrink, "D");

                        _mySqlRepository.DecrementDrink(item.idDrink, quantity);

                        quantity = this._mySqlRepository.GetComboQuantity(item.ComboId, item.idSide, "S");

                        _mySqlRepository.DecrementSide(item.idSide, quantity);

                        IList<Product> products = this._mySqlRepository.getProductsByCombo(item.ComboId);

                        foreach( var product in products)
                        {
                            quantity = this._mySqlRepository.GetComboQuantity(item.ComboId, product.idProduct, "P");
                            _mySqlRepository.DecrementProduct(product.idProduct, quantity);
                        }

                        break;
                    case "EP":
                        _mySqlRepository.DecrementProduct(Convert.ToInt32(item.ComboId), 1);
                        break;
                    case "ES":
                        _mySqlRepository.DecrementSide(Convert.ToInt32(item.ComboId), 1);
                        break;
                    case "E":
                        _mySqlRepository.DecrementDrink(item.idDrink, 1);
                        break;
                }
            }

            //setear combos pedidos a tabla en sql...
            return PartialView("_FinishOrder", newOrder);
        }

        public IActionResult FinishJournal()
        {
            TotalJournal totalJournal = new TotalJournal();

            int idJournal = _mySqlRepository.endCashJournal();
            totalJournal = _mySqlRepository.GetTotalJournal(idJournal);

            //RedirectToAction("Index", "Home");
            return PartialView("_FinishJournal", totalJournal);
        }

    }
}