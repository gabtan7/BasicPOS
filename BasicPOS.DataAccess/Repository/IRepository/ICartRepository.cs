using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void Update(Cart obj);
        int IncrementCount(Cart obj, int quantity);
        int DecrementCount(Cart obj, int quantity);
        void UpdateCartItemStatus(Cart obj, string status);
        void UpdateCartItemStatus(IEnumerable<Cart> cartList, string status);
    }
}
