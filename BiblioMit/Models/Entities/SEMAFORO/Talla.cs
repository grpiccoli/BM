using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Talla
    {
        public int Id { get; set; }
        public int SpecieSeedId { get; set; }
        public virtual SpecieSeed SpecieSeed { get; set; }
        public Range Range { get; set; }
        [Range(0,100)]
        public double Proportion { get; set; }
    }
}
