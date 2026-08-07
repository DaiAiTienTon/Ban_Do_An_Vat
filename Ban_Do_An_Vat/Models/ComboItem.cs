using System.ComponentModel.DataAnnotations;

namespace Ban_Do_An_Vat.Models
{
    public class ComboItem
    {
        public int Id { get; set; }

        [Required]
        public int ComboId { get; set; }
        public Combo? Combo { get; set; }

        [Required]
        public int SnackId { get; set; }
        public Snack? Snack { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } = 1;
    }
}
