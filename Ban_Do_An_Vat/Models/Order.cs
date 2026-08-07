using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ban_Do_An_Vat.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        public string CustomerPhone { get; set; }

        [Required]
        [StringLength(300)]
        public string DeliveryAddress { get; set; }

        [StringLength(500)]
        public string OrderNotes { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "COD"; // COD, Momo, VietQR

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Failed

        [StringLength(450)]
        public string? UserId { get; set; } // Nullable for guest checkout

        [StringLength(50)]
        public string? CouponCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
