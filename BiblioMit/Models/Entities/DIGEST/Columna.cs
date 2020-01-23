using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Columna
    {
        public int Id { get; set; }

        [Display(Name = "Atributo")]
        public string Name { get; set; }

        [Display(Name = "Planilla")]
        public int ExcelId { get; set; }
        public virtual Excel Excel { get; set; }

        [Display(Name = "Nombre columna")]
        public string Description { get; set; }

        [Display(Name ="Conversión unidades")]
        public string Operation { get; set; }
    }
}
