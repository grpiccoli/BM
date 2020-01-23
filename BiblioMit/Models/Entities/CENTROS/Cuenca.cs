using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Cuenca
    {
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Comuna> Comunas { get; set; }
        public ICollection<Coordinate> Coordinates { get; set; }
    }
}
