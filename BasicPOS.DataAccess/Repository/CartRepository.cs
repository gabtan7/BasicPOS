using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using BasicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            obj.UpdatedDate = DateTime.Now;
            _db.Carts.Update(obj);
        }
        public int IncrementCount(Cart obj, int quantity)
        {
            obj.UpdatedDate = DateTime.Now;
            obj.Quantity += quantity;
            return obj.Quantity;
        }
        public int DecrementCount(Cart obj, int quantity)
        {
            obj.UpdatedDate = DateTime.Now;
            obj.Quantity -= quantity;
            return obj.Quantity;
        }
        public void UpdateCartItemStatus(Cart obj, string status)
        {
            obj.Status = status;
            _db.Carts.Update(obj);
        }

        public void UpdateCartItemStatus(IEnumerable<Cart> cartList, string status)
        {
            foreach(var cart in cartList)
            {
                cart.Status = status;
                _db.Carts.Update(cart);
            }
        }
    }
}
