using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = unitOfWork.Product.GetAll();

            return View("Index", products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Add(product);
                unitOfWork.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            return View("Create");
        }


    }
}
