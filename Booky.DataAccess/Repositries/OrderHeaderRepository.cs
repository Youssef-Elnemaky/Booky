using Booky.DataAccess.Data;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repositries
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _context;

        public OrderHeaderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _context.orderHeaders.FirstOrDefault(o => o.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrWhiteSpace(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _context.orderHeaders.FirstOrDefault(o => o.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }

            }
        }
    }
}
