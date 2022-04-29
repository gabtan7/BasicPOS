using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using BasicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order obj)
        {
            obj.UpdatedDate = DateTime.Now;
            _db.Orders.Update(obj);
        }
        public void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId, decimal orderTotal)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);

            if (orderFromDb != null)
            {
                orderFromDb.UpdatedDate = DateTime.Now;
                orderFromDb.OrderTotal = orderTotal;
                orderFromDb.SessionId = sessionId;
                orderFromDb.PaymentIntentId = paymentIntentId;
            }
        }
        public void UpdateStatus(int id, string status)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);

            if (orderFromDb != null)
            {
                if (status == SD.OrderStatus_Paid)
                    orderFromDb.PaymentDate = DateTime.Now;
                else if(status == SD.OrderStatus_Done)
                    orderFromDb.DateApproved = DateTime.Now;

                orderFromDb.UpdatedDate = DateTime.Now;
                orderFromDb.OrderStatus = status;
            }
        }
    }
}
