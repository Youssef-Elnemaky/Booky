using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var productVM = new ProductVM();
            productVM.Product = new Product();
            productVM.CategoryList = unitOfWork.Category.GetAll()
                .Select(c=>new SelectListItem { Value = c.Id.ToString(), Text = c.Name});

            return View("Create", productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Add(productVM.Product);
                unitOfWork.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index");
            } else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View("Create", productVM);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();

            var product = unitOfWork.Product.Get(p => p.Id == id);
            if(product == null) return NotFound();

            return View("Edit", product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if(ModelState.IsValid)
            {
                unitOfWork.Product.Update(product);
                unitOfWork.Save();
                TempData["success"] = "Product updated successfully.";
                return RedirectToAction("Index");
            }

            return View("Edit", product);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id == 0) return NotFound();

            var product = unitOfWork.Product.Get(p=>p.Id == id);
            if(product == null) return NotFound();

            return View("Delete", product);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            if (id == 0) return NotFound();

            var product = unitOfWork.Product.Get(p=>p.Id ==id);
            if(product == null) return NotFound();

            unitOfWork.Product.Delete(product);
            unitOfWork.Save();
            TempData["success"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
