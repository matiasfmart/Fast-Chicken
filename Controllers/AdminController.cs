using System.Security.Claims;
using FastChicken.Interfaces;
using FastChicken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


using Microsoft.AspNetCore.Mvc;

namespace FastChicken.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private IMySqlRepository _mySqlRepository;

        public AdminController(ILogger<AdminController> logger, IMySqlRepository mySqlRepository)
        {
            _logger = logger;
            _mySqlRepository = mySqlRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel loginModel = new LoginViewModel();

            return View(loginModel);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Login(LoginViewModel loginModel)
        {
            if( this._mySqlRepository.checkPassword(loginModel.User, loginModel.Password) )
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.User)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = loginModel.RememberMe
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Index", "Admin");
            } else
            {
                loginModel.errorMessage = "Usuario inválido";
            }

            return View(loginModel);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Admin");
        }
        public IActionResult Productos()
        {
            return View();
        }
        public IActionResult ListaProductos()
        {
            IList<Product> products = this._mySqlRepository.getProducts();

            ViewBag.Products = products;

            return View();
        }
        public IActionResult Bebidas()
        {
            return View();
        }
        public IActionResult ListaBebidas()
        {
            IList<Drink> drinks = this._mySqlRepository.getDrinks();

            ViewBag.Drinks = drinks;

            return View();
        }
        public IActionResult ListaCombos()
        {
            IList<Combo> combos = this._mySqlRepository.getCombos();

            ViewBag.Combos = combos;

            return View();
        }
        public IActionResult Guarniciones()
        {
            return View();
        }
        public IActionResult ListaGuarniciones()
        {
            IList<Side> sides = this._mySqlRepository.getSides();

            ViewBag.Sides = sides;

            return View();
        }
        public IActionResult Combos()
        {
            return View();
        }
        public IActionResult Combo(string id)
        {
            IList<Side> sides = this._mySqlRepository.getSides();

            ViewBag.Sides = sides;

            IList<Drink> drinks = this._mySqlRepository.getDrinks();

            ViewBag.Drinks = drinks;

            IList<Product> products = this._mySqlRepository.getProducts();

            ViewBag.Products = products;

            ViewBag.DictProductos = this._mySqlRepository.getQuantityById(id, "P");
            ViewBag.DictGuarniciones = this._mySqlRepository.getQuantityById(id, "S");
            ViewBag.DictBebidas = this._mySqlRepository.getQuantityById(id, "D");

            foreach( var producto in ViewBag.DictProductos )
            {
                ViewBag.CantidadProductos = producto.Value;
            }

            foreach (var guarnicion in ViewBag.DictGuarniciones)
            {
                ViewBag.CantidadGuarniciones = guarnicion.Value;
            }

            foreach (var bebida in ViewBag.DictBebidas)
            {
                ViewBag.CantidadBebidas = bebida.Value;
            }

            Combo combo = this._mySqlRepository.GetCombo(id);

            if( combo != null)
            {
                ViewBag.Combo = combo;
            }

            return View();
        }
    }
}
