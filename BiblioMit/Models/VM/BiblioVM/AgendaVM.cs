using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models.VM
{
    public class AgendaVM
    {
        [Display(Name = "Institución")]
        public Company Company { get; set; }

        [Display(Name = "Título")]
        public string Title { get; set; }

        [Display(Name = "Fondo Concursable")]
        public string Fund { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Sitio Principal")]
        public Uri MainUrl { get; set; }

        [Display(Name = "Apertura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public DateTime? Start { get; set; }

        [Display(Name = "Cierre")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public DateTime? End { get; set; }
    }
}
