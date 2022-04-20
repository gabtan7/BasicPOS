using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        ApplicationDbContext _db;
        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Item obj)
        {
            _db.Items.Update(obj);
        }
    }
}