using BasicPOS.DataAccess.Data;
using BasicPOS.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICartRepository Cart { get; private set; }
        public IItemRepository Item { get; private set; }
        public IOrderLineRepository OrderLine { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IStockRepository Stock { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Cart = new CartRepository(_db);
            Item = new ItemRepository(_db);
            Order = new OrderRepository(_db);
            OrderLine = new OrderLineRepository(_db);
            Stock = new StockRepository(_db);
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
