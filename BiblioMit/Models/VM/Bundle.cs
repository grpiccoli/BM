using System.Collections.Generic;

namespace BiblioMit.Models.VM
{
    public class BundleConfig
    {
        public string OutputFileName { get; set; }
        public List<string> InputFiles { get; } = new List<string>();
    }
}
