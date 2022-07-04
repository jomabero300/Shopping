using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TSShopping.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }
        [Display(Name="Ciudad")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        public int StateId { get; set; }
        
    }
}