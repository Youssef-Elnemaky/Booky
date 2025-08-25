using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Booky.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace BookyWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileService fileService;

        public ProductController(IUnitOfWork unitOfWork, IFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
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
            PopulateCateogryList(productVM);

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
            //creating and empty image upload
            if (productVM.Product.Id == 0 && productVM.File == null)
            {
                ModelState.AddModelError("File", "Must upload an image when creating a product.");
            }

            if (ModelState.IsValid)
            {
                if (productVM.File != null)
                {
                    productVM.Product.ImageUrl = fileService.Replace(productVM.Product.ImageUrl, productVM.File, "images/products");
                }
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
                PopulateCateogryList(productVM);
                return View("Upsert", productVM);
            }
        }

        private void PopulateCateogryList(ProductVM productVM)
        {
            productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
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
