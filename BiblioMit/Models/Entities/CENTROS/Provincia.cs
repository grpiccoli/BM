using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Provincia
    {
        [Display(Name = "Código Único Territorial")]
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int RegionId { get; set; }
        public virtual Region Region { get; set; }

        [Display(Name = "Nombre de Provincia")]
        public string Name { get; set; }
        [Display(Name = "Superficie")]
        public int Surface { get; set; }
        [Display(Name = "Población")]
        public int Population { get; set; }
        public virtual ICollection<Comuna> Comunas { get; } = new List<Comuna>();
        public virtual ICollection<Polygon> Polygons { get; } = new List<Polygon>();
        public virtual ICollection<AreaCodeProvincia> AreaCodeProvincias { get; } = new List<AreaCodeProvincia>();
    }
}
