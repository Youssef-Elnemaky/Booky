using BookyWeb.Models;
using BookyWeb.Repositries;
using Microsoft.AspNetCore.Mvc;

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
            var categoryList = categoryRepo.GetAll().OrderBy(c=>c.DisplayOrder);

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
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if(id == 0) return NotFound();
            
            var model = categoryRepo.GetById(id);
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
                return RedirectToAction("Index");
            }

            return View("Edit");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id == 0) return NotFound();
            
            var model = categoryRepo.GetById(id);
            if(model == null) return NotFound();
            
            return View("Delete", model);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            categoryRepo.Delete(id);
            categoryRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
