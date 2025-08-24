using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookyWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
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
                if (productVM.File != null)
                {
                    // Generate unique filename
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productVM.File.FileName);

                    // Get path to wwwroot/images/products (for example)
                    string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "products");

                    // Ensure folder exists
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    // Combine path and filename
                    string filePath = Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        productVM.File.CopyTo(fileStream);
                    }

                    // Store relative path in DB
                    productVM.Product.ImageUrl = "/images/products/" + fileName;
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
                productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View("Upsert", productVM);
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
