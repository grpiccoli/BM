namespace BiblioMit.Models.ViewModels
{
    public class FilterVM
    {
        public int Rpp { get; set; }

        public bool Asc { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Srt { get; set; }

        public string[] Val { get; set; }
    }
}
