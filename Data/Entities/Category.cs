using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}