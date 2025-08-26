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

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileService fileService;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var companies = unitOfWork.Company.GetAll();

            return View("Index", companies);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company;

            // creating (inserting) a new product
            if (id == null || id == 0)
            {
                company = new Company();
            } else
            {
                company = unitOfWork.Company.Get(c => c.Id == id);
            }

            return View("Upsert", company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {

                if (company.Id == 0)
                {
                    unitOfWork.Company.Add(company);
                    // Notification
                    TempData["success"] = "Company created successfully.";
                }
                else 
                { 
                    unitOfWork.Company.Update(company);
                    // Notification
                    TempData["success"] = "Company updated successfully.";
                }

                unitOfWork.Save();

                return RedirectToAction("Index");
            } else
            {
                
                return View("Upsert", company);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companies = unitOfWork.Company.GetAll().ToList();
            return Json(new {data = companies});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = unitOfWork.Company.Get(c => c.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            
            unitOfWork.Company.Delete(companyToBeDeleted);
            unitOfWork.Save();

            return Json(new { success = true, message = "Company deleted successfully." });
        }
    }
}
