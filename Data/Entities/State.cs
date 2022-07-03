using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TSShopping.Data.Entities
{
    [Table("States",Schema="Sho")]
    public class State
    {
        public int Id { get; set; }
        [Display(Name="País")]
        [MaxLength(100,ErrorMessage="El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage="El campo {0} es obligatorio.")]
        public string Name { get; set; }

        public Country Country { get; set; }

        public ICollection<City> Cities { get; set; }
        
        [Display(Name="Ciudades")]
        public int CityNumber =>Cities==null ? 0 : Cities.Count;
    }
}