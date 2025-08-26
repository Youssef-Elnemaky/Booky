using Booky.Models;
using Microsoft.AspNetCore.Mvc;
using Booky.DataAccess.Repositries.IRepository;
using Microsoft.AspNetCore.Authorization;
using Booky.Utility;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categoryList = unitOfWork.Category.GetAll().OrderBy(c => c.DisplayOrder).ToList();

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
                unitOfWork.Category.Add(newCategory);
                unitOfWork.Category.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if(id == 0) return NotFound();

            var model = unitOfWork.Category.Get(c => c.Id == id);
            if(model == null) return NotFound();

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Category.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            return View("Edit");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id == 0) return NotFound();
            
            var model = unitOfWork.Category.Get(c=>c.Id == id);
            if(model == null) return NotFound();
            
            return View("Delete", model);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            var category = unitOfWork.Category.Get(c=>c.Id==id);
            unitOfWork.Category.Delete(category);
            unitOfWork.Category.Save();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
