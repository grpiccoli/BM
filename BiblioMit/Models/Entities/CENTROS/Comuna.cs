using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Comuna
    {
        [Display(Name = "Código Único Territorial")]
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }

        public int CuencaId { get; set; }
        public virtual Cuenca Cuenca { get; set; }

        [Display(Name = "Nombre de Comuna")]
        public string Name { get; set; }

        [Display(Name = "Distrito Electoral")]
        public int DE { get; set; }

        [Display(Name = "Circunscripción Senatorial")]
        public int CS { get; set; }

        public virtual ICollection<Centre> Centres { get; set; }
        public virtual ICollection<PSMB> PSMBs { get; set; }
        public virtual ICollection<Polygon> Polygons { get; set; }
    }
}
