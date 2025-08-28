using Booky.DataAccess.Repositries;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IActionResult Index()
        {

            return View("Index");
        }

        [HttpGet]
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new OrderVM
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderHeader.Id == orderId, includeProperties: "Product")
            };
            return View("Details", orderVM);
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            // we need to update the database with the new values
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader
                .Get(o => o.Id == orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            var refreshedOrderVM = new OrderVM
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(
                                o => o.Id == orderVM.OrderHeader.Id,
                                includeProperties: "ApplicationUser"),

                OrderDetails = _unitOfWork.OrderDetail.GetAll(
                                o => o.OrderHeaderId == orderVM.OrderHeader.Id,
                            includeProperties: "Product")
            };

            return View("Details", refreshedOrderVM);

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfuly.";

            return RedirectToAction("Details", new {orderId = orderVM.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            OrderHeader orderHeaderDB = _unitOfWork.OrderHeader.Get(o => o.Id == orderVM.OrderHeader.Id);
            orderHeaderDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeaderDB.Carrier = orderVM.OrderHeader.Carrier;
            orderHeaderDB.OrderStatus = SD.StatusShipped;
            orderHeaderDB.ShippingDate = DateTime.Now;
            if(orderHeaderDB.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderDB.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeaderDB);
            _unitOfWork.Save();

            TempData["success"] = "Order Shipped Successfuly.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderVM.OrderHeader.Id);

            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            } else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            _unitOfWork.Save();
            TempData["success"] = "Order Canceled Successfuly.";
            return RedirectToAction("Details", new { orderId = orderHeader.Id });
        }

        public IActionResult PaymentConfirmation(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderId);
            if (orderHeader == null) return NotFound();

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
    
            }


            return View("PaymentConfirmation", orderId);
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_Pay_Now(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderVM.OrderHeader.Id);
            var orderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product");

            //StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            var domain = "http://localhost:5214/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.price * 100),
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

            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            IEnumerable<OrderHeader> orderHeaders;

            //get all the orders if the user is an admin or employee
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                //get the orders of the signed in user
                orderHeaders = _unitOfWork.OrderHeader.GetAll(o=>o.ApplicationUserId == userId,
                    includeProperties: "ApplicationUser");
            }

            
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                        break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }
    }
}
