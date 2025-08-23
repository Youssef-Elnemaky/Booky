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

        public IActionResult Index()
        {
            var categoryList = categoryRepo.GetAll();

            return View("Index", categoryList);
        }
    }
}
