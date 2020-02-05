using System.Collections.Generic;

namespace BiblioMit.Models.VM
{
    public class LibManLibrary
    {
        public string Library { get; set; }
        public string Destination { get; set; }
        public List<string> Files { get; } = new List<string>();
        public string Provider { get; set; }
    }
}
