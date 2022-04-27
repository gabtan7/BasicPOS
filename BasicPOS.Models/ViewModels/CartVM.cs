using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Cart> CartList { get; set; }
        public decimal CartTotal { get; set; }
    }
}
