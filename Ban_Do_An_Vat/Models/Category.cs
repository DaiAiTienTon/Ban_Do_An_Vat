using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ban_Do_An_Vat.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<Snack> Snacks { get; set; } = new List<Snack>();
    }
}
