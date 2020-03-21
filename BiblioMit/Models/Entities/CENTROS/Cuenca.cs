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
        public virtual ICollection<Comuna> Comunas { get; } = new List<Comuna>();
        public virtual ICollection<Coordinate> Coordinates { get; } = new List<Coordinate>();
    }
}
