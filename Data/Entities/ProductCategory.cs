using System.ComponentModel.DataAnnotations.Schema;

namespace TSShopping.Data.Entities
{
    [Table("ProductCategories",Schema="Sho")]
    public class ProductCategory
    {
    public int Id { get; set; }

    public Product Product { get; set; }

    public Category Category { get; set; }
    }
}