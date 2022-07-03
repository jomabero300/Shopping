using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TSShopping.Data.Entities
{
    [Table("Cities",Schema="Sho")]
    public class City
    {
        public int Id { get; set; }
        [Display(Name="Ciudad")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        public State State { get; set; }
    }
}