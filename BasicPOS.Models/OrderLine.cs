using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.Models
{
    public class OrderLine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Order { get; set; }
        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        [ValidateNever]
        public Item Item { get; set; }
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        [ValidateNever]
        public Stock Stock { get; set; }
        public int Quantity { get; set; }
    }
}
