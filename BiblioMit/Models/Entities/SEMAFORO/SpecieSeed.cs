using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class SpecieSeed
    {
        public int Id { get; set; }
        public int SpecieId { get; set; }
        public Specie Specie { get; set; }
        public int SeedId { get; set; }
        public virtual Seed Seed { get; set; }
        public int Capture { get; set; }
        [Range(0,100)]
        public double Proportion { get; set; }
        public virtual ICollection<Talla> Tallas { get; } = new List<Talla>();
    }
}
