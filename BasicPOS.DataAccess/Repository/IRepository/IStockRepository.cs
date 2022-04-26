using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public interface IStockRepository : IRepository<Stock>
    {
        void Update(Stock obj);
        decimal DecrementStock(Stock obj, decimal quantity);
    }
}
