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
    }
}
