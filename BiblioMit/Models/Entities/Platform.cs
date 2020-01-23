using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models
{
    public class Platform
    {
        public int Id { get; set; }

        public Plataforma Plataforma { get; set; }

        public virtual ICollection<PlataformaUser> PlataformaUser { get; set; }
    }
    public enum Plataforma
    {
        [Display(Name = "BiblioMit")]
        bibliomit = 1,
        [Display(Name = "MytiliDB")]
        mytilidb = 2,
        [Display(Name = "Boletín Productivo")]
        boletin = 3,
        [Display(Name = "Plataforma Ambiental")]
        psmb = 4
    }
}
