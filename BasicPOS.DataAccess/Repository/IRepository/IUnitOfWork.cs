using BasicPOS.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        ICartRepository Cart { get; }
        IItemRepository Item { get; }
        IOrderLineRepository OrderLine { get; }
        IOrderRepository Order { get; }
        IStockRepository Stock { get; }
        Task Save();
    }
}
