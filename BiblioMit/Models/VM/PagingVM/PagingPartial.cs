using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models.ViewModels
{
    public class PagingPartial
    {
        public int Pg { get; set; }
        public int Last { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Srt { get; set; }
        public bool Asc { get; set; }
        public int Rpp { get; set; }
    }
}
