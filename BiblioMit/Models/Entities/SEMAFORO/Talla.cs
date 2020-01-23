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
    public enum Range
    {
        mm0_1 = 0,
        mm1_2 = 1,
        mm2_5 = 2,
        mm5_10 = 3,
        mm10_15 = 4,
        mm15_20 = 5,
        mm20_25 = 6,
        mm25_30 = 7
    }
}
