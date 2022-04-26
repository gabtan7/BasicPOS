using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        ApplicationDbContext _db;
        public StockRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Stock obj)
        {
            _db.Stocks.Update(obj);
        }
        public decimal DecrementStock(Stock obj, decimal quantity)
        {
            obj.Quantity -= quantity;
            return obj.Quantity;
        }
    }
}
