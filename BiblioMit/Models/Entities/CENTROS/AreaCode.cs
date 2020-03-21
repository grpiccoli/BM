using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class AreaCode
    {
        [Display(Name = "Código de Área Telefónico")]
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public virtual ICollection<AreaCodeProvincia> AreaCodeProvincias { get; } = new List<AreaCodeProvincia>();
    }
}
