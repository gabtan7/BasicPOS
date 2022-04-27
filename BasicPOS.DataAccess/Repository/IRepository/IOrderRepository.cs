using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order obj);
        void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId, decimal orderTotal);
        void UpdateStatus(int id, string status);
    }
}
