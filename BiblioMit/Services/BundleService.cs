using BiblioMit.Models.VM;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BiblioMit.Services
{
    public static class Bundler
    {
        public static List<Bundle> LoadJson()
        {
            using (StreamReader r = new StreamReader("bundleconfig.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Bundle>>(json);
            }
        }
    }
}
