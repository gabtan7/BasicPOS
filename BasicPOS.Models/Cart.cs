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
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public int Quantity { get; set; }
        public string Status { get; set; } = "INPROGRESS";
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        [ValidateNever]
        public Item Item { get; set; }
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        [ValidateNever]
        public Stock Stock { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
        [ValidateNever]
        public DateTime? UpdatedDate { get; set; }
        [ValidateNever]
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
