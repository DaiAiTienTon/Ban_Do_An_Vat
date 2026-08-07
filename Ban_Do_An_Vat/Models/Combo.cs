using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ban_Do_An_Vat.Models
{
    public class Combo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Tên combo")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ảnh")]
        public string? ImageUrl { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000000)]
        [Display(Name = "Giá gốc")]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000000)]
        [Display(Name = "Giá khuyến mãi")]
        public decimal SalePrice { get; set; }

        [Display(Name = "Đang bán")]
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

        // Computed
        [NotMapped]
        public decimal DiscountPercent =>
            OriginalPrice > 0 ? Math.Round((1 - SalePrice / OriginalPrice) * 100, 0) : 0;
    }
}
