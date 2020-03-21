using BiblioMit.Models.VM;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace BiblioMit.Services
{
    public static class Libman
    {
        private static Libs Libs { get; set; } = new Libs();

        public static Libs LoadJson()
        {
            using StreamReader r = new StreamReader("libman.json");
            string json = r.ReadToEnd();
            Libs = JsonConvert.DeserializeObject<Libs>(json);
            return Libs;
        }

        public static LibManLibrary GetLibs(string lib)
        {
            var libs = Libs.Libraries.SingleOrDefault(m => 
            m.Library.StartsWith($"{lib}@", StringComparison.InvariantCulture));
            return libs;
        }
    }
}
