using System.ComponentModel.DataAnnotations;

namespace TSShopping.Models
{
    public class StateViewModel
    {
        public int Id { get; set; }
        [Display(Name="País")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        public int CountryId { get; set; }
        
    }
}