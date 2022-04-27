using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.Models.ViewModels
{
    public class OrderVM
    {
        Order Order { get; set; }
        IEnumerable<OrderLine> OrderLine { get; set; }
    }
}
