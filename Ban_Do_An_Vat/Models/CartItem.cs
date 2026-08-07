namespace Ban_Do_An_Vat.Models
{
    public class CartItem
    {
        public int SnackId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public string Weight { get; set; } = "";
        public int Quantity { get; set; }

        // Combo support
        public int ComboId { get; set; }       // > 0 nếu là combo item
        public bool IsCombo { get; set; }
        public string ComboLabel { get; set; } = "";  // vd: "3 món • Tiết kiệm 25%"

        public decimal TotalPrice => Price * Quantity;
    }
}
