using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Larva
    {
        public int Id { get; set; }
        public int LarvaeId { get; set; }
        public virtual Larvae Larvae { get; set; }
        public int SpecieId { get; set; }
        public Specie Specie { get; set; }
        public LarvaType LarvaType { get; set; }
        [Display(Description = "Larvas/m3")]
        public double Count { get; set; }
    }
    public enum LarvaType
    {
        D = 0,
        U = 1,
        O = 2
    }
}
