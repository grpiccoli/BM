using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class PSMB
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public int Id { get; set; }

        public int ComunaId { get; set; }
        public virtual Comuna Comuna { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Centre> Centres { get; } = new List<Centre>();

        public virtual ICollection<Coordinate> Coordinates { get; } = new List<Coordinate>();
    }
}
