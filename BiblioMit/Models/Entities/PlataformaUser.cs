using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models
{
    public class PlataformaUser
    {
        public string AppUserId { get; set; }

        public virtual AppUser AppUser { get; set; }

        public virtual Platform Plataform { get; set; }

        public int PlataformId { get; set; }
    }
}
