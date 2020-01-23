using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Excel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Columna> Columnas { get; set; }
    }
}
