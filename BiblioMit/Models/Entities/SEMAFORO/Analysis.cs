using System;

namespace BiblioMit.Models
{
    public class Analysis
    {
        public int Id { get; set; }
        public int CentreId { get; set; }
        public virtual Centre Centre { get; set; }
        public DateTime Date { get; set; }
    }
}
