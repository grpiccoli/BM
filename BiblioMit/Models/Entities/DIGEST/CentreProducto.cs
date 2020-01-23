using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models
{
    public class CentreProducto
    {
        [InsertOff]
        public int CentreId { get; set; }
        public virtual Centre Planta { get; set; }
        public string ProductoId { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
