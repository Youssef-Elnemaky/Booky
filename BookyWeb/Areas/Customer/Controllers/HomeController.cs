using System.Diagnostics;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Customer.Controllers
{
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

        public IActionResult Details(int productId)
        {
            var product = unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category");
            return View("Details", product);
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
