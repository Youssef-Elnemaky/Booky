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
        public IActionResult Upsert(int? id)
        {   
            var productVM = new ProductVM();
            productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });

            // creating (inserting) a new product
            if (id == null || id == 0)
            {
                productVM.Product = new Product();
            } else
            {
                productVM.Product = unitOfWork.Product.Get(p => p.Id == id);
            }

            return View("Upsert", productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                // .Update is able to check the id
                // If the id exists, it will update if not, it will create a new index
                unitOfWork.Product.Update(productVM.Product);
                // Notification
                if (productVM.Product.Id == 0) TempData["success"] = "Product created successfully.";
                else TempData["success"] = "Product updated successfully.";

                unitOfWork.Save();

                return RedirectToAction("Index");
            } else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View("Create", productVM);
            }
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
