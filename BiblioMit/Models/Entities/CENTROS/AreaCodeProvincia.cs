using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models
{
    public class AreaCodeProvincia
    {
        [InsertOff]
        public int ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }

        public int AreaCodeId { get; set; }
        public virtual AreaCode AreaCode { get; set; }
    }
}
