using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;


        public CartController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            //calculating total price
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View("Index", shoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);
            if (shoppingCart == null) return NotFound();

            shoppingCart.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);
            if (shoppingCart == null) return NotFound();

            shoppingCart.Count -= 1;
            if (shoppingCart.Count <= 0) shoppingCart.Count = 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);
            if (shoppingCart == null) return NotFound();

            _unitOfWork.ShoppingCart.Delete(shoppingCart);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult Summary()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId,
               includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            //calculating total price
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View("Summary", shoppingCartVM);
        }

        [HttpPost]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM)
        {
            //user submits after the summary 
            //what do we need to do?
            //order header that will contain most information about the order
            //order detail(s) each order detail will contain information about the line item
            //we need to make a payment so we need to initiate a payment session with stripe

            //get the user
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;


            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId,
                    includeProperties: "Product");

            //make a break point an try to understand what values you get from the shoppingCartVM
            //we need to add: applicationuserid order header id is 0 paymentstatus order status order total payment date? paymentdue date
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ApplicationUser appUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            //calculating total price
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                // normal user not a company
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //company user state
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    price = item.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                // initiate a stripe session
                Console.WriteLine("Stripe key loaded? " + (_config["Stripe:SecretKey"] != null));
                StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
                var domain = "http://localhost:5214/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach(var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction("OrderConfirmation", new { shoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == id);
            if (orderHeader == null) return NotFound();

            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
                _unitOfWork.Save();
            }

            
            return View("OrderConfirmation", id);
        }

        private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50) return shoppingCart.Product.Price;
            else if (shoppingCart.Count <= 100) return shoppingCart.Product.Price50;
            else return shoppingCart.Product.Price100;
        }
    }
}
