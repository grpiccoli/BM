using System.Collections.Generic;

namespace BiblioMit.Models.ViewModels
{
    public class GraphVM
    {
        public Dictionary<string,
                        Dictionary<string, List<string>>> Graphs { get; set; }

        public int Version { get; set; }

        public string[] Reportes { get; set; }

        public int Year { get; set; }

        public string Start { get; set; }

        public string End { get; set; }
    }
}
