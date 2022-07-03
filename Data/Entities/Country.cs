using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSShopping.Data.Entities
{
    [Table("Countries",Schema="Sho")]
    public class Country
    {
        public int Id { get; set; }
        [Display(Name="País")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        public ICollection<State> States { get; set; }
        
        [Display(Name="Departamentos/Estados")]
        public int StatesNumber=> States==null ? 0 : States.Count;
    }
}