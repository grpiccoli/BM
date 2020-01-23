using System.Collections.Generic;

namespace BiblioMit.Models
{
    public class Specie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sp { get; set; }
        public ICollection<SpecieSeed> SpecieSeeds { get; set; }
    }
}
