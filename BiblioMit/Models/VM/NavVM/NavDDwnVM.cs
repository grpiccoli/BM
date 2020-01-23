using System.Collections.Generic;

namespace BiblioMit.Models.VM.NavVM
{
    public class NavDDwnVM
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public string Logo { get; set; }

        public string Title { get; set; }

        public Dictionary<string, string[][]> Sections { get; set; }
    }
}
