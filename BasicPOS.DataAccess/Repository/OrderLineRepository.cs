using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class OrderLineRepository : Repository<OrderLine>, IOrderLineRepository
    {
        ApplicationDbContext _db;
        public OrderLineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderLine obj)
        {
            _db.OrderLines.Update(obj);
        }
    }
}
