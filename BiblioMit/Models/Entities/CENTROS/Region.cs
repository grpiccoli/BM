using BiblioMit.Models.VM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Region
    {
        [Display(Name = "Código Único Territorial")]
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Superficie")]
        public int Surface { get; set; }

        [Display(Name = "Población 2002")]
        public int Pop2002 { get; set; }

        [Display(Name = "Población 2010")]
        public int Pop2010 { get; set; }

        [Display(Name = "Nombre de Región")]
        public string Name { get; set; }

        public virtual ICollection<Provincia> Provincias { get; } = new List<Provincia>();
        public virtual ICollection<Polygon> Polygons { get; } = new List<Polygon>();
    }
}
