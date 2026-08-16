using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ban_Do_An_Vat.Models
{
    public class Snack
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public byte[]? ImageData { get; set; }

        [StringLength(50)]
        public string? ImageContentType { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        [Range(0, 10000)]
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(0, 5)]
        public double Rating { get; set; } = 5.0;

        [StringLength(100)]
        public string? Weight { get; set; } // e.g. "150g"

        [StringLength(500)]
        public string? Ingredients { get; set; } // e.g. "Khoai tây, dầu thực vật, muối, bột phô mai"
    }
}
