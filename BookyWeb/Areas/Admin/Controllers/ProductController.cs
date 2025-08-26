using Booky.DataAccess.Repositries;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Booky.Services;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = SD.Role_Admin)]

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
            var products = unitOfWork.Product.GetAll(includeProperties: "Category");

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

                if (productVM.Product.Id == 0)
                {
                    unitOfWork.Product.Add(productVM.Product);
                    // Notification
                    TempData["success"] = "Product created successfully.";
                }
                else 
                { 
                    unitOfWork.Product.Update(productVM.Product);
                    // Notification
                    TempData["success"] = "Product updated successfully.";
                }

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
        public IActionResult GetAll()
        {
            List<Product> products = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            fileService.Delete(productToBeDeleted.ImageUrl);

            unitOfWork.Product.Delete(productToBeDeleted);
            unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
