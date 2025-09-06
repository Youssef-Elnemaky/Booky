using Booky.DataAccess.Repositries;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            string? userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                //logged in case
                int? sessionCartCount = HttpContext.Session.GetInt32(SD.SessionCart);
                if (sessionCartCount == null)
                {
                    sessionCartCount = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId).Count();
                    HttpContext.Session.SetInt32(SD.SessionCart, (int)sessionCartCount);
                    
                }
                return View(sessionCartCount);
            } else
            {
                //logged out case
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
