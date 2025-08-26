using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId,
                includeProperties: "Product")
            };

            //calculating total price
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View("Index", shoppingCartVM);
        }

        private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50) return shoppingCart.Product.Price;
            else if (shoppingCart.Count <= 100) return shoppingCart.Product.Price50;
            else return shoppingCart.Product.Price100;
        }
    }
}
