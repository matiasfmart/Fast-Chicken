using FastChicken.DBConnection;
using FastChicken.Interfaces;
using FastChicken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastChicken.Controllers.Api
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private IMySqlRepository _mySqlRepository;
        public AdminController(ILogger<AdminController> logger, IMySqlRepository mySqlRepository)
        {
            _logger = logger;
            _mySqlRepository = mySqlRepository;
        }

        [HttpPost]
        public ResultModel ActualizaStockBebidas(IList<Drink> drinkValues)
        {
            var result = new ResultModel();

            try
            {
                foreach( var drink in drinkValues )
                {
                    this._mySqlRepository.UpdateDrinksQty(drink.idDrink, drink.Quantity);
                }

                result.Status = "OK";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = ex.Message;
            }
  
            return result;
        }

        [HttpPost]
        public ResultModel ActualizaStockGuarniciones(IList<Side> sideValues)
        {
            var result = new ResultModel();

            try
            {
                foreach (var side in sideValues)
                {
                    this._mySqlRepository.UpdateSidesQty(side.idSide, side.Quantity);
                }

                result.Status = "OK";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = ex.Message;
            }

            return result;
        }

        [HttpPost]
        public ResultModel ActualizaStockProductos(IList<Product> productValues)
        {
            var result = new ResultModel();

            try
            {
                foreach (var product in productValues)
                {
                    this._mySqlRepository.UpdateProductsQty(product.idProduct, product.Quantity);
                }

                result.Status = "OK";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = ex.Message;
            }

            return result;
        }
        [HttpPost]
        public ResultModel GrabarCombo(Combo combo)
        {
            var result = new ResultModel();

            try
            {
                this._mySqlRepository.GrabarCombo(combo);

                result.Status = "OK";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
