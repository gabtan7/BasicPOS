using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Cart obj)
        {
            _db.Carts.Update(obj);
        }
    }
}
