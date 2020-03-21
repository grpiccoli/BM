using System.Collections.Generic;

namespace BiblioMit.Models.ViewModels
{
    public class GraphVM
    {
        public Dictionary<string,
                        Dictionary<string, List<string>>> Graphs { get; } = new Dictionary<string, Dictionary<string, List<string>>>();

        public int Version { get; set; }

        public List<string> Reportes { get; } = new List<string>();

        public int Year { get; set; }

        public string Start { get; set; }

        public string End { get; set; }
    }
}
