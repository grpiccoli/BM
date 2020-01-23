using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Producto
    {
        [Display(Name = "Producto")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public string Id { get; set; }

        public virtual ICollection<CentreProducto> Plantas { get; set; }
    }
}
