using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Polygon
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public int Id { get; set; }
        public int? ComunaId { get; set; }
        public Comuna Comuna { get; set; }
        public int? ProvinciaId { get; set; }
        public Provincia Provincia { get; set; }
        public int? RegionId { get; set; }
        public Region Region { get; set; }
        public virtual ICollection<Coordinate> Coordinates { get; } = new List<Coordinate>();
    }
}
