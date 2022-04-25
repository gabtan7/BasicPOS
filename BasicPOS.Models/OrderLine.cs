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
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
        [ValidateNever]
        public DateTime? UpdatedDate { get; set; }
        [ValidateNever]
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
