using TSShopping.Data.Entities;

namespace TSShopping.Models
{
    public class HomeViewModel
    {
        public ICollection<Product> Products { get; set; }

        public float Quantity { get; set; }

    }
}