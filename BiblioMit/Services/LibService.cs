using BiblioMit.Models.VM;
using Newtonsoft.Json;
using System.IO;

namespace BiblioMit.Services
{
    public static class Libman
    {
        public static Libs LoadJson()
        {
            using (StreamReader r = new StreamReader("libman.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Libs>(json);
            }
        }
    }
}
