using TSShopping.Common;
using TSShopping.Data.Entities;

namespace TSShopping.Models
{
    public class HomeViewModel
    {
        public PaginatedList<Product> Products { get; set; }
        public ICollection<Category> Categories { get; set; }
        public float Quantity { get; set; }

    }
}