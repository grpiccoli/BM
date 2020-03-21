using BiblioMit.Models.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BiblioMit.Services
{
    public static class Bundler
    {
        private static List<BundleConfig> Bundles { get; } = new List<BundleConfig>();
        public static List<BundleConfig> LoadJson()
        {
            using StreamReader r = new StreamReader("bundleconfig.json");
            string json = r.ReadToEnd();
            Bundles.Clear();
            Bundles.AddRange(JsonConvert.DeserializeObject<List<BundleConfig>>(json));
            return Bundles;
        }
        public static IEnumerable<BundleConfig> GetBundles(string lib)
        {
            return Bundles.Where(m => m.OutputFileName.Contains($"/{lib}.", StringComparison.InvariantCulture));
        }
    }
}
