using Booky.DataAccess.Repositries;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area(areaName: "Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = unitOfWork.Product.GetAll(includeProperties: "Category");
            return View("Index", products);
        }

        [HttpGet]
        public IActionResult Details(int productId)
        {
            var product = unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category");
            if(product == null) return NotFound();
            
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                Count = 1,
                ProductId = productId
            };
            
            return View("Details", cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            var shoppingCartDb = unitOfWork.ShoppingCart.Get(sc => 
                sc.ApplicationUserId == userId && sc.ProductId == shoppingCart.ProductId);

            if(shoppingCartDb == null)
            {
                // create a new one
                unitOfWork.ShoppingCart.Add(shoppingCart);
            } else
            {
                // update it
                shoppingCartDb.Count += shoppingCart.Count;
            }
            unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            return RedirectToAction("Index");
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
    }
}
