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
            var categoryList = categoryRepo.GetAll();

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

    }
}
