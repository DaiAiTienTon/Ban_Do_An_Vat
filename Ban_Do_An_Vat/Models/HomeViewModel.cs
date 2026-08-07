using System.Collections.Generic;

namespace Ban_Do_An_Vat.Models
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Snack> PopularSnacks { get; set; } = new List<Snack>();
    }
}
