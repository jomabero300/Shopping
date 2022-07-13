using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSShopping.Data.Entities
{
    [Table("Categories",Schema="Sho")]
    public class Category
    {
        public int Id { get; set; }

        [Display(Name="Categoria")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [Display(Name = "# Productos")]
        public int ProductsNumber => ProductCategories == null ? 0 : ProductCategories.Count();
        
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}