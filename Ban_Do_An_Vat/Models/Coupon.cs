using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ban_Do_An_Vat.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000, ErrorMessage = "Giá trị giảm không hợp lệ")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000)]
        public decimal MinOrderAmount { get; set; } = 0; // Minimum order value required to use this coupon

        [Required]
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);

        public bool IsActive { get; set; } = true;
    }
}
