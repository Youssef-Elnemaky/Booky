using Booky.Models;
using Microsoft.AspNetCore.Mvc;
using Booky.DataAccess.Repositries.IRepository;

namespace BookyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categoryList = categoryRepo.GetAll().OrderBy(c => c.DisplayOrder).ToList();

            return View("Index", categoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Add(newCategory);
                categoryRepo.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if(id == 0) return NotFound();

            var model = categoryRepo.Get(c => c.Id == id);
            if(model == null) return NotFound();

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Update(category);
                categoryRepo.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            return View("Edit");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id == 0) return NotFound();
            
            var model = categoryRepo.Get(c=>c.Id == id);
            if(model == null) return NotFound();
            
            return View("Delete", model);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            var category = categoryRepo.Get(c=>c.Id==id);
            categoryRepo.Delete(category);
            categoryRepo.Save();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
